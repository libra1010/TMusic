using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tts.AsyncSocket.AsyncSocketServer;

namespace Tts.TMusic.Service.Server
{
    public class ServerListen
    {
        private IServerSocket _server;
        
        public void Start() 
        {
            _server = new IServerSocket(10, 1024 * 4);
            _server.Init();
            _server.Start("0.0.0.0", 2112);
            _server.OnClientConnect += _server_OnClientConnect;
            _server.OnClientRead += _server_OnClientRead;
            _server.OnClientDisconnect += _server_OnClientDisconnect;
        }

        void _server_OnClientDisconnect(object sender, AsyncSocket.AsyncUserToken e)
        {
            throw new NotImplementedException();
        }

        void _server_OnClientRead(object sender, AsyncSocket.AsyncUserToken e)
        {
            throw new NotImplementedException();
        }

        void _server_OnClientConnect(object sender, AsyncSocket.AsyncUserToken e)
        {
            throw new NotImplementedException();
        }
    }
}
