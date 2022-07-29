namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.Mqtt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using Common;
    using Elements;
    using Elements.Formats;
    using Elements.Valuations;
    using Logger;
    using RunControlState;

    internal class CommChannelSetupProfileProvider : NotifyPropertyChangedBase
    {
        private readonly ILogger _log;
        private readonly IBaseElement _mqttRoot;
        private readonly MqttServerNetAdapter _serverNetAdapter;
        private readonly RunControlStateMachine _commChannelRunControlStateMachine;
        private readonly State _init = new State("init");
        private readonly State _running = new State("running");
        private readonly State _stopped = new State("stopped");
        private readonly State _error = new State("error");
        
        private IDataElement _brokerIpElement;
        private IDataElement _brokerPortElement;
        private IDataElement _brokerTopicElement;
        private IStructureElement _commChannelElement;
        private IDataElement _replyTopicElement;
        private IStructureElement _commChannelSetupElement;
        private string _preset = "stopped";
        private IDataElement _statusDataElement;
        private readonly IElementManager _elementManager;

        public CommChannelSetupProfileProvider(IElementManager elementManager,
            IBaseElement mqttRoot,
            MqttServerNetAdapter serverNetAdapter, 
            IPEndPoint ipEndPoint,
            string commandTopicOnStart,
            ILogger logger)
        {
            this._elementManager = elementManager;
            this._log = logger;
            this._mqttRoot = mqttRoot ?? throw new ArgumentNullException(nameof(mqttRoot));
            this._commChannelRunControlStateMachine = new RunControlStateMachine(logger);
            this._serverNetAdapter = serverNetAdapter ?? throw new ArgumentNullException(nameof(serverNetAdapter));
            this._serverNetAdapter.StateChanged += this.OnMqttServerNetAdapterStateChanged;
            
            this.MqttCommChannelSetup = new MqttCommChannelSetup
            {
                BrokerIp = ipEndPoint.Address.ToString(), 
                BrokerPort = ipEndPoint.Port, 
                CommandTopic = commandTopicOnStart, 
                ReplyTopic = "replyTopic"
            };

            this.MqttCommChannelSetup.PropertyChanged += MqttCommandChannelSetupOnPropertyChanged;
            this.MqttCommChannelSetup.PropertyChanging += MqttCommandChannelSetupOnPropertyChanging;

            this.InitializeStateMachine();
            this._commChannelRunControlStateMachine.PropertyChanged += CommChannelRunControlStateMachineOnPropertyChanged;
        }

        public string Name { get; } = "cmdChannel";
        public MqttCommChannelSetup MqttCommChannelSetup { get; }

        public void Build()
        {
            if (this._commChannelElement == null)
            {
                this._commChannelElement = new StructureElement("mqttcmdchannel", profiles: new List<string> { "commchannel" });
                _mqttRoot.AddChild(_commChannelElement);

                this._statusDataElement = new ReadOnlyDataElement<string>("status",
                    element => this._commChannelRunControlStateMachine.CurrentState?.Value?.ToString() ?? "unknown state",
                    createDataChangedEventElement: true);

                _commChannelElement.AddChild(_statusDataElement);

                this._commChannelRunControlStateMachine.PropertyChanged += CommChannelRunControlStateMachineOnPropertyChanged;

                var runControlProfileBuilder = new RunControlProfileBuilder(targetElement: this._statusDataElement,
                    startElement: new ActionServiceElement("start",
                        (element, i) =>
                        {
                            try
                            {
                                this._serverNetAdapter.Start();
                            }
                            catch
                            {
                                this._commChannelRunControlStateMachine.TrySetState(this._error, "start");
                                this._log.Error($"Failed to run 'start' of runcontrol in commchannel at: '{this._commChannelElement.Address}'.");
                                throw;
                            }
                        })                        
                    ,
                    stopElement: new ActionServiceElement("stop", 
                        (element, i) =>
                        {
                            try
                            {
                                this._serverNetAdapter.Stop();
                            }
                            catch
                            {
                                this._commChannelRunControlStateMachine.TrySetState(this._error, "stop");
                                this._log.Error($"Failed to run 'stop' of runcontrol in commchannel at: '{this._commChannelElement.Address}'.");
                                throw;
                            }
                        }),
                    resetElement: new ActionServiceElement("reset",
                        (element, i) =>
                            {
                                if (this._commChannelRunControlStateMachine.IsCurrentState(this._stopped)) return;
                                if (this._commChannelRunControlStateMachine.IsCurrentState(this._running)) return;

                                this._log.Debug("Reset service called.");
                                try
                                {
                                    this._serverNetAdapter.Stop();
                                }
                                catch
                                {
                                    this._log.Error($"Failed to stop in reset service.");
                                }

                                this._commChannelRunControlStateMachine.TrySetState(this._init, "reset");
                            }),
                    presetElement: new DataElement<string>("preset",
                        element => this._preset));

                runControlProfileBuilder.Build();

                var typeElement = new ReadOnlyDataElement<string>("type", 
                    element => "mqtt");

                _commChannelElement.AddChild(typeElement);
            }

            if (this._commChannelSetupElement == null)
            {
                this._commChannelSetupElement = new StructureElement("mqttCmdChannelSetup", profiles:new List<string>{ "mqttcmdchannelsetup" });
                _commChannelElement.AddChild(_commChannelSetupElement);
            }

            if (this._brokerIpElement == null)
            {
                this._brokerIpElement = new DataElement<string>( "brokerIP",
                    element => this.MqttCommChannelSetup.BrokerIp,
                    (element, value) =>
                    {
                        if (this.MqttCommChannelSetup.BrokerIp == (string)value) return;
                        this.MqttCommChannelSetup.BrokerIp = (string)value;
                    },
                    format: new StringFormat(new StringValuation(0, 50), "utf-8", "JSON"),
                    profiles: new List<string> {"parameter"});
                // ToDo: Add a datachanged event?

                _commChannelSetupElement.AddChild(_brokerIpElement);

            }

            if (this._brokerPortElement == null)
            {
                this._brokerPortElement = new DataElement<int>("brokerPort",
                    element => this.MqttCommChannelSetup.BrokerPort,
                    (element, token) =>
                    {
                        if (this.MqttCommChannelSetup.BrokerPort == (int)token) return;
                        this.MqttCommChannelSetup.BrokerPort = (int)token;
                    }, 
                    format: new IntegerFormat(new IntegerValuation(0, 65535), "JSON"), 
                    profiles:new List<string>{ "parameter" });
                // ToDo: Add a datachanged event?

                _commChannelSetupElement.AddChild(_brokerPortElement);
            }

            if (this._brokerTopicElement == null)
            {
                this._brokerTopicElement = new DataElement<string>("cmdTopic", 
                    element => this.MqttCommChannelSetup.CommandTopic,
                    (element, value) =>
                    {
                        if (this.MqttCommChannelSetup.CommandTopic == (string)value) return;
                        this.MqttCommChannelSetup.CommandTopic = (string)value;
                    },
                    format: new StringFormat(new StringValuation(0, 50), "utf-8", "JSON"), 
                    profiles: new List<string> { "parameter" });
                // ToDo: Add a datachanged event?

                _commChannelSetupElement.AddChild(_brokerTopicElement);
            }

            if (this._replyTopicElement == null)
            {
                this._replyTopicElement = new DataElement<string>("defaultReplyTopic", 
                    element => this.MqttCommChannelSetup.ReplyTopic,
                    (element, value) =>
                    {
                        if (this.MqttCommChannelSetup.ReplyTopic == (string)value) return;
                        this.MqttCommChannelSetup.ReplyTopic = (string)value;
                    },
                    format: new StringFormat(new StringValuation(0, 50), "utf-8", "JSON"), profiles: new List<string> { "parameter" });
                // ToDo: Add a datachanged event?

                _commChannelSetupElement.AddChild(_replyTopicElement);
            }
        }
        
        public void Dispose()
        {
            this._commChannelRunControlStateMachine.PropertyChanged -= this.CommChannelRunControlStateMachineOnPropertyChanged;
            this._serverNetAdapter.StateChanged -= this.OnMqttServerNetAdapterStateChanged;
            this.MqttCommChannelSetup.PropertyChanged -= MqttCommandChannelSetupOnPropertyChanged;
            this.MqttCommChannelSetup.PropertyChanging -= MqttCommandChannelSetupOnPropertyChanging;

            this._commChannelSetupElement.RemoveChild(this._brokerIpElement);
            this._commChannelSetupElement.RemoveChild(this._brokerPortElement);
            this._commChannelSetupElement.RemoveChild(this._brokerTopicElement);
            this._commChannelSetupElement.RemoveChild(this._replyTopicElement);
            this._commChannelElement.RemoveChild(_commChannelSetupElement);
        }

        private void SetState(State nextState)
        {
            if (this._commChannelRunControlStateMachine.TrySetState(nextState))
            {
                this._statusDataElement.RaiseDataChanged();
            }
        }

        private void OnMqttServerNetAdapterStateChanged(object sender, MqttServerNetAdapterState state)
        {
            switch (state)
            {
                case MqttServerNetAdapterState.Started:
                    if (this._commChannelRunControlStateMachine.IsCurrentState(this._running)) return;
                    if (this._commChannelRunControlStateMachine.IsCurrentState(this._stopped))
                    {
                        this.SetState(this._init);
                    }
                    this.SetState(this._running);
                    
                    this._preset = "running";
                    break;
                case MqttServerNetAdapterState.Stopped:
                    this.SetState(this._stopped);
                    this._preset = "stopped";
                    break;
            }
        }

        private void MqttCommandChannelSetupOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.MqttCommChannelSetup.BrokerIp):

                    try
                    {
                        this._serverNetAdapter.Start();
                    }
                    catch
                    {
                        this._log.Error("Could not start netadapter.");
                    }

                    break;
                case nameof(this.MqttCommChannelSetup.BrokerPort):

                    try
                    {
                        this._serverNetAdapter.Start();
                    }
                    catch
                    {
                        this._log.Error("Could not start netadapter.");
                    }

                    break;
                case nameof(this.MqttCommChannelSetup.CommandTopic):

                    if (!string.IsNullOrEmpty(this.MqttCommChannelSetup.CommandTopic))
                    {
                        try
                        {
                            this._serverNetAdapter.Start();
                        }
                        catch
                        {
                            this._log.Error("Could not start netadapter.");
                        }
                    }

                    break;
                case nameof(this.MqttCommChannelSetup.ReplyTopic):
                    break;
            }
        }

        private void MqttCommandChannelSetupOnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.MqttCommChannelSetup.BrokerIp):

                    try
                    {
                        this._serverNetAdapter.Stop();
                    }
                    catch
                    {
                        this._log.Error("Could not stop netadapter.");
                    }

                    break;
                case nameof(this.MqttCommChannelSetup.BrokerPort):

                    try
                    {
                        this._serverNetAdapter.Stop();
                    }
                    catch
                    {
                        this._log.Error("Could not stop netadapter.");
                    }

                    break;
                case nameof(this.MqttCommChannelSetup.CommandTopic):

                    try
                    {
                        this._serverNetAdapter.Stop();
                    }
                    catch
                    {
                        this._log.Error("Could not stop netadapter.");
                    }

                    break;
                case nameof(this.MqttCommChannelSetup.ReplyTopic):
                    break;
            }
        }

        private void InitializeStateMachine()
        {
            this._commChannelRunControlStateMachine.RegisterStateTransition(new StateTransition(this._init, this._running, Enumerable.Empty<StateTransitionCondition>()));
            this._commChannelRunControlStateMachine.RegisterStateTransition(new StateTransition(this._init, this._error, Enumerable.Empty<StateTransitionCondition>()));
            this._commChannelRunControlStateMachine.RegisterStateTransition(new StateTransition(this._running, this._error, Enumerable.Empty<StateTransitionCondition>()));
            this._commChannelRunControlStateMachine.RegisterStateTransition(new StateTransition(this._running, this._init, Enumerable.Empty<StateTransitionCondition>()));
            this._commChannelRunControlStateMachine.RegisterStateTransition(new StateTransition(this._running, this._running, Enumerable.Empty<StateTransitionCondition>()));
            this._commChannelRunControlStateMachine.RegisterStateTransition(new StateTransition(this._running, this._stopped, Enumerable.Empty<StateTransitionCondition>()));
            this._commChannelRunControlStateMachine.RegisterStateTransition(new StateTransition(this._stopped, this._stopped, Enumerable.Empty<StateTransitionCondition>()));
            this._commChannelRunControlStateMachine.RegisterStateTransition(new StateTransition(this._stopped, this._init, Enumerable.Empty<StateTransitionCondition>()));

            this._commChannelRunControlStateMachine.SetState(this._init);
        }

        private void CommChannelRunControlStateMachineOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _statusDataElement?.RaiseDataChanged();

            if (e.PropertyName == nameof(RunControlStateMachine.CurrentState))
            {
                if (this._commChannelRunControlStateMachine.IsCurrentState(this._init))
                {
                    try
                    {
                        this._commChannelRunControlStateMachine.SetState(this._running);
                        this._serverNetAdapter.Start();
                    }
                    catch (Exception exception)
                    {
                        this._log.Error(exception.Message);
                        this._commChannelRunControlStateMachine.SetState(this._error);
                    }
                }
            }
        }
    }
}