﻿namespace StringAlgorithms.SuffixStructures;

/// <summary>
/// Extension methods for all <see cref="ISuffixStructureEdge{TEdge, TNode, TBuilder}"/> edge concretions.
/// </summary>
public static class SuffixStructureEdgeExtensions
{
    /// <summary>
    /// Whether the first edge is in adjacency order w.r.t. the second edge.
    /// </summary>
    /// <param name="first">The edge to be compared for adjacency.</param>
    /// <param name="second">The edge to compare the first edge against.</param>
    /// <param name="order">The adjacency relationship order to use for comparison.</param>
    /// <returns>True if the specified adjacency relationship is respected.</returns>
    public static bool IsAdjacentTo<TEdge, TNode, TBuilder>(
        this ISuffixStructureEdge<TEdge, TNode, TBuilder> first,
        TEdge second,
        AdjacencyOrders order = AdjacencyOrders.BeforeOrAfter)
        where TEdge : ISuffixStructureEdge<TEdge, TNode, TBuilder>
        where TNode : ISuffixStructureNode<TEdge, TNode, TBuilder>
        where TBuilder : ISuffixStructureBuilder<TEdge, TNode, TBuilder> =>
        (order.HasFlag(AdjacencyOrders.Before) && first.Start + first.Length == second.Start) ||
        (order.HasFlag(AdjacencyOrders.After) && second.Start + second.Length == first.Start);
}
