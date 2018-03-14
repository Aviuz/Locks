:: ========= Copy ==========
 
rd "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks" /s /q
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks"

:: About
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\About"
xcopy "About\*.*" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\About" /e

:: Assemblies
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\Assemblies"
xcopy "Assemblies\*.*" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\Assemblies" /e

:: Defs 
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\Defs"
xcopy "Defs\*.*" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\Defs" /e

:: Languages
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\Languages"
xcopy "Languages\*.*" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\Languages" /e

:: Textures
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\Textures"
xcopy "Textures\*.*" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\Textures" /e

:: changelog.txt
copy "changelog.txt" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\Locks\changelog.txt"



:: ========= Run ==========

start steam://rungameid/294100