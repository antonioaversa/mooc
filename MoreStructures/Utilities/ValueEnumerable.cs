﻿using System.Collections;

namespace MoreStructures.Utilities;

/// <summary>
/// A <see cref="IValueEnumerable{T}"/> implementation, wrapping a generic <see cref="IEnumerable{T}"/>.
/// </summary>
/// <typeparam name="T">The type of objects of the wrapped enumerable.</typeparam>
/// <remarks>
/// Useful to preserve equality by value in records and other value structures which contain enumerable objects.
/// </remarks>
public class ValueEnumerable<T> : IValueEnumerable<T>
{
    private IEnumerable<T> Enumerable { get; }

    /// <summary>
    /// Builds a <see cref="ValueEnumerable{T}"/> around the provided <paramref name="enumerable"/>.
    /// </summary>
    /// <param name="enumerable">The enumerable to wrap.</param>
    public ValueEnumerable(IEnumerable<T> enumerable)
    {
        Enumerable = enumerable;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Enumerable.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => (Enumerable as IEnumerable).GetEnumerator();

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// In the specific case, equality is based on the 
    /// <see cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/> of the wrapped
    /// <see cref="IEnumerable{T}"/> objects.
    /// </remarks>
    public override bool Equals(object? obj) => 
        obj is ValueEnumerable<T> other && Enumerable.SequenceEqual(other.Enumerable);

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// In the specific case, the hash is calculated as an aggregation of the hash codes of the elements of the wrapped
    /// <see cref="Enumerable"/> object.
    /// </remarks>
    public override int GetHashCode() =>
        Enumerable.Aggregate(0.GetHashCode(), (acc, item) => acc ^ (item?.GetHashCode() ?? 0));

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// In the specific case, the string calculation is delegated to the wrapped <see cref="IEnumerable{T}"/> object.
    /// </remarks>
    public override string ToString() =>
        Enumerable.ToString()!;
}