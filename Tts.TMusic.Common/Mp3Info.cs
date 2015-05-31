using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tts.TMusic.Common
{
    /// <summary>
    /// Mp3信息结构 
    /// </summary>
    public struct Mp3Info
    {
        /// <summary>
        /// TAG，三个字节 
        /// </summary>
        public string identify;     //TAG，三个字节 
        /// <summary>
        /// 歌曲名,30个字节 
        /// </summary>
        public string Title;        //歌曲名,30个字节 
        /// <summary>
        /// 歌手名,30个字节 
        /// </summary>
        public string Artist;       //歌手名,30个字节 
        /// <summary>
        /// 所属唱片,30个字节 
        /// </summary>
        public string Album;        //所属唱片,30个字节 
        /// <summary>
        /// 年,4个字符 
        /// </summary>
        public string Year;         //年,4个字符 
        /// <summary>
        /// 注释,28个字节 
        /// </summary>
        public string Comment;      //注释,28个字节 
        public char reserved1;      //保留位，一个字节 
        public char reserved2;      //保留位，一个字节 
        public char reserved3;      //保留位，一个字节 
    }

    /// <summary>
    /// Mp3文件信息类
    /// </summary>
    public class Mp3FileInfo
    {
        Mp3Info info;
        private string _path;
        /// <summary>
        /// 构造函数,输入文件名即得到信息
        /// </summary>
        /// <param name="mp3FilePos"></param>
        public Mp3FileInfo(String mp3FilePos)
        {
            this._path = mp3FilePos;
            info = getMp3Info(getLast128(mp3FilePos));
        }

        public Mp3Info GetMp3()
        {
            info = getMp3Info(getLast128(this._path));
            return info;
        }

        /// <summary>
        /// 获取整理后的Mp3文件名,这里以标题和艺术家名定文件名
        /// </summary>
        /// <returns></returns>
        public String GetOriginalName()
        {
            return formatString(info.Title.Trim()) + "-" + formatString(info.Artist.Trim());
        }

        /// <summary>
        /// 去除\0字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private String formatString(String str)
        {
            return str.Replace("\0", "");
        }

        /// <summary> 
        /// 获取MP3文件最后128个字节 
        /// </summary> 
        /// <param name="FileName">文件名</param> 
        /// <returns>返回字节数组</returns> 
        private byte[] getLast128(string FileName)
        {

            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            Stream stream = fs;
            stream.Seek(-128, SeekOrigin.End);

            const int seekPos = 128;
            int rl = 0;
            byte[] Info = new byte[seekPos];
            rl = stream.Read(Info, 0, seekPos);

            fs.Close();
            stream.Close();

            return Info;
        }

        /// <summary> 
        /// 获取MP3歌曲的相关信息 
        /// </summary> 
        /// <param name = "Info">从MP3文件中截取的二进制信息</param> 
        /// <returns>返回一个Mp3Info结构</returns> 
        private Mp3Info getMp3Info(byte[] Info)
        {

            Mp3Info mp3Info = new Mp3Info();

            string str = null;
            int i;
            int position = 0;//循环的起始值 
            int currentIndex = 0;//Info的当前索引值 

            //获取TAG标识 

            for (i = currentIndex; i < currentIndex + 3; i++)
            {
                str = str + (char)Info[i];
                position++;
            }

            currentIndex = position;
            mp3Info.identify = str;


            //获取歌名 
            str = null;
            byte[] bytTitle = new byte[30];//将歌名部分读到一个单独的数组中 
            int j = 0;
            for (i = currentIndex; i < currentIndex + 30; i++)
            {
                bytTitle[j] = Info[i];
                position++;
                j++;
            }

            currentIndex = position;

            mp3Info.Title = this.byteToString(bytTitle);


            //获取歌手名 

            str = null;
            j = 0;
            byte[] bytArtist = new byte[30];//将歌手名部分读到一个单独的数组中 

            for (i = currentIndex; i < currentIndex + 30; i++)
            {

                bytArtist[j] = Info[i];
                position++;
                j++;
            }

            currentIndex = position;
            mp3Info.Artist = this.byteToString(bytArtist);



            //获取唱片名 
            str = null;
            j = 0;
            byte[] bytAlbum = new byte[30];//将唱片名部分读到一个单独的数组中 

            for (i = currentIndex; i < currentIndex + 30; i++)
            {

                bytAlbum[j] = Info[i];
                position++;
                j++;
            }
            currentIndex = position;
            mp3Info.Album = this.byteToString(bytAlbum);



            //获取年 
            str = null;
            j = 0;
            byte[] bytYear = new byte[4];//将年部分读到一个单独的数组中 

            for (i = currentIndex; i < currentIndex + 4; i++)
            {
                bytYear[j] = Info[i];
                position++;
                j++;
            }

            currentIndex = position;
            mp3Info.Year = this.byteToString(bytYear);

            //获取注释 

            str = null;
            j = 0;
            byte[] bytComment = new byte[28];//将注释部分读到一个单独的数组中 

            for (i = currentIndex; i < currentIndex + 25; i++)
            {
                bytComment[j] = Info[i];
                position++;
                j++;
            }

            currentIndex = position;
            mp3Info.Comment = this.byteToString(bytComment);


            //以下获取保留位 
            mp3Info.reserved1 = (char)Info[++position];
            mp3Info.reserved2 = (char)Info[++position];
            mp3Info.reserved3 = (char)Info[++position];

            return mp3Info;

        }

        /// <summary>
        /// 将字节数组转换成字符串 
        /// </summary> 
        /// <param name = "b">字节数组</param> 
        /// <returns>返回转换后的字符串</returns>
        private string byteToString(byte[] b)
        {

            Encoding enc = Encoding.GetEncoding("GB2312");
            string str = enc.GetString(b);
            str = str.Substring(0, str.IndexOf("#CONTENT#") >= 0 ? str.IndexOf("#CONTENT#") : str.Length);//去掉无用字符             
            return str;
        }
    }
}
