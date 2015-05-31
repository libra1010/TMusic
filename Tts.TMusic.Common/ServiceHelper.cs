using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Tts.TMusic.Common
{
    public class ServiceHelper
    {
        #region 安装服务
        public static void InstallService(IDictionary stateSaver, string filepath,string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            if (!ServiceIsExisted(serviceName))
            {
                //Install Service  
                AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();
                myAssemblyInstaller.UseNewContext = true;
                myAssemblyInstaller.Path = filepath;
                myAssemblyInstaller.Install(stateSaver);
                myAssemblyInstaller.Commit(stateSaver);
                myAssemblyInstaller.Dispose();
                //--Start Service  
                service.Start();
            }
            else
            {
                if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running && service.Status != System.ServiceProcess.ServiceControllerStatus.StartPending)
                {
                    service.Start();
                }
            }

        }
        #endregion

        #region 卸载windows服务
        public static void UnInstallService(string filepath,string serviceName)
        {
            if (ServiceIsExisted(serviceName))
            {
                //UnInstall Service  
                AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();
                myAssemblyInstaller.UseNewContext = true;
                myAssemblyInstaller.Path = filepath;
                myAssemblyInstaller.Uninstall(null);
                myAssemblyInstaller.Dispose();
            }

        }
        #endregion

        #region 判断window服务是否存在
        public static bool ServiceIsExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 判断window服务是否启动
        /// <summary>  
        /// 判断某个Windows服务是否启动  
        /// </summary>  
        /// <returns></returns>  
        public static bool IsServiceStart(string serviceName)
        {
            ServiceController psc = new ServiceController(serviceName);
            bool bStartStatus = false;
            try
            {
                if (!psc.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    bStartStatus = true;
                }

                return bStartStatus;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 启动服务
        public static bool StartService(string serviceName)
        {
            bool flag = true;
            if (ServiceIsExisted(serviceName))
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
                if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running && service.Status != System.ServiceProcess.ServiceControllerStatus.StartPending)
                {
                    service.Start();
                    for (int i = 0; i < 60; i++)
                    {
                        service.Refresh();
                        System.Threading.Thread.Sleep(1000);
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                        {
                            break;
                        }
                        if (i == 59)
                        {
                            flag = false;
                        }
                    }
                }
            }
            return flag;
        }
        #endregion

        #region 停止服务
        public static bool StopService(string serviceName)
        {
            bool flag = true;
            if (ServiceIsExisted(serviceName))
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    service.Stop();
                    for (int i = 0; i < 60; i++)
                    {
                        service.Refresh();
                        System.Threading.Thread.Sleep(1000);
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                        {
                            break;
                        }
                        if (i == 59)
                        {
                            flag = false;
                        }
                    }
                }
            }
            return flag;
        }
        #endregion
    }
}
