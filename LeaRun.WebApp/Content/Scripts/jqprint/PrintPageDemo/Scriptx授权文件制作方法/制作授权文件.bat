%SystemRoot%\system32\MAKECAB /D CompressionType=LZX /D CompressionMemory=21 /D MaxDiskSize=CDROM /D Cabinet=On /D Compress=On /D FolderSizeThreshold=5000000 /D DiskDirectoryTemplate=.\ /D CabinetNameTemplate=sxlic.mlf /F list.tmp
Del /F /S /Q SETUP.INF SETUP.RPT >NUL
signcode -v mead.pvk -spc mead.spc -t http://timestamp.verisign.com/scripts/timstamp.dll sxlic.mlf