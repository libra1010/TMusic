using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tts.TMusic.Service.Updates
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string [] args)
        {
            //if (args == null || args.Length <= 0)
            //{
            //    System.IO.File.WriteAllText("c:\\b.txt", "com");
            //    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("Tts.TMusic.Service.Updates.exe");
            //    info.Arguments = " /U";
            //    info.UseShellExecute = false;

            //    System.Diagnostics.Process.Start(info);
            //    System.IO.File.WriteAllText("c:\\c.txt", "com");
            //    System.Diagnostics.Process.GetCurrentProcess().Kill();
            //    System.IO.File.WriteAllText("c:\\d.txt", "com");
            //}
            //System.IO.File.WriteAllText("c:\\a.txt", "com");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UpdateForm());
        }
    }
}
