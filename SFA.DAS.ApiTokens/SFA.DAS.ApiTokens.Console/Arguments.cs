using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFA.DAS.ApiTokens.Console
{
    /// <summary>
    /// Command line argument parsing class.
    /// See http://www.codeproject.com/KB/recipes/command_line.aspx
    /// also http://jake.ginnivan.net/2009/07/c-argument-parser/.
    /// Made a couple of changes - nothing major.
    /// </summary>
    public class Arguments : Dictionary<string, string>
    {
        /// <summary>
        /// Parses the specified arguments and returns a Dictionary
        /// derived wrapper
        /// </summary>
        /// <param name="args">Arguments to parse</param>
        /// <returns>Parsed arguments</returns>
        internal static Arguments Parse(string[] args)
        {
            return new Arguments(args);
        }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Arguments"/> class.
        /// Valid parameters forms:
        ///   {-,/,--}param{ ,=,:}((",')value(",'))
        /// 
        /// Examples:
        ///   -param1 value1 
        ///   --param2 
        ///   /param3:"Test-:-work" 
        ///   /param4=happy 
        ///   -param5 '--=nice=--'
        /// </summary>
        /// <param name="Args">The command line args to parse</param>
        private Arguments(string[] Args)
        {
            if (Args != null)
            {
                var splitter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                string parameter = null;

                foreach (var txt in Args)
                {
                    // Look for new parameters (-,/ or --) and a
                    // possible enclosed value (=,:)
                    string[] parts = splitter.Split(txt, 3);

                    switch (parts.Length)
                    {
                        // Found a value (for the last parameter 
                        // found (space separator))
                        case 1:
                            if (parameter != null)
                            {
                                if (!base.ContainsKey(parameter))
                                {
                                    parts[0] = remover.Replace(parts[0], "$1");
                                    base.Add(parameter, parts[0]);
                                }
                                parameter = null;
                            }
                            // else Error: no parameter waiting for a value (skipped)
                            break;

                        // Found just a parameter
                        case 2:
                            // The last parameter is still waiting. 
                            // With no value, set it to true.
                            if (parameter != null)
                            {
                                if (!base.ContainsKey(parameter))
                                {
                                    base.Add(parameter, "true");
                                }
                            }
                            parameter = parts[1].ToUpperInvariant();
                            break;

                        // Parameter with enclosed value
                        case 3:
                            // The last parameter is still waiting. 
                            // With no value, set it to true.
                            if (parameter != null)
                            {
                                if (!base.ContainsKey(parameter))
                                    base.Add(parameter, "true");
                            }

                            parameter = parts[1].ToUpperInvariant();

                            // Remove possible enclosing characters (",')
                            if (!base.ContainsKey(parameter))
                            {
                                parts[2] = remover.Replace(parts[2], "$1");
                                base.Add(parameter, parts[2]);
                            }
                            parameter = null;
                            break;
                    }
                }

                // In case a parameter is still waiting
                if (parameter != null)
                {
                    if (!base.ContainsKey(parameter))
                        base.Add(parameter, "true");
                }
            }
        }

        #endregion
    }
}
