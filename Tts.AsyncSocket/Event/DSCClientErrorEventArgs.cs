using System;
using System.Net.Sockets;

namespace Tts.AsyncSocket
{
    public class DSCClientErrorEventArgs : EventArgs
    {
        public SocketException exception;

        public DSCClientErrorEventArgs(SocketException e)
        {
            this.exception = e;
        }
    }
}