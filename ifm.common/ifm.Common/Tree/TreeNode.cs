namespace ifm.Common.Tree;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/// <summary>
/// Represents a tree node.
/// Initializes an instance of the class.
/// </summary>
/// <typeparam name="T">The type of the value in the tree node.</typeparam>
/// <param name="name">The name of the node.</param>
/// <param name="value">The value in the tree node.</param>
/// <param name="uniqueId">The unique node identifier.</param>
/// <param name="timeout">The timeout for the lock.</param>
public class TreeNode<T>(string name, 
    T value = default, 
    string uniqueId = null, 
    int timeout = 3000)
{
    private readonly ReaderWriterLockSlim _lock = new();

    /// <summary>
    /// Gets the node name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the value in the tree node.
    /// </summary>
    public T Value { get; set; } = value;

    /// <summary>
    /// Gets or sets the unique node identifier.
    /// </summary>
    public string UniqueId { get; private set; } = uniqueId;

    /// <summary>
    /// Gets the parent of the tree node.
    /// </summary>
    public TreeNode<T> Parent { get; private set; }

    /// <summary>
    /// Gets the references of the tree node.
    /// </summary>
    public ReferenceTable<TreeNode<T>> References { get; } = new();

    /// <summary>
    /// Enters the read lock.
    /// </summary>
    /// <exception cref="Exception">Lock error.</exception>
    public void EnterReadLock()
    {
        if (!_lock.TryEnterReadLock(timeout))
        {
            throw new Exception("The node is locked");
        }
    }

    /// <summary>
    /// Exits the read lock.
    /// </summary>
    public void ExitReadLock()
    {
        _lock.ExitReadLock();
    }

    /// <summary>
    /// Enters the upgradable read lock.
    /// </summary>
    /// <exception cref="Exception">Lock error.</exception>
    public void EnterUpgradeableReadLock()
    {
        if (!_lock.TryEnterUpgradeableReadLock(timeout))
        {
            throw new Exception("The element is locked");
        }
    }

    /// <summary>
    /// Exits the upgradable read lock.
    /// </summary>
    public void ExitUpgradeableReadLock()
    {
        _lock.ExitUpgradeableReadLock();
    }

    /// <summary>
    /// Enters the write lock.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void EnterWriteLock()
    {
        if (!_lock.TryEnterWriteLock(timeout))
        {
            throw new Exception("The node is locked");
        }
    }

    /// <summary>
    /// Exits the write lock.
    /// </summary>
    public void ExitWriteLock()
    {
        _lock.ExitWriteLock();
    }

    /// <summary>
    /// Gets a snapshot copy of the forward references.
    /// </summary>
    public IReadOnlyList<Reference<TreeNode<T>>> ForwardReferences
    {
        get
        {
            EnterReadLock();
            try
            {
                return References.ForwardReferences?.Select(item => new Reference<TreeNode<T>>(item.SourceNode, item.TargetNode, item.Type, item.Direction, item.Identifier)).ToList();
            }
            finally
            {
                ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Gets a snapshot copy of the inverse references.
    /// </summary>
    public IReadOnlyList<Reference<TreeNode<T>>> InverseReferences
    {
        get
        {
            EnterReadLock();
            try
            {
                return References.InverseReferences?.Select(item => new Reference<TreeNode<T>>(item.SourceNode, item.TargetNode, item.Type, item.Direction, item.Identifier)).ToList();
            }
            finally
            {
                ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Adds a child reference to the provided node.
    /// </summary>
    /// <param name="node">The node to add the reference to.</param>
    /// <param name="raiseTreeChanged">If true a tree changed event is raised; otherwise not.</param>
    /// <exception cref="Exception">Adding reference error.</exception>
    public void AddChild(TreeNode<T> node, bool raiseTreeChanged = false)
    {
        ExceptionHelpers.ThrowIfNull(node, nameof(node));

        EnterWriteLock();
        try
        {
            if (References.ForwardReferences?.FirstOrDefault(x => x.TargetNode == node) != null)
            {
                throw new Exception("Reference to node already exists");
            }

            if (IsCircularDependency(this, node))
            {
                throw new Exception("Adding node creates circular dependency");
            }

            node.EnterWriteLock();
            try
            {
                if (node.Parent != null)
                {
                    throw new Exception("Node already has a parent");
                }

                node.Parent = this;
                node.References.AddInverseReference(this, node, ReferenceTypes.Child, node.Name);
                node.TreeChanged += OnTreeChanged;
            }
            finally
            {
                node.ExitWriteLock();
            }

            References.AddForwardReference(this, node, ReferenceTypes.Child, node.Name);
        }
        finally
        {
            ExitWriteLock();
        }

        if (raiseTreeChanged) RaiseTreeChanged(new TreeChangedEventArgs<TreeNode<T>>(TreeChangedActions.ChildAdded, this, node, node.Name));
    }

    /// <summary>
    /// Removes a child reference to the provided node.
    /// </summary>
    /// <param name="node">The node to remove the reference to.</param>
    /// <param name="raiseTreeChanged">If true a tree changed event is raised; otherwise not.</param>
    /// <exception cref="Exception">Removing reference error.</exception>
    public void RemoveChild(TreeNode<T> node, bool raiseTreeChanged = false)
    {
        ExceptionHelpers.ThrowIfNull(node, nameof(node));

        EnterWriteLock();
        try
        {
            if (References.ForwardReferences != null)
            {
                if (References.ForwardReferences.FirstOrDefault(x => x.TargetNode == node) == null)
                {
                    throw new Exception("Reference to node does not exist");
                }
            }

            node.EnterWriteLock();
            try
            {
                if (IsLinked(node))
                {
                    throw new Exception("The node is linked or contains linked nodes");
                }

                node.Parent = null;
                node.References.RemoveInverseReference(this, node);
                node.TreeChanged -= OnTreeChanged;
            }
            finally
            {
                node.ExitWriteLock();
            }

            References.RemoveForwardReference(this, node);
        }
        finally
        {
            ExitWriteLock();
        }

        if (raiseTreeChanged) RaiseTreeChanged(new TreeChangedEventArgs<TreeNode<T>>(TreeChangedActions.ChildRemoved, this, node, null));
    }

    /// <summary>
    /// Adds a link reference to the provided node.
    /// </summary>
    /// <param name="node">The node to add the reference to.</param>
    /// <param name="identifier">The identifier for the reference.</param>
    /// <param name="raiseTreeChanged">If true a tree changed event is raised; otherwise not.</param>
    /// <exception cref="Exception">Adding reference error.</exception>
    public void AddLink(TreeNode<T> node, string identifier = null, bool raiseTreeChanged = false)
    {
        ExceptionHelpers.ThrowIfNull(node, nameof(node));

        identifier ??= node.Name;

        EnterWriteLock();
        try
        {
            if (References.ForwardReferences?.FirstOrDefault(x => x.TargetNode == node) != null)
            {
                throw new Exception("Reference to node already exists");
            }

            if (IsCircularDependency(this, node))
            {
                throw new Exception("Adding node creates circular dependency");
            }

            node.EnterWriteLock();
            try
            {
                node.References.AddInverseReference(this, node, ReferenceTypes.Link, identifier);
            }
            finally
            {
                node.ExitWriteLock();
            }

            References.AddForwardReference(this, node, ReferenceTypes.Link, identifier);
        }
        finally
        {
            ExitWriteLock();
        }

        if (raiseTreeChanged) RaiseTreeChanged(new TreeChangedEventArgs<TreeNode<T>>(TreeChangedActions.LinkAdded, this, node, identifier));
    }

    /// <summary>
    /// Removes a link reference to the provided node.
    /// </summary>
    /// <param name="node">The node to remove the reference to.</param>
    /// <param name="raiseTreeChanged">If true a tree changed event is raised; otherwise not.</param>
    public void RemoveLink(TreeNode<T> node, bool raiseTreeChanged = false)
    {
        ExceptionHelpers.ThrowIfNull(node, nameof(node));

        EnterWriteLock();
        try
        {
            if (References.ForwardReferences != null)
            {
                if (References.ForwardReferences.FirstOrDefault(x => x.TargetNode == node) == null)
                {
                    throw new Exception("Reference to node does not exist");
                }
            }

            node.EnterWriteLock();
            try
            {
                node.References.RemoveInverseReference(this, node);
            }
            finally
            {
                node.ExitWriteLock();
            }

            References.RemoveForwardReference(this, node);
        }
        finally
        {
            ExitWriteLock();
        }

        if (raiseTreeChanged) RaiseTreeChanged(new TreeChangedEventArgs<TreeNode<T>>(TreeChangedActions.LinkRemoved, this, node, null));
    }

    /// <summary>
    /// The tree changed event handler.
    /// </summary>
    public event EventHandler<TreeChangedEventArgs<TreeNode<T>>> TreeChanged;

    /// <summary>
    /// Raises a tree changed event.
    /// </summary>
    /// <param name="args">The tree changed event argument.</param>
    public void RaiseTreeChanged(TreeChangedEventArgs<TreeNode<T>> args)
    {
        TreeChanged.Raise(this, args);
    }

    /// <summary>
    /// Gets the first node that fulfills the predicate from the tree in forward direction.
    /// </summary>
    /// <param name="predicate">The predicate the node has to fulfill.</param>
    /// <param name="recurse">If true the tree is searched recursively; otherwise not.</param>
    /// <param name="includeSelf">If true the node itself is included in the search; otherwise not.</param>
    /// <returns>The first node that fulfills the predicate or null.</returns>
    public TreeNode<T> GetNode(Predicate<TreeNode<T>> predicate, bool recurse = true, bool includeSelf = true)
    {
        ExceptionHelpers.ThrowIfNull(predicate, nameof(predicate));

        EnterReadLock();
        try
        {
            if (includeSelf && predicate(this)) return this;
            if (References.ForwardReferences == null) return null;
            foreach (var item in References.ForwardReferences)
            {
                if (!item.IsChild) continue;
                var result = GetNode(item.TargetNode, predicate, recurse);
                if (result != null) return result;
            }
            return null;
        }
        finally
        {
            ExitReadLock();
        }
    }

    private static TreeNode<T> GetNode(TreeNode<T> node, Predicate<TreeNode<T>> predicate, bool recurse)
    {
        if (predicate(node)) return node;
        if (!recurse) return null;

        node.EnterReadLock();
        try
        {
            if (node.References.ForwardReferences == null) return null;
            foreach (var item in node.References.ForwardReferences)
            {
                if (!item.IsChild) continue;
                var result = GetNode(item.TargetNode, predicate, true);
                if (result != null) return result;
            }
            return null;
        }
        finally
        {
            node.ExitReadLock();
        }
    }

    /// <summary>
    /// Gets the first node that fulfills the predicate from the tree in inverse direction.
    /// </summary>
    /// <param name="predicate">The predicate the node has to fulfill.</param>
    /// <param name="recurse">If true the tree is searched recursively; otherwise not.</param>
    /// <param name="includeSelf">If true the node itself is included in the search; otherwise not.</param>
    /// <returns>The first node that fulfills the predicate or null.</returns>
    public TreeNode<T> GetNodeInverse(Predicate<TreeNode<T>> predicate, bool recurse = true, bool includeSelf = true)
    {
        ExceptionHelpers.ThrowIfNull(predicate, nameof(predicate));

        EnterReadLock();
        try
        {
            if (includeSelf && predicate(this)) return this;
            if (References.InverseReferences == null) return null;
            foreach (var item in References.InverseReferences)
            {
                if (!item.IsChild) continue;
                var result = GetNodeInverse(item.SourceNode, predicate, recurse);
                if (result != null) return result;
            }
            return null;
        }
        finally
        {
            ExitReadLock();
        }
    }

    private static TreeNode<T> GetNodeInverse(TreeNode<T> node, Predicate<TreeNode<T>> predicate, bool recurse)
    {
        if (predicate(node)) return node;
        if (!recurse) return null;

        node.EnterReadLock();
        try
        {
            if (node.References.InverseReferences == null) return null;
            foreach (var item in node.References.InverseReferences)
            {
                if (!item.IsChild) continue;
                var result = GetNodeInverse(item.SourceNode, predicate, true);
                if (result != null) return result;
            }
            return null;
        }
        finally
        {
            node.ExitReadLock();
        }
    }

    /// <summary>
    /// Gets nodes that fulfill the predicate from the tree in forward direction.
    /// </summary>
    /// <param name="predicate">The predicate the nodes have to fulfill.</param>
    /// <param name="recurse">If true the tree is searched recursively; otherwise not.</param>
    /// <param name="includeSelf">If true the node itself is included in the search; otherwise not.</param>
    /// <returns>The list of nodes that fulfill the predicate.</returns>
    public IList<TreeNode<T>> GetNodes(Predicate<TreeNode<T>> predicate, bool recurse = true, bool includeSelf = true)
    {
        ExceptionHelpers.ThrowIfNull(predicate, nameof(predicate));

        EnterReadLock();
        try
        {
            var result = new List<TreeNode<T>>();
            if (includeSelf && predicate(this)) result.Add(this);
            if (References.ForwardReferences == null) return null;
            foreach (var item in References.ForwardReferences)
            {
                if (item.IsChild) GetNodes(item.TargetNode, predicate, recurse, result);
            }
            return result;
        }
        finally
        {
            ExitReadLock();
        }
    }

    private static void GetNodes(TreeNode<T> node, Predicate<TreeNode<T>> predicate, bool recurse, IList<TreeNode<T>> result)
    {
        if (predicate(node)) result.Add(node);
        if (!recurse) return;

        node.EnterReadLock();
        try
        {
            if (node.References.ForwardReferences == null) return;
            foreach (var item in node.References.ForwardReferences)
            {
                if (item.IsChild) GetNodes(item.TargetNode, predicate, true, result);
            }
        }
        finally
        {
            node.ExitReadLock();
        }
    }

    private void OnTreeChanged(object sender, TreeChangedEventArgs<TreeNode<T>> args)
    {
        TreeChanged.Raise(this, args);
    }

    private static bool IsCircularDependency(TreeNode<T> sourceNode, TreeNode<T> targetNode)
    {
        if (sourceNode == targetNode) return true;
        return targetNode.References.ForwardReferences != null && targetNode.References.ForwardReferences.Any(x => IsCircularDependency(sourceNode, x.TargetNode));
    }

    private static bool IsLinked(TreeNode<T> targetNode)
    {
        if (targetNode.References.InverseReferences.FirstOrDefault(x => x.IsLink) != null) return true;
        return targetNode.References.ForwardReferences != null && targetNode.References.ForwardReferences.Any(x => IsLinked(x.TargetNode));
    }
}
