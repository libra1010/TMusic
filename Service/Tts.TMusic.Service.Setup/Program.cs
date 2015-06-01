using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tts.TMusic.Common;

namespace Tts.TMusic.Service.Setup
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string [] args)
        {
            if (args != null && args.Length == 1)
            {
                if (args[0] == "-A")
                {
                    string file = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Tts.TMusic.Service.exe");
                    if (ServiceHelper.ServiceIsExisted("TMusicService"))
                    {
                        //if (MessageBox.Show("TMusicService 服务已经安装,需要卸载重新安装吗？", "Ary you ok", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        //    return;

                        if (ServiceHelper.IsServiceStart("TMusicService"))
                            ServiceHelper.StopService("TMusicService");

                        ServiceHelper.UnInstallService(file, "TMusicService");
                    }

                    ServiceHelper.InstallService(null, file, "TMusicService");
                }               
                return;
            }
            else if (args != null && args.Length == 2)
            {
                Stopwatch timer = Stopwatch.StartNew();
                CMDHelper.ExecuteCMD(args[1], 0);
                if (timer.ElapsedMilliseconds < 1000)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        System.Threading.Thread.Sleep(999);
                    }
                }
                System.IO.File.Delete(args[1]);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ServiceAdd());
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TMusicSetup());
        }
    }
}
