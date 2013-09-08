Update Version
=============

Utility application for updating .NET version numbers.  Based on the UpdateVersion utility originally developed by Matt Griffith.

Added an option to generate the build number based on the year and day of year.

For example:
1.0.7048.2

This tells us that this is version 1.0, last built on the 48th day of 2007, and built a total of 2 times, which is a lot more useful than just letting the numbers increment automatically.

The original version of UpdateVersion can be downloaded from <a href="http://code.mattgriffith.net/UpdateVersion/" target="_blank">http://code.mattgriffith.net/UpdateVersion/</a> (no longer available).

My updated version also includes some small modifications to <em>Options.cs</em>, changing the way that text strings are parsed to their corresponding enum values and making it easier to add new options.

It also includes a VBScript to make it easier to call UpdateVersion in your pre-build events. To update your AssemblyInfo.cs before building, simply add the following to your pre-build event in the project info:

<em>C:\path\to\updateversion\UpdateVersion.vbs &#8220;$(ProjectDir)&#8221;</em>

This will automatically update your AssemblyInfo file with the generated version information, and is ideal for use with the YearMonthDayOfYear option (though perhaps not with the MonthDay option, as this generates the version information using the project start date, which will be different for each project).

Original blog entry:<br/>
http://www.munsplace.com/blog/2007/02/17/assembly-version-numbers-and-net/

Stackoverflow Question:<br/>
http://stackoverflow.com/questions/826777/how-to-have-an-auto-incrementing-version-number-visual-studio
