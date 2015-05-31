using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tts.TMusic.Common;

namespace Tts.TMusic.TPlayer
{
    /// <summary>
    /// 使用NAudio实现的播放器
    /// </summary>
    public class TNAudioPlayer
    {
         private LinkedList<Song> _list = null;
        private LinkedListNode<Song> _current = null;
        private IWavePlayer _waveOut;
        private AudioFileReader _audioFileReader;

        public TNAudioPlayer() { }

        public TNAudioPlayer(LinkedList<Song> playList)
        {
            _list = playList;
        }


        public LinkedListNode<Song> Current
        {
            get
            {
                if (_current == null && _list == null)
                    return null;
                else if (_current == null)
                {
                    _current = _list.First;
                    return _current;
                }
                else
                {
                    return _current;
                }
            }
        }

        public void SetPlayList(LinkedList<Song> playList)
        {
            _list = playList;
        }

        public void Play()
        {
            if (Current == null)
                throw new Exception("当前播放列表为空");
            if (_waveOut != null)
            {
                if (_waveOut.PlaybackState == PlaybackState.Playing)
                {
                    return;
                }
                else if (_waveOut.PlaybackState == PlaybackState.Paused)
                {
                    _waveOut.Play();
                    return;
                }
            }

            try
            {
                CreateWaveOut();
            }
            catch (Exception driverCreateException)
            {
                throw new Exception("播放器创建失败");
            }
        }

        private void w_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            CloseWaveOut();
        }

        public void Play(LinkedListNode<Song> song)
        {
            if (song == null)
                throw new Exception("当前播放歌曲不能为空");
            if (song.List == null)
                throw new Exception("歌曲必须隶属于播放列表");
            if (song.List != _list)
                SetPlayList(song.List);
            if (_current != song)
            {
                CloseWaveOut();
            }
            else
            {
                if (_audioFileReader != null)
                {
                    _audioFileReader.CurrentTime = TimeSpan.Zero;
                }
            }
            _current = song;
            Play();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Paush()
        {
            if (_waveOut != null)
                this._waveOut.Pause();
        }

        public void Stop()
        {
            CloseWaveOut();
        }

        public void Next()
        {
            if (Current == null)
                throw new Exception("当前播放列表为空");
            Play(Current.Next);
        }

        public void Prev()
        {
            if (Current == null)
                throw new Exception("当前播放列表为空");
            Play(Current.Previous);
        }

        private void CreateWaveOut()
        {
            IWavePlayer w = new WaveOutEvent();
            var file = new AudioFileReader(Current.Value.FileName);
            //file.TotalTime.TotalMilliseconds;
            // file.CurrentTime;
            file.Volume = 1;
            w.Init(file);
            w.PlaybackStopped += w_PlaybackStopped;
            w.Play();
        }

        private void CloseWaveOut()
        {
            if (_waveOut != null)
            {
                _waveOut.Stop();
            }
            if (_audioFileReader != null)
            {
                _audioFileReader.Dispose();
                // setVolumeDelegate = null;
                _audioFileReader = null;
            }
            if (_waveOut != null)
            {
                _waveOut.Dispose();
                _waveOut = null;
            }
        }
    }
}
