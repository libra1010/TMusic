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
            string file = System.IO.Path.Combine(_mPath, "Tts.TMusic.Service.exe");
            if (ServiceHelper.ServiceIsExisted("TMusicService"))
            {
                if (ServiceHelper.IsServiceStart("TMusicService"))
                {
                    lsbMsg.Items.Add("开始停止服务..");
                    ServiceHelper.StopService("TMusicService");
                    lsbMsg.Items.Add("服务已停止..");
                }

                lsbMsg.Items.Add("开始卸载服务..");
                ServiceHelper.UnInstallService(file, "TMusicService");
                lsbMsg.Items.Add("服务卸载完成..");
                await Task.Delay(2000);
            }

            lsbMsg.Items.Add("开始备份文件..");
            MoveFile();
            lsbMsg.Items.Add("文件备份完成..准备解压文件");
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
                        if (System.IO.File.Exists(target))
                        {                           
                            System.IO.File.Delete(target);
                        }

                        thisexf.MoveTo(target);
                    }
                    diEx.Delete(true);
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(Path.Combine(_mPath, "log.txt"), ex.Message);
            }

            lsbMsg.Items.Add("解压完成，准备安装...");
            ServiceHelper.InstallService(null, file, "TMusicService");
            lsbMsg.Items.Add("安装完成..");
            await Task.Delay(800);
            lsbMsg.Items.Add("安装完成..");
            this.Close();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
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
