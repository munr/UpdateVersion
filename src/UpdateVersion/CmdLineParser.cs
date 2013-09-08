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
using System.Collections;
using System.Threading;

namespace NArgs
{
	/// <summary>
	/// The abstract base class for exceptions that the command-line parser can throw.
	/// </summary>
	public abstract class OptionException : ApplicationException
	{
		/// <summary>
		/// Initializes a new OptionException with the given error message.
		/// </summary>
		/// <param name="message">The error message.</param>
		public OptionException(string message) :
			base(message)
		{
		}
	}

	/// <summary>
	/// Thrown when the command-line contains an option that is not
	/// recognised. 
	/// </summary>
	/// <remarks>The <c>Message</c> property contains
	/// an error string suitable for reporting the error to the user (in
	/// English).</remarks>
	public class UnknownOptionException : OptionException
	{
		private string _optionName;

		/// <summary>
		/// Initializes a new UnknownOptionException.
		/// </summary>
		/// <param name="optionName">The name of the unknown option. The name is used to
		/// generate an error message.</param>
		public UnknownOptionException(string optionName) :
			base("Unknown option '" + optionName + "'")
		{
			_optionName = optionName;
		}

		/// <summary>
		/// Gets the name of the unknown option.
		/// </summary>
		public string OptionName
		{
			get { return _optionName; }
		}
	}

	/// <summary>
	/// DuplicateOptionException is thrown when a duplicate option is added to a CmdLineParser 
	/// instance. 
	/// </summary>
	/// <remarks>Every option added to a CmdLineParser must have a unique long form. Every 
	/// option with a non-null or non-empty short form, must have a unique short form. 
	/// The <c>Message</c> property contains an error string suitable for reporting the error 
	/// to the user (in English).</remarks>
	public class DuplicateOptionException : OptionException
	{
		private string _optionName;

		/// <summary>
		/// Initializes a new DuplicateOptionException.
		/// </summary>
		/// <param name="optionName">The duplicate option name. The name is used to
		/// generate an error message.</param>
		public DuplicateOptionException(string optionName) : base("Option '" + optionName + "' already exists. Please provide a unique name for each option.")
		{
			_optionName = optionName;
		}

		/// <summary>
		/// Gets the duplicate option name.
		/// </summary>
		public string OptionName
		{
			get { return _optionName; }
		}
	}

	/// <summary>
	/// Thrown when an illegal or missing value is given by the user for
	/// an option that requires a value. 
	/// </summary>
	/// <remarks>The <c>Message</c> property contains
	/// an error string suitable for reporting the error to the user (in
	/// English).</remarks>
	public class IllegalOptionValueException : OptionException
	{
		/// <summary>
		/// Stores the option that caused the error.
		/// </summary>
		private Option _option;

		/// <summary>
		/// Stores the illegal value supplied by the user.
		/// </summary>
		private string _value;

		/// <summary>
		/// Initializes a new IllegalOptionValueException.
		/// </summary>
		/// <param name="option">The option with an invalid or missing value.</param>
		/// <param name="value">The invalid value supplied by the user.</param>
		public IllegalOptionValueException(Option option, string value) : base("Illegal value '" + value + "' for option --" + option.LongForm)
		{
			_option = option;
			_value = value;
		}

		/// <summary>
		/// Gets the option whose value was illegal.
		/// </summary>
		public Option Option
		{
			get { return _option; }
		}

		/// <summary>
		/// Gets the illegal value that the user provided.
		/// </summary>
		public string Value
		{
			get { return _value; }
		}
	}

	/// <summary>
	/// The abstract base class for all command-line options.
	/// </summary>
	public abstract class Option
	{
		/// <summary>
		/// Stores the short form for the option.
		/// </summary>
		private readonly string _shortForm;

		/// <summary>
		/// Stores the long form for the option.
		/// </summary>
		private readonly string _longForm;

		/// <summary>
		/// Stores a value indicating whether this option requires a value or not.
		/// </summary>
		private readonly bool _requiresValue;

		/// <summary>
		/// Stores the default value for this option.
		/// </summary>
		private readonly object _defaultValue = null;

		/// <summary>
		/// Stores the value for this option.
		/// </summary>
		private object _value = null;

		/// <summary>
		/// Stores the format provider used to parse values for this option.
		/// </summary>
		private readonly IFormatProvider _formatProvider;

		/// <summary>
		/// Initializes a new Option instance.
		/// </summary>
		/// <param name="shortForm">The optional short form for this command-line option.</param>
		/// <param name="longForm">The long form for this command-line option.</param>
		/// <param name="requiresValue">Indicates whether this option requires a value.</param>
		/// <param name="defaultValue">The default value for this option.</param>
		/// <param name="provider">The FormatProvider used to parse values for this option.</param>
		protected Option(string shortForm, string longForm,
		                 bool requiresValue, object defaultValue, IFormatProvider provider)
		{
			if (longForm == null)
				throw new ArgumentNullException("longForm", "The long form is not optional.");

			_shortForm = shortForm;
			_longForm = longForm;
			_requiresValue = requiresValue;
			_defaultValue = defaultValue;
			_formatProvider = provider;
		}

		/// <summary>
		/// Gets the short form for this option.
		/// </summary>
		public string ShortForm
		{
			get { return _shortForm; }
		}

		/// <summary>
		/// Gets the long form for this option.
		/// </summary>
		public string LongForm
		{
			get { return _longForm; }
		}

		/// <summary>
		/// Gets a value indicating whether this option requires a value.
		/// </summary>
		public bool RequiresValue
		{
			get { return _requiresValue; }
		}

		/// <summary>
		/// Gets the default value for this option. This is a generic implementation and derived 
		/// types should consider overriding it so users of the type won’t have to cast to the 
		/// appropriate type before using the default value.
		/// </summary>
		protected virtual object DefaultValue
		{
			get { return _defaultValue; }
		}

		/// <summary>
		/// Gets true if this option was missing from the command-line. Otherwise returns false.
		/// </summary>
		public bool IsMissing
		{
			get { return (null == _value); }
		}

		/// <summary>
		/// Gets the Format Provider used to parse values for this option.
		/// </summary>
		public IFormatProvider FormatProvider
		{
			get { return _formatProvider; }
		}

		/*
		/// <summary>
		/// Parses the given argument for this option.
		/// </summary>
		/// <param name="argument">The argument supplied for this</param>
		/// <returns></returns>
		public object GetValue( string argument )
		{
			
		}
		*/

		/// <summary>
		/// Sets the value for this option.
		/// </summary>
		/// <param name="argument">The value the user provided for this option.</param>
		/// <exception cref="NArgs.IllegalOptionValueException">The value is null but
		/// the option requires a value.</exception>
		/// <exception cref="NArgs.IllegalOptionValueException">The value is not valid
		/// for the current option type.</exception>
		public void SetValue(string argument)
		{
			if (_requiresValue)
			{
				if (null == argument)
					throw new IllegalOptionValueException(this, argument);

				_value = ParseValue(argument);
			}
			else
			{
				_value = true;
			}
		}

		/// <summary>
		/// Gets the value for this option.
		/// </summary>
		public object Value
		{
			get { return _value; }
		}

		/// <summary>
		/// When overridden by a derived class ParseValue extracts and converts an option value 
		/// passed on the command-line.
		/// </summary>
		/// <param name="argument">The option.</param>
		/// <returns>The option value.</returns>
		/// <exception cref="NArgs.IllegalOptionValueException">The option has
		/// an illegal or missing value.</exception>
		protected virtual object ParseValue(string argument)
		{
			return null;
		}
	}

	/// <summary>
	/// Represents a simple switch option. Switch options do not have values. The option’s 
	/// presence or absence is all that is important.
	/// </summary>
	public class BooleanOption : Option
	{
		public BooleanOption(string shortForm, string longForm) :
			base(shortForm, longForm, false, false, null)
		{
		}

		public BooleanOption(string shortForm, string longForm, bool defaultValue) :
			base(shortForm, longForm, false, defaultValue, null)
		{
		}

		/// <summary>
		/// Gets the default value for this option. The default value is returned by the Value 
		/// property when IsMissing is true.
		/// </summary>
		public new bool DefaultValue
		{
			get { return (bool) base.DefaultValue; }
		}

		/// <summary>
		/// Gets the value for this option. Returns the DefaultValue if IsMissing equals true.
		/// </summary>
		public new bool Value
		{
			get
			{
				if (null != base.Value)
					return (bool) base.Value;
				else
					return DefaultValue;
			}
		}
	}

	/// <summary>
	/// Represents an option which requires an integer value.
	/// </summary>
	public class IntegerOption : Option
	{
		/// <summary>
		/// Initializes a new IntegerOption.The default value will be equal to 0 and the 
		/// FormatProvider will equal 
		/// <see cref="Thread.CurrentThread.CurrentCulture"/>.
		/// </summary>
		/// <param name="shortForm">The optional short form for this option.</param>
		/// <param name="longForm">The long form for this option.</param>
		public IntegerOption(string shortForm, string longForm) :
			base(shortForm, longForm, true, 0, Thread.CurrentThread.CurrentCulture)
		{
		}

		/// <summary>
		/// Initializes a new IntegerOption. Uses the specified default value.
		/// </summary>
		/// <param name="shortForm">The optional short form for this option.</param>
		/// <param name="longForm">The long form for this option.</param>
		/// <param name="defaultValue">The default value returned if the option is missing.</param>
		/// <param name="provider">The IFormatProvider to use when parsing values for this option.</param>
		public IntegerOption(string shortForm, string longForm, int defaultValue, IFormatProvider provider) :
			base(shortForm, longForm, true, defaultValue, provider)
		{
		}

		protected override object ParseValue(string argument)
		{
			int result;

			try
			{
				result = int.Parse(argument, FormatProvider);
				return result;
			}
			catch
			{
				throw new IllegalOptionValueException(this, argument);
			}
		}

		/// <summary>
		/// Gets the default value for this option. The default value is returned by the Value 
		/// property when IsMissing is true.
		/// </summary>
		public new int DefaultValue
		{
			get { return (int) base.DefaultValue; }
		}

		/// <summary>
		/// Gets the value for this option. Returns the DefaultValue if IsMissing equals true.
		/// </summary>
		public new int Value
		{
			get
			{
				if (null != base.Value)
					return (int) base.Value;
				else
					return DefaultValue;
			}
		}
	}

	/// <summary>
	/// Represents an option which requires a floating-point value.
	/// </summary>
	public class DoubleOption : Option
	{
		/// <summary>
		/// Initializes a new DoubleOption. The default value will be equal to 
		/// 0.0 and the FormatProvider will equal 
		/// <see cref="Thread.CurrentThread.CurrentCulture"/>.
		/// </summary>
		/// <param name="shortForm">The optional short form for this option.</param>
		/// <param name="longForm">The long form for this option.</param>
		public DoubleOption(string shortForm, string longForm) :
			base(shortForm, longForm, true, 0.0, Thread.CurrentThread.CurrentCulture)
		{
		}

		/// <summary>
		/// Initializes a new DoubleOption using the default value provided.
		/// </summary>
		/// <param name="shortForm">The optional short form for this option.</param>
		/// <param name="longForm">The long form for this option.</param>
		/// <param name="defaultValue">The Default Value returned when the user does not provide a value
		/// for this option.</param>
		/// <param name="provider">The IFormatProvider to use when parsing values for this option.</param>
		public DoubleOption(string shortForm, string longForm, double defaultValue, IFormatProvider provider) :
			base(shortForm, longForm, true, defaultValue, provider)
		{
		}

		protected override object ParseValue(string argument)
		{
			double result;

			try
			{
				result = double.Parse(argument, FormatProvider);
				return result;
			}
			catch
			{
				throw new IllegalOptionValueException(this, argument);
			}
		}

		/// <summary>
		/// Gets the default value for this option. The default value is returned by the Value 
		/// property when IsMissing is true.
		/// </summary>
		public new double DefaultValue
		{
			get { return (double) base.DefaultValue; }
		}

		/// <summary>
		/// Gets the value for this option. Returns the DefaultValue if IsMissing equals true.
		/// </summary>
		public new double Value
		{
			get
			{
				if (null != base.Value)
					return (double) base.Value;
				else
					return DefaultValue;
			}
		}
	}

	/// <summary>
	/// Represents an option which requires a date value.
	/// </summary>
	public class DateOption : Option
	{
		/// <summary>
		/// Initializes a new DateOption. The default value will be equal to 
		/// <see cref="System.DateTime.MinValue"/> and the FormatProvider will equal 
		/// <see cref="Thread.CurrentThread.CurrentCulture"/>.
		/// </summary>
		/// <param name="shortForm">The optional short form for this option.</param>
		/// <param name="longForm">The long form for this option.</param>
		public DateOption(string shortForm, string longForm) :
			base(shortForm, longForm, true, DateTime.MinValue, Thread.CurrentThread.CurrentCulture)
		{
		}

		/// <summary>
		/// Initializes a new DateOption using the specified DefaultValue and IFormatProvider.
		/// </summary>
		/// <param name="shortForm">The optional short form for this option.</param>
		/// <param name="longForm">The long form for this option.</param>
		/// <param name="defaultValue">The Default Value returned when the user does not provide a value
		/// for this option.</param>
		/// <param name="provider">The IFormatProvider to use when parsing values for this option.</param>
		public DateOption(string shortForm, string longForm, DateTime defaultValue, IFormatProvider provider) :
			base(shortForm, longForm, true, defaultValue, provider)
		{
		}

		protected override object ParseValue(string argument)
		{
			DateTime result;

			try
			{
				result = DateTime.Parse(argument, FormatProvider);
				return result;
			}
			catch
			{
				throw new IllegalOptionValueException(this, argument);
			}
		}

		/// <summary>
		/// Gets the default value for this option. The default value is returned by the Value 
		/// property when IsMissing is true.
		/// </summary>
		public new DateTime DefaultValue
		{
			get { return (DateTime) base.DefaultValue; }
		}

		/// <summary>
		/// Gets the value for this option. Returns the DefaultValue if IsMissing equals true.
		/// </summary>
		public new DateTime Value
		{
			get
			{
				if (null != base.Value)
					return (DateTime) base.Value;
				else
					return DefaultValue;
			}
		}
	}

	/// <summary>
	/// Represents an option which requires a string value.
	/// </summary>
	public class StringOption : Option
	{
		/// <summary>
		/// Initializes a new StringOption using <see cref="System.String.Empty"/> for 
		/// the default value.
		/// </summary>
		/// <param name="shortForm">The optional short form for this option.</param>
		/// <param name="longForm">The long form for this option.</param>
		public StringOption(string shortForm, string longForm) :
			base(shortForm, longForm, true, string.Empty, null)
		{
		}

		/// <summary>
		/// Initializes a new StringOption using the default value provided.
		/// </summary>
		/// <param name="shortForm">The optional short form for this option.</param>
		/// <param name="longForm">The long form for this option.</param>
		/// <param name="defaultValue">The Default Value returned when the user does not provide a value
		/// for this option.</param>
		public StringOption(string shortForm, string longForm, string defaultValue) :
			base(shortForm, longForm, true, defaultValue, null)
		{
		}

		protected override object ParseValue(string argument)
		{
			return argument;
		}

		/// <summary>
		/// Gets the default value for this option. The default value is returned by the Value 
		/// property when IsMissing is true.
		/// </summary>
		public new string DefaultValue
		{
			get { return base.DefaultValue as string; }
		}

		/// <summary>
		/// Gets the value for this option. Returns the DefaultValue if IsMissing equals true.
		/// </summary>
		public new string Value
		{
			get
			{
				if (null != base.Value)
					return base.Value as string;
				else
					return DefaultValue;
			}
		}
	}

	public class CmdLineParser
	{
		private string[] _remainingArgs = null;
		private Hashtable _options = new Hashtable(10);
		//private Hashtable _values = new Hashtable(10);

		/// <summary>
		/// Add the given Option to the list of accepted options.
		/// </summary>
		/// <param name="option">The option to add.</param>
		/// <returns>Returns the option after adding it.</returns>
		/// <exception cref="NArgs.DuplicateOptionException">The short form
		/// is not unique.</exception>
		/// <exception cref="NArgs.DuplicateOptionException">The long form
		/// is not unique.</exception>
		/// <remarks>Every option added to a CmdLineParser must have a unique long form. Every 
		/// option with a non-null or non-empty short form, must have a unique short form. 
		/// The <c>Message</c> property contains an error string suitable for reporting the error 
		/// to the user (in English).</remarks>
		public Option AddOption(Option option)
		{
			string shortForm = option.ShortForm;
			string fullShortForm = "-" + shortForm;
			string longForm = option.LongForm;
			string fullLongForm = "--" + longForm;

			#region Validate Parameters

			if (null != shortForm && string.Empty != shortForm)
				if (_options.ContainsKey(fullShortForm))
					throw new DuplicateOptionException(shortForm);

			if (_options.ContainsKey(fullLongForm))
				throw new DuplicateOptionException(longForm);

			#endregion

			if (null != shortForm && string.Empty != shortForm)
				_options.Add(fullShortForm, option);

			_options.Add(fullLongForm, option);

			return option;
		}

		/// <summary>
		/// Creates a new StringOption and adds it to the list of accepted options.
		/// </summary>
		/// <param name="shortForm">The optional short form for the option.</param>
		/// <param name="longForm">The long form for the option.</param>
		/// <returns>The newly created Option.</returns>
		/// <exception cref="System.ArgumentNullException">The long form is null.</exception>
		public StringOption AddStringOption(string shortForm, string longForm)
		{
			if (null == longForm)
				throw new ArgumentNullException("longForm", "The long form is not optional.");

			StringOption option = new StringOption(shortForm, longForm);
			AddOption((Option) option);
			return option;
		}

		/// <summary>
		/// Creates a new IntegerOption and adds it to the list of accepted options.
		/// </summary>
		/// <param name="shortForm">The optional short form for the option.</param>
		/// <param name="longForm">The long form for the option.</param>
		/// <returns>The newly created Option.</returns>
		/// <exception cref="System.ArgumentNullException">The long form is null.</exception>
		public IntegerOption AddIntegerOption(string shortForm, string longForm)
		{
			if (null == longForm)
				throw new ArgumentNullException("longForm", "The long form is not optional.");

			IntegerOption option = new IntegerOption(shortForm, longForm);
			AddOption((Option) option);
			return option;
		}

		/// <summary>
		/// Creates a new DoubleOption and adds it to the list of accepted options.
		/// </summary>
		/// <param name="shortForm">The optional short form for the option.</param>
		/// <param name="longForm">The long form for the option.</param>
		/// <returns>The newly created Option.</returns>
		/// <exception cref="System.ArgumentNullException">The long form is null.</exception>
		public DoubleOption AddDoubleOption(string shortForm, string longForm)
		{
			if (null == longForm)
				throw new ArgumentNullException("longForm", "The long form is not optional.");

			DoubleOption option = new DoubleOption(shortForm, longForm);
			AddOption((Option) option);
			return option;
		}

		/// <summary>
		/// Creates a new DateOption and adds it to the list of accepted options.
		/// </summary>
		/// <param name="shortForm">The optional short form for the option.</param>
		/// <param name="longForm">The long form for the option.</param>
		/// <returns>The newly created Option.</returns>
		/// <exception cref="System.ArgumentNullException">The long form is null.</exception>
		public DateOption AddDateOption(string shortForm, string longForm)
		{
			if (null == longForm)
				throw new ArgumentNullException("longForm", "The long form is not optional.");

			DateOption option = new DateOption(shortForm, longForm);
			AddOption((Option) option);
			return option;
		}

		/// <summary>
		/// Creates a new BooleanOption and adds it to the list of accepted options.
		/// </summary>
		/// <param name="shortForm">The optional short form for the option.</param>
		/// <param name="longForm">The long form for the option.</param>
		/// <returns>The newly created Option.</returns>
		/// <exception cref="System.ArgumentNullException">The long form is null.</exception>
		public BooleanOption AddBooleanOption(string shortForm, string longForm)
		{
			if (null == longForm)
				throw new ArgumentNullException("longForm", "The long form is not optional.");

			BooleanOption option = new BooleanOption(shortForm, longForm);
			AddOption((Option) option);
			return option;
		}

		/// <summary>
		/// Gets the non-option arguments that the user provided.
		/// </summary>
		/// <returns>An array of the remain user supplied arguments.</returns>
		public string[] GetRemainingArgs()
		{
			return _remainingArgs;
		}

		/// <summary>
		/// Extracts the options and non-option arguments from the given
		/// list of command-line arguments. Stops parsing at the first -- or non-option.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		/// <exception cref="NArgs.IllegalOptionValueException">An option that
		/// requires a value has a </exception>
		/// <exception cref="NArgs.UnknownOptionException">An unknown option is 
		/// encountered.</exception>
		public void Parse(string[] args)
		{
			ArrayList otherArgs = new ArrayList();

			int position = 0;

			while (position < args.Length)
			{
				string currentArgument = args[position];
				if (currentArgument.StartsWith("-"))
				{
					if (currentArgument == "--")
					{
						// end of options
						position += 1;
						break;
					}

					string valueArgument = null;
					if (currentArgument.StartsWith("--"))
					{
						// handle --arg=value
						int equalsPosition = currentArgument.IndexOf("=");
						if (equalsPosition != -1)
						{
							valueArgument = currentArgument.Substring(equalsPosition + 1);
							currentArgument = currentArgument.Substring(0, equalsPosition);
						}
					}

					Option option = _options[currentArgument] as Option;
					if (option == null)
					{
						throw new UnknownOptionException(currentArgument);
					}

					//object value = null;
					if (option.RequiresValue)
					{
						if (valueArgument == null || valueArgument == string.Empty)
						{
							position += 1;
							valueArgument = null;
							if (position < args.Length)
							{
								valueArgument = args[position];
							}
						}
						option.SetValue(valueArgument);
						//value = option.Value;
					}
					else
					{
						option.SetValue(null);
						//value = option.Value;
					}

					//this._values.Add(option.LongForm, value);
					position += 1;
				} // currentArgument.StartsWith("-")
				else
				{
					// We encountered a non-option argument. Quit parsing.
					break;
				}
			}

			for (; position < args.Length; ++position)
			{
				otherArgs.Add(args[position]);
			}

			int count = otherArgs.Count;

			_remainingArgs = new string[count];

			for (int i = 0; i < count; i++)
			{
				_remainingArgs[i] = (string) otherArgs[i];
			}
		}
	}
}