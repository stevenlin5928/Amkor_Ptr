using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagPrinterWPF
{
    class DataUtility
    {
        private SqlConnection connSQL;
        

        public DataUtility()
        {
            //App.SQLConnection = Utility.LoadDataBaseConfig(".\\dbConfig.txt");
        }

        public SqlConnection connectdb()
        {
            
            //Utility.Log($"SQLConnection: {SQLConnection}");
            try
            {
                connSQL = new SqlConnection(App.SQLConnection);
                connSQL.Open();
            }
            catch (Exception ex)
            {
                Utility.Log("無法連線到SQL Server資料庫.");
            }

            return connSQL;
        }

        public SqlConnection connectdb(string connectionStr)
        {
            try
            {
                connSQL = new SqlConnection(connectionStr);
                connSQL.Open();
            }
            catch (Exception ex)
            {
                Utility.Log("無法連線到SQL Server資料庫.");
            }

            return connSQL;
        }

        //查詢 係統設定

        public void Closedb()
        {
            try
            {
                //conn.Close();
                connSQL.Close();

                //utility.Log("關閉資料庫.");
            }
            catch (Exception ex)
            {
                //utility.Log("ERROR:" + ex.Message);
            }
        }

    }
}
