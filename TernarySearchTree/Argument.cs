using System;

namespace TernarySearchTree
{
    internal static class Argument
    {
        internal static void IsNotNullAndNotEmpty(string argument, string argumentName)
        {
            IsNotNull(argument, argumentName);
            IsNotEmpty(argument, argumentName);
        }

        internal static void IsNotNull<T>(T argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        internal static void IsNotEmpty(string argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentException("Argument cannot be an empty string.", argumentName);
            }
        }

        internal static void IsWithinRange(bool expression, string argumentName)
        {
            if (expression == false)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }            
        }
    }
}
