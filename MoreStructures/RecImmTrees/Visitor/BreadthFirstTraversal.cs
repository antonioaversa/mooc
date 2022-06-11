﻿namespace MoreStructures.RecImmTrees.Visitor;

/// <inheritdoc cref="IVisitStrategy{TNode, TVisitContext}" path="//*[not(self::summary or self::remarks)]"/>
/// <summary>
/// Base class for all BFT strategies, i.e. all traversing strategies which visit all the nodes at the current depth, 
/// along any path of the tree, before going deeper or shallower, exploring nodes with higher or lower depth.
/// </summary>
/// <example>
///     Given the following tree structure:
///     <code>
///     0
///     |- 0 -> 1
///     |       |- 1 -> 2
///     |       |- 2 -> 3
///     |       |       |- 3 -> 4
///     |       |- 4 -> 5
///     |- 5 -> 6
///     |- 6 -> 7
///             |- 7 -> 8
///                     |- 8 -> 9
///                     |- 9 -> 10
///     </code>
///     A BFT visit strategy "parent first" would visit nodes and edges in either of the following ways, depending on
///     how children are sorted (lower-id edge first, lower-id edge last, median-id edge first, ...):
///     <br/>
///     - { (null, 0), (0, 1), (5, 6), (6, 7), (1, 2), (2, 3), (4, 5), (7, 8), (3, 4), (8, 9), (9, 10) }
///     <br/>
///     - { (null, 0), (6, 7), (5, 6), (0, 1), (7, 8), (4, 5), (2, 3), (1, 2), (9, 10), (8, 9), (3, 4) }
///     <br/>
///     - { (null, 0), (5, 6), (6, 7), (0, 1), (7, 8), (2, 3), (4, 5), (1, 2), (9, 10), (8, 9), (3, 4) }
///     <br/>
///     - ...
///     <br/>
///     <br/>
///     A BFT visit strategy "children first" would visit nodes and edges in either of the following ways, depending on
///     how children are sorted:
///     <br/>
///     - { (3, 4), (8, 9), (9, 10), (1, 2), (2, 3), (4, 5), (7, 8), (0, 1), (5, 6), (6, 7), (null, 0) }
///     <br/>
///     - { (9, 10), (8, 9), (3, 4), (7, 8), (4, 5), (2, 3), (1, 2), (6, 7), (5, 6), (0, 1), (null, 0) }
///     <br/>
///     - ...
/// </example>
public abstract class BreadthFirstTraversal<TEdge, TNode>
    : IVisitStrategy<TNode, TreeTraversalContext<TEdge, TNode>>
    where TEdge : IRecImmDictIndexedTreeEdge<TEdge, TNode>
    where TNode : IRecImmDictIndexedTreeNode<TEdge, TNode>
{
    /// <summary>
    /// The traversal order between parent and its children, to be applied when visiting the tree. By default 
    /// <see cref="TreeTraversalOrder.ParentFirst"/> is applied, meaning that the parent node is visited before
    /// its children.
    /// </summary>
    public TreeTraversalOrder TraversalOrder { get; init; } = TreeTraversalOrder.ParentFirst;

    /// <summary>
    /// The order of visit of the children. By default <see cref="IRecImmDictIndexedTreeNode{TEdge, TNode}.Children"/>
    /// is returned as is, and no specific order is imposed to the sequence of (edge, node) couples, during the visit.
    /// </summary>
    /// <remarks>
    /// Specifying a well-defined, deterministic order ensures that children are visited in a consistent and 
    /// reproducible way across executions of the visit.
    /// </remarks>
    public Func<IEnumerable<KeyValuePair<TEdge, TNode>>, IEnumerable<KeyValuePair<TEdge, TNode>>> ChildrenSorter
    {
        get;
        set;
    } = children => children;

    /// <inheritdoc/>
    /// <example>
    ///     <inheritdoc cref="BreadthFirstTraversal{TEdge, TNode}" path="/example"/>
    /// </example>
    public abstract void Visit(TNode node, Visitor<TNode, TreeTraversalContext<TEdge, TNode>> visitor);
}
