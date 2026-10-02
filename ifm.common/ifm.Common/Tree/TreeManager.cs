namespace ifm.Common.Tree;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Manages a node tree.
/// Initializes an instance of the class.
/// </summary>
/// <param name="timeout">The timeout in milliseconds.</param>
public class TreeManager<T>(int timeout = 10000)
{
    private TreeNode<T> _root;
    private readonly List<TreeChangedEventArgs<TreeNode<T>>> _treeChangedActions = [];
    private readonly ConcurrentDictionary<string, TreeNode<T>> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ReaderWriterLockSlim _lock = new();

    /// <summary>
    /// Enters the read lock.
    /// </summary>
    public void EnterReadLock()
    {
        if (!_lock.TryEnterReadLock(timeout))
        {
            throw new Exception("The tree manager is locked");
        }
    }

    /// <summary>
    /// Exists the read lock.
    /// </summary>
    public void ExitReadLock()
    {
        _lock.ExitReadLock();
    }

    /// <summary>
    /// Enters the write lock.
    /// </summary>
    public void EnterWriteLock()
    {
        if (!_lock.TryEnterWriteLock(timeout))
        {
            throw new Exception("The tree manager is locked");
        }
    }

    /// <summary>
    /// Exists the write lock.
    /// </summary>
    public void ExitWriteLock()
    {
        _lock.ExitWriteLock();
    }

    /// <summary>
    /// Return true if the read lock is held; otherwise false.
    /// </summary>
    public bool IsReadLockHeld => _lock.IsReadLockHeld;

    /// <summary>
    /// Return true if the write lock is held; otherwise false.
    /// </summary>
    public bool IsWriteLockHeld => _lock.IsWriteLockHeld;

    /// <summary>
    /// Gets or sets the tree root node.
    /// </summary>
    public TreeNode<T> Root
    {
        get => _root;
        set
        {
            ExceptionHelpers.ThrowIfNull(value, nameof(value));
            if (_root == value) return;
            _root = value;
            RaiseTreeChanged(TreeChangedActions.TreeChanged, _root, null, null, true);
        }
    }

    /// <summary>
    /// Adds a node to the tree.
    /// </summary>
    /// <param name="parentNode">The parent node.</param>
    /// <param name="childNode">The node to add.</param>
    /// <param name="raiseTreeChanged">If true a tree changed event is raised; otherwise not.</param>
    /// <param name="acquireLock">If true the write lock is acquired; otherwise not.</param>
    public void AddNode(TreeNode<T> parentNode, TreeNode<T> childNode, bool raiseTreeChanged = false, bool acquireLock = true)
    {
        ExceptionHelpers.ThrowIfNull(parentNode, nameof(parentNode));
        ExceptionHelpers.ThrowIfNull(childNode, nameof(childNode));

        if (acquireLock) EnterWriteLock();
        try
        {
            parentNode.AddChild(childNode);
        }
        finally
        {
            if (acquireLock) ExitWriteLock();
        }

        RaiseTreeChanged(TreeChangedActions.ChildAdded, parentNode, childNode, null, raiseTreeChanged);
    }

    /// <summary>
    /// Removes a node from the tree.
    /// </summary>
    /// <param name="parentNode">The parent node.</param>
    /// <param name="childNode">The node to remove.</param>
    /// <param name="raiseTreeChanged">If true a tree changed event is raised; otherwise not.</param>
    /// <param name="acquireLock">If true the write lock is acquired; otherwise not.</param>
    public void RemoveChild(TreeNode<T> parentNode, TreeNode<T> childNode, bool raiseTreeChanged = false, bool acquireLock = true)
    {
        ExceptionHelpers.ThrowIfNull(parentNode, nameof(parentNode));
        ExceptionHelpers.ThrowIfNull(childNode, nameof(childNode));

        if (acquireLock) EnterWriteLock();
        try
        {
            parentNode.RemoveChild(childNode);
        }
        finally
        {
            if (acquireLock) ExitWriteLock();
        }

        RaiseTreeChanged(TreeChangedActions.ChildRemoved, parentNode, childNode, null, raiseTreeChanged);
    }

    /// <summary>
    /// Adds a link to a node to the tree.
    /// </summary>
    /// <param name="sourceNode">The source node.</param>
    /// <param name="targetNode">The node to link.</param>
    /// <param name="identifier">The identifier for the link. If null the identifier of the target node is used.</param>
    /// <param name="raiseTreeChanged">If true a tree changed event is raised; otherwise not.</param>
    /// <param name="acquireLock">If true the write lock is acquired; otherwise not.</param>
    public void AddLink(TreeNode<T> sourceNode, TreeNode<T> targetNode, string identifier, bool raiseTreeChanged = false, bool acquireLock = true)
    {
        ExceptionHelpers.ThrowIfNull(sourceNode, nameof(sourceNode));
        ExceptionHelpers.ThrowIfNull(targetNode, nameof(targetNode));

        if (acquireLock) EnterWriteLock();
        try
        {
            sourceNode.AddLink(targetNode, identifier);
        }
        finally
        {
            if (acquireLock) ExitWriteLock();
        }

        RaiseTreeChanged(TreeChangedActions.LinkAdded, sourceNode, targetNode, identifier, raiseTreeChanged);
    }

    /// <summary>
    /// Removes a link to a node from the tree.
    /// </summary>
    /// <param name="sourceNode">The source node.</param>
    /// <param name="targetNode">The node to link.</param>
    /// <param name="raiseTreeChanged">If true a tree changed event is raised; otherwise not.</param>
    /// <param name="acquireLock">If true the write lock is acquired; otherwise not.</param>
    public void RemoveLink(TreeNode<T> sourceNode, TreeNode<T> targetNode, bool raiseTreeChanged = false, bool acquireLock = true)
    {
        ExceptionHelpers.ThrowIfNull(sourceNode, nameof(sourceNode));
        ExceptionHelpers.ThrowIfNull(targetNode, nameof(targetNode));

        if (acquireLock) EnterWriteLock();
        try
        {
            sourceNode.RemoveLink(targetNode);
        }
        finally
        {
            if (acquireLock) ExitWriteLock();
        }

        RaiseTreeChanged(TreeChangedActions.LinkRemoved, sourceNode, targetNode, null, raiseTreeChanged);
    }

    /// <summary>
    /// The tree changed event handler.
    /// </summary>
    public event EventHandler<List<TreeChangedEventArgs<TreeNode<T>>>> TreeChanged;

    /// <summary>
    /// Raises a tree changed event or adds the action to the list of actions.
    /// </summary>
    /// <param name="action">The action that caused the event.</param>
    /// <param name="sourceNode">The node that was affected by the change.</param>
    /// <param name="targetNode">The node involved in the change.</param>
    /// <param name="identifier">The identifier of the reference.</param>
    /// <param name="raiseTreeChanged">If true the event is raised; if false the action is added to the list of actions.</param>
    public void RaiseTreeChanged(TreeChangedActions action, TreeNode<T> sourceNode, TreeNode<T> targetNode, string identifier, bool raiseTreeChanged)
    {
        ExceptionHelpers.ThrowIfNull(sourceNode, nameof(sourceNode));
        ExceptionHelpers.ThrowIfNull(targetNode, nameof(targetNode));

        var args = new TreeChangedEventArgs<TreeNode<T>>(action, sourceNode, targetNode, identifier);
        _treeChangedActions.Add(args);
        if (raiseTreeChanged)
        {
            sourceNode.RaiseTreeChanged(args);
            TreeChanged?.Invoke(this, _treeChangedActions);
            _treeChangedActions.Clear();
        }
    }

    /// <summary>
    /// Gets the node with the unique id from the tree in forward direction.
    /// </summary>
    /// <param name="parentNode">The node to start the search.</param>
    /// <param name="uniqueId">The unique node id.</param>
    /// <param name="recurse">If true the tree is searched recursively; otherwise not.</param>
    /// <param name="includeSelf">If true the parent node is included in the search; otherwise not.</param>
    /// <param name="acquireLock">If true the read lock is acquired; otherwise not.</param>
    /// <returns>The node with the unique node id or null.</returns>
    public TreeNode<T> GetNode(TreeNode<T> parentNode, string uniqueId, bool recurse = true, bool includeSelf = true, bool acquireLock = true)
    {
        ExceptionHelpers.ThrowIfNull(parentNode, nameof(parentNode));

        if (acquireLock) EnterReadLock();
        try
        {
            if (_cache.TryGetValue(uniqueId, out var node))
            {
                return node;
            }
            node = parentNode.GetNode(x => string.Equals(x.UniqueId, uniqueId, StringComparison.OrdinalIgnoreCase), recurse, includeSelf);
            if (node != null)
            {
                _cache.TryAdd(uniqueId, node);
            }
            return node;
        }
        finally
        {
            if (acquireLock) ExitWriteLock();
        }
    }

    /// <summary>
    /// Gets the first node that fulfills the predicate from the tree in forward direction.
    /// </summary>
    /// <param name="parentNode">The node to start the search.</param>
    /// <param name="predicate">The predicate the node has to fulfill.</param>
    /// <param name="recurse">If true the tree is searched recursively; otherwise not.</param>
    /// <param name="includeSelf">If true the parent node is included in the search; otherwise not.</param>
    /// <param name="acquireLock">If true the read lock is acquired; otherwise not.</param>
    /// <returns>The first node that fulfills the predicate or null.</returns>
    public TreeNode<T> GetNode(TreeNode<T> parentNode, Predicate<TreeNode<T>> predicate, bool recurse = true, bool includeSelf = true, bool acquireLock = true)
    {
        ExceptionHelpers.ThrowIfNull(parentNode, nameof(parentNode));
        ExceptionHelpers.ThrowIfNull(predicate, nameof(predicate));

        if (acquireLock) EnterReadLock();
        try
        {
            return parentNode.GetNode(predicate, recurse, includeSelf);
        }
        finally
        {
            if (acquireLock) ExitWriteLock();
        }
    }

    /// <summary>
    /// Gets the first node that fulfills the predicate from the tree in inverse direction.
    /// </summary>
    /// <param name="parentNode">The node to start the search.</param>
    /// <param name="predicate">The predicate the node has to fulfill.</param>
    /// <param name="recurse">If true the tree is searched recursively; otherwise not.</param>
    /// <param name="includeSelf">If true the parent node is included in the search; otherwise not.</param>
    /// <param name="acquireLock">If true the read lock is acquired; otherwise not.</param>
    /// <returns>The first node that fulfills the predicate or null.</returns>
    public TreeNode<T> GetNodeInverse(TreeNode<T> parentNode, Predicate<TreeNode<T>> predicate, bool recurse = true, bool includeSelf = true, bool acquireLock = true)
    {
        ExceptionHelpers.ThrowIfNull(parentNode, nameof(parentNode));
        ExceptionHelpers.ThrowIfNull(predicate, nameof(predicate));

        if (acquireLock) EnterReadLock();
        try
        {
            return parentNode.GetNodeInverse(predicate, recurse, includeSelf);
        }
        finally
        {
            if (acquireLock) ExitWriteLock();
        }
    }

    /// <summary>
    /// Gets nodes that fulfill the predicate from the tree in forward direction.
    /// </summary>
    /// <param name="parentNode">The node to start the search.</param>
    /// <param name="predicate">The predicate the nodes have to fulfill.</param>
    /// <param name="recurse">If true the tree is searched recursively; otherwise not.</param>
    /// <param name="includeSelf">If true the parent node is included in the search; otherwise not.</param>
    /// <param name="acquireLock">If true the read lock is acquired; otherwise not.</param>
    /// <returns>The list of nodes that fulfill the predicate.</returns>
    public IList<TreeNode<T>> GetNodes(TreeNode<T> parentNode, Predicate<TreeNode<T>> predicate, bool recurse = true, bool includeSelf = true, bool acquireLock = true)
    {
        ExceptionHelpers.ThrowIfNull(parentNode, nameof(parentNode));
        ExceptionHelpers.ThrowIfNull(predicate, nameof(predicate));

        if (acquireLock) EnterReadLock();
        try
        {
            return parentNode.GetNodes(predicate, recurse, includeSelf);
        }
        finally
        {
            if (acquireLock) ExitWriteLock();
        }
    }
}
