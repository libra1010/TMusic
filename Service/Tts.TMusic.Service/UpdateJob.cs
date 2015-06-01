using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tts.TMusic.Common;

namespace Tts.TMusic.Service
{
    public class UpdateJob : IJob
    {
        private static bool IsExec = false;
        public void Execute(IJobExecutionContext context)
        {
            if (IsExec)
                return;
            IsExec = true;

            try
            {
                if (TMusic._config == null)
                    return;
                System.Net.HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp(TMusic._config.LastVersionAddr);
                using (var rsponse = request.GetResponse())
                {
                    var stream = rsponse.GetResponseStream();
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        string version = reader.ReadToEnd();
                        Version netVer = Version.Parse(version);
                        if (this.GetType().Assembly.GetName().Version < netVer)
                        {
                            string startInfo = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Updates", "Tts.TMusic.Service.Updates.exe");
                            CMDHelper.ExecuteCMD("start \"Update\" \"" + startInfo + "\"", 0);

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
