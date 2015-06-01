using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tts.TMusic.Data
{
    internal class ConsString
    {
        /// <summary>
        /// 连接字符串
        /// Data Source=local\\tmpdata.db.lock;Pooling=true;FailIfMissing=false;Password=yiwowoyi.password
        /// </summary>
        internal readonly static string CON_STR = "Data Source=" + System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config.lock.db") + ";Pooling=true;FailIfMissing=false;";
    }
}
