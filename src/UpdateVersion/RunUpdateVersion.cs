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
using System.IO;
using System.Text;

namespace MattGriffith.UpdateVersion
{
	internal class RunUpdateVersion
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static int Main(string[] args)
		{
			Options options = null;
			string input = null;
			VersionUpdater updater = null;

			///////////////////////////////////////////////////////////////////
			// Parse the command line
			try
			{
				options = new Options(args);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("Error parsing command line options.");
				Console.Error.WriteLine(e);
				Console.Error.WriteLine();
				Console.Error.WriteLine(Options.Usage);
				return 1;
			}

			///////////////////////////////////////////////////////////////////
			// Get the input
			try
			{
				input = GetInput(options);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("Error reading input.");
				Console.Error.WriteLine(e);
				return 2;
			}

			///////////////////////////////////////////////////////////////////
			// Update the version number in the input
			try
			{
				updater = new VersionUpdater(input, options);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("Error updating version.");
				Console.Error.WriteLine(e);
				return 3;
			}

			///////////////////////////////////////////////////////////////////
			// Write the output
			try
			{
				WriteOutput(updater.Output, options);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("Error writing output.");
				Console.Error.WriteLine(e);
				return 4;
			}

			// Return normally
			return 0;
		}

		/// <summary>
		/// Private helper method that gets the input string from the appropriate source.
		/// </summary>
		/// <param name="options">The command line options.</param>
		/// <returns>
		/// Returns the input string.
		/// </returns>
		private static string GetInput(Options options)
		{
			string input = null;

			if (null == options.InputFilename)
			{
				// The input file name was not specified on the command line wo we will
				// get the input from the standard input stream.
				input = Console.In.ReadToEnd();
			}
			else
			{
				// An input file was specified on the command line. 
				input = ReadFile(options.InputFilename);
			}

			return input;
		}

		/// <summary>
		/// Private helper that reads the input string from a file.
		/// </summary>
		/// <param name="filename">The name of the file to read.</param>
		/// <returns>The string representing the data stored in the input file.</returns>
		private static string ReadFile(string filename)
		{
			string result = null;

			if (!File.Exists(filename))
				throw new ArgumentException("File does not exist.", "filename");

			using (FileStream stream = File.OpenRead(filename))
			{
				StreamReader reader = new StreamReader(stream, Encoding.Default, true);
				result = reader.ReadToEnd();
			}

//			using(StreamReader reader = File.OpenText(filename))
//			{
//				result = reader.ReadToEnd();
//			}

			return result;
		}

		/// <summary>
		/// Writes the output string to the appropriate target.
		/// </summary>
		/// <param name="output">
		/// The output string.
		/// </param>
		/// <param name="options">
		/// The command line options.
		/// </param>
		private static void WriteOutput(string output, Options options)
		{
			if (null == output)
				throw new ArgumentNullException("output", "Output is null.");

			if (null == options.OutputFilename)
			{
				// The output file name was not specified on the command line wo we will
				// write the output to the standard output stream.
				Console.Out.Write(output);
			}
			else
			{
				// An output file was specified on the command line. So we will write the
				// output to the specified file.
//				using(StreamWriter writer = 
//						  File.CreateText(options.OutputFilename))
//				{
//					writer.Write(output);
//				}
				using (StreamWriter writer = new StreamWriter(options.OutputFilename, false, Encoding.Default))
				{
					writer.Write(output);
				}
			}
		}
	}
}