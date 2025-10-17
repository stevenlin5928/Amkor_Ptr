using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TagPrinterWPF.App;

namespace TagPrinterWPF
{
    public class  daoPrintList
    {
        private string _RequestNo = "";
        public string RequestNo
        {
            get { return _RequestNo; }
            set { _RequestNo = value; }
        }

        private string _SID = "";
        public string SID
        {
            get { return _SID; }
            set { _SID = value; }
        }

        private string _TagID = "";
        public string TagID
        {
            get { return _TagID; }
            set { _TagID = value; }
        }

        private int _PrintState = 0;
        public int PrintState
        {
            get { return _PrintState; }
            set { _PrintState = value; }
        }

        private int _PrintCount = 0;
        public int Count
        {
            get { return _PrintCount; }
            set { _PrintCount = value; }
        }

        private string _remark="";
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }
    }

    public class objPrintList
    {

        public xResult CreatePrintList(daoPrintList dao)
        {
            xResult result = xResult.ERROR_CREATE_PRINTLIST_FAIL;
            DataUtility data = new DataUtility();
            SqlConnection conn = data.connectdb();

            objSystemSerialNo serial = new objSystemSerialNo();
            int startSerial = serial.getSerialNo("TAGID");
            Utility.Log($"startSerial:{startSerial}, Hex:{startSerial:X6}");
            bool r = serial.UpdateSerialNo("TAGID", startSerial + dao.Count);
            Utility.Log($"UpdateSerialNo result: {r}");

            try
            {
                for (int i = 0; i < dao.Count; i++)
                {
                    // Generate a new TagID for each print
                    startSerial++;
                    string tagID = $"{startSerial:X6}";
                    Utility.Log($"Creating print list for RequestNo: {dao.RequestNo}, SID: {dao.SID}, TagID: {tagID}, PrintState: {dao.PrintState}");


                    string sql = $"INSERT INTO PrintList (RequestNo, SID, TagID, PrintState, Remark) VALUES ('{dao.RequestNo}', '{dao.SID}', '{tagID}', {dao.PrintState}, '{dao.Remark}')";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Utility.Log("Print list created successfully.");
                        }
                        else
                        {
                            Utility.Log("Failed to create print list.");
                        }
                    }
                }

                result = xResult.OK;
                conn.Close();
            }
            catch (Exception ex)
            {
                Utility.Log($"Error: {ex}");
            }
            return result;
        }


        public daoPrintList QueryTagBySID(string SID)
        {
            daoPrintList daoP = new daoPrintList();
            DataUtility data = new DataUtility();
            SqlConnection conn = data.connectdb();

            try
            {
                string sql = $"select * from PrintList where SID='{SID}' and PrintState=0";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader myData = cmd.ExecuteReader();
                if (myData.HasRows)
                {
                    // 讀取資料並且顯示出來
                    while (myData.Read())
                    {
                        daoP.RequestNo = myData.GetString(myData.GetOrdinal("requestNo"));
                        daoP.TagID = myData.GetString(myData.GetOrdinal("TagID"));
                        daoP.SID = myData.GetString(myData.GetOrdinal("SID"));
                        daoP.Remark = myData.GetString(myData.GetOrdinal("Remark"));
                        break;
                    }

                    myData.Close();
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                Utility.Log($"Error: {ex}");
            }
            return daoP;
        }

        public bool UpdatePrintState(daoPrintList dao)
        {
            bool result = false;

            DataUtility data = new DataUtility();
            SqlConnection conn = data.connectdb();
            string sql = $"UPDATE PrintList SET PrintState = 1 WHERE RequestNo = '{dao.RequestNo}' AND SID = '{dao.SID}' AND TagID = '{dao.TagID}'";
            try {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Utility.Log("Print list created successfully.");
                    }
                    else
                    {
                        Utility.Log("Failed to create print list.");
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Log($"Error updating print state: {ex}");
                return result;
            }

            return result;
        }
    }
}
