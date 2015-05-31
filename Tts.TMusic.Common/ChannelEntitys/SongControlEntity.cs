using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tts.TMusic.Common.ChannelEntitys
{
    /// <summary>
    /// 音乐播放传输实体
    /// </summary>
    [Serializable]
    public class SongControlEntity
    {
        /// <summary>
        /// 播放曲目ID
        /// </summary>
        public int? SongID { get; set; }

        /// <summary>
        /// 播放列表ID
        /// </summary>
        public int? PlayListID { get; set; }
    }
}
