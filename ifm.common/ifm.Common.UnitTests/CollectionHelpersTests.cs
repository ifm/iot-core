namespace ifm.Common.UnitTests;

using System;
using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class CollectionHelpersTests
{
    [Test]
    public void DictionaryRemoveAllInvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((IDictionary<int, int>)null).RemoveAllByValue(null));

        var dic = new Dictionary<int, int>();
        Assert.Throws<ArgumentNullException>(() => dic.RemoveAllByValue(null));
    }

    [Test]
    public void DictionaryRemoveAll_Success()
    {
        var dic = new Dictionary<int, int> { { 1, 1 }, { 2, 1 }, { 3, 3 }, { 4, 4 } };

        dic.RemoveAllByValue(x => x == 1);

        Assert.That(dic.Count == 2);
    }

    [Test]
    public void ListAddIfNotNullInvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((IList<int?>)null).AddIfNotNull(null));
    }

    [Test]
    public void ListAddIfNotNull_Success()
    {
        var list = new List<int?>();

        list.AddIfNotNull(1);
        Assert.That(list.Count == 1);

        list.AddIfNotNull(null);
        Assert.That(list.Count == 1);
    }
}