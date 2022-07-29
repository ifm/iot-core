namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders
{
    using System;
    using Elements;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class ConnectionsProfileBuilder
    {
        private IBaseElement _connectionsElement;
        private readonly IDeviceElement _root;

        public ConnectionsProfileBuilder(IDeviceElement root)
        {
            this._root = root ?? throw new ArgumentNullException(nameof(root));
        }

        public string Name => "connections";
        public IBaseElement ConnectionsElement => this._connectionsElement;

        public void Build()
        {
            var connectionsElement = this._root.GetElementByProfile(this.Name);
            if (connectionsElement != null)
            {
                this._connectionsElement = connectionsElement;
                return;
            }

            this._connectionsElement = _root.AddChild(new StructureElement(this.Name, profiles: new List<string> {this.Name}), true);
        }

        public void Dispose()
        {
            var connectionsElement = this._root.GetElementByProfile(this.Name);
            if (connectionsElement != null && connectionsElement.Subs?.Any() == false)
            {
                _root.RemoveChild(connectionsElement);
            }
        }
    }
}