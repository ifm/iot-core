namespace ifm.Common.Tree;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines the references of a node.
/// </summary>
/// <typeparam name="T">The type of the node.</typeparam>
public class ReferenceTable<T> where T : class
{
    private List<Reference<T>> _forwardReferences;
    private List<Reference<T>> _inverseReferences;

    /// <summary>
    /// Gets the forward references.
    /// </summary>
    public IReadOnlyList<Reference<T>> ForwardReferences => _forwardReferences;

    /// <summary>
    /// Gets the inverse references.
    /// </summary>
    public IReadOnlyList<Reference<T>> InverseReferences => _inverseReferences;

    /// <summary>
    /// Adds a forward reference.
    /// </summary>
    /// <param name="sourceNode">The source node of the reference.</param>
    /// <param name="targetNode">The target node of the reference.</param>
    /// <param name="type">The type of the reference.</param>
    /// <param name="identifier">The identifier of the reference.</param>
    /// <returns>The new reference.</returns>
    public Reference<T> AddForwardReference(T sourceNode, T targetNode, ReferenceTypes type, string identifier)
    {
        ExceptionHelpers.ThrowIfNull(sourceNode, nameof(sourceNode));
        ExceptionHelpers.ThrowIfNull(targetNode, nameof(targetNode));

        var reference = new Reference<T>(sourceNode, targetNode, type, ReferenceDirections.Forward, identifier);
        _forwardReferences ??= [];
        _forwardReferences.Add(reference);
        return reference;
    }

    /// <summary>
    /// Adds an inverse reference.
    /// </summary>
    /// <param name="sourceNode">The source node of the reference.</param>
    /// <param name="targetNode">The target node of the reference.</param>
    /// <param name="type">The type of the reference.</param>
    /// <param name="identifier">The identifier of the reference.</param>
    /// <returns>The new reference.</returns>
    public Reference<T> AddInverseReference(T sourceNode, T targetNode, ReferenceTypes type, string identifier)
    {
        ExceptionHelpers.ThrowIfNull(sourceNode, nameof(sourceNode));
        ExceptionHelpers.ThrowIfNull(targetNode, nameof(targetNode));

        var reference = new Reference<T>(sourceNode, targetNode, type, ReferenceDirections.Inverse, identifier);
        _inverseReferences ??= [];
        _inverseReferences.Add(reference);
        return reference;
    }

    /// <summary>
    /// Removes a forward reference.
    /// </summary>
    /// <param name="sourceNode">The source node of the reference.</param>
    /// <param name="targetNode">The target node of the reference.</param>
    /// <returns>The removed reference or null, if the reference does not exist.</returns>
    public Reference<T> RemoveForwardReference(T sourceNode, T targetNode)
    {
        ExceptionHelpers.ThrowIfNull(sourceNode, nameof(sourceNode));
        ExceptionHelpers.ThrowIfNull(targetNode, nameof(targetNode));

        var reference = _forwardReferences?.FirstOrDefault(x => x.SourceNode == sourceNode && x.TargetNode == targetNode);
        if (reference == null) return null;
        _forwardReferences.Remove(reference);
        if (_forwardReferences.Count == 0) _forwardReferences = null;
        return reference;
    }

    /// <summary>
    /// Removes an inverse reference.
    /// </summary>
    /// <param name="sourceNode">The source node of the reference.</param>
    /// <param name="targetNode">The target node of the reference.</param>
    /// <returns>The removed reference or null, if the reference does not exist.</returns>
    public Reference<T> RemoveInverseReference(T sourceNode, T targetNode)
    {
        ExceptionHelpers.ThrowIfNull(sourceNode, nameof(sourceNode));
        ExceptionHelpers.ThrowIfNull(targetNode, nameof(targetNode));

        var reference = _inverseReferences?.FirstOrDefault(x => x.SourceNode == sourceNode && x.TargetNode == targetNode);
        if (reference == null) return null;
        _inverseReferences.Remove(reference);
        if (_inverseReferences.Count == 0) _inverseReferences = null;
        return reference;
    }
}
