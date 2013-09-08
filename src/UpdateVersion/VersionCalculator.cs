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
using System.Globalization;
using System.Threading;

namespace MattGriffith.UpdateVersion
{
	/// <summary>
	/// Specifies the algorithm to use when calculating a new build number.
	/// </summary>
	enum BuildNumberType
	{
		/// <summary>
		/// Indicates the build number should not change.
		/// </summary>
		Fixed,

		/// <summary>
		/// Indicates the build number should be calculated using the number of months 
		/// since the start of the project as the plus the current day, in the 
		/// current month, as the build number.
		/// </summary>
		MonthDay,

		/// <summary>
		/// Indicates the build number should be calculated using the year, plus
		/// the current day of the year, as the build number.
		/// </summary>
		YearDayOfYear,

		/// <summary>
		/// Indicates the build number should be calculated by incrementing the existing 
		/// build number by one.
		/// </summary>
		Increment
	}

	/// <summary>
	/// Specifies the algorithm to use when calculating a new revision number.
	/// </summary>
	enum RevisionNumberType
	{
		/// <summary>
		/// Calculates the revision number based on the number of seconds since midnight divided by 10.
		/// </summary>
		Automatic,

		/// <summary>
		/// Increments the existing revision number by one.
		/// </summary>
		Increment,

		/// <summary>
		/// Makes no changes to the Revision Number
		/// </summary>
		Fixed
	}

	/// <summary>
	/// Specifies the version attribute type that should be updated.
	/// </summary>
	enum VersionType
	{
		/// <summary>
		/// Updates the AssemblyVersion attribute.
		/// </summary>
		Assembly,

		/// <summary>
		/// Updates the AssemblyFileVersion attribute.
		/// </summary>
		File
	}

	/// <summary>
	/// Represents a calculator that can calculate new version numbers.
	/// </summary>
	class VersionCalculator
	{
		/// <summary>
		/// Stores the version number that the calculated version is based on.
		/// </summary>
		private Version _OriginalVersion;

		/// <summary>
		/// Stores the project start date used to calculate new build numbers
		/// when using the MonthDay algorithm.
		/// </summary>
		private DateTime _StartDate = DateTime.MinValue;

		/// <summary>
		/// Stores the algorithm to use when calculating the build number.
		/// </summary>
		private BuildNumberType _BuildNumberType = BuildNumberType.Fixed;

		/// <summary>
		/// Stores the algorithm to use when calculating the revision number.
		/// </summary>
		private RevisionNumberType _RevisionNumberType = RevisionNumberType.Automatic;

		/// <summary>
		/// Initializes a new VersionCalculator with the specified version.
		/// </summary>
		/// <param name="originalVersion">
		/// A string containing the major, minor, build, and 
		/// revision numbers, where each number is delimited with a period character ('.').
		/// </param>
		public VersionCalculator(string originalVersion) : this (new Version(originalVersion))
		{
		}

		/// <summary>
		/// Initializes a new VersionCalculator with the specified version.
		/// </summary>
		/// <param name="originalMajor">The original major version number.</param>
		/// <param name="originalMinor">The original minor version number.</param>
		/// <param name="originalBuild">The original build version number.</param>
		/// <param name="originalRevision">The original revision version number.</param>
		public VersionCalculator(int originalMajor, int originalMinor, int originalBuild, int originalRevision)  : this(new Version(originalMajor, originalMinor, originalBuild, originalRevision))
		{
		}

		/// <summary>
		/// Initializes a new VersionCalculator with the specified version.
		/// </summary>
		/// <param name="originalVersion">The original version.</param>
		public VersionCalculator(Version originalVersion)
		{
			this._OriginalVersion = originalVersion;
		}

		/// <summary>
		/// Gets the version number that the calculated version is based on.
		/// </summary>
		public Version OriginalVersion
		{
			get { return this._OriginalVersion; }
		}

		/// <summary>
		/// Gets the project start date used to calculate new build numbers when
		/// using the MonthDay algorithm. 
		/// </summary>
		public DateTime StartDate
		{
			get { return this._StartDate; }
			set
			{
				if (DateTime.MinValue != value && value > DateTime.Now)
					throw new ArgumentOutOfRangeException("value", "The start date can not be after today's date.");

				if(DateTime.MinValue == value && BuildNumberType.MonthDay == this._BuildNumberType)
					this._BuildNumberType = BuildNumberType.Fixed;

				this._StartDate = value;
			}
		}

		/// <summary>
		/// Gets the algorithm to use when calculating the build number.
		/// </summary>
		public BuildNumberType BuildNumberType
		{
			get { return this._BuildNumberType; }
			set
			{
				if (DateTime.MinValue == this._StartDate && BuildNumberType.MonthDay == value)
					throw new ArgumentException("You must set the StartDate before setting the BuildNumberType to MonthDay.");
				
				this._BuildNumberType = value;
			}
		}
        
		/// <summary>
		/// Gets the algorithm to use when calculating the revision number.
		/// </summary>
		public RevisionNumberType RevisionNumberType
		{
			get { return this._RevisionNumberType; }
			set { this._RevisionNumberType = value; }
		}

		/// <summary>
		/// Gets the new calculated version.
		/// </summary>
		public Version NewVersion
		{
			get
			{
				int major = this._OriginalVersion.Major;
				int minor = this._OriginalVersion.Minor;
				int build = CalculateBuildNumber();
				int revision = CalculateRevisionNumber();

				Version newVersion = new Version(major, minor, build, revision);
				
				return newVersion;
			}
		}

		/// <summary>
		/// Private helper method that calculates the build number.
		/// </summary>
		/// <returns>The new calculated build number.</returns>
		private int CalculateBuildNumber()
		{
			int newBuildNumber = this._OriginalVersion.Build;
			
			switch(this._BuildNumberType)
			{
				case BuildNumberType.Fixed:
					newBuildNumber = this._OriginalVersion.Build;
					break;

				case BuildNumberType.Increment:
					newBuildNumber++;
					break;

				case BuildNumberType.MonthDay:
					Calendar calendar = Thread.CurrentThread.CurrentCulture.Calendar;
					int months = ((calendar.GetYear(DateTime.Today) - calendar.GetYear(this._StartDate)) * 12) + calendar.GetMonth(DateTime.Today) - calendar.GetMonth(this._StartDate);
					int day = DateTime.Now.Day;
					newBuildNumber = (months * 100) + day;
					break;

				case BuildNumberType.YearDayOfYear:
					DateTime d2 = DateTime.Now;
					newBuildNumber = int.Parse(string.Format("{0:00}{1:000}", (d2.Year%10), d2.DayOfYear));
					break;
			}

			return newBuildNumber;
		}

		/// <summary>
		/// Private helper that calculates the revision number.
		/// </summary>
		/// <returns>The new calculated revision number.</returns>
		private int CalculateRevisionNumber()
		{
			int newRevisionNumber = this._OriginalVersion.Revision;

			switch (this._RevisionNumberType)
			{
				case RevisionNumberType.Increment:
					newRevisionNumber++;
					break;

				case RevisionNumberType.Fixed:
					break;

				case RevisionNumberType.Automatic:
					TimeSpan difference = DateTime.Now.Subtract(DateTime.Today);
					newRevisionNumber = (int)(difference.TotalSeconds / 10);
					break;
			}

			return newRevisionNumber;
		}
	}

}
