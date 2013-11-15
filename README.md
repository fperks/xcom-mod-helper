XCOM Enemy Unknown/Enemy Within Mod Helper
===========================================

This tool allows for patching of XCOM Enemy Unknown and Enemy Within files to help mods. For example, the tool can patch the game to allow for changes to be made to DefaultGameCore.ini,
and force the game to read it from the configuration directory. Without the need of using modpatcher. The functionality of this tool is entirely dependant on the hex patches specified in the configuration files.

Credits
-------
* johnnylump (for the idea/telling me what i should patch)
* Grenademagnet (for pointing out bugs)

Quickstart (Enemy Within)
----------------------------
To allow for reading of DefaultGameCore.ini from XEW/XCOMGame/Config folder

* Download the latest release
* Unzip to a directory
* Open Command prompt, and navigate to the directory you unzipped it to
* Execute: XCOMModHelper -c EWConfig.xml

Output:
```
[INFO]> ==========XCOMModHelper========
[INFO]> Attempting to Locate XCOM Root Directory
[INFO]> Found XCOM Root Directory of [ x:\steam\SteamApps\common\XCom-Enemy-Unknown ]
[INFO]> Backup Target Directory is [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\Backup ]
[INFO]> Decompressed UPK Folder [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\UpkUnpacked ]
[INFO]> Patching [2] Targets
[INFO]> Patching Target [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\Binaries\Win32\XComEW.exe ]
[INFO]> Applying Patch [ Read DefaultGameCore.ini from Config Folder ]
[INFO]> Applying Patch at Index [ 21464864 ]
[INFO]> Patch Successfully Applied [ Read DefaultGameCore.ini from Config Folder ]
[INFO]> File [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\Binaries\Win32\XComEW.exe ] Backed up to [ x:\steam\Stea
mApps\common\XCom-Enemy-Unknown\XEW\Binaries\Win32\XComEW.exe ]
[INFO]> Saving Changes to [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\Binaries\Win32\XComEW.exe ]
[INFO]> Patching Target [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\XComGame\CookedPCConsole\XComGame.upk ]
[INFO]> Decompressed Upk File [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\UpkUnpacked\XComGame.upk ]
[INFO]> Applying Patch [ Changes Gender Chance from 1/2 chance of female, to 1/8 chance of female ]
[INFO]> Applying Patch at Index [ 10231835 ]
[INFO]> Patch Successfully Applied [ Changes Gender Chance from 1/2 chance of female, to 1/8 chance of female ]
[INFO]> File [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\XComGame\CookedPCConsole\XComGame.upk.uncompressed_size ] Backed up to [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\XComGame\CookedPCConsole\XComGame.upk.uncompressed_size ]
[INFO]> File [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\XComGame\CookedPCConsole\XComGame.upk ] Backed up to [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\XComGame\CookedPCConsole\XComGame.upk ]
[INFO]> Saving Changes to [ x:\steam\SteamApps\common\XCom-Enemy-Unknown\XEW\XComGame\CookedPCConsole\XComGame.upk ]
[INFO]> Finished
```

Usage
------

```
Usage: XCOMModHelper -c <config file>.xml

  -c, --config     Required. The required configuration file

  -v, --verbose    (Default: False) Prints Detailed Output

  -t, --test       (Default: False) Attempts to apply the patches but does not
                   write the changes

  -x, --xcomdir    Specify the Path to XCOM Root Install Directory, this is
                   optional and will be automatically determined

  --help           Display this help screen.

```

The application takes a configuration file as a parameter, which specifies which hex strings need to be patched. 
Only unique matches are patched, so if the hex string is not unique it will result in an error. 

Note: You do not need to specify the XCOM Directory, it is automatically determined through the registry. 

Example Usage
---------------

```
> XCOMModHelper -c EWConfig.xml
```

This would patch the enemy within executable with the enemy within configuration file.

Enemy Within Config
---------------------------

```xml
<?xml version="1.0" encoding="utf-8"?>
<PatcherConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <!-- The backup Directory, this is a relative path from XCOM Root, that is expanded -->
  <BackupDirectory>XEW\Backup</BackupDirectory>
  <!-- The folder to temporarily put decompressed UPK files Directory -->
  <DecompressedUPKOutputDirectory>XEW\UpkUnpacked</DecompressedUPKOutputDirectory>
  <!-- These are the Patching target files, each patch target specifies a single file-->
  <PatchTargets>
    <PatchTarget>
      <!-- The file we want to patch-->
      <TargetPath>XEW\Binaries\Win32\XComEW.exe</TargetPath>
      <!-- true or false, true if the file is a UPK file, this tells it to decompress the UPK folder -->
      <IsUPKFile>false</IsUPKFile>
      <Patches>
        <PatchEntry>
          <!-- descriptive name for the patch -->
          <Description>Read DefaultGameCore.ini from Config Folder</Description>
          <!-- The pattern value we are trying to match, this must be unique -->
          <FindValue>   25 00 64 00 00 00 00 00 49 00 6e 00 69 00 56 00 65 00 72 00 73 00 69 00 6f 00 6e 00 00 00 00 00 2e 00 2e 00 5c 00 2e 00 2e 00 5c 00 58 00 43 00 </FindValue>
          <!-- The replacement value we are using, this must be unique -->
          <ReplaceValue>   25 00 64 00 00 00 00 00 49 00 6e 00 69 00 56 00 65 00 72 00 73 00 69 00 6f 00 6e 00 00 00 00 00 2e 00 2e 00 5c 00 2e 00 2e 00 5c 00 57 00 43 00 </ReplaceValue>
        </PatchEntry>
      </Patches>
    </PatchTarget>
    <PatchTarget>
      <!-- Example of trying to patch a UPK File-->
      <TargetPath>XEW\XComGame\CookedPCConsole\XComGame.upk</TargetPath>
      <IsUPKFile>true</IsUPKFile>
      <Patches>
        <PatchEntry>
          <Description>Changes Gender Chance from 1/2 chance of female, to 1/8 chance of female</Description>
          <FindValue>45 9A A7 2C 02 16</FindValue>
          <ReplaceValue>45 9A A7 2C 08 16</ReplaceValue>
        </PatchEntry>
      </Patches>
    </PatchTarget>
  </PatchTargets>
</PatcherConfiguration>
```

Enemy Unknown Config
----------------------------

```xml
<?xml version="1.0" encoding="utf-8"?>
<PatcherConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <BackupDirectory>Backup</BackupDirectory>
  <DecompressedUPKOutputDirectory>UpkUnpacked</DecompressedUPKOutputDirectory>
  <PatchTargets>
    <PatchTarget>
      <TargetPath>Binaries\Win32\XComGame.exe</TargetPath>
      <IsUPKFile>false</IsUPKFile>
      <Patches>
        <PatchEntry>
          <Description>Read DefaultGameCore.ini from Config Folder</Description>
          <FindValue>   25 00 64 00 00 00 00 00 49 00 6e 00 69 00 56 00 65 00 72 00 73 00 69 00 6f 00 6e 00 00 00 00 00 2e 00 2e 00 5c 00 2e 00 2e 00 5c 00 58 00 43 00 </FindValue>
          <ReplaceValue>   25 00 64 00 00 00 00 00 49 00 6e 00 69 00 56 00 65 00 72 00 73 00 69 00 6f 00 6e 00 00 00 00 00 2e 00 2e 00 5c 00 2e 00 2e 00 5c 00 57 00 43 00 </ReplaceValue>
        </PatchEntry>
        <PatchEntry>
          <Description>Read XComGame.upk from CookedPCConsole Folder without size check</Description>
          <FindValue>78 63 6f 6d 67 61 6d 65 2e 75 70 6b</FindValue>
          <ReplaceValue>77 63 6f 6d 67 61 6d 65 2e 75 70 6b</ReplaceValue>
        </PatchEntry>
        <PatchEntry>
          <Description>Read XComStrategyGame.upk from CookedPCConsole Folder without size check </Description>
          <FindValue>78 63 6f 6d 73 74 72 61 74 65 67 79 67 61 6d 65 2e 75 70 6b</FindValue>
          <ReplaceValue>77 63 6f 6d 73 74 72 61 74 65 67 79 67 61 6d 65 2e 75 70 6b</ReplaceValue>
        </PatchEntry>
      </Patches>
    </PatchTarget>
  </PatchTargets>
</PatcherConfiguration>
```