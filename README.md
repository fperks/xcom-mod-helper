XCOM Enemy Unknown Mod Helper
=============================

This tool allows for automatic patching of XCOM Enemy Unknown. 

Usage
------

```
Usage: XCOMEUModHelper -c <config file>.xml

  -c, --config     Required. The required configuration file

  -v, --verbose    (Default: False) Prints Detailed Output

  -x, --xcomdir    Specify the Path to XCOM Root Directory, this is optional
                   and will be automatically determined

  --help           Display this help screen.
```

The application takes a configuration file as a parameter, which specifies which hex strings need to be patched. 

Example Config
--------------

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Config>
  <Patch>
    <!-- Read XComGame.upk from CookedPCConsole Folder without size check -->
    <Find>   78 63 6f 6d 67 61 6d 65 2e 75 70 6b</Find>
    <Replace>77 63 6f 6d 67 61 6d 65 2e 75 70 6b</Replace>
  </Patch>
</Config>
```

