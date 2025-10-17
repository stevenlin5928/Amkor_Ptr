using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using static TagPrinterWPF.App;

namespace TagPrinterWPF
{
    public class daoInventoryStock
    {
        private string _SerialNo = "";
        public string SerialNo
        {
            get {  return _SerialNo; }
            set { _SerialNo = value; }
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


        private string _Vendor_lot = "";
        public string Vendor_lot
        {
            get { return _Vendor_lot; }
            set { _Vendor_lot = value; }
        }


        private float _Mtr_qty = 0;
        public float Mtr_qty
        {
            get { return _Mtr_qty; }
            set { _Mtr_qty = value; }
        }

        private string _Color = "";
        public string Color
        {
            get { return _Color; }
            set { _Color = value; }
        }
    }

    internal class objInventoryStock
    {
        private SqlConnection conn;

        public string ParserString(string str)
        {
            if (str == null)
            {
                str = "";
            }
            return str;
        }

        public objInventoryStock()
        {
        }

        public string getInvSerialNo()
        {
            string _serialno = "";
            
            //DataUtility data = new DataUtility();
            //conn = data.connectdb();

            string sql = $"select serialno from inventorystockhead where enable=0";
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader myData = cmd.ExecuteReader();
                if (myData.HasRows)
                {
                    // 讀取資料並且顯示出來
                    while (myData.Read())
                    {
                        _serialno = ParserString(myData.GetString(myData.GetOrdinal("SerialNo")));
                        break;
                    }
                }
                myData.Close();
                //data.Closedb();

                //result = xResult.OK;
            }
            catch (Exception ex)
            {
                //Console.Beep();
                Utility.Log($"ERROR -{ex}");
            }
            return _serialno;
        }

        public Queue<daoInventoryStock> getInventoryStock()
        {
            Queue<daoInventoryStock> _inv = new Queue<daoInventoryStock>();
            DataUtility data = new DataUtility();
            conn = data.connectdb();

            string _serialno = getInvSerialNo();
            if(_serialno == "")
            {
                return _inv;
            }

            Utility.Log($"Serial No: {_serialno}");

            string sql = $"select * from inventorystock where serialno='{_serialno}'";
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader myData = cmd.ExecuteReader();
                if (myData.HasRows)
                {
                    // 讀取資料並且顯示出來
                    while (myData.Read())
                    {
                        daoInventoryStock invstock = new daoInventoryStock();

                        invstock.SerialNo = _serialno;
                        invstock.SID = ParserString(myData.GetString(myData.GetOrdinal("SID")));
                        invstock.TagID = invstock.SID + ParserString(myData.GetString(myData.GetOrdinal("TagID")));
                        invstock.Mtr_qty = (int)myData.GetDouble(myData.GetOrdinal("Mtr_Qty"));
                        invstock.Color = "Black";

                        _inv.Enqueue(invstock);

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

            return _inv;
        }

        public bool setInventoryStockEnable(string serialno, int _enable, string _empid)
        {
            bool result = false;

            DataUtility data = new DataUtility();
            conn = data.connectdb();
            string sql = "";
            int _no = 0;

            try
            {
                SqlCommand cmd;



                sql = $"update inventorystockhead set enable={_enable}, userid='{_empid}' where SerialNo='{serialno}'";

                cmd = new SqlCommand(sql, conn);
                _no = cmd.ExecuteNonQuery();
                if (_no == 1)
                {
                    Utility.Log($"sql:{sql}, success!");
                    result = true;
                }
                else
                {
                    Utility.Log($"sql:{sql}, ERROR!");
                    //result = xResult.ERROR_ADD_WHITELIST_FAIL;
                }

                

                data.Closedb();

            }
            catch (Exception e)
            {
                Utility.Log($"ERROR: {e.Message}");
                Utility.Log($"sql: {sql}");
                //result = xResult.ERROR_ADD_WHITELIST_FAIL;
            }

            return result;
        }
    }
}
