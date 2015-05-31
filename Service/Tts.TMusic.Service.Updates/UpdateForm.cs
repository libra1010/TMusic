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
using Tts.TMusic.Common;

namespace Tts.TMusic.Service.Updates
{
    public partial class UpdateForm : Form
    {
        private bool _downEnd = true;
        public UpdateForm()
        {
            InitializeComponent();
        }

        protected async override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string downFile = "last.zip";

            System.Net.WebClient c = new System.Net.WebClient();
            c.DownloadFileCompleted += c_DownloadFileCompleted;
            _downEnd = false;
            c.DownloadFileAsync(new Uri(""), downFile);
            lsbMsg.Items.Add("文件下载中.");

            while (true)
            {
                if (_downEnd)
                    break;
                await Task.Delay(800);
                lsbMsg.Items.Add("文件下载中..");
            }

            lsbMsg.Items.Add("文件下载完成..准备卸载服务..");
            string file = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Tts.TMusic.Service.exe");
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
            }

            lsbMsg.Items.Add("开始备份文件..");
            MoveFile();
            lsbMsg.Items.Add("文件备份完成..准备解压文件");
            using (ZipArchive zip = ZipFile.OpenRead(downFile))
            {
                zip.ExtractToDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
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
            var files = (from f in (System.IO.Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory))
                        where f.EndsWith(".exe",StringComparison.CurrentCultureIgnoreCase) || f.EndsWith(".dll",StringComparison.CurrentCultureIgnoreCase)
                             select f).ToArray();
            
            string file =   System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyy_MM_dd"),".zip");
            int i = 0;
            while(true)
            {
                if(System.IO.File.Exists(file))
                {
                    file =   System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyy_MM_dd"),i.ToString(),".zip");
                    i++;
                }
                else
                {
                    break;
                }
            }
            using(ZipArchive zip = ZipFile.Open(file, ZipArchiveMode.Create))
            {
                foreach(var f in files)
                {
                     zip.CreateEntryFromFile(f,f);
                }
               
            }
        }

       

        void c_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _downEnd = true;
         
        }
    }
}
