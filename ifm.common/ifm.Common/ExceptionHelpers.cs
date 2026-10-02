namespace ifm.Common;

using System;

/// <summary>
/// Provides helper methods for validating method arguments and throwing appropriate exceptions when arguments are null,
/// empty, out of range, or otherwise invalid.
/// </summary>
/// <remarks>Use the methods in this class to enforce argument validation in public APIs and internal methods.
/// These helpers standardize exception throwing for common argument validation scenarios, improving code clarity and
/// consistency. All methods throw exceptions immediately if validation fails, and do not return a value.</remarks>
public static class ExceptionHelpers
{
    /// <summary>
    /// Throws an exception if the specified argument is null.
    /// </summary>
    /// <param name="argument">The object to validate for null. If this value is null, an exception is thrown.</param>
    /// <param name="name">The name of the parameter to include in the exception message. This should correspond to the name of the
    /// argument being checked.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="argument"/> is null.</exception>
    public static void ThrowIfNull(object argument, string name)
    {
        if (argument == null) throw new ArgumentNullException(name);
    }

    /// <summary>
    /// Throws an exception if the specified string argument is null or an empty string.
    /// </summary>
    /// <remarks>Use this method to enforce that a string parameter is neither null nor empty before
    /// proceeding with further logic. This helps ensure that required string inputs are provided and avoids null or
    /// empty value errors later in execution.</remarks>
    /// <param name="argument">The string argument to validate. If null or empty, an exception is thrown.</param>
    /// <param name="name">The name of the parameter to include in the exception message. This should correspond to the name of the
    /// argument being validated.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="argument"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="argument"/> is an empty string.</exception>
    public static void ThrowIfNullOrEmpty(string argument, string name)
    {
        switch (argument)
        {
            case null:
                throw new ArgumentNullException(name);
            case "":
                throw new ArgumentException(name);
        }
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if the specified condition evaluates to true.
    /// </summary>
    /// <param name="condition">A delegate that returns <see langword="true"/> if the argument is out of the valid range; otherwise, <see
    /// langword="false"/>.</param>
    /// <param name="name">The name of the parameter to include in the exception if the condition is met.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="condition"/> evaluates to <see langword="true"/>, indicating that the argument is
    /// out of range.</exception>
    public static void ThrowIfOutOfRange(Func<bool> condition, string name)
    {
        if (condition()) throw new ArgumentOutOfRangeException(name);
    }

    /// <summary>
    /// Throws an ArgumentException if the specified condition evaluates to true.
    /// </summary>
    /// <param name="condition">A delegate that returns <see langword="true"/> if the argument is out of the valid range; otherwise, <see
    /// langword="false"/>.</param>
    /// <param name="name">The name of the parameter to include in the exception if the condition is met.</param>
    /// <exception cref="ArgumentException">Always thrown to indicate that the argument specified by <paramref name="name"/> is invalid.</exception>
    public static void ThrowIfInvalid(Func<bool> condition, string name)
    {
        if (condition()) throw new ArgumentException(name);
    }
}