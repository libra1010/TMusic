using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Tts.TMusic.Service
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string [] args)
        {
            if (args != null && !string.IsNullOrEmpty(args.FirstOrDefault(p => p == "-C")))
            {
                new TMusic().Start(args);
                Console.ReadLine();
                return;
            }
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new TMusic() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
