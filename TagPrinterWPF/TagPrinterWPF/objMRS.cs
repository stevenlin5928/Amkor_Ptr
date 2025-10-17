using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace TagPrinterWPF
{
    class daoMRS
    {
        int _index = 0;
        public int index
        {
            get { return _index; }
            set { _index = value; }
        }

        string _BgColor = "Blue";
        public string BgColor
        {
            get {
              
                return _BgColor; 
            }
            set { _BgColor = value; }
        }

        bool _Checked = false;
        public bool Checked
        {
            get { return _Checked; }
            set {
                
                _Checked = value;

            }
        }

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

        private int _Qty = 0;
        public int Qty
        {
            get { return _Qty; }
            set { _Qty = value; }
        }

        private string _Unit = "";
        public string Unit
        {
            get { return _Unit; }
            set { _Unit = value; }
        }

        private string _UpdateTime = "";
        public string UpdateTime
        {
            get { return _UpdateTime; }
            set { _UpdateTime = value; }
        }

        private string _remark = "";
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }

        private string _Memo = "";
        public string Memo
        {
            get { return _Memo; }
            set { _Memo = value; }
        }
    }
    class objMRS
    {
        
        public Queue<daoMRS> _mrs = new Queue<daoMRS>();
        SqlConnection conn; 

        public objMRS()
        {

        }

        public bool MarkAlreadyPtr(string RequestNo, string sid, string Memo)
        {
            bool result = false;

            DataUtility data = new DataUtility();
            conn = data.connectdb();
            if(Memo.Contains("PTR") == false)
            {
                return result; 
            }
            string sql = string.Format($"update white_list_from_mrs set Memo='{Memo}' where RequestNo='{RequestNo}' and SID='{sid}'");

            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                int i = cmd.ExecuteNonQuery();
                if (i != 1)
                {
                    Utility.Log($"{sql}");
                    Utility.Log($"ERROR: Update 0 record! white_list_from_mrs:{RequestNo}");
                }
                else
                {
                    result = true;
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                Utility.Log($"ERROR: {ex.Message}");
            }

            return result;
        }


        public bool Load()
        {
            bool result = false;
            _mrs.Clear();
            int index = 0;
            DataUtility data = new DataUtility();
            conn = data.connectdb();

            string sql = string.Format("select * from white_list_from_mrs");


            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader myData = cmd.ExecuteReader();
                if (myData.HasRows)
                {
                    // 讀取資料並且顯示出來
                    while (myData.Read())
                    {
                        index++;

                        daoMRS mrs = new daoMRS();
                        mrs.index = index;
                        mrs.Checked = false;
                        mrs.RequestNo = myData.GetString(myData.GetOrdinal("RequestNo"));
                        mrs.SID = myData.GetString(myData.GetOrdinal("SID"));
                        mrs.Qty = (int)myData.GetDouble(myData.GetOrdinal("mtr_qty"));
                        mrs.Unit = myData.GetString(myData.GetOrdinal("mtr_unit"));
                        mrs.UpdateTime = myData.GetDateTime(myData.GetOrdinal("updatetime")).ToString("yyyy/MM/dd HH:mm:ss");
                        mrs.Remark = myData.GetString(myData.GetOrdinal("Remark"));
                        mrs.Memo = myData.GetString(myData.GetOrdinal("Memo"));
                        if (mrs.Memo.Contains("PTR"))
                        {
                            mrs.BgColor = "Red";
                        }
                        _mrs.Enqueue(mrs);
                    }
                }
                myData.Close();
                data.Closedb();

                result = true;
            }
            catch (Exception ex)
            {
                //Console.Beep();
                Utility.Log("ERROR:" + ex.Message);
            }


            return result;
        }

    }
}
