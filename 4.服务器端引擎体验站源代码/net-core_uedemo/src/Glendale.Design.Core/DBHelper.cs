using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Glendale.Design
{
    public class DBHelper
    {
        public static string _connectionString;
        private static MySqlConnection _mySqlConnection = null;

        #region 基础方法

        /// <summary>
        /// 得到数据库连接方法 
        /// </summary>
        /// <returns>数据库连接</returns>
        private static void Getconn()
        {
            if (_mySqlConnection == null)
                _mySqlConnection = new MySqlConnection(_connectionString);
        }


        /// <summary>
        /// 获得并打开数据库连接方法
        /// </summary>
        /// <returns></returns>
        public static void OpenCon()
        {
            Getconn();
            if (_mySqlConnection.State == ConnectionState.Open)
                return;
            if (_mySqlConnection.State != ConnectionState.Closed)
                _mySqlConnection.Close();
            _mySqlConnection.Open();
        }

        /// <summary>
        /// 关闭数据库连接方法
        /// </summary>
        public static void CloseCon()
        {
            if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Closed)
                _mySqlConnection.Close();
        }
        #region 释放资源的方法

        private static void DisponseCmd(MySqlCommand cmd)
        {
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }
        }

        #endregion

        #endregion      
        /// <summary>
        /// 执行增删改的命令
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteNonQuery(string conns, string sqlString, params MySqlParameter[] cmdParas)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = conns;
            }
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                Getconn();
                OpenCon();
                PrepareCommand(cmd, _mySqlConnection, sqlString, cmdParas, null, CommandType.Text);
                int rows =await cmd.ExecuteNonQueryAsync();
                cmd.Parameters.Clear();
                return rows;

            }
            catch (MySqlConnector.MySqlException e)
            {
                throw;
            }
            finally
            {
                DisponseCmd(cmd);
                CloseCon();
            }
        }

        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, string cmdText, MySqlParameter[] cmdParms, MySqlTransaction trans = null, CommandType commandType = CommandType.Text)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = commandType;
            if (cmdParms != null)
            {
                if (cmd.Parameters.Count > 0)
                {
                    cmd.Parameters.Clear();
                }
                foreach (MySqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
    }
}
