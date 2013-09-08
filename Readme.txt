*******************************************************************************
Overview
*******************************************************************************

UpdateVersion searches its input for a .NET AssemblyVersion attribute and calculates a new version number using one of several algorithms. UpdateVersion can use a file or the standard input stream for input and it can write its output to a file or the standard output stream.

UpdateVersion calculates and outputs new version numbers using one of several algorithms. You can use it with Visual Studio .NET to update your AssemblyInfo.* file on demand. You can use it from a NAnt script to update your assembly version numbers every time you build your project. You can also run it directly from the command line, in a batch file, or from a make file.

UpdateVersion will calculate a new revision number only or it can calculate a new build number and a new revision number at the same time.

UpdateVersion can calculate the build number by incrementing the existing build number or it can calculate the build number based on the project start date. 

UpdateVersion can calculate the revision number by incrementing the existing revision number or it can calculate the revision number based on the number of seconds since midnight.

*******************************************************************************
Revision History
*******************************************************************************

Version 1.2

	Matt Griffith - http://mattgriffith.net

		* Added Version option.

		* Attempted to improve Unicode handling by outputting to file using the default encoding.

	Scott Hanselman - http://www.hanselman.com/blog/ 

		* Added BuildDay algorithm for build number calculation.

		* Added Fixed algorithm for revision number calculation.

		* Updated the Regular expression to handle this condition: [assembly: AssemblyVersionAttribute("1.0.3296.1")]

		* Refactored VersionUpdater.cs.

Version 1.1
	Mike Gunderloy - http://www.larkware.com

		* Added the Pin feature.


*******************************************************************************
Known issues
*******************************************************************************

UpdateVersion uses the default encoding to open input files. This can cause UpdateVersion to drop characters in the output if the input is ANSI encoded and uses extended character codes. 

To work around this problem make sure your input files are saved as UTF-8 or don't use extended characters. 

A future version of UpdateVersion may allow you to specify the encoding of the input and output files.

*******************************************************************************
Installing UpdateVersion
*******************************************************************************

Unzip to a directory. You can then run UpdateVersion.exe from the bin directory. You can add the bin directory to your PATH for easier access.

*******************************************************************************
Building UpdateVersion
*******************************************************************************

To build using NAnt run:

> nant dist
(Builds UpdateVersion and prepares it for distribution)

> nant
(Builds UpdateVersion - the new EXE is in the build directory)

To build using Visual Studio .NET:

Double-click the UpdateVersion.csproj file and press Ctrl+Shift+B