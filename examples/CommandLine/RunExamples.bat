@cls
@echo.***********************************************************
@echo.Using UpdateVersion from the command line or batch file
@echo.***********************************************************
@echo.-----------------------------------------------------------
@echo.No options
echo AssemblyVersion("1.0.0.0") | ..\..\bin\UpdateVersion
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Increment build number
echo AssemblyVersion("1.0.0.0") | ..\..\bin\UpdateVersion -b Increment
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Increment build number and revision number
echo AssemblyVersion("1.0.0.0") | ..\..\bin\UpdateVersion -b Increment -r Increment
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Use MonthDay to calculate the build number
echo AssemblyVersion("1.0.0.0") | ..\..\bin\UpdateVersion -b MonthDay -s 2002-11-23
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Use MonthDay to calculate the build number (with US formatted date)
echo AssemblyVersion("1.0.0.0") | ..\..\bin\UpdateVersion -b MonthDay -s 11/23/2002
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Use Pin to use a fixed version number
echo AssemblyVersion("1.0.0.0") | ..\..\bin\UpdateVersion -p 1.2.3.4
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Use BuildDay to calculate the build number.
echo AssemblyVersion("1.0.0.0") | ..\..\bin\UpdateVersion -b BuildDay
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Use -r Fixed to fix the revision number.
echo AssemblyVersion("1.0.0.0") | ..\..\bin\UpdateVersion -b Increment -r Fixed
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Use Version to update the AssemblyFileVersion attribute.
echo AssemblyFileVersion("1.0.0.0") | ..\..\bin\UpdateVersion -b BuildDay
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Redirecting to and from standard input and standard output
..\..\bin\UpdateVersion -b Increment < Input.txt > Output.txt
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Specify the input and output files on the command line
..\..\bin\UpdateVersion -b Increment -i Input.txt -o Output.txt
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Specify an input file on the command line and write the output to standard output
..\..\bin\UpdateVersion -b Increment -i Input.txt
@echo.
@pause
@cls
@echo.-----------------------------------------------------------
@echo.Specify the output file on the command line and read the input from standard input
echo AssemblyVersion("1.0.0.0") | ..\..\bin\UpdateVersion -b Increment -o Output.txt
@echo.
@pause