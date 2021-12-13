makecert -r -eku 1.3.6.1.5.5.7.3.1 -n "CN=192.18.2.11" -sv 192-8094.pvk 192-8094.cer
mono httpcfg.exe -add -port 8094 -cert 192-8094.cer -pvk 192-8094.pvk


# -eku Add some extended key usage OID to the certificate.
# -eku 1.3.6.1.5.5.7.3.1
# is adding the "Server Authentication" extended usage IOD to the certificate.

#       Mono MakeCert - version 5.12.0.226
#       X.509 Certificate Builder
#       Copyright 2002, 2003 Motus Technologies. Copyright 2004-2008 Novell. BSD licensed.
#       
#       ERROR: Missing output filename
#       
#       Usage: makecert [options] certificate
#       
#        -# num
#               Certificate serial number
#        -n dn
#               Subject Distinguished Name
#        -in dn
#               Issuer Distinguished Name
#        -r
#               Create a self-signed (root) certificate
#        -sv pkvfile
#               Private key file (.PVK) for the subject (created if missing)
#        -iv pvkfile
#               Private key file (.PVK) for the issuer
#        -ic certfile
#               Extract the issuer's name from the specified certificate
#        -?
#               help (display this help message)
#        -!
#               extended help (for advanced options)
#       
#       
#       Mono MakeCert - version 5.12.0.226
#       X.509 Certificate Builder
#       Copyright 2002, 2003 Motus Technologies. Copyright 2004-2008 Novell. BSD licensed.
#       
#       Usage: makecert [options] certificate
#       
#        -a hash        Select hash algorithm. Only MD5 and SHA1 (default) are supported.
#        -b date        The date since when the certificate is valid (notBefore).
#        -cy [authority|end]    Basic constraints. Select Authority or End-Entity certificate.
#        -e date        The date until when the certificate is valid (notAfter).
#        -eku oid[,oid] Add some extended key usage OID to the certificate.
#        -h number      Add a path length restriction to the certificate chain.
#        -in name       Take the issuer's name from the specified parameter.
#        -m number      Certificate validity period (in months).
#        -p12 pkcs12file password       Create a new PKCS#12 file with the specified password.
#        -?     help (display basic message)
#       
#       