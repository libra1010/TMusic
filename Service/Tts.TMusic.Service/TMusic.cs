﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Tts.TMusic.Service.Server;

namespace Tts.TMusic.Service
{
    public partial class TMusic : ServiceBase
    {
        public TMusic()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string name = System.Environment.MachineName;
            var win = new ServiceWindow();
            win.Name = "TMusic.Service."+name;
            win.Visible = false;
            win.Show();
            ServerListen listen = new ServerListen();
            listen.Start();
        }

        protected override void OnStop()
        {
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }
    }
}