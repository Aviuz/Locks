﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace Locks.HarmonyPatches
{
  internal static class HPatcher
  {
    public static void Init()
    {
      var harmony = new Harmony("Harmony_Locks");
      try
      {
        harmony.PatchAll(Assembly.GetExecutingAssembly());
      }
      catch (Exception e)
      {
        Log.Error($"Locks Mod Exception, failed to proceed harmony patches: {e.Message}");
      }
    }

    /// <summary>
    ///   CIL Debugging method. Creates debug file on desktop that list all CIL code instructions in the method.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="withReturn"></param>
    public static void CreateDebugFileOnDesktop(string fileName, IEnumerable<CodeInstruction> instr)
    {
      // Set a variable to the Desktop path.
      var myDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

      // Write the string array to a new file.
      using (var outputFile = new StreamWriter(myDesktopPath + @"\" + fileName + ".txt"))
      {
        outputFile.WriteLine("================");
        outputFile.WriteLine("Body of " + fileName + " method", fileName);
        outputFile.WriteLine("================");
        foreach (var instruction in instr)
        {
          var instructionString = instruction.opcode.ToString();
          instructionString += " | ";
          instructionString += instruction.operand is Label
            ? $"Label {instruction.operand.GetHashCode()}"
            : instruction.operand;
          instructionString += " | ";
          if (instruction.labels.Count > 0)
            foreach (var label in instruction.labels)
              instructionString += $"Label {label.GetHashCode()}";
          else
            instructionString += "no labels";
          outputFile.WriteLine(instructionString);
        }
      }
    }

    /// <summary>
    ///   This method is used to add some CIL instructions after certain fragment in original code.
    ///   It should be used inside foreach loop, and return true if particular iteration is the desired one.
    /// </summary>
    /// <param name="opCodes"></param>
    /// <param name="operands"></param>
    /// <param name="instr"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static bool IsFragment(OpCode[] opCodes, string[] operands, CodeInstruction instr, ref int step)
    {
      if (opCodes.Length != operands.Length)
        return false;
      if (step < 0 || step >= opCodes.Length)
        return false;

      var finalStep = opCodes.Length;

      if (instr.opcode == opCodes[step] && (instr.operand == null || instr.operand.ToString() == operands[step]))
        step++;
      else
        step = 0;

      if (step == finalStep)
      {
        step++;
        return true;
      }
      return false;
    }

    /// <summary>
    ///   This method is used to find particular label that is assigned to last instruction's operand
    /// </summary>
    /// <param name="opCodes"></param>
    /// <param name="operands"></param>
    /// <param name="instr"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static object FindOperandAfter(OpCode[] opCodes, string[] operands, IEnumerable<CodeInstruction> instr)
    {
      if (opCodes.Length != operands.Length)
        return null;

      var finalStep = opCodes.Length;

      var step = 0;
      foreach (var ci in instr)
      {
        if (ci.opcode == opCodes[step] && (ci.operand == null || ci.operand.ToString() == operands[step]))
          step++;
        else
          step = 0;

        if (step == finalStep)
          return ci.operand;
      }

      return null;
    }
  }
}