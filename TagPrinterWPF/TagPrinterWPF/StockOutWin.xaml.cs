using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static TagPrinterWPF.App;

namespace TagPrinterWPF
{
    /// <summary>
    /// StockOutWin.xaml 的互動邏輯
    /// </summary>
    public partial class StockOutWin : Window
    {
        public StockOutWin()
        {
            InitializeComponent();

            login();
            if (App.login_user == "")
            {
                Lbl_Empid.Foreground = new SolidColorBrush(Colors.Red);
                Lbl_Empid.Content = $"Not logged in!";
            }
            Lbl_Msg.Content = "";
            Txt_Barcode.Text = "";
            Txt_Barcode.Focus();
        }

        public void login()
        {
            string opid = Interaction.InputBox("PLEASE INPUT OP ID：", "OP ID", "");
            if (string.IsNullOrEmpty(opid))
            {
                MessageBox.Show("Operator ID cannot be empty!");
                Utility.Log($"Operator ID cannot be empt!");
                Console.Beep();
                return;
            }

            if (opid.Length != 8)
            {
                MessageBox.Show("Operator ID format error!");
                Utility.Log($"Operator ID :{opid} format error!");
                Console.Beep();
                return;
            }

            if (opid.Substring(0, 2) == "50" || opid.Substring(0, 2) == "90")
            {
                //正確的格式
                opid = opid.Substring(2, 5);
            }
            else
            {
                //錯誤的格式
                MessageBox.Show("Operator ID must start with 50 or 90!");
                Utility.Log($"Operator ID :{opid} must start with 50 or 90!");
                Console.Beep();
                return;
            }

            App.login_user = opid;
            Lbl_Empid.Foreground = new SolidColorBrush(Colors.Blue);
            Lbl_Empid.Content = $"EmpID: {App.login_user}";

        }

        private void Txt_Barcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(App.login_user == "")
                {
                    DispMessage("請先登入操作員ID", "Red");
                    Txt_Barcode.Text = "";
                    return;
                }

                string barcode = Txt_Barcode.Text.Trim();
                if (barcode.Length < 1)
                {
                    DispMessage("請輸入條碼", "Red");
                    return;
                }
                //else if(barcode.Length < 31)
                //{
                //    DispMessage("條碼格式錯誤!", "Red");
                //    return;
                //}
                else if(barcode.Length == 31 || barcode.Length == 30)
                {
                    string[] parts = barcode.Split('-');
                    string SID = parts[0];
                    SID = SID.PadLeft(18, '0');

                    string TagID = parts[1];
                    string RequestNo = parts[2];

                    DispMessage($"SID: {SID}\nTagID: {TagID}\nRequestNo: {RequestNo}", "Green");
                    

                    //出庫
                    daoWhiteList white = new daoWhiteList(Utility.getDatetime2());
                    white.SID = SID;
                    white.TagID = TagID;
                    white.RequestNo = RequestNo;
                    white.Empid = App.login_user;
                    white.InOutBound = 1;
                    Queue<daoWhiteList> _q = new Queue<daoWhiteList>();
                    _q.Enqueue(white);

                    objWhiteList _o = new objWhiteList();
                    xResult r = _o.Add(_q); // 將資料加入白名單
                    if(r == xResult.OK)
                    {
                        Txt_Barcode.Text = "";
                    }
                    else
                    {
                        Txt_Barcode.Foreground = new SolidColorBrush(Colors.Red);
                        Console.Beep(3500, 500);

                        MessageBox.Show($"Error: 加入出庫白名單失敗！");
                        return;
                    }
                    _q.Clear();

                }
                else
                {
                    DispMessage("條碼格式錯誤!", "Red");
                    return;
                }
            }
        }

        private void DispMessage(string msg, string color)
        {
            if(color == "Red")
            {
                Lbl_Msg.Foreground = new SolidColorBrush(Colors.Red);
                Console.Beep(3500, 500);
            }
            else if (color == "Green")
            {
                Lbl_Msg.Foreground = new SolidColorBrush(Colors.Green);
                Console.Beep();
            }
            else
            {
                Lbl_Msg.Foreground = new SolidColorBrush(Colors.Black);
            }
            Lbl_Msg.Content = msg;
            Utility.Log(msg);
            
        }
    }
}
