HTTPS Server
------------

The class HttpListener works with ssl only on linux,windows/mono and windows/dotnetcore.
What is not working is dotnetcore/linux.

The mono runtime searches for the server certificates in a hardcoded folder:

	string dirname = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
	string path = Path.Combine (dirname, ".mono");
	path = Path.Combine (path, "httplistener");

Following post suggests, that the dotnetcore runtime on windows searches for the certificates in the certificate store:
https://stackoverflow.com/questions/11403333/httplistener-with-https-support/33905011#33905011

But for a dotnetcore runtime running on linux, there is no such solution.

That means for a general solution for dotnetcore/linux ASP.NET and the Kestrel implementation must be used:
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-5.0
(Kestrel is a cross-platform web server for ASP.NET Core.)

BUT kestrel is not available on mono.

That means two implementations, one with HTTPListener/mono and another with ASP.NET/Kestrel/dotnetcore.

See:
https://github.com/dotnet/runtime/issues/19752
and
https://github.com/dotnet/runtime/issues/27391

So it must be distinguished between an application that runs on a mono runtime and on a 