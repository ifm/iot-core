namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.Mqtt
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Elements;
    using Elements.Formats;
    using Elements.Valuations;

    internal sealed class MqttCommIfSetupProfileBuilder : NotifyPropertyChangedBase
    {
        private readonly IBaseElement _targetElement;
        private IDataElement _qosElement;
        private IDataElement _versionElement;
        private int _qos;

        /// <summary>
        /// Initializes a new instance of <see cref="MqttCommIfSetupProfileBuilder"/>.
        /// </summary>
        /// <param name="targetElement">The targetelement which contains the mqtt specific commifsetup profile.</param>
        public MqttCommIfSetupProfileBuilder(IBaseElement targetElement)
        {
            this._targetElement = targetElement ?? throw new ArgumentNullException(nameof(targetElement));
        }

        /// <summary>
        /// Gets the name of the profile.
        /// </summary>
        public string Name = "mqttsetup";

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
            this._qosElement = new DataElement<int>("QoS",
                element => this.Qos,
                (element, i) =>
                {
                    if (this.Qos == (int)i)
                    {
                        return;
                    }

                    this.Qos = (int)i;
                }, 
                profiles: new List<string>{ "parameter" });
            // ToDo: Add a datachanged event?

            _targetElement.AddChild(_qosElement);

            this._versionElement = new ReadOnlyDataElement<string>("version",
                element => "3.1.1",
                format: new StringFormat(new StringValuation(0, 100), ns: "JSON"));

            _targetElement.AddChild(_versionElement);
        }

        public void Dispose()
        {
            if (_qosElement != null) _targetElement.RemoveChild(_qosElement);
            if (_versionElement != null) _targetElement.RemoveChild(_versionElement);
        }
    }
}