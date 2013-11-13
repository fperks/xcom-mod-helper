XCOM Enemy Unknown Mod Helper
=============================

This tool allows for automatic patching of XCOM Enemy Unknown. 

Usage
------

```
Usage: XCOMModHelper -g <EU|EW> -c <config file>.xml

  -g, --gametype    Required. Valid values are are <EU> (Enemy Unknown) or <EW> (Enemy Within)

  -c, --config      Required. The required configuration file

  -v, --verbose     (Default: False) Prints Detailed Output

  -x, --xcomdir     Specify the Path to XCOM Root Install Directory, this is
                    optional and will be automatically determined

  --help            Display this help screen.
```

The application takes a configuration file as a parameter, which specifies which hex strings need to be patched. 
Only unique matches are patched, so if the hex string is not unique it will result in an error. 

Note: You do not need to specify the XCOM Directory, it is automatically determined through the registry. 

Example Usage
---------------

```
> XCOMModHelper -g EW -c EWConfig.xml
```

This would patch the enemy within executable with the corresponding configuration file

Example Config
--------------

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Config>
  <Patch>
    <!-- Read DefaultGameCore.ini from Config Folder  -->
    <Find>   25 00 64 00 00 00 00 00 49 00 6e 00 69 00 56 00 65 00 72 00 73 00 69 00 6f 00 6e 00 00 00 00 00 2e 00 2e 00 5c 00 2e 00 2e 00 5c 00 58 00 43 00 </Find>
    <Replace>25 00 64 00 00 00 00 00 49 00 6e 00 69 00 56 00 65 00 72 00 73 00 69 00 6f 00 6e 00 00 00 00 00 2e 00 2e 00 5c 00 2e 00 2e 00 5c 00 57 00 43 00 </Replace>
  </Patch>
</Config>
```

