﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Serilog.Events;

namespace Serilog.Expressions
{
    /// <summary>
    /// Looks up the implementations of functions that appear in expressions.
    /// </summary>
    public abstract class NameResolver
    {
        /// <summary>
        /// Match a function name to a method that implements it.
        /// </summary>
        /// <param name="name">The function name as it appears in the expression source. Names are not case-sensitive.</param>
        /// <param name="implementation">A <see cref="MethodInfo"/> implementing the function.</param>
        /// <returns><c>True</c> if the name could be resolved; otherwise, <c>false</c>.</returns>
        /// <remarks>The method implementing a function should be <c>static</c>, return <see cref="LogEventPropertyValue"/>,
        /// and accept parameters of type <see cref="LogEventPropertyValue"/>. If the <c>ci</c> modifier is supported,
        /// a <see cref="StringComparison"/> should be in the first argument position.</remarks>
        public abstract bool TryResolveFunctionName(string name, [MaybeNullWhen(false)] out MethodInfo implementation);
    }
}