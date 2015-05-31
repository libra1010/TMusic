using System;
using System.Net.Sockets;

namespace Tts.AsyncSocket
{
    internal class DSCClientDisconnectedEventArgs : EventArgs
    {
        public Socket socket;

        public DSCClientDisconnectedEventArgs(Socket soc)
        {
            this.socket = soc;
        }
    }
}