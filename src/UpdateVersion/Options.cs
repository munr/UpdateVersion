/*

  Copyright (c) 2002 Matt Griffith

  Permission is hereby granted, free of charge, to any person obtaining 
  a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation 
  the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the 
  Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in 
  all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
  
*/
using System;
using NArgs;

namespace MattGriffith.UpdateVersion
{
	/// <summary>
	/// Represents the command line options for this application.
	/// </summary>
	class Options
	{
		/// <summary>
		/// Describes the available command line options and their proper usage.
		/// </summary>
		public static string Usage = "usage:\nUpdateVersion [{-s,--startdate} Date]\n" +
			"[{-b,--build} Fixed | MonthDay | Increment | BuildDay]\n" +
			"[(-p,--pin) x.x.x.x]\n" +
			"[{-r,--revision} Fixed | Automatic | Increment]\n" +
			"[{-i,--inputfile} Filename]\n" +
			"[{-o,--outputfile} Filename]\n" +
			"[{-v,--version} Assembly | File]\n" ;
		
		/// <summary>
		/// Stores the startdate command line option.
		/// </summary>
		private readonly DateOption _StartDateOption;

		/// <summary>
		/// Stores the build command line option.
		/// </summary>
		private readonly StringOption _BuildOption;

		/// <summary>
		/// Stores the pin command line option.
		/// </summary>
		private readonly StringOption _PinOption;

		/// <summary>
		/// Stores the revision command line option.
		/// </summary>
		private readonly StringOption _RevisionOption;

		/// <summary>
		/// Stores the inputfile command line option.
		/// </summary>
		private readonly StringOption _InputFile;

		/// <summary>
		/// Stores the outputfile command line option.
		/// </summary>
		private readonly StringOption _OutputFile;

		/// <summary>
		/// Stores the version command line option.
		/// </summary>
		private readonly StringOption _VersionOption;

		/// <summary>
		/// Initializes a new instance of the Options class with the specified command
		/// line arguments.
		/// </summary>
		/// <param name="args">The arguments passed on the command line.</param>
		public Options(string[] args)
		{
			// Initialize the options
			_StartDateOption = new DateOption("s", "startdate");
			_BuildOption = new StringOption("b", "build", "Fixed");
			_PinOption = new StringOption("p", "pin", "0.0.0.0");
			_RevisionOption = new StringOption("r", "revision", "Automatic");
			_InputFile = new StringOption("i", "inputfile");
			_OutputFile = new StringOption("o", "outputfile");
			_VersionOption = new StringOption("v", "version");

			// Create a new command line parser and add our options
			CmdLineParser parser = new CmdLineParser();
			parser.AddOption(_StartDateOption);
			parser.AddOption(_BuildOption);
			parser.AddOption(_PinOption);
			parser.AddOption(_RevisionOption);
			parser.AddOption(_InputFile);
			parser.AddOption(_OutputFile);
			parser.AddOption(_VersionOption);

			// Try to parse our options
			try
			{
				parser.Parse(args);
				ValidatePinOption();
			}
			catch(Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the StartDate specified on the command line.
		/// </summary>
		/// <value>Returns DateTime.MinValue if this option was not provided.</value>
		public DateTime StartDate
		{
			get
			{
				DateTime startDateSetting = DateTime.MinValue;

				if(this._StartDateOption.IsMissing)
				{
					startDateSetting = this._StartDateOption.DefaultValue;
				}
				else
				{
					startDateSetting = this._StartDateOption.Value;
				}

				return startDateSetting;
			}
		}

		/// <summary>
		/// Gets the BuildNumberType specified on the command line.
		/// </summary>
		/// <value>
		/// Returns the default BuildNumberType if the build option 
		/// was not specified on the command line.
		/// </value>
		public BuildNumberType BuildNumberType
		{
			get
			{
				BuildNumberType buildSetting = BuildNumberType.Fixed;

				if(this._BuildOption.IsMissing)
				{
					buildSetting = ToBuildNumberType(this._BuildOption.DefaultValue);
				}
				else
				{
					buildSetting = ToBuildNumberType(this._BuildOption.Value);
				}

				return buildSetting;
			}
		}

		/// <summary>
		/// Private helper that converts a string to the equivilent BuildNumberType.
		/// </summary>
		/// <param name="buildNumberDescription">
		/// The string representing a BuildNumberType.
		/// </param>
		/// <returns>
		/// Returns the default BuildNumberType if the string is not a recognized BuildNumberType.
		/// </returns>
		private static BuildNumberType ToBuildNumberType(string buildNumberDescription)
		{
			try
			{
				return (BuildNumberType) Enum.Parse(typeof(BuildNumberType), buildNumberDescription, true);
			}
			catch
			{
				return BuildNumberType.Fixed;
			}
		}

		/// <summary>
 		/// Gets the PinVersion specified on the command line.
 		/// </summary>
 		/// <value>
 		/// Returns null if the pin option 
 		/// was not specified on the command line.
 		/// </value>
 		public Version PinVersion
 		{
 			get
 			{
 				Version pinVersion = null;
 
 				if(this._PinOption.IsMissing)
 				{
 					pinVersion = null;
 				}
 				else
 				{
 					pinVersion = new Version(this._PinOption.Value);
 				}
 
 				return pinVersion;
 			}
 		}

		/// <summary>
		/// Indicates whether the version number is pinned.
		/// </summary>
		/// <value>
		/// Returns true if the pin option was specified on the command line. Otherwise
		/// returns false.
		/// </value>
		public bool VersionIsPinned
		{
			get
			{
				bool pinned = false;

				if(this._PinOption.IsMissing)
				{
					pinned = false;
				}
				else
				{
					pinned = true;
				}

				return pinned;
			}
		}

		/// <summary>
		/// Gets the RevisionNumberType specified on the command line.
		/// </summary>
		/// <value>
		/// Returns the default RevisionNumberType if the revision option 
		/// was not specified on the command line.
		/// </value>
		public RevisionNumberType RevisionNumberType
		{
			get
			{
				RevisionNumberType revisionSetting = RevisionNumberType.Automatic;

				if(this._RevisionOption.IsMissing)
				{
					revisionSetting = ToRevisionNumberType(this._RevisionOption.DefaultValue);
				}
				else
				{
					revisionSetting = ToRevisionNumberType(this._RevisionOption.Value);
				}

				return revisionSetting;
			}
		}

		/// <summary>
		/// Private helper that converts a string to the equivilent RevisionNumberType.
		/// </summary>
		/// <param name="revisionNumberDescription">
		/// The string representing a RevisionNumberType.
		/// </param>
		/// <returns>
		/// Returns the default RevisionNumberType if the string is not a recognized RevisionNumberType.
		/// </returns>
		private static RevisionNumberType ToRevisionNumberType(string revisionNumberDescription)
		{
			try
			{
				return (RevisionNumberType) Enum.Parse(typeof(RevisionNumberType), revisionNumberDescription, true);
			}
			catch
			{
				return RevisionNumberType.Automatic;
			}
		}

		/// <summary>
		/// Gets the inputfile specified on the command line.
		/// </summary>
		/// <value>
		/// Returns null if the inputfile was not specified on the command line.
		/// </value>
		public string InputFilename
		{
			get
			{
				string filename = null;
				
				if(this._InputFile.IsMissing)
				{
					filename = null;
				}
				else
				{
					filename = this._InputFile.Value;
				}

				return filename;
			}
		}

		/// <summary>
		/// Gets the outputfile specified on the command line.
		/// </summary>
		/// <value>
		/// Returns null if the outputfile was not specified on the command line.
		/// </value>
		public string OutputFilename
		{
			get
			{
				string filename = null;
				
				if(this._OutputFile.IsMissing)
				{
					filename = null;
				}
				else
				{
					filename = this._OutputFile.Value;
				}

				return filename;
			}
		}

		/// <summary>
		/// Gets the VersionType specified on the command line.
		/// </summary>
		/// <value>
		/// Returns the default VersionType if the version option 
		/// was not specified on the command line.
		/// </value>
		public VersionType VersionType
		{
			get
			{
				VersionType versionSetting = VersionType.Assembly;

				if(this._VersionOption.IsMissing)
				{
					versionSetting = ToVersionType(this._VersionOption.DefaultValue);
				}
				else
				{
					versionSetting = ToVersionType(this._VersionOption.Value);
				}

				return versionSetting;
			}
		}

		/// <summary>
		/// Private helper that converts a string to the equivilent VersionType.
		/// </summary>
		/// <param name="versionDescription">
		/// The string representing a VersionType.
		/// </param>
		/// <returns>
		/// Returns the default VersionType if the string is not a recognized RevisionNumberType.
		/// </returns>
		private static VersionType ToVersionType(string versionDescription)
		{
			try
			{
				return (VersionType) Enum.Parse(typeof(VersionType), versionDescription, true);
			}
			catch
			{
				return VersionType.Assembly;
			}
		}

		/// <summary>
		/// A private helper method that verifies the pin option specified on the commandline.
		/// </summary>
		/// <exception cref="System.ApplicationException">
		/// The pin option is not a valid version number. See 
		/// <seealso cref="System.Version">Version</see> for more information.
		/// </exception>
		private void ValidatePinOption()
		{
			if(!this._PinOption.IsMissing)
			{
				try
				{
					Version version = new Version(this._PinOption.Value);
				}
				catch(Exception e)
				{
					throw new ApplicationException("The version number specified for the pin option is not valid. Please provide a version number in the correct format.", e);
				}
			}
		}
	}
}
