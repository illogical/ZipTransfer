# ZipTransfer
A utility to zip Unity projects and transfer them to a NAS. 

Due to Unity projects having tons of small files, it is very slow to transfer them to a NAS. This utility will zip the project and transfer the zip file instead.

## Usage
There are 2 possible approaches to using this utility. 
1. The first is to use the command line arguments to specify the transfers to perform. 
2. The second is to use the configuration file to specify the transfers to perform.
```
ZipTransfer.exe <source> <destination> <tempPath>
```


## Configuration.json Sample
```
{
    "Transfers": [
        {
            "Source": "C:\\temp\\Transfers\\source",
            "Destination": "C:\\temp\\Transfers\\destination",
            "ZipSubdirectories": false,     //optional
            "Versions": 2                   //optional
        }
    ],
    "TempLocation": "C:\\temp"
}
```
