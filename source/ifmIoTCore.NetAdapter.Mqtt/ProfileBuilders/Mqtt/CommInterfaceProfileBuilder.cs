namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.Mqtt
{
    using System;
    using Elements;
    using Elements.Formats;
    using Elements.Valuations;

    internal sealed class CommInterfaceProfileBuilder
    {
        private readonly IBaseElement _targetElement;
        private readonly IBaseElement _runControlBaseElement;
        private readonly Func<string> _typeFunc;
        private readonly IBaseElement _commIfSetupElement;
        private readonly IBaseElement _commChannelBaseElement;
        private IDataElement<string> _typeElement;
        private readonly IElementManager _elementManager;

        /// <summary>
        /// Initializes a new instance of <see cref="CommInterfaceProfileBuilder"/>.
        /// </summary>
        /// <param name="elementManager">The element manager</param>
        /// <param name="targetElement">The element on which the profile is going to be built.</param>
        /// <param name="runControlBaseElement">The element that provides the status element and that implements the runcontrol profile.</param>
        /// <param name="typeFunc">A function that delivers the value for the type data-element of the comminterface profile.</param>
        /// <param name="commIfSetupElement">The commifsetup element of the comminterface profile.</param>
        /// <param name="commChannelBaseElement">The commchannel element of the comminterface profile.</param>
        internal CommInterfaceProfileBuilder(IElementManager elementManager, IBaseElement targetElement, IBaseElement runControlBaseElement, Func<string> typeFunc, IBaseElement commIfSetupElement, IBaseElement commChannelBaseElement = null)
        {
            this._elementManager = elementManager;
            this._targetElement = targetElement ?? throw new ArgumentNullException(nameof(targetElement));
            this._runControlBaseElement = runControlBaseElement ?? throw new ArgumentNullException(nameof(runControlBaseElement));
            this._typeFunc = typeFunc ?? throw new ArgumentNullException(nameof(typeFunc));
            this._commIfSetupElement = commIfSetupElement ?? throw new ArgumentNullException(nameof(commIfSetupElement));
            this._commChannelBaseElement = commChannelBaseElement;
        }

        public void Dispose()
        {
            this._targetElement.RemoveProfile("commInterface");
            _elementManager.RemoveElement(this._targetElement, this._runControlBaseElement);
            _elementManager.RemoveElement(this._targetElement, this._typeElement);
            _elementManager.RemoveElement(this._targetElement, this._commIfSetupElement);
            if (this._commChannelBaseElement != null)
            {
                _elementManager.RemoveElement(this._targetElement, this._commChannelBaseElement);
            }
        }

        public string Name => "commInterface";
        
        public void Build()
        {
            this._targetElement.AddProfile(this.Name);
            this._typeElement = _elementManager.CreateDataElement<string>(_targetElement,
                "type",
                element => this._typeFunc(),
                null,
                true, false,
                format: new StringFormat(new StringValuation(0, 100)));
        }
    }
}