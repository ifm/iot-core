namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.Mqtt
{
    using Utilities;

    public class MqttCommChannelSetup : NotifyPropertyChangedBase
    {
        private string _brokerIp;
        private int _brokerPort;
        private string _commandTopic;
        private string _replyTopic;

        public string BrokerIp
        {
            get => this._brokerIp;
            set
            {
                if (this._brokerIp == value) return;
                this.RaisePropertyChanging();
                this._brokerIp = value;
                this.RaisePropertyChanged();
            }
        }

        public int BrokerPort
        {
            get => this._brokerPort;
            set
            {
                if (this._brokerPort == value) return;
                this.RaisePropertyChanging();
                this._brokerPort = value;
                this.RaisePropertyChanged();
            }
        }

        public string CommandTopic
        {
            get => this._commandTopic;
            set
            {
                if (this._commandTopic == value) return;
                this.RaisePropertyChanging();
                this._commandTopic = value;
                this.RaisePropertyChanged();
            }
        }

        public string ReplyTopic
        {
            get => this._replyTopic;
            set
            {
                if (this._replyTopic == value) return;
                this.RaisePropertyChanging();
                this._replyTopic = value;
                this.RaisePropertyChanged();
            }
        }
    }
}