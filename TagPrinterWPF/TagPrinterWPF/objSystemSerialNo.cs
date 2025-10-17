using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TagPrinterWPF
{
    class daoSystemSerialNo
    {
        string _serialName = "";
        public string serialName
        {
            get { return _serialName; }
            set { _serialName = value; }
        }

        int _serialNo;
        public int serialNo
        {
            get { return _serialNo; }
            set { _serialNo = value; }
        }

    }

    class objSystemSerialNo
    {
        SqlConnection conn;

        public int getSerialNo(string SerialName)
        {
            int serialno = -1;

            DataUtility data = new DataUtility();
            conn = data.connectdb();

            if (SerialName == "")
                return -1;

            string sql = string.Format($"select * from SystemSerialNo where SerialName='{SerialName}'");


            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader myData = cmd.ExecuteReader();
                if (myData.HasRows)
                {
                    // 讀取資料並且顯示出來
                    myData.Read();
                    serialno = myData.GetInt32(myData.GetOrdinal("serialNo"));

                }
                
                myData.Close();
                data.Closedb();
            }
            catch (Exception ex)
            {
                Utility.Log($"ERROR: {ex.Message}");
            }

            return serialno;
        }

        public bool UpdateSerialNo(string SerialName, int count)
        {
            bool rsp = false;
            DataUtility data = new DataUtility();
            conn = data.connectdb();

            try
            {
                string sql = string.Format($"update SystemSerialNo set serialNo={count} where SerialName='{SerialName}'");
                SqlCommand cmd = new SqlCommand(sql, conn);
                int i = cmd.ExecuteNonQuery();
                if (i != 1)
                {
                    Utility.Log($"ERROR: Update 0 record! SerialName:{SerialName}");
                }
                else
                {
                    rsp = true;
                }

                conn.Close();
            }
            catch(Exception ex)
            {
                Utility.Log($"ERROR: {ex.Message}"); 
            }

            return rsp;
        }
    }
}
