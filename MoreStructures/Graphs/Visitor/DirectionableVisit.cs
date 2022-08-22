﻿namespace MoreStructures.Graphs.Visitor;

/// <summary>
/// Any A <see cref="IVisitStrategy"/> implementation which can perform the visit taking into account or not the
/// direction of the edges of the graph, based on the <see cref="DirectedGraph"/> property.
/// </summary>
public abstract class DirectionableVisit : IVisitStrategy
{
    /// <summary>
    /// Whether the provided <see cref="IGraph"/> should be considered as directed (i.e. edges should be 
    /// traversed from start to end) or inderected (i.e. edges should be traversed both from start to end and from
    /// end to start).
    /// </summary>
    public bool DirectedGraph { get; }

    /// <summary>
    ///     <inheritdoc cref="DirectionableVisit"/>
    /// </summary>
    /// <param name="directedGraph">
    ///     <inheritdoc cref="DirectedGraph" path="/summary"/>
    /// </param>
    protected DirectionableVisit(bool directedGraph)
    {
        DirectedGraph = directedGraph;
    }

    /// <inheritdoc/>
    public event EventHandler<VisitEventArgs> VisitingVertex = delegate { };

    /// <summary>
    /// Invoke the <see cref="VisitingVertex"/> event with the provided <paramref name="args"/>.
    /// </summary>
    /// <param name="args">The arguments to be provided, when raising the event.</param>
    protected virtual void RaiseVisitingVertex(VisitEventArgs args) => VisitingVertex.Invoke(this, args);

    /// <inheritdoc/>
    public event EventHandler<VisitEventArgs> VisitedVertex = delegate { };

    /// <summary>
    /// Invoke the <see cref="VisitedVertex"/> event with the provided <paramref name="args"/>.
    /// </summary>
    /// <param name="args">The arguments to be provided, when raising the event.</param>
    protected virtual void RaiseVisitedVertex(VisitEventArgs args) => VisitedVertex.Invoke(this, args);

    /// <inheritdoc/>
    public abstract IEnumerable<int> DepthFirstSearch(IGraph graph);

    /// <inheritdoc/>
    public abstract IDictionary<int, int> ConnectedComponents(IGraph graph);

    /// <inheritdoc/>
    public abstract IEnumerable<int> Visit(IGraph graph, int start);
}
