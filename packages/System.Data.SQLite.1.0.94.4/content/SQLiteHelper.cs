using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Threading;

namespace DBUtility
{
	
	#region LockHelper
	internal class LockHelper
	{
		#region dblock
		private class DbLock
		{
			private string _str;
			public DbLock(string str)
			{
				_str = str;
			}
			
			/// <summary>
			/// 用于控制 写锁
			/// </summary>
			private  ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
			
			/// <summary>
			/// 用于控制 写锁
			/// </summary>
			internal ReaderWriterLockSlim ReadWirteLock{get{return _lock;}}
			
			/// <summary>
			/// 用于控制多进程 写锁
			/// </summary>
			private Mutex _mutex;
			
			private object obj = new object();
			
			/// <summary>
			/// 用于控制多进程 写锁
			/// </summary>
			internal Mutex Mutex
			{
				get
				{
					if(_mutex==null)
					{
						lock(obj)
						{
							if(_mutex==null)
							{
								if(!Mutex.TryOpenExisting("yiwowoyi.sqlite.lock.mutex."+_str,out _mutex))
									_mutex = new Mutex(false,"yiwowoyi.sqlite.lock.mutex."+_str);
							}
						}
					}
					return _mutex;
				}
			}
		}
		#endregion	
		
		/// <summary>
		/// 用于保存每个连接的锁对象
		/// </summary>
		private readonly static IDictionary<string,DbLock> _dic = new Dictionary<string,DbLock>();
		
		private static object obj = new object();
		
		internal static void EnterReadLock(string conn_str,int time)
		{
			if(_dic.ContainsKey(conn_str))
			{
				bool isSuccess = _dic[conn_str].ReadWirteLock.TryEnterReadLock(time);
				if(!isSuccess)
					throw new LockTimeoutException("获取读锁超时");
				return;
			}
			else
			{
				lock(obj)
				{
					if(_dic.ContainsKey(conn_str))
					{
						_dic[conn_str].ReadWirteLock.TryEnterReadLock(time);
						return;
					}
					_dic[conn_str] = new LockHelper.DbLock(conn_str);
					bool isSuccess =_dic[conn_str].ReadWirteLock.TryEnterReadLock(time);
					if(!isSuccess)
					throw new LockTimeoutException("获取读锁超时");
				}
			}
		}
		
		internal static void ExitReadLock(string conn_str)
		{
			if(_dic.ContainsKey(conn_str))
			{
				_dic[conn_str].ReadWirteLock.ExitReadLock();
				return;
			}
			
			throw new Exception("必须先加锁");
		}
		
		internal static void EnterWriteLock(string conn_str,int time)
		{
			if(_dic.ContainsKey(conn_str))
			{
				//先加写锁，用于控制当前进程，其他线程不可以使用
				bool isSuccess =_dic[conn_str].ReadWirteLock.TryEnterWriteLock(time);
				if(!isSuccess)
					throw new LockTimeoutException("获取写锁超时");
				//获取到写锁后，在重新获取经常互斥体，用于控制多进程只有一个线程可以访问
				isSuccess = _dic[conn_str].Mutex.WaitOne(time);
				if(!isSuccess)
					throw new ProccessLockTimeoutException("获取进程级写锁超时");
				return;
			}
			else
			{
				lock(obj)
				{
					if(_dic.ContainsKey(conn_str))
					{
						_dic[conn_str].ReadWirteLock.TryEnterReadLock(time);
						return;
					}
					_dic[conn_str] = new LockHelper.DbLock(conn_str);
					//先加写锁，用于控制当前进程，其他线程不可以使用
					bool isSuccess =_dic[conn_str].ReadWirteLock.TryEnterWriteLock(time);
					if(!isSuccess)
						throw new LockTimeoutException("获取写锁超时");
					//获取到写锁后，在重新获取经常互斥体，用于控制多进程只有一个线程可以访问
					isSuccess = _dic[conn_str].Mutex.WaitOne(time);
					if(!isSuccess)
						throw new ProccessLockTimeoutException("获取进程级写锁超时");
				}
			}
		}
		
		internal static void ExitWirteLock(string conn_str)
		{
			if(_dic.ContainsKey(conn_str))
			{
				_dic[conn_str].Mutex.ReleaseMutex();
				_dic[conn_str].ReadWirteLock.ExitWriteLock();
				return;
			}
			
			throw new Exception("必须先加锁");
		}
	}
	#endregion
	
	#region SafeSQLiteHelper
	/// <summary>
	/// 线程安全的SQLite辅助类
	/// </summary>
	public class SafeSQLiteHelper
	{
		private static int timeout = 60*1000;
		
		#region ExecuteNonQuery 执行SQL命令，返回影响行数
        /// 
        /// 执行SQL命名 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
        	bool isLock = false;
        	try 
        	{	
        		LockHelper.EnterWriteLock(connectionString,timeout);
        		isLock = true;
	        	int result = SQLiteHelper.ExecuteNonQuery(connectionString,commandType,commandText);		        	
    			return result;
        	}
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitWirteLock(connectionString);
        	}
        	
        }
        /// 
        /// 不支持存储过程，但可以参数化查询 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            bool isLock = false;
        	try 
        	{	
        		LockHelper.EnterWriteLock(connectionString,timeout);
        		isLock = true;
	        	int result = SQLiteHelper.ExecuteNonQuery(connectionString,commandType,commandText,commandParameters);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitWirteLock(connectionString);
        	}
        }
        public static int ExecuteNonQuery(SQLiteConnection connection, CommandType commandType, string commandText)
        {
             bool isLock = false;
        	try 
        	{	
        		LockHelper.EnterWriteLock(connection.ConnectionString,timeout);
        		isLock=true;
	        	int result = SQLiteHelper.ExecuteNonQuery(connection,commandType,commandText);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitWirteLock(connection.ConnectionString);
        	}
        }
        public static int ExecuteNonQuery(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            bool isLock = false;
        	try 
        	{	
        		 LockHelper.EnterWriteLock(connection.ConnectionString,timeout);
        		 isLock=true;
	        	int result = SQLiteHelper.ExecuteNonQuery(connection,commandType,commandText,commandParameters);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitWirteLock(connection.ConnectionString);
        	}
        }

        public static int ExecuteNonQuery(SQLiteTransaction trans, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
        	bool isLock = false;
        	try 
        	{	
        		//由于使用了 事物 ，所以事务不提交 其实并不会更改文件
	        	//so only get ReadLock
	        	//在事务提交时在获取写锁
          		LockHelper.EnterReadLock(trans.Connection.ConnectionString,timeout);
          		 isLock=true;
	        	int result = SQLiteHelper.ExecuteNonQuery(trans,commandType,commandText,commandParameters);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(trans.Connection.ConnectionString);
        	}
        }

        public static SafeSQLiteTransaction GetTransaction(string connection) 
        {
           var trans =	SQLiteHelper.GetTransaction(connection);
           return new SafeSQLiteTransaction(trans);
        }
        #endregion

        #region ExecuteDataSet 执行SQL查询，并将返回数据填充到DataSet
        public static DataSet ExecuteDataset(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
        	bool isLock = false;            
        	try 
        	{	
        		LockHelper.EnterReadLock(connection.ConnectionString,timeout);
        		isLock=true;
	        	DataSet result = SQLiteHelper.ExecuteDataset(connection,commandType,commandText,commandParameters);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connection.ConnectionString);
        	}
        }
        public static DataSet ExecuteDataset(SQLiteConnection connection, CommandType commandType, string commandText)
        {
            bool isLock = false;        
        	try 
        	{	
        		LockHelper.EnterReadLock(connection.ConnectionString,timeout);
        		isLock=true;
	        	DataSet result = SQLiteHelper.ExecuteDataset(connection,commandType,commandText);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connection.ConnectionString);
        	}
        }
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            bool isLock = false;        
        	try 
        	{	
        		LockHelper.EnterReadLock(connectionString,timeout);
        		isLock=true;
	        	DataSet result = SQLiteHelper.ExecuteDataset(connectionString,commandType,commandText,commandParameters);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connectionString);
        	}
        }
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
        	bool isLock = false;   
        	try 
        	{	
        		LockHelper.EnterReadLock(connectionString,timeout);
        		isLock=true;
	        	DataSet result = SQLiteHelper.ExecuteDataset(connectionString,commandType,commandText);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connectionString);
        	}
        }
        #endregion

        #region ExecuteReader 执行SQL查询,返回DbDataReader
//        public static SQLiteDataReader ExecuteReader(SQLiteConnection connection, SQLiteTransaction transaction, CommandType commandType, string commandText, SQLiteParameter[] commandParameters, DbConnectionOwnership connectionOwnership)
//        {
//            LockHelper.EnterReadLock(connectionString,timeout);
//        	try 
//        	{	
//	        	SQLiteDataReader result = SQLiteHelper.ExecuteNonQuery(connection,transaction,commandType,commandText,commandParameters);		        	
//    			return result;
//        	} 
//        	finally
//        	{
//        		LockHelper.ExitReadLock(connectionString);
//        	}
//        }

        /// 
        ///读取数据后将自动关闭连接 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static SQLiteDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            bool isLock = false;
        	try 
        	{	
        		LockHelper.EnterReadLock(connectionString,timeout);
        		isLock=true;
	        	SQLiteDataReader result = SQLiteHelper.ExecuteReader(connectionString,commandType,commandText,commandParameters);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connectionString);
        	}
        }
        
        /// 
        /// 读取数据后将自动关闭连接 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static SQLiteDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
           bool isLock = false;
        	try 
        	{	
        		LockHelper.EnterReadLock(connectionString,timeout);
        		isLock=true;
	        	SQLiteDataReader result = SQLiteHelper.ExecuteReader(connectionString,commandType,commandText);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connectionString);
        	}
        }
        /// 
        /// 读取数据以后需要自行关闭连接 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static SQLiteDataReader ExecuteReader(SQLiteConnection connection, CommandType commandType, string commandText)
        {
        	
            bool isLock = false;
        	try 
        	{	
        		LockHelper.EnterReadLock(connection.ConnectionString,timeout);
        		isLock=true;
	        	SQLiteDataReader result = SQLiteHelper.ExecuteReader(connection,commandType,commandText);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connection.ConnectionString);
        	}
        }
        /// 
        /// 读取数据以后需要自行关闭连接 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static SQLiteDataReader ExecuteReader(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
           bool isLock = false;
        	try 
        	{	
        		 LockHelper.EnterReadLock(connection.ConnectionString,timeout);
        		 isLock=true;
	        	SQLiteDataReader result = SQLiteHelper.ExecuteReader(connection,commandType,commandText,commandParameters);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connection.ConnectionString);
        	}
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
       	  	bool isLock = false;
        	try 
        	{	
        		 LockHelper.EnterReadLock(connectionString,timeout);
        		 isLock=true;
	        	object result = SQLiteHelper.ExecuteScalar(connectionString,commandType,commandText);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connectionString);
        	}
        }
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            	bool isLock = false;
        	try 
        	{	
        		LockHelper.EnterReadLock(connectionString,timeout);
        		isLock=true;
	        	object result = SQLiteHelper.ExecuteScalar(connectionString,commandType,commandText,commandParameters);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connectionString);
        	}
        }
        public static object ExecuteScalar(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
        	bool isLock = false;
            
        	try 
        	{	
        		LockHelper.EnterReadLock(connection.ConnectionString,timeout);
        		isLock = true;
	        	object result = SQLiteHelper.ExecuteScalar(connection,commandType,commandText,commandParameters);		        	
    			return result;
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitReadLock(connection.ConnectionString);
        	}
        }
        #endregion
	}
	#endregion
	
	#region NotSafe SQLiteHelper
    /// <summary>
    /// SQLite辅助类
    /// 没有线程安全
    /// 线程安全请使用 SafeSQLiteHelper
    /// </summary>
    public class SQLiteHelper
    {
        private SQLiteHelper()
        {
            // 
            // TODO: 在此处添加构造函数逻辑 
            // 
        }

        #region 静态私有方法
        /// 
        /// 附加参数 
        /// 
        /// 
        /// 
        private static void AttachParameters(SQLiteCommand command, SQLiteParameter[] commandParameters)
        {
            command.Parameters.Clear();
            foreach (SQLiteParameter p in commandParameters)
            {
                if (p.Direction == ParameterDirection.InputOutput && p.Value == null)
                    p.Value = DBNull.Value;
                command.Parameters.Add(p);
            }
        }
        /// 
        /// 分配参数值 
        /// 
        /// 
        /// 
        private static void AssignParameterValues(SQLiteParameter[] commandParameters, object[] parameterValues)
        {
            if (commandParameters == null || parameterValues == null)
                return;
            if (commandParameters.Length != parameterValues.Length)
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }
        /// 
        /// 预备执行command命令 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        private static void PrepareCommand(SQLiteCommand command,
        SQLiteConnection connection, SQLiteTransaction transaction,
        CommandType commandType, string commandText, SQLiteParameter[] commandParameters
        )
        {
            if (commandType == CommandType.StoredProcedure)
            {
                throw new ArgumentException("SQLite 暂时不支持存储过程");
            }
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
                command.Transaction = transaction;
            command.CommandType = commandType;
            if (commandParameters != null)
                AttachParameters(command, commandParameters);
            return;
        }
        #endregion

        #region 加密
        public static void ChangePassword(string connectionString, string password) 
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString)) 
            {
                connection.Open();
                connection.ChangePassword(password);
                connection.Close();
            }          
        }
        #endregion

        #region ExecuteNonQuery 执行SQL命令，返回影响行数
        /// 
        /// 执行SQL命名 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (SQLiteParameter[])null);
        }
        /// 
        /// 不支持存储过程，但可以参数化查询 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                return ExecuteNonQuery(conn, commandType, commandText, commandParameters);
            }
        }
        public static int ExecuteNonQuery(SQLiteConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, (SQLiteParameter[])null);
        }
        public static int ExecuteNonQuery(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            PrepareCommand(cmd, connection, (SQLiteTransaction)null, commandType, commandText, commandParameters);
            int retval = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return retval;
        }

        public static int ExecuteNonQuery(SQLiteTransaction trans, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            PrepareCommand(cmd, trans.Connection, trans, commandType, commandText, commandParameters);
            int retval = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return retval;
        }

        public static SQLiteTransaction GetTransaction(string connection) 
        {
            SQLiteConnection conn = new SQLiteConnection(connection);            
            conn.Open();            
            return conn.BeginTransaction();
        }
        #endregion

        #region ExecuteDataSet 执行SQL查询，并将返回数据填充到DataSet
        public static DataSet ExecuteDataset(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            PrepareCommand(cmd, connection, (SQLiteTransaction)null, commandType, commandText, commandParameters);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }
        public static DataSet ExecuteDataset(SQLiteConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, (SQLiteParameter[])null);
        }
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {

            using (SQLiteConnection cn = new SQLiteConnection(connectionString))
            {
                cn.Open();

                return ExecuteDataset(cn, commandType, commandText, commandParameters);
            }
        }
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, (SQLiteParameter[])null);
        }
        #endregion

        #region ExecuteReader 执行SQL查询,返回DbDataReader
        public static SQLiteDataReader ExecuteReader(SQLiteConnection connection, SQLiteTransaction transaction, CommandType commandType, string commandText, SQLiteParameter[] commandParameters, DbConnectionOwnership connectionOwnership)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);
            SQLiteDataReader dr;
            if (connectionOwnership == DbConnectionOwnership.External)
                dr = cmd.ExecuteReader();
            else
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return dr;
        }
        /// 
        ///读取数据后将自动关闭连接 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static SQLiteDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            SQLiteConnection cn = new SQLiteConnection(connectionString);
            cn.Open();
            try
            {
                return ExecuteReader(cn, null, commandType, commandText, commandParameters, DbConnectionOwnership.Internal);
            }
            catch
            {
                cn.Close();
                throw;
            }
        }
        /// 
        /// 读取数据后将自动关闭连接 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static SQLiteDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, (SQLiteParameter[])null);
        }
        /// 
        /// 读取数据以后需要自行关闭连接 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static SQLiteDataReader ExecuteReader(SQLiteConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (SQLiteParameter[])null);
        }
        /// 
        /// 读取数据以后需要自行关闭连接 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        public static SQLiteDataReader ExecuteReader(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            return ExecuteReader(connection, (SQLiteTransaction)null, commandType, commandText, commandParameters, DbConnectionOwnership.External);
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, (SQLiteParameter[])null);
        }
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            SQLiteConnection cn = new SQLiteConnection(connectionString);
            cn.Open();
            try
            {
                return ExecuteScalar(cn, commandType, commandText, commandParameters);
            }
            catch
            {

                return 0; ;
            }
            finally
            {
                cn.Close();
            }

        }
        public static object ExecuteScalar(SQLiteConnection connection, CommandType commandType, string commandText, params SQLiteParameter[] commandParameters)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            PrepareCommand(cmd, connection, (SQLiteTransaction)null, commandType, commandText, commandParameters);
            var obj = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return obj;
        }
        #endregion
    }
    #endregion

    public class ProccessLockTimeoutException:LockTimeoutException
    {
    	public ProccessLockTimeoutException():base(){}
    	
    	public ProccessLockTimeoutException(string msg):base(msg){}
    }
    
    public class LockTimeoutException:TimeoutException
    {
   		 public LockTimeoutException():base(){}
    	
    	public LockTimeoutException(string msg):base(msg){}
    }
    
    
    public class SafeSQLiteTransaction:IDisposable
    {
    	private int timeout = 60*1000;
    	public SafeSQLiteTransaction(SQLiteTransaction trans)
    	{
    		Transaction = trans;
    	}
    	
    	public SQLiteTransaction Transaction
    	{
    		get;
    		private set;
    	}
    	
    	public void Commit()
    	{
    		bool isLock = false;
    		
        	try 
        	{	
        		LockHelper.EnterWriteLock(Transaction.Connection.ConnectionString,timeout);
        		isLock=true;
        		Transaction.Commit();
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitWirteLock(Transaction.Connection.ConnectionString);
        	}
    	}
    	
    	public void RollBack()
    	{
    		bool isLock = false;
    		
        	try 
        	{	
        		LockHelper.EnterWriteLock(Transaction.Connection.ConnectionString,timeout);
        		isLock=true;
        		Transaction.Rollback();
        	} 
        	finally
        	{
        		if(isLock)
        			LockHelper.ExitWirteLock(Transaction.Connection.ConnectionString);
        	}
    	}
    	
		public void Dispose()
		{
			if(Transaction!=null)
				Transaction.Dispose();
		}
    }
    
    /// <SUMMARY></SUMMARY> 
    /// DbConnectionOwnership DataReader以后是否自动关闭连接 
    /// 
    public enum DbConnectionOwnership
    {
        /// <SUMMARY></SUMMARY> 
        /// 自动关闭 
        /// 
        Internal,
        /// <SUMMARY></SUMMARY> 
        /// 手动关闭 
        /// 
        External,
    }
}
