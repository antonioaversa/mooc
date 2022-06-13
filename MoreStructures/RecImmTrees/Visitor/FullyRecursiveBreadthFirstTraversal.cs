﻿namespace MoreStructures.RecImmTrees.Visitor;

/// <inheritdoc cref="BreadthFirstTraversal{TEdge, TNode}" path="//*[not(self::summary or self::remarks)]"/>
/// <summary>
/// A lazy, fully-recursive, breadth-first <see cref="IVisitStrategy{TNode, TVisitContext}"/> implementation, i.e. a 
/// traversing strategy which visits all the nodes at the current depth, along any path of the tree, before going 
/// deeper or shallower, exploring nodes with higher or lower depth.
/// </summary>
/// <remarks>
///     <inheritdoc cref="BreadthFirstTraversal{TEdge, TNode}" path="/remarks"/>
///     <para>
///     Implemented fully recursively, so limited by stack depth and usable with tree of a "reasonable" height.
///     </para>
/// </remarks>
public class FullyRecursiveBreadthFirstTraversal<TEdge, TNode>
    : BreadthFirstTraversal<TEdge, TNode>
    where TEdge : IRecImmDictIndexedTreeEdge<TEdge, TNode>
    where TNode : IRecImmDictIndexedTreeNode<TEdge, TNode>
{
    private record struct NodeWithLevel(TNode? ParentNode, TEdge? IncomingEdge, TNode Node, int Level);

    /// <inheritdoc 
    ///     cref="TreeTraversal{TEdge, TNode}.Visit(TNode)"
    ///     path="//*[not(self::summary or self::remarks)]" />
    /// <summary>
    /// <b>Lazily and recursively</b> visits the structure of the provided <paramref name= "node" />, returning the
    /// sequence of <see cref="IRecImmDictIndexedTreeNode{TEdge, TNode}"/> of the structure, in breadth-first order.
    /// </summary>
    /// <remarks>
    ///     <inheritdoc cref="FullyRecursiveBreadthFirstTraversal{TEdge, TNode}" path="/remarks"/>
    ///     <para id = "algo" >
    ///     - The algorithm first lazily visits all nodes in structure in natural recursion/depth-first order, 
    ///       returning an <see cref="IEnumerable{T}"/> instance of all nodes with their level in the structure.
    ///       <br/>
    ///     - Then, it lazily sort and visit them level by level, according to 
    ///       <see cref="TreeTraversal{TEdge, TNode}.TraversalOrder"/>, yielding to the output sequence, so that the 
    ///       client code implementing the visitor can lazily process the nodes.
    ///     </para>
    ///     <para id="complexity1">
    ///     - Excluding visitor, constant time work is done for each of the n nodes of the tree (such as destructuring
    ///       of <see cref="IEnumerable{T}"/> items and construction of the input record for the recursive call). 
    ///       <br/>
    ///     - Recursive traversal, as well as sorting, are lazily executed. Iteration-cost is constant w.r.t. n.
    ///       <see cref="TreeTraversal{TEdge, TNode}.ChildrenSorter"/> cost depends on the actual algorithm used.
    ///       When no sorting, Counting Sort or QuickSort is applied (respectively O(1), O(n), O(n * log(n)), the cost
    ///       is tipically equalized or exceeded by sorting cost (see below).
    ///       <br/>
    ///     - So Time Complexity is dominated by the two operations on the <see cref="IEnumerable{T}"/> generated by 
    ///       the recursive traversal: sorting and visitor.
    ///       <br/>
    ///     - Sorting done on the <see cref="IEnumerable{T}"/> of all the n nodes retrieved during recursive traversal 
    ///       via the LINQ functionalities
    ///       <see cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/> and 
    ///       <see cref="Enumerable.OrderByDescending{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>.
    ///       <br/>
    ///     - Visitor is client code invoked during iteration of the output sequence, containing each of the n nodes of 
    ///       the sorted <see cref="IEnumerable{T}"/>.
    ///       <br/>
    ///     - If the size of alphabet of elements of the tree is a small constant c, sorting could be done in linear 
    ///       time via Counting Sort. Otherwise, a comparison-based sorting takes at best a time proportional to 
    ///       n * log(n). However, LINQ sorting by 
    ///       <see cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/> and
    ///       <see cref="Enumerable.OrderByDescending{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    ///       is QuickSort based, and has a O(n * log(n)) average runtime, with O(n^2) worst case.
    ///       <br/>
    ///     </para>
    ///     <para id="complexity2">
    ///     In conclusion:
    ///     <br/>.
    ///     - Time Complexity is O(n * (log(n) + Ts)), where Ts is the amortized time cost of
    ///       <see cref="TreeTraversal{TEdge, TNode}.ChildrenSorter"/> per node. Taking into account the visit of
    ///       each emitted node, Time Complexity is O(n * (log(n) + Ts + Tv)), where Tv is the time cost of the 
    ///       visitor per node.
    ///       <br/>
    ///     - Space Complexity is O(n). Taking into account the visit of each emitted node, Space Complexity is 
    ///       O(n * Sv), where Sv is the space cost of visitor per node.
    ///     </para>
    /// </remarks>
    public override IEnumerable<TreeTraversalVisit<TEdge, TNode>> Visit(TNode node)
    {
        var nodesWithLevel = GetAllNodesWithLevel(new(default, default, node, 0));
        var sortedNodesWithLevel = TraversalOrder switch
        {
            TreeTraversalOrder.ParentFirst => nodesWithLevel.OrderBy(nodeWitLevel => nodeWitLevel.Level),
            TreeTraversalOrder.ChildrenFirst => nodesWithLevel.OrderByDescending(nodeWitLevel => nodeWitLevel.Level),
            _ => throw new NotSupportedException($"{nameof(TreeTraversalOrder)} {TraversalOrder} is not supported"),
        };

        foreach (var (parentNode, incomingEdge, node1, level) in sortedNodesWithLevel)
            yield return new(node1, new(parentNode, incomingEdge));
    }

    private IEnumerable<NodeWithLevel> GetAllNodesWithLevel(NodeWithLevel nodeWithLevel)
    {
        yield return nodeWithLevel;

        var (_, _, node, level) = nodeWithLevel;

        foreach (var child in ChildrenSorter(node.Children))
            foreach (var childSubNode in GetAllNodesWithLevel(new(node, child.Key, child.Value, level + 1)))
                yield return childSubNode;
    }
}