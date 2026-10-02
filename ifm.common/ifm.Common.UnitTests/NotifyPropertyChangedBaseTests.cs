namespace ifm.Common.UnitTests;

using NUnit.Framework;

public class NotifyPropertyChangedDerived : NotifyPropertyChangedBase
{
    public void RaisePropertyChanged() => base.RaisePropertyChanged();
    public void RaisePropertyChanging() => base.RaisePropertyChanging();

}

[TestFixture]
public class NotifyPropertyChangedBaseTests
{
    [Test]
    public void RaiseNotifyPropertyChanged_EventIsRaised()
    {
        var propertyChanged = false;
        var o = new NotifyPropertyChangedDerived();
        o.PropertyChanged += (_, _) => { propertyChanged = true; };

        o.RaisePropertyChanged();

        Assert.That(propertyChanged);
    }

    [Test]
    public void RaiseNotifyPropertyChanged_OmitHandler_EventIsNotRaised()
    {
        var o = new NotifyPropertyChangedDerived();

        o.RaisePropertyChanged();

        Assert.That(true);
    }

    [Test]
    public void RaiseNotifyPropertyChanging_EventIsRaised()
    {
        var propertyChanged = false;
        var o = new NotifyPropertyChangedDerived();
        o.PropertyChanging += (_, _) => { propertyChanged = true; };

        o.RaisePropertyChanging();

        Assert.That(propertyChanged);
    }

    [Test]
    public void RaiseNotifyPropertyChanging_OmitHandler_EventIsNotRaised()
    {
        var o = new NotifyPropertyChangedDerived();

        o.RaisePropertyChanging();

        Assert.That(true);
    }
}