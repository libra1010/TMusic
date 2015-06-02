using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tts.TMusic.TPlayer;

namespace Tts.TMusic.Service.Controller
{
    /// <summary>
    /// 当前服务上下文
    /// </summary>
    public  class ServiceContext
    {
        private static ServiceContext _current = null;
        private static object _lock = new object();


        private ServiceContext(IPlayer player) 
        {
            Player = player;
        }

        public static void CreateContext(IPlayer palyer)
        {
            if (_current == null)
            {
                lock (_lock)
                {
                    if (_current == null)
                        _current = new ServiceContext(palyer);
                }
            }
        }


        public static ServiceContext Current 
        {
            get { return _current; }
        }



        public IPlayer Player { get; private set; }

        
    }
}
