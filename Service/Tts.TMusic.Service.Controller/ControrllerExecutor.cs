using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tts.TMusic.Service.Controller
{
    public class ControrllerExecutor
    {
        private readonly static IList<IController> _controllers = new List<IController>();

        static ControrllerExecutor() 
        {
            _controllers.Add(new SongController());
        }

        public static void Exec(byte[] bytes)
        {
            if (bytes.Length < 3 || Convert.ToChar(bytes[2]) != '#')
                throw new Exception("错误的指令");
            byte[] cmdFlag = new byte[] { bytes[0],bytes[1]};
            Common.CommandFlag flag =  (Common.CommandFlag)BitConverter.ToInt16(cmdFlag, 0);
            //由于功能很简单，这里就不在进行复杂判断了
            foreach (var controller in _controllers)
            { 
                controller.Exec(flag,bytes,2,bytes.Length-3);
            }
        }
    }
}
