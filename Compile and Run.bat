:: ========= Build ==========

devenv "D:\Git Repository\Locks\Source\Locks.sln" /build Debug



:: ========= Copy ==========
 
rd "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks" /s /q
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks"

:: About
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\About"
xcopy "D:\Git Repository\Locks\About\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\About" /e

:: Assemblies
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\Assemblies"
xcopy "D:\Git Repository\Locks\Assemblies\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\Assemblies" /e

:: Defs 
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\Defs"
xcopy "D:\Git Repository\Locks\Defs\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\Defs" /e

:: Languages
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\Languages"
xcopy "D:\Git Repository\Locks\Languages\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\Languages" /e

:: Textures
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\Textures"
xcopy "D:\Git Repository\Locks\Textures\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\Textures" /e

:: changelog.txt
copy "D:\Git Repository\Locks\changelog.txt" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\Locks\changelog.txt"



:: ========= Run ==========

start steam://rungameid/294100