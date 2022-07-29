using ifmIoTCore.Elements;

namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders
{
    using System;

    internal class RunControlProfileBuilder
    {
        private readonly IDataElement _targetElement;
        private readonly IServiceElement _startElement;
        private readonly IServiceElement _stopElement;
        private readonly IDataElement _presetElement;
        private readonly IServiceElement _suspendElement;
        private readonly IServiceElement _resetElement;

        internal RunControlProfileBuilder(IDataElement targetElement,
            IServiceElement startElement,
            IServiceElement stopElement,
            IDataElement presetElement = null,
            IServiceElement suspendElement = null,
            IServiceElement resetElement = null)
        {
            this._targetElement = targetElement ?? throw new ArgumentNullException(nameof(targetElement));
            this._startElement = startElement?? throw new ArgumentNullException(nameof(startElement));
            this._stopElement = stopElement ?? throw new ArgumentNullException(nameof(stopElement));
            this._presetElement = presetElement;
            this._suspendElement = suspendElement;
            this._resetElement = resetElement;
        }

        public string Name { get; } = "runcontrol";
        
        public void Build()
        {
            _targetElement.AddProfile(this.Name);
            _targetElement.AddChild(_startElement);
            _targetElement.AddChild(_stopElement);

            if (_presetElement != null) _targetElement.AddChild(_presetElement);
            if (_suspendElement != null) _targetElement.AddChild(_suspendElement);
            if (_resetElement != null) _targetElement.AddChild(_resetElement);
        }

        public void Dispose()
        {
            this._targetElement.RemoveProfile("runcontrol");

            _targetElement.RemoveChild(_startElement);
            _targetElement.RemoveChild(_stopElement);

            if (this._presetElement != null) _targetElement.RemoveChild(this._presetElement);
            if (this._suspendElement != null) _targetElement.RemoveChild(this._suspendElement);
            if (this._resetElement != null) _targetElement.RemoveChild(this._resetElement);
        }

    }
}