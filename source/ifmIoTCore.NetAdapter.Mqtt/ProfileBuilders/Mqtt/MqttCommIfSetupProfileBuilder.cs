namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.Mqtt
{
    using System;
    using System.Collections.Generic;
    using Elements;
    using Elements.Formats;
    using Elements.Valuations;
    using Utilities;

    internal sealed class MqttCommIfSetupProfileBuilder : NotifyPropertyChangedBase
    {
        private readonly IBaseElement _targetElement;
        private IDataElement<int> _qosElement;
        private IDataElement<string> _versionElement;
        private int _qos;
        private readonly IElementManager _elementManager;


        /// <summary>
        /// Initializes a new instance of <see cref="MqttCommIfSetupProfileBuilder"/>.
        /// </summary>
        /// <param name="elementManager">The element manager</param>
        /// <param name="targetElement">The targetelement which contains the mqtt specific commifsetup profile.</param>
        public MqttCommIfSetupProfileBuilder(IElementManager elementManager, IBaseElement targetElement)
        {
            this._elementManager = elementManager;
            this._targetElement = targetElement ?? throw new ArgumentNullException(nameof(targetElement));
        }

        /// <summary>
        /// Gets the name of the profile.
        /// </summary>
        public string Name { get; } = "mqttsetup";

        /// <summary>
        /// Gets or sets the quality of service property.
        /// </summary>
        public int Qos
        {
            get => this._qos;
            set
            {
                if (value == this._qos) return;
                this.RaisePropertyChanging();
                this._qos = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Builds the profile into the tree.
        /// </summary>
        public void Build()
        {
            this._qosElement = _elementManager.CreateDataElement<int>(_targetElement,
                "QoS",
                element => this.Qos,
                (element, i) =>
                {
                    if (this.Qos == i)
                    {
                        return;
                    }

                    this.Qos = i;
                }, 
                true, true,
                profiles: new List<string>{ "parameter" });
            // ToDo: Add a datachanged event?

            this._versionElement = _elementManager.CreateDataElement<string>(_targetElement,
                "version",
                element => "3.1.1", 
                null, 
                true, false, 
                format: new StringFormat(new StringValuation(0, 100), ns: "JSON"));
        }

        public void Dispose()
        {
            _elementManager.RemoveElement(this._targetElement, this._qosElement);
            _elementManager.RemoveElement(this._targetElement, this._versionElement);
        }
    }
}