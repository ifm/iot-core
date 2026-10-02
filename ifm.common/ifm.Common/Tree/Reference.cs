namespace ifm.Common.Tree;

using System;

/// <summary>
/// Defines the reference types.
/// </summary>
public enum ReferenceTypes
{
    /// <summary>
    /// A child reference.
    /// </summary>
    Child,

    /// <summary>
    /// A link reference.
    /// </summary>
    Link
}

/// <summary>
/// Defines the reference directions.
/// </summary>
public enum ReferenceDirections
{
    /// <summary>
    /// A forward reference.
    /// </summary>
    Forward,

    /// <summary>
    /// An inverse reference.
    /// </summary>
    Inverse
}

/// <summary>
/// Defines a reference between two nodes.
/// Initializes a new instance of the class.
/// </summary>
/// <typeparam name="T">The type of the node.</typeparam>
/// <param name="sourceNode">The source node of the reference.</param>
/// <param name="targetNode">The target node of the reference.</param>
/// <param name="type">The type of the reference.</param>
/// <param name="direction">The direction of the reference.</param>
/// <param name="identifier">The identifier of the reference.</param>
public class Reference<T>(T sourceNode,
    T targetNode,
    ReferenceTypes type,
    ReferenceDirections direction,
    string identifier) where T : class
{
    /// <summary>
    /// The source node of the reference.
    /// </summary>
    public T SourceNode { get; } = sourceNode ?? throw new ArgumentNullException(nameof(sourceNode));

    /// <summary>
    /// The target node of the reference.
    /// </summary>
    public T TargetNode { get; } = targetNode ?? throw new ArgumentNullException(nameof(targetNode));

    /// <summary>
    /// The type of the reference.
    /// </summary>
    public ReferenceTypes Type { get; } = type;

    /// <summary>
    /// The direction of the reference.
    /// </summary>
    public ReferenceDirections Direction { get; } = direction;

    /// <summary>
    /// The identifier of the reference.
    /// </summary>
    public string Identifier { get; } = identifier;

    /// <summary>
    /// Checks if the reference is a child reference.
    /// </summary>
    public bool IsChild => Type == ReferenceTypes.Child;

    /// <summary>
    /// Checks if the reference is a link reference.
    /// </summary>
    public bool IsLink => Type == ReferenceTypes.Link;

    /// <summary>
    /// Checks if the reference is a forward reference.
    /// </summary>
    public bool IsForward => Direction == ReferenceDirections.Forward;

    /// <summary>
    /// Checks if the reference is an inverse reference.
    /// </summary>
    public bool IsInverse => Direction == ReferenceDirections.Inverse;

    /// <summary>
    /// Converts this instance to a human readable string.
    /// </summary>
    public override string ToString()
    {
        return IsForward ? $"{SourceNode} --> {TargetNode}" : $"{SourceNode} <-- {TargetNode}";
    }
}