using Common.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Tts.TMusic.Common;
using Tts.TMusic.Data;
using Tts.TMusic.Service.Server;

namespace Tts.TMusic.Service
{
    public partial class TMusic : ServiceBase
    {
        internal static ServiceConfig _config = null;
        public TMusic()
        {
            InitializeComponent();
        }

        ServiceWindow win = null;
        ServerListen listen = null;
        protected override void OnStart(string[] args)
        {
            _config = ConfigData.LoadConfig();
            EexcUpdate();
            string name = System.Environment.MachineName;
            win = new ServiceWindow();
            win.Name = "TMusic.Service."+name;
            win.Visible = false;
            win.Show();
            listen = new ServerListen();
            listen.Start(_config.Port);
        }

        private IScheduler sched = null;
        private void EexcUpdate() 
        {
            ILog log = LogManager.GetLogger(typeof(UpdateJob));

            log.Info("------- Initializing ----------------------");

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            sched = sf.GetScheduler();

            log.Info("------- Initialization Complete -----------");


            log.Info("------- Scheduling Job  -------------------");

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<UpdateJob>()
                .WithIdentity("job1", "group1")
                .Build();

          //  SimpleTriggerImpl trigger = new SimpleTriggerImpl("simpleTrig", "simpleGroup",-1, DateTime.Now.AddSeconds(5) - DateTime.Now);
           // IOperableTrigger trigger = new CronTriggerImpl("trigName", "group1", "0 1 * * * ?"); 
            // Trigger the job to run on the next round minute
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow().WithCronSchedule("0 0/1 * * * ?")
                .Build();

            // Tell quartz to schedule the job using our trigger
            sched.ScheduleJob(job, trigger);
           // log.Info(string.Format("{0} will run at: {1}", job.Key, runTime.ToString("r")));

            // Start up the scheduler (nothing can actually run until the 
            // scheduler has been started)
            sched.Start();
            log.Info("------- Started Scheduler -----------------");

            // wait long enough so that the scheduler as an opportunity to 
            // run the job!
            log.Info("------- Waiting 65 seconds... -------------");

           
        }

        protected override void OnStop()
        {
            if (win != null)
                win.Close();
            if (listen != null)
                listen = null;
            if(sched!=null)
                sched.Shutdown(true);
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }
    }
}
