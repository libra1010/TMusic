using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tts.TMusic.Common
{
    /// <summary>
    /// 歌曲
    /// </summary>
    public class Song
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 歌曲名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件名称
        /// 只是文件名和后缀名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 演唱者
        /// </summary>
        public string Singer { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
