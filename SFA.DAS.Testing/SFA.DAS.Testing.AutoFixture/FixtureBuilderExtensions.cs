using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;

namespace SFA.DAS.Testing.AutoFixture;
public static class FixtureBuilderExtensions
{
    /// <summary>
    /// Builds a property from the given collection of "values". 
    /// 
    /// When creating many if the length is greater than count of "values" then "values" are recycled
    /// </summary>
    public static IPostprocessComposer<T> WithValues<T, TProperty>(
        this IPostprocessComposer<T> composer,
        Expression<Func<T, TProperty>> property,
        params TProperty[] values)
    {
        var queue = new Queue<TProperty>(values);

        return composer.With(property, () =>
        {
            if (queue.Count == 0) queue = new Queue<TProperty>(values);
            return queue.Dequeue();
        });
    }

    /// <summary>
    /// Creates as many objects as many of given "values".
    /// 
    /// For each object created, builds the "property" from the given values
    /// </summary>
    public static IEnumerable<T> CreateMany<T, TProperty>(
        this IPostprocessComposer<T> composer,
        Expression<Func<T, TProperty>> property,
        params TProperty[] values)
    {
        var queue = new Queue<TProperty>(values);

        ISpecimenBuilder builder = composer.With(property, () => queue.Dequeue());

        return builder.CreateMany<T>(values.Length);
    }
}
