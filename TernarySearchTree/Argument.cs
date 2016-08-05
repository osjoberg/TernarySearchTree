namespace TernarySearchTree
{
    using System;

    internal static class Argument
    {
        internal static void IsNotNullAndNotEmpty(string argument, string argumentName)
        {
            IsNotNull(argument, argumentName);
            IsNotEmpty(argument, argumentName);
        }

        internal static void IsNotNull<T>(T argument, string arguentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(arguentName);
            }
        }

        internal static void IsNotEmpty(string argument, string arguentName)
        {
            if (argument == null)
            {
                throw new ArgumentException("Argument cannot be an empty string.", arguentName);
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
