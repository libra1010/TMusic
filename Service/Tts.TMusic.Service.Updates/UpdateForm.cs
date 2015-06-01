using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Tts.TMusic.Service.Updates
{
    public partial class UpdateForm : Form
    {
        private static DirectoryInfo _di = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
        private string _mPath = _di.Parent.FullName;

        private bool _downEnd = true;
        public UpdateForm()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 执行DOS命令，返回DOS命令的输出
        /// </summary>
        /// <param name="dosCommand">dos命令</param>
        /// <param name="milliseconds">等待命令执行的时间（单位：毫秒），
        /// 如果设定为0，则无限等待</param>
        /// <returns>返回DOS命令的输出</returns>
        public static string ExecuteCMD(string command, int seconds)
        {
            string output = ""; //输出字符串
            if (command != null && !command.Equals(""))
            {
                Process process = new Process();//创建进程对象
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令
                startInfo.Arguments = "/C " + command;//“/C”表示执行完命令后马上退出
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动
                startInfo.RedirectStandardInput = false;//不重定向输入
                startInfo.RedirectStandardOutput = true; //重定向输出
                startInfo.CreateNoWindow = true;//不创建窗口
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())//开始进程
                    {
                        if (seconds == 0)
                        {
                            process.WaitForExit();//这里无限等待进程结束
                        }
                        else
                        {
                            process.WaitForExit(seconds); //等待进程结束，等待时间为指定的毫秒
                        }
                        output = process.StandardOutput.ReadToEnd();//读取进程的输出
                    }
                }
                catch
                {
                }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
            return output;
        }

        protected async override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            string downFile = System.IO.Path.Combine(_mPath, "last.zip");
            var _config = ConfigData.LoadConfig();
            if (_config == null || string.IsNullOrWhiteSpace(_config.DownloadAddr))
            {
                lsbMsg.Items.Add("没有配置更新地址.");
                return;
            }

            System.IO.FileInfo f = new FileInfo(downFile);
            if (f.Exists)
                f.Delete();
            System.Net.WebClient c = new System.Net.WebClient();
            c.DownloadFileCompleted += c_DownloadFileCompleted;
            _downEnd = false;
            c.DownloadFileAsync(new Uri(_config.DownloadAddr), downFile);
            lsbMsg.Items.Add("文件下载中.");

            while (true)
            {
                if (_downEnd)
                    break;
                await Task.Delay(800);
                lsbMsg.Items.Add("文件下载中..");
            }

            lsbMsg.Items.Add("文件下载完成..准备卸载服务..");
            UnInstallService();
            //string file = System.IO.Path.Combine(_mPath, "Tts.TMusic.Service.exe");
            //if (ServiceHelper.ServiceIsExisted("TMusicService"))
            //{
            //    if (ServiceHelper.IsServiceStart("TMusicService"))
            //    {
            //        lsbMsg.Items.Add("开始停止服务..");
            //       //ServiceHelper.StopService("TMusicService");
            //        ExecuteCMD("net start TMusicService", 0);
            //        lsbMsg.Items.Add("服务已停止..");
            //    }

            //    lsbMsg.Items.Add("开始卸载服务..");
            //    ExecuteCMD("sc delete TMusicService", 0);
            //   // ServiceHelper.UnInstallService(file, "TMusicService");
            //    lsbMsg.Items.Add("服务卸载完成..");
            //    await Task.Delay(2000);
            //}

            lsbMsg.Items.Add("开始备份文件..");
            MoveFile();
            lsbMsg.Items.Add("文件备份完成..准备解压文件");
            StringBuilder removeBat = new StringBuilder("timeout /t 5");
           
            try
            {
                using (ZipArchive zip = ZipFile.OpenRead(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, downFile)))
                {
                    string exPath = System.IO.Path.Combine(_mPath, "_tmp_" + Guid.NewGuid().ToString("N"));
                    DirectoryInfo diEx = new DirectoryInfo(exPath);
                    if (diEx.Exists)
                        diEx.Delete(true);
                    diEx.Create();
                    zip.ExtractToDirectory(exPath);
                    foreach (var thisexf in diEx.GetFiles())
                    {
                        string target = System.IO.Path.Combine(_mPath, thisexf.Name);
                        removeBat.AppendLine().Append("move /y ").Append(thisexf.FullName).Append("  ").Append(target);
                        //if (System.IO.File.Exists(target))
                        //{
                        //      System.IO.File.Delete(target);
                        //}

                        //thisexf.MoveTo(target);
                    }                   
                    removeBat.AppendLine().Append("rd /q /s ").Append(diEx.FullName);
                  
                    diEx.Delete(true);
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(Path.Combine(_mPath, "log.txt"), ex.Message);
            }


            string batPath = System.IO.Path.Combine(_mPath, Guid.NewGuid().ToString("N") + ".bat");
            System.IO.File.WriteAllText(batPath, removeBat.ToString());
            File.SetAttributes(batPath, FileAttributes.Hidden);
            //ExecuteCMD(batPath, 0);
          //  System.IO.File.Delete(batPath);


            //lsbMsg.Items.Add("解压完成，准备安装...");
            //InstallService();
          
            ////ServiceHelper.InstallService(null, file, "TMusicService");
            //lsbMsg.Items.Add("安装完成..");
            //await Task.Delay(800);
            //lsbMsg.Items.Add("安装完成..");
            InstallService(batPath);
            this.Close();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void UnInstallService() 
        {
            System.Diagnostics.Process proc = new Process();
            proc.StartInfo.FileName = System.IO.Path.Combine(_mPath,"Tts.TMusic.Service.Setup.exe");
            proc.StartInfo.Arguments = "-U";
            proc.Start();
            proc.WaitForExit();
        }

        private void InstallService(string cmdPath) 
        {
            System.Diagnostics.Process proc = new Process();
            proc.StartInfo.FileName = System.IO.Path.Combine(_mPath, "Tts.TMusic.Service.Setup.exe");
            proc.StartInfo.Arguments = "-A " + cmdPath;
            proc.Start();
           // proc.WaitForExit();
        }

  
      

        private void MoveFile()
        {
            var files = (from f in (_di.Parent.GetFiles())
                         where f.Name.EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase) || f.Name.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)
                         select f).ToArray();

            string file = System.IO.Path.Combine(_mPath, DateTime.Now.ToString("yyyy_MM_dd") + ".zip");
            int i = 0;
            while (true)
            {
                if (System.IO.File.Exists(file))
                {
                    file = System.IO.Path.Combine(_mPath, DateTime.Now.ToString("yyyy_MM_dd") + "_" + i + ".zip");
                    i++;
                }
                else
                {
                    break;
                }
            }
            using (ZipArchive zip = ZipFile.Open(file, ZipArchiveMode.Create))
            {
                foreach (var f in files)
                {
                    zip.CreateEntryFromFile(f.FullName, f.Name);
                }
            }
        }



        void c_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _downEnd = true;

        }
    }
}
