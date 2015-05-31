using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tts.TMusic.Common;

namespace Tts.TMusic.Service.Setup
{
    public partial class TMusicSetup : Form
    {
        public TMusicSetup()
        {
            InitializeComponent();
        }

        private async void btnSetup_Click(object sender, EventArgs e)
        {
            await Task.Run(() => 
            {
                string file = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Tts.TMusic.Service.exe");
                if (ServiceHelper.ServiceIsExisted("TMusicService")) 
                {
                    if (MessageBox.Show("TMusicService 服务已经安装,需要卸载重新安装吗？", "Ary you ok", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;

                    if (ServiceHelper.IsServiceStart("TMusicService"))
                        ServiceHelper.StopService("TMusicService");
                   
                    ServiceHelper.UnInstallService(file,"TMusicService");
                }

                ServiceHelper.InstallService(null, file, "TMusicService");
                MessageBox.Show("服务安装完成");
            });
        }
    }
}
