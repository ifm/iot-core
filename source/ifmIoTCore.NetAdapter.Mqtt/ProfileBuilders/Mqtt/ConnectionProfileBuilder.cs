namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.Mqtt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using Elements;
    using MQTTnet.Protocol;
    using RunControlState;
    using Utilities;

    public sealed class MqttConnectionProfileBuilder : NotifyPropertyChangedBase
    {
        private readonly IElementManager _elementManager;
        private readonly ILogger _log;
        private readonly IPEndPoint _ipEndPoint;
        private readonly MqttServerNetAdapter _serverNetAdapter;
        private readonly RunControlStateMachine _commInterfaceRunControlStateMachine;
        private readonly State _init = new State("init");
        private readonly State _running = new State("running");
        private readonly State _stopped = new State("stopped");
        private readonly State _error = new State("error");
        private readonly string _commandTopicOnStart;
        private readonly IDeviceElement _deviceElement;

        private IStructureElement _commInterfaceStructureElement;
        private CommChannelSetupProfileProvider _commChannelProfileProvider;
        private MqttCommIfSetupProfileBuilder _mqttCommIfSetupProfileBuilder;
        private string _preset = "stopped";
        private IDataElement<string> _runControlElement;


        /// <summary>
        /// Initialized a new instance of <see cref="MqttConnectionProfileBuilder"/>.
        /// </summary>
        /// <param name="elementManager">The element manager</param>
        /// <param name="deviceElement">The root element of the iotcore in which the element resides, that the profile will be build on.</param>
        /// <param name="serverNetAdapter">The mqttservernetadapter which </param>
        /// <param name="ipEndPoint">The endpoint to which this adapter will connect to.</param>
        /// <param name="commandTopic">The topic on which this adapter will listen for commands on the broker at <paramref name="ipEndPoint"/>.</param>
        /// <param name="logger">The logger instance to use.</param>
        public MqttConnectionProfileBuilder(IElementManager elementManager, IDeviceElement deviceElement, MqttServerNetAdapter serverNetAdapter, IPEndPoint ipEndPoint, string commandTopic, ILogger logger)
        {
            this._elementManager = elementManager;
            this._log = logger ?? new NullLogger();
            this._deviceElement = deviceElement;
            this._serverNetAdapter = serverNetAdapter;
            this._ipEndPoint = ipEndPoint;
            this._commandTopicOnStart = commandTopic ?? "cmdTopic/#";
            _commInterfaceRunControlStateMachine = new RunControlStateMachine(logger);
            serverNetAdapter.StateChanged += this.OnMqttServerNetAdapterStateChanged;

            this.InitializeStateMachine();
        }

        

        /// <summary>
        /// This event will fire, when runcontrol->start of this mqttconnection is called.
        /// </summary>
        public event EventHandler<RunControlEventType> RunControlEvent;

        /// <summary>
        /// Gets the name of the profile.
        /// </summary>
        public string Name { get; } = "mqttconnection";

        /// <summary>
        /// Gets the current value of replytopic.
        /// </summary>
        public string ReplyTopic => this._commChannelProfileProvider.MqttCommChannelSetup.ReplyTopic;

        public Uri Uri => new UriBuilder(MqttServerNetAdapter.ProtocolName,
            this._commChannelProfileProvider.MqttCommChannelSetup.BrokerIp,
            this._commChannelProfileProvider.MqttCommChannelSetup.BrokerPort).Uri;

        public string CommandTopic => this._commChannelProfileProvider.MqttCommChannelSetup.CommandTopic;

        /// <summary>
        /// Builds the profile into the tree.
        /// </summary>
        public void Build()
        {
            this._commInterfaceStructureElement = _elementManager.CreateStructureElement(_deviceElement.GetElementByIdentifier("connections"), $"mqttconnection_{ Guid.NewGuid() }");

            this._runControlElement = _elementManager.CreateDataElement<string>(_commInterfaceStructureElement,
                "status",
                element => this._commInterfaceRunControlStateMachine.CurrentState?.Value?.ToString() ?? "unknown state",
                null, 
                true, false,
                profiles: new List<string> { "runcontrol" });
            _runControlElement.DataChangedEventElement = _elementManager.CreateEventElement(_runControlElement, Identifiers.DataChanged);

            var runControlProfileBuilder = new RunControlProfileBuilder(_elementManager,
                targetElement: this._runControlElement,
                startElement: _elementManager.CreateActionServiceElement(_runControlElement,
                    "start",
                    (element, i) =>
                        {
                            this.RaiseRunControlEvent(RunControlEventType.Started);
                        }),
                stopElement: _elementManager.CreateActionServiceElement(_runControlElement,
                    "stop",
                    (element, i) =>
                        {
                            this.RaiseRunControlEvent(RunControlEventType.Stopped);
                        }),
                presetElement: _elementManager.CreateDataElement<string>(_runControlElement,
                    "preset",
                element => this._preset, 
                (element, s) =>
                        {
                            if (this._preset == s) return;
                            switch (s)
                            {
                                case "running":
                                    this._preset = s;
                                    return;
                                case "stopped":
                                    this._preset = s;
                                    return;
                                default:
                                    this._log.Error($"Unknown value : '{s}' passed for preset. Valid values are 'running' and 'stopped'.");
                                    return;
                            }
                                
                        },
                true, true),
                // ToDo: Add a datachanged event?

                suspendElement: _elementManager.CreateActionServiceElement(_runControlElement,
                    "suspend",
                    (element, i) =>
                        {
                            this.RaiseRunControlEvent(RunControlEventType.Suspend);
                        }),
                resetElement: _elementManager.CreateActionServiceElement(_runControlElement,
                    "reset",
                    (element, i) =>
                    {
                        if (this._commInterfaceRunControlStateMachine.IsCurrentState(this._stopped)) return;
                        if (this._commInterfaceRunControlStateMachine.IsCurrentState(this._running)) return;

                        this.RaiseRunControlEvent(RunControlEventType.Reset);
                    }));

            runControlProfileBuilder.Build();

            var commIfSetupElement = _elementManager.CreateStructureElement(_commInterfaceStructureElement,
                "mqttsetup", 
                profiles: new List<string>{ "mqttsetup" });
            this._mqttCommIfSetupProfileBuilder = new MqttCommIfSetupProfileBuilder(_elementManager, commIfSetupElement);
            this._mqttCommIfSetupProfileBuilder.Build();
            this._mqttCommIfSetupProfileBuilder.PropertyChanging += CommIfSetupProfileBuilderOnPropertyChanging;
            this._mqttCommIfSetupProfileBuilder.PropertyChanged += CommIfSetupProfileBuilderOnPropertyChanged;

            this._commChannelProfileProvider = new CommChannelSetupProfileProvider(_elementManager, this._commInterfaceStructureElement, this._serverNetAdapter, this._ipEndPoint, this._commandTopicOnStart, this._log);
            
            this._commChannelProfileProvider.Build();

            var commChannel = this._commInterfaceStructureElement.GetElementByIdentifier("commchannel");
            var commInterfaceProfileBuilder = new CommInterfaceProfileBuilder(_elementManager, this._commInterfaceStructureElement, this._runControlElement, () => "mqtt", commIfSetupElement, commChannel);
            commInterfaceProfileBuilder.Build();

            var connectionsProfileBuilder = new ConnectionsProfileBuilder(_elementManager, this._deviceElement);
            connectionsProfileBuilder.Build();

            _commInterfaceRunControlStateMachine.PropertyChanged += CommInterfaceRunControlStateMachineOnPropertyChanged;

           // var connectionsElement = connectionsProfileBuilder.ConnectionsElement;
            //_elementManager.AddChild(connectionsElement, this._commInterfaceStructureElement);
        }

        private void CommInterfaceRunControlStateMachineOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _runControlElement?.RaiseDataChanged();
        }

        public MqttQualityOfServiceLevel GetQualityOfService()
        {
            MqttQualityOfServiceLevel result;

            switch (this._mqttCommIfSetupProfileBuilder.Qos)
            {
                case 0:
                    result = MqttQualityOfServiceLevel.AtMostOnce;
                    break;
                case 1:
                    result = MqttQualityOfServiceLevel.AtLeastOnce;
                    break;
                case 2:
                    result = MqttQualityOfServiceLevel.ExactlyOnce;
                    break;
                default:
                    result = MqttQualityOfServiceLevel.AtLeastOnce;
                    break;
            }

            return result;
        }

        public void Dispose()
        {
            _commInterfaceRunControlStateMachine.PropertyChanged -= CommInterfaceRunControlStateMachineOnPropertyChanged;
            this._serverNetAdapter.StateChanged -= this.OnMqttServerNetAdapterStateChanged;
            this._mqttCommIfSetupProfileBuilder.PropertyChanged -= this.CommIfSetupProfileBuilderOnPropertyChanged;
            this._mqttCommIfSetupProfileBuilder.PropertyChanging -= this.CommIfSetupProfileBuilderOnPropertyChanging;

            var connectionsElement = this._deviceElement.GetElementByProfile("connections");
            if (connectionsElement != null)
            {
                _elementManager.RemoveElement(connectionsElement, this._commInterfaceStructureElement);
            }
        }

        private void CommIfSetupProfileBuilderOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(MqttCommIfSetupProfileBuilder.Qos)) return;
            this.RaisePropertyChanged(nameof(MqttCommIfSetupProfileBuilder.Qos));
        }

        private void CommIfSetupProfileBuilderOnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (e.PropertyName != nameof(MqttCommIfSetupProfileBuilder.Qos)) return;
            this.RaisePropertyChanging(nameof(MqttCommIfSetupProfileBuilder.Qos));
        }

        private void RaiseRunControlEvent(RunControlEventType runControlEvent)
        {
            try
            {
                this.RunControlEvent.Raise(this, runControlEvent, true);
            }
            catch (Exception e)
            {
                this._log.Error(e.Message);

                if (e is AggregateException aggregateException)
                {
                    foreach (var exception in aggregateException.InnerExceptions)
                    {
                        this._log.Error(exception.Message);
                    }
                }

                this.SetState(this._error);
            }
        }

        public void OnMqttServerNetAdapterStateChanged(object sender, MqttServerNetAdapterState e)
        {
            switch (e)
            {
                case MqttServerNetAdapterState.Started:
                    if (this._commInterfaceRunControlStateMachine.IsCurrentState(this._stopped))
                    {
                        this.SetState(this._init);
                    }
                    this.SetState(this._running);
                    break;
                case MqttServerNetAdapterState.Stopped:
                    this.SetState(this._stopped);
                    break;
            }
        }

        private void SetState(State state)
        {
            if (this._commInterfaceRunControlStateMachine.TrySetState(state))
            {
                this._runControlElement.RaiseDataChanged();
            }
        }

        private void InitializeStateMachine()
        {
            this._commInterfaceRunControlStateMachine.RegisterStateTransition(new StateTransition(this._init, this._running));
            this._commInterfaceRunControlStateMachine.RegisterStateTransition(new StateTransition(this._init, this._error));
            this._commInterfaceRunControlStateMachine.RegisterStateTransition(new StateTransition(this._running, this._error));
            this._commInterfaceRunControlStateMachine.RegisterStateTransition(new StateTransition(this._running, this._init));
            this._commInterfaceRunControlStateMachine.RegisterStateTransition(new StateTransition(this._running, this._running));
            this._commInterfaceRunControlStateMachine.RegisterStateTransition(new StateTransition(this._running, this._stopped));
            this._commInterfaceRunControlStateMachine.RegisterStateTransition(new StateTransition(this._stopped, this._stopped));
            this._commInterfaceRunControlStateMachine.RegisterStateTransition(new StateTransition(this._stopped, this._init));

            this._commInterfaceRunControlStateMachine.SetState(this._init);
        }
    }
}