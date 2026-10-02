namespace ifm.Common.UnitTests.Tree;

using System.Linq;
using Common.Tree;
using NUnit.Framework;

[TestFixture]
public class TreeNodeTests
{
    [Test]
    public void AddChild()
    {
        var root = new TreeNode<string>("Root");
        root.AddChild(new TreeNode<string>("Child"));
        var child = root.ForwardReferences.FirstOrDefault(x => x.TargetNode.Name == "Child");
        Assert.That(child != null);
    }
}