using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                            ExecuteCMD("start \"Update\" \"" + startInfo + "\"", 0);
                           
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
    }
}
