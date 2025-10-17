using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.Security.Cryptography;
using static TagPrinterWPF.App;

namespace TagPrinterWPF
{
    internal class daoWhiteList
    {
        public daoWhiteList() { }
        public daoWhiteList(string _id)
        {
            _whitelist_id = _id;
        }

        private string _whitelist_id = "";
        public string whitelist_id
        {
            get { return _whitelist_id; }
            set { _whitelist_id = value; }
        }

        private string _requestno;
        public string RequestNo
        {
            get { return _requestno; }
            set { _requestno = value; }
        }



        private string _gate_id = "";
        public string gate_id
        {
            get { return _gate_id; }
            set { _gate_id = value; }
        }

        private DateTime _UpdateTime;
        public DateTime UpdateTime
        {
            get { return _UpdateTime; }
            set { _UpdateTime = value; }
        }

        private int _in_out_bound = 0;
        public int InOutBound
        {
            get { return _in_out_bound; }
            set { _in_out_bound = value; }
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

        private string _Vendor_Lot = "";
        public String Vendor_Lot
        {
            get { return _Vendor_Lot; }
            set { _Vendor_Lot = value; }
        }

        private double _mtr_qty = 0;

        public double Mtr_Qty
        {
            get { return _mtr_qty; }
            set { _mtr_qty = value; }
        }

        private string _mtr_unit = "";
        public String Mtr_Unit
        {
            get { return _mtr_unit; }
            set { _mtr_unit = value; }
        }

        private string _remark = "";
        public String Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }

        private string _lot = "";
        public String lot
        {
            get { return _lot; }
            set { _lot = value; }
        }

        private string _mc = "";
        public String MC
        {
            get { return _mc; }
            set { _mc = value; }
        }

        private string _empid = "";
        public string Empid
        {
            get { return _empid; }
            set { _empid = value; }
        }

        private string _mtGroup = "";
        public string MTGroup
        {
            get { return _mtGroup; }
            set { _mtGroup = value; }
        }

        private int _enable = 0;
        public int Enable
        {
            get { return _enable; }
            set { _enable = value; }
        }

        private string _Date1;
        public string Date1
        {
            get { return _Date1; }
            set { _Date1 = value; }
        }
        private string _Date2;
        public string Date2
        {
            get { return _Date2; }
            set { _Date2 = value; }
        }

        bool _Checked = false;
        public bool Checked
        {
            get { return _Checked; }
            set { _Checked = value; }
        }
    }


    internal class objWhiteList
    {
        private SqlConnection conn;
        private string whitelist_id = "";
       

        public objWhiteList()
        {

        }
        public objWhiteList(string _whitelist_id)
        { 
            whitelist_id= _whitelist_id;
        }

        public string ParserString(string str)
        {
            if (str == null)
            {
                str = "";
            }
            return str;
        }

        

        // 將Enable設為1，代表白名單為出庫 or 入庫狀態
        public xResult SetStockEnable(List<daoWhiteList> wlist, int enable, string empid)
        {
            xResult result = xResult.ERROR_UPDATE_WHITELIST_FAIL;

            DataUtility data = new DataUtility();
            conn = data.connectdb();
            string sql = "";
            int _no = 0;

            try
            {
                SqlCommand cmd;
                // 出庫 or 入庫完成
                foreach(daoWhiteList item in wlist)
                {
                    sql = $"update white_list set enable={enable}, StockinEmpid='{empid}', Date2='{Utility.getDatetime()}' where requestNo='{item.RequestNo}' AND SID='{item.SID}' AND TagID='{item.TagID}'";

                    Utility.Log(sql);

                    cmd = new SqlCommand(sql, conn);
                    _no = cmd.ExecuteNonQuery();
                    if (_no == 1)
                    {
                        result = xResult.OK;
                    }
                    else
                    {
                        Utility.Log($"ERROR: {sql}, no=0");
                        result = xResult.ERROR_UPDATE_WHITELIST_FAIL;
                    }
                }
                

                data.Closedb();
            }
            catch (Exception ex)
            {
                Utility.Log($"ERROR: {ex}");
            }

            return result;
        }

        public xResult UpdateEmpid(string SID, string TagID, string empid)
        {
            xResult result = xResult.ERROR_UPDATE_WHITELIST_FAIL;

            DataUtility data = new DataUtility();
            conn = data.connectdb();
            string sql = "";
            int _no = 0;

            try
            {
                SqlCommand cmd;
                // 出庫 or 入庫完成
                sql = $"update white_list set empid='{empid}' where SID='{SID}' AND TagID='{TagID}'";

                Utility.Log(sql);

                cmd = new SqlCommand(sql, conn);
                _no = cmd.ExecuteNonQuery();
                if (_no == 1)
                {
                    result = xResult.OK;
                }

                data.Closedb();
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        //
        // 從 AMS 讀取--物料室出庫線邊的數據，add到 white_list_head， white_list
        //
        public xResult Add2(Queue<daoWhiteList> _q)
        {
            xResult result = xResult.OK;

            DataUtility data = new DataUtility();
            conn = data.connectdb();
            string sql = "";
            int _no = 0;

            try
            {
                SqlCommand cmd;
                
                foreach(var item in _q)
                {
                        
                    sql = $"insert into white_list (whitelist_id, RequestNo, SID, TagID, lot, mc, empid, mtgroup, vendor_lot, mtr_qty, mtr_unit, remark, InOutBound,enable,Date1) values " +
                    $"('{item.whitelist_id}', '{item.RequestNo}', '{item.SID}', '{item.TagID}', '{item.lot}', '{item.MC}', '{item.Empid}', " +
                    $"'{item.MTGroup}', '{item.Vendor_Lot}', {item.Mtr_Qty}, '{item.Mtr_Unit}', '{item.Remark}', {item.InOutBound}, {item.Enable}, '{item.Date1}')";

                    cmd = new SqlCommand(sql, conn);
                    _no = cmd.ExecuteNonQuery();
                    if (_no == 1)
                    {
                        Utility.Log($"sql:{sql}, success!");
                    }
                    else
                    {
                        Utility.Log($"sql:{sql}, ERROR!");
                        result = xResult.ERROR_ADD_WHITELIST_FAIL;
                    }
                        
                }

                data.Closedb();
                
            }
            catch (Exception e)
            {
                Utility.Log($"ERROR: {e.Message}");
                Utility.Log($"sql: {sql}");
                result = xResult.ERROR_ADD_WHITELIST_FAIL;
            }

            return result;
        }

        public xResult Add(Queue<daoWhiteList> _q)
        {
            xResult result = xResult.OK;

            DataUtility data = new DataUtility();
            conn = data.connectdb();
            string sql = "";
            int _no = 0;

            try
            {
                SqlCommand cmd;

                foreach (var item in _q)
                {

                    sql = $"insert into white_list (whitelist_id, RequestNo, SID, TagID, lot, mc, empid, mtgroup, vendor_lot, mtr_qty, mtr_unit, remark, InOutBound,enable) values " +
                    $"('{item.whitelist_id}', '{item.RequestNo}', '{item.SID}', '{item.TagID}', '{item.lot}', '{item.MC}', '{item.Empid}', " +
                    $"'{item.MTGroup}', '{item.Vendor_Lot}', {item.Mtr_Qty}, '{item.Mtr_Unit}', '{item.Remark}', {item.InOutBound}, {item.Enable})";

                    cmd = new SqlCommand(sql, conn);
                    _no = cmd.ExecuteNonQuery();
                    if (_no == 1)
                    {
                        Utility.Log($"sql:{sql}, success!");
                    }
                    else
                    {
                        Utility.Log($"sql:{sql}, ERROR!");
                        result = xResult.ERROR_ADD_WHITELIST_FAIL;
                    }

                }

                data.Closedb();

            }
            catch (Exception e)
            {
                Utility.Log($"ERROR: {e.Message}");
                Utility.Log($"sql: {sql}");
                result = xResult.ERROR_ADD_WHITELIST_FAIL;
            }

            return result;
        }

        //
        //1. 載入白名單明細表
        //
        public List<daoWhiteList> Load(string _whiteid)
        {
            //xResult result = xResult.ERROR_LOAD_WHITELIST_FAIL;

            List<daoWhiteList> whitelistDetail = new List<daoWhiteList>();
            whitelistDetail.Clear();
            DataUtility data = new DataUtility();
            conn = data.connectdb();

            string sql = string.Format("select * from white_list");
            if (_whiteid != "")
            {
                sql = $"select * from white_list where whitelist_id='{_whiteid}'";
            }

            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader myData = cmd.ExecuteReader();
                if (myData.HasRows)
                {
                    // 讀取資料並且顯示出來
                    while (myData.Read())
                    {
                        daoWhiteList _whitelist = new daoWhiteList();

                        _whitelist.whitelist_id =   ParserString(myData.GetString(myData.GetOrdinal("whitelist_id")));
                        
                        _whitelist.SID =            ParserString(myData.GetString(myData.GetOrdinal("SID")));
                        _whitelist.TagID =          ParserString(myData.GetString(myData.GetOrdinal("TagID")));
                        _whitelist.Vendor_Lot =     ParserString(myData.GetString(myData.GetOrdinal("Vendor_Lot")));
                        _whitelist.RequestNo  =     ParserString(myData.GetString(myData.GetOrdinal("RequestNo")));
                        _whitelist.Mtr_Qty =        (int)myData.GetDouble(myData.GetOrdinal("Mtr_Qty"));
                        _whitelist.Mtr_Unit =       ParserString(myData.GetString(myData.GetOrdinal("Mtr_Unit")));
                        _whitelist.Remark =         ParserString(myData.GetString(myData.GetOrdinal("Remark")));
                        // From MRS
                        _whitelist.lot =            ParserString(myData.GetString(myData.GetOrdinal("lot")));
                        _whitelist.Empid =          ParserString(myData.GetString(myData.GetOrdinal("Empid")));
                        _whitelist.MTGroup =        ParserString(myData.GetString(myData.GetOrdinal("MTGroup")));
                        _whitelist.MC =             ParserString(myData.GetString(myData.GetOrdinal("mc")));
                        _whitelist.Enable =         myData.GetInt32(myData.GetOrdinal("enable"));

                        whitelistDetail.Add( _whitelist );
                    }
                }
                myData.Close();
                data.Closedb();

                //result = xResult.OK;
            }
            catch (Exception ex)
            {
                //Console.Beep();
                Utility.Log($"ERROR -{ex}");
            }


            return whitelistDetail;
        }

        //
        //1. 載入白名單明細表
        //
        public List<daoWhiteList> Load2(int _enable)
        {
            //xResult result = xResult.ERROR_LOAD_WHITELIST_FAIL;

            List<daoWhiteList> whitelistDetail = new List<daoWhiteList>();
            whitelistDetail.Clear();
            DataUtility data = new DataUtility();
            conn = data.connectdb();

            string sql = string.Format($"select * from white_list where enable={_enable}");
            

            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader myData = cmd.ExecuteReader();
                if (myData.HasRows)
                {
                    // 讀取資料並且顯示出來
                    while (myData.Read())
                    {
                        daoWhiteList _whitelist = new daoWhiteList();

                        _whitelist.whitelist_id = ParserString(myData.GetString(myData.GetOrdinal("whitelist_id")));

                        _whitelist.SID = ParserString(myData.GetString(myData.GetOrdinal("SID")));
                        _whitelist.TagID = ParserString(myData.GetString(myData.GetOrdinal("TagID")));
                        _whitelist.Vendor_Lot = ParserString(myData.GetString(myData.GetOrdinal("Vendor_Lot")));
                        _whitelist.RequestNo = ParserString(myData.GetString(myData.GetOrdinal("RequestNo")));
                        _whitelist.Mtr_Qty = (int)myData.GetDouble(myData.GetOrdinal("Mtr_Qty"));
                        _whitelist.Mtr_Unit = ParserString(myData.GetString(myData.GetOrdinal("Mtr_Unit")));
                        _whitelist.Remark = ParserString(myData.GetString(myData.GetOrdinal("Remark")));
                        // From MRS
                        _whitelist.lot = ParserString(myData.GetString(myData.GetOrdinal("lot")));
                        _whitelist.Empid = ParserString(myData.GetString(myData.GetOrdinal("Empid")));
                        _whitelist.MTGroup = ParserString(myData.GetString(myData.GetOrdinal("MTGroup")));
                        _whitelist.MC = ParserString(myData.GetString(myData.GetOrdinal("mc")));
                        _whitelist.Enable = myData.GetInt32(myData.GetOrdinal("enable"));

                        whitelistDetail.Add(_whitelist);
                    }
                }
                myData.Close();
                data.Closedb();

                //result = xResult.OK;
            }
            catch (Exception ex)
            {
                //Console.Beep();
                Utility.Log($"ERROR -{ex}");
            }


            return whitelistDetail;
        }
    }
}
