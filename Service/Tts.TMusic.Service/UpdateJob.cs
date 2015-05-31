using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tts.TMusic.Service.Updates;

namespace Tts.TMusic.Service
{
    public class UpdateJob:IJob
    {
        private static bool IsExec = false;
        public void Execute(IJobExecutionContext context)
        {
            if (IsExec)
                return;
            IsExec = true;

            try
            {
                System.Net.HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp("");
                using (var rsponse = request.GetResponse())
                {
                    var stream = rsponse.GetResponseStream();
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        string version = reader.ReadToEnd();
                        Version netVer = Version.Parse(version);
                        if (this.GetType().Assembly.GetName().Version < netVer)
                        {
                            string file = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Tts.TMusic.Service.Updates.exe");
                            System.Diagnostics.Process.Start(file);
                            return; //不在执行检察
                            //UpdateForm form = new UpdateForm();
                            //form.Show();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(UpdateJob));
                log.Error("UpdateJob", ex);
            }
            
            

            IsExec = false;
        }
    }
}
