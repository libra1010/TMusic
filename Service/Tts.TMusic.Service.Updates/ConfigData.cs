using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tts.TMusic.Service.Updates
{
    public class ConfigData
    {
        private static ServiceConfig _config = null;

        private readonly static string CONN_STR = GetConnStr();

        private static string GetConnStr() 
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
           return "Data Source=" + System.IO.Path.Combine(di.Parent.FullName, "Config.lock.db") + ";Pooling=true;FailIfMissing=false;";
        }

        /// <summary>
        /// 加载CONFIG
        /// </summary>
        /// <returns></returns>
        public static ServiceConfig LoadConfig(bool isReLoad = false) 
        {
            if (_config != null && !isReLoad)
                return _config;

            using (SQLiteDataReader reader = DBUtility.SQLiteHelper.ExecuteReader(CONN_STR, System.Data.CommandType.Text, "SELECT * FROM Configs WHERE Status=1")) 
            {
                while (reader.Read())
                {
                    if (_config == null)
                        _config = new ServiceConfig();
                    string key = reader["Key"].ToString();
                    var p = _config.GetType().GetProperty(key);
                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(_config, reader["Value"].ToString());
                    }
                    else if (p.PropertyType == typeof(int))
                    {
                        p.SetValue(_config, Convert.ToInt32(reader["Value"]));
                    }
                    else if (p.PropertyType == typeof(double))
                    {
                        p.SetValue(_config, Convert.ToDouble(reader["Value"]));
                    }
                    else if (p.PropertyType == typeof(decimal))
                    {
                        p.SetValue(_config, Convert.ToDecimal(reader["Value"]));
                    }
                    else if (p.PropertyType == typeof(DateTime))
                    {
                        p.SetValue(_config, Convert.ToDateTime(reader["Value"]));
                    }
                    else if (p.PropertyType.IsEnum)
                    {
                        string v = reader["Value"].ToString();
                        if(System.Text.RegularExpressions.Regex.IsMatch(v,"[\\d]{1,}"))
                        {
                            p.SetValue(_config, Convert.ToInt32(v));
                        }
                        else
                        {
                            object val = Enum.Parse(p.PropertyType,v);
                            p.SetValue(_config, val);
                        }
                    }
                }
            }
            return _config;
        }
    }
}
