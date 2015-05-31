using System;
using System.Net.Sockets;

namespace Tts.AsyncSocket
{
    public class DSCClientConnectedEventArgs : EventArgs
    {
        public Socket socket;

        public DSCClientConnectedEventArgs(Socket soc)
        {
            this.socket = soc;
        }
    }
}