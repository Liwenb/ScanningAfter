using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScanningAfter
{
   public class MySqlHelper
    {
        private static readonly string connectionString = "Server=153.37.217.112;Database=stock;Uid=tyre;Pwd =KKBC@20161009+ip120*adm;CharSet=utf8;";
        public static DataTable ExecuteDataTable(string sql, MySqlParameter[] parameters)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    DataTable dt = new DataTable();

                    connection.Open();

                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    da.Fill(dt);

                    return dt;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
                return null;
            }
        }
        public static int ExecuteDataSet(string sql, MySqlParameter[] parameters)
        {

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    int i = cmd.ExecuteNonQuery();
                    return i;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
                return 0;
            }

        }
    }
}
