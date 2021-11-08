using System;

namespace RTSPViewer.CustomEventArgs
{
    public class ConnectedStatusEventArgs : EventArgs
    {
        public ConnectedStatusEventArgs()
        {

        }
        public ConnectedStatusEventArgs(bool connectionStatus)
        {
            Connected = connectionStatus;
        }
        public bool Connected { get; set; }
    }
}
