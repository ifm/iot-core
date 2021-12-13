This is for the version 5.12 of the mono runtime as it is present on the pdm3 firmeare 1.0.x.x (for later firmwares the workflow may change, for instance since the tool Httpcfg.exe may be already present on the system...)
---------------------------------------------------------------------------------------------------------------------------------------------

The tool Httpcfg.exe is not present on the pdm. Here in Httpcfg.cs is the source,
it can be compiled with:

mcs Httpcfg.cs /reference:Mono.Security.dll

on the pdm.

Then run the commands as explained in makecert.sh.
Adjust the IP for the certificate.

Then just run the iotcore with a https prefix...
/usr/bin/mono /opt/ifm/iotcore/Launch.exe -u https://192.168.2.11:8094 -i 00-02-01-0D-3A-21