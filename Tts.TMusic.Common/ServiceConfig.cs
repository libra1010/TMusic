using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tts.TMusic.Common
{
    public class ServiceConfig
    {
        /// <summary>
        /// 监听的端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 音乐路径
        /// </summary>
        public string MusicPath { get; set; }

        /// <summary>
        /// 最新版本获取地址
        /// </summary>
        public string LastVersionAddr { get; set; }
    }
}
