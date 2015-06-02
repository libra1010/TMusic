using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tts.TMusic.Common;

namespace Tts.TMusic.Service.Controller
{
    public class SongController:IController
    {

        public void Exec(Common.CommandFlag cmd, byte[] content)
        {
            Exec(cmd, content, 0, content == null ? 0 : content.Length);
        }

        public void Exec(Common.CommandFlag cmd, byte[] content, int startIndex, int count)
        {
            switch (cmd)
            {
                case CommandFlag.Play://开始
                    Common.SerializeHelper.DeserializeObject(content, startIndex,count);
                    ServiceContext.Current.Player.Play();
                    break;
                case CommandFlag.Paush://暂停
                    ServiceContext.Current.Player.Paush();
                    break;
                case CommandFlag.Stop://停止
                    ServiceContext.Current.Player.Stop();
                    break;
                case CommandFlag.Next://下一曲
                    ServiceContext.Current.Player.Next();
                    break;
                case CommandFlag.Prev://上一曲
                    ServiceContext.Current.Player.Prev();
                    break;
                default:
                    break;
            }
        }
    }
}
