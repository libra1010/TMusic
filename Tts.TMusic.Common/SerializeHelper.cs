using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Tts.TMusic.Common
{
    public class SerializeHelper
    {
        /// <summary>
        /// 把对象序列化并返回相应的字节
        /// </summary>
        /// <param name="pObj">需要序列化的对象</param>
        /// <returns>byte[]</returns>
        public static byte[] SerializeObject(object pObj)
        {
            if (pObj == null)
                return null;
            System.IO.MemoryStream _memory = new System.IO.MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_memory, pObj);
            _memory.Position = 0;
            byte[] read = new byte[_memory.Length];
            _memory.Read(read, 0, read.Length);
            _memory.Close();
            return read;
        }

        public static byte[] SerializeObject<T>(T pObj)
        {
            return SerializeObject(pObj);
        }
        
        /// <summary>
        /// 把字节反序列化成相应的对象
        /// </summary>
        /// <param name="pBytes">字节流</param>
        /// <returns>object</returns>
        public static object DeserializeObject(byte[] pBytes, int index, int count)
        {
            object _newOjb = null;
            if (pBytes == null)
                return _newOjb;
            System.IO.MemoryStream _memory = new System.IO.MemoryStream(pBytes, index, count);
            _memory.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            _newOjb = formatter.Deserialize(_memory);
            _memory.Close();
            return _newOjb;
        }

        public static object DeserializeObject(byte[] pBytes)
        {
            return DeserializeObject(pBytes, 0, pBytes.Length);
        }

        public static T DeserializeObject<T>(byte[] pBytes, int index, int count)
        {
            return (T)DeserializeObject(pBytes, index, count);
        }

        public static T DeserializeObject<T>(byte[] pBytes)
        {
            return DeserializeObject<T>(pBytes, 0, pBytes.Length);
        }
    }
}
