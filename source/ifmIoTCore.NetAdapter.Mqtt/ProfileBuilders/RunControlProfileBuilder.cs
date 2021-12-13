using ifmIoTCore.Elements;

namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders
{
    using System;

    internal class RunControlProfileBuilder
    {
        private readonly IDataElement<string> _targetElement;
        private readonly IActionServiceElement _startElement;
        private readonly IActionServiceElement _stopElement;
        private readonly IDataElement<string> _presetElement;
        private readonly IActionServiceElement _suspendElement;
        private readonly IActionServiceElement _resetElement;
        private readonly IElementManager _elementManager;

        internal RunControlProfileBuilder(IElementManager elementManager,
            IDataElement<string> targetElement,
            IActionServiceElement startElement,
            IActionServiceElement stopElement,
            IDataElement<string> presetElement = null,
            IActionServiceElement suspendElement = null,
            IActionServiceElement resetElement = null)
        {
            this._elementManager = elementManager;

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
            this._targetElement.AddProfile(this.Name);
        }

        public void Dispose()
        {
            this._targetElement.RemoveProfile("runcontrol");

            _elementManager.RemoveElement(this._targetElement, this._startElement);
            _elementManager.RemoveElement(this._targetElement, this._stopElement);
            if (this._presetElement != null) _elementManager.RemoveElement(this._targetElement, this._presetElement);
            if (this._suspendElement != null) _elementManager.RemoveElement(this._targetElement, this._suspendElement);
            if (this._resetElement != null) _elementManager.RemoveElement(this._targetElement, this._resetElement);
        }

    }
}