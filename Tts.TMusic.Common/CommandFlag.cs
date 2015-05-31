using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tts.TMusic.Common
{
    public enum CommandFlag:short
    {
        /// <summary>
        /// 播放
        /// </summary>
        Play=1,
        /// <summary>
        /// 停止
        /// </summary>
        Stop =2,

        /// <summary>
        /// 暂停
        /// </summary>
        Paush =3,

        /// <summary>
        /// 下一首
        /// </summary>
        Next=4,

        /// <summary>
        /// 上一首
        /// </summary>
        Prev = 5,

        /// <summary>
        /// 添加播放列表
        /// </summary>
        AddPlayList = 6,

        /// <summary>
        /// 添加歌曲到播放列表
        /// </summary>
        AddSongToList=7
    }
}
