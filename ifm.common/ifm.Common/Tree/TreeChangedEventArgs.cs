namespace ifm.Common.Tree;

using System;

/// <summary>
/// Defines the actions that caused a tree changed event. 
/// </summary>
public enum TreeChangedActions
{
    /// <summary>
    /// A child was added.
    /// </summary>
    ChildAdded,

    /// <summary>
    /// A child was removed.
    /// </summary>
    ChildRemoved,

    /// <summary>
    /// A link was added.
    /// </summary>
    LinkAdded,

    /// <summary>
    /// A link was removed.
    /// </summary>
    LinkRemoved,

    /// <summary>
    /// Multiple changes done.
    /// </summary>
    TreeChanged
}

/// <summary>
/// Provides data for the tree changed event.
/// Initializes an instance of the class.
/// </summary>
/// <typeparam name="T">The type of the node.</typeparam>
/// <param name="action">The action that caused the event.</param>
/// <param name="sourceNode">The node that was affected by the change.</param>
/// <param name="targetNode">The node involved in the change.</param>
/// <param name="identifier">The identifier of the reference.</param>
public class TreeChangedEventArgs<T>(TreeChangedActions action,
    T sourceNode,
    T targetNode,
    string identifier) : EventArgs where T : class
{
    /// <summary>
    /// Gets the action that caused the event.
    /// </summary>
    public TreeChangedActions Action { get; } = action;

    /// <summary>
    /// Gets the node that was affected by the change.
    /// </summary>
    public T SourceNode { get; } = sourceNode;

    /// <summary>
    /// Gets the node involved in the change.
    /// </summary>
    public T TargetNode { get; } = targetNode;

    /// <summary>
    /// Gets the identifier of the reference.
    /// </summary>
    public string Identifier { get; } = identifier;
}