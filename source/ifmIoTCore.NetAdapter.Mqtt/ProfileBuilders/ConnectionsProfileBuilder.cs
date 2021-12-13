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
        private readonly IElementManager _elementManager;

        public ConnectionsProfileBuilder(IElementManager elementManager, IDeviceElement root)
        {
            this._elementManager = elementManager;
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

            this._connectionsElement = _elementManager.CreateStructureElement(_root, this.Name, profiles: new List<string> {this.Name});
        }

        public void Dispose()
        {
            var connectionsElement = this._root.GetElementByProfile(this.Name);
            if (connectionsElement != null && connectionsElement.Subs?.Any() == false)
            {
                this._elementManager.RemoveElement(_root, connectionsElement);
            }
        }
    }
}