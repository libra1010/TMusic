using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tts.TMusic.Common;

namespace Tts.TMusic.TPlayer
{
    /// <summary>
    /// 播放器
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// 当前播放曲目
        /// </summary>
        LinkedListNode<Song> Current { get; }

        /// <summary>
        /// 设置当前播放歌单
        /// </summary>
        /// <param name="playList"></param>
        void SetPlayList(LinkedList<Song> playList);

        /// <summary>
        /// 开始播放
        /// </summary>
        void Play();

        /// <summary>
        /// 开始播放
        /// </summary>
        /// <param name="song"></param>
        void Play(LinkedListNode<Song> song);

        /// <summary>
        /// 暂停
        /// </summary>
        void Paush();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();

        /// <summary>
        /// 下一首
        /// </summary>
        void Next();

        /// <summary>
        /// 上一首
        /// </summary>
        void Prev();
    }
}
