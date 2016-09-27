using System;

namespace SFA.DAS.ApiTokens.Console
{
    /// <summary>
    /// Extension methods for parsing <see cref="Arguments"/> values.
    /// </summary>
    internal static class ArgumentsExtensions
    {
        /// <summary>
        /// Returns a value which matches the key specified. 
        /// Multiple keys can be specified (they are treated as aliases).
        /// If more than one matching value is found, the first one is returned.
        /// If no matches are found, the specified default is returned.
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="arguments"><see cref="Arguments"/> instance containing argument key/value pairs.</param>
        /// <param name="defaultValue">Default value to return if no keyed values are found.</param>
        /// <param name="keys">One or more keys to look for in the arguments.</param>
        /// <returns>Value associated with one of the keys or the specified default.</returns>
        internal static T GetValueOrDefault<T>(this Arguments arguments, T defaultValue, params string[] keys) where T : class
        {
            foreach (var key in keys)
            {
                var lowerKey = key.ToUpperInvariant();
                if (arguments.ContainsKey(lowerKey))
                    return arguments[lowerKey] as T;
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns a value which matches the key specified. 
        /// Multiple keys can be specified (they are treated as aliases).
        /// If more than one matching value is found, the first one is returned.
        /// If no matches are found, the specified default is returned.
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="arguments"><see cref="Arguments"/> instance containing argument key/value pairs.</param>
        /// <param name="defaultValue">Default value to return if no keyed values are found.</param>
        /// <param name="keys">One or more keys to look for in the arguments.</param>
        /// <returns>Value associated with one of the keys or the specified default.</returns>
        internal static T GetEnumOrDefault<T>(this Arguments arguments, T defaultValue, params string[] keys)
        {
            foreach (var key in keys)
            {
                var lowerKey = key.ToUpperInvariant();
                if (arguments.ContainsKey(lowerKey))
                    return (T)Enum.Parse(typeof(T), arguments[lowerKey], true);
            }

            return defaultValue;
        }

        /// <summary>
        /// Verifies that the value of the argument is not "true" which would indicate that a command line value
        /// has not been set (i.e. it has been specified as a switch).
        /// Multiple keys can be specified (they are treated as aliases).
        /// </summary>
        /// <param name="arguments"><see cref="Arguments"/> instance containing argument key/value pairs.</param>
        /// <param name="keys">One or more keys that may contain the value to be checked.</param>
        internal static void EnsureArgumentIsNotASwitch(this Arguments arguments, params string[] keys)
        {
            foreach (var key in keys)
            {
                var lowerKey = key.ToUpperInvariant();
                if (arguments.ContainsKey(lowerKey))
                {
                    if (arguments[lowerKey] == "true")
                        throw new ArgumentException(string.Format("Argument {0} should have a value specified", key));
                }
            }
        }

        /// <summary>
        /// Verifies that the value of the argument is "true" which indicates it is a command line switch.
        /// Multiple keys can be specified (they are treated as aliases).
        /// </summary>
        /// <param name="arguments"><see cref="Arguments"/> instance containing argument key/value pairs.</param>
        /// <param name="keys">One or more keys that may contain the value to be checked.</param>
        internal static void EnsureArgumentIsASwitch(this Arguments arguments, params string[] keys)
        {
            foreach (var key in keys)
            {
                var lowerKey = key.ToUpperInvariant();
                if (arguments.ContainsKey(lowerKey))
                {
                    if (arguments[lowerKey] != "true")
                        throw new ArgumentException(string.Format("Argument {0} should not have a value specified", key));
                }
            }
        }

    }
}