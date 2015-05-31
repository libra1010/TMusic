using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tts.TMusic.Common;

namespace Tts.TMusic.Service.Server
{
    /// <summary>
    /// 用于执行
    /// 开始
    /// 暂停
    /// 停止
    /// 上一曲
    /// 下一曲
    /// </summary>
    public  class SongControlCommand:ICommand
    {
        public void Exec(CommandFlag cmd, byte[] content)
        {
            switch (cmd)
            { 
                case CommandFlag.Play://开始
                    
                    break;
                case CommandFlag.Paush://暂停
                    //ServiceContext.Current.Player
                    break;
                case CommandFlag.Stop://停止
                    break;
                case CommandFlag.Next://下一曲
                    break;
                case CommandFlag.Prev://上一曲
                    break;
                default:
                    break;
            }
        }
    }
}
