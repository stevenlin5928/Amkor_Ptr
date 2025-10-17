using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static TagPrinterWPF.App;

namespace TagPrinterWPF
{
    /// <summary>
    /// Barcode2Print.xaml 的互動邏輯
    /// </summary>
    public partial class Barcode2Print : Window
    {
        //UtilityPrt prt;
        private DispatcherTimer _timer;
        int timeout = 0;
        int defaultTimeOut = 30;
        public Barcode2Print()
        {
            InitializeComponent();
            Lbl_Request.Content = "";
            Lbl_SID.Content = "";
            Lbl_TagID.Content = "";
            Txt_SID.Text = "";
            Txt_SID.Focus();

            //prt = new UtilityPrt();
            bool r = UtilityPrt.ConnectToPrt(App.PTR_IP);
            if (r == false)
            {
                Lbl_Request.Foreground = new SolidColorBrush(Colors.Red);
                Lbl_Request.Content = "無法連接到打印機！";
                Console.Beep(3500, 500);
            }
            else
            {
                Lbl_Request.Foreground = new SolidColorBrush(Colors.Green);
                Lbl_Request.Content = "已連接到打印機!";
            }

            timeout = defaultTimeOut;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // Set interval
            _timer.Tick += Timer_Tick;
            _timer.Start();

            

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (timeout == 0)
            {
                UtilityPrt.ClosePrt();
                Utility.Log("print time out!");
                timeout = -1;
            }
            else if(timeout > 0) 
            {
                timeout--;
            }
        }

        private void Txt_SID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                timeout = defaultTimeOut;

                if (UtilityPrt.ConnectToPrt(App.PTR_IP) == false)
                {
                    Lbl_Request.Foreground = new SolidColorBrush(Colors.Red);
                    Lbl_Request.Content = "無法連接到打印機！";
                    Console.Beep(3500, 500);
                    return;
                }


                string SID = Txt_SID.Text.Trim();
                string str= SID.TrimStart('0'); // 去掉前面的0
                SID= str.PadLeft(18, '0');
                if (SID.Length < 1)
                {
                    Lbl_Request.Content = "請輸入SID";
                    return;
                }
                else
                {
                    //MessageBox.Show($"SID: {SID} 已輸入，請繼續輸入Tag ID");
                    daoPrintList dao = new daoPrintList();
                    objPrintList obj = new objPrintList();
                    dao = obj.QueryTagBySID(SID);

                    if(dao.TagID == "")
                    {
                        Lbl_Request.Foreground = new SolidColorBrush(Colors.Red);
                        Lbl_Request.Content = "沒有找到對應的SID可以列印！";
                        Lbl_SID.Content = "";
                        Lbl_TagID.Content = "";
                        Console.Beep(3500, 500);
                    }
                    else
                    {
                        // 列印
                        bool r = UtilityPrt.SendPrintString(SID.TrimStart('0'), dao.TagID, dao.RequestNo, App.login_user);
                        if(r == false)
                        {
                            Lbl_Request.Foreground = new SolidColorBrush(Colors.Red);
                            Lbl_Request.Content = "列印失敗！";
                            Console.Beep(1500, 500);
                            return;
                        }
                        else
                        {
                            
                            

                            daoWhiteList wl;
                            Queue<daoWhiteList> _q = new Queue<daoWhiteList>();
                            wl = new daoWhiteList(Utility.getDatetime2());
                            wl.SID = SID;
                            wl.TagID = dao.TagID;
                            wl.RequestNo = dao.RequestNo;
                            wl.Empid = App.login_user;
                            wl.Mtr_Qty = 1;
                            wl.Remark = dao.Remark;
                            _q.Enqueue(wl);

                            objWhiteList _o = new objWhiteList();
                            if(xResult.OK != _o.Add(_q))
                            {
                                Txt_SID.Foreground = new SolidColorBrush(Colors.Red);
                                Lbl_Request.Foreground = new SolidColorBrush(Colors.Blue);
                                Lbl_Request.Content = $"RequestNo:{dao.RequestNo}";

                                Lbl_SID.Foreground = new SolidColorBrush(Colors.Blue);
                                Lbl_SID.Content = $"SID:{SID}";

                                Lbl_TagID.Foreground = new SolidColorBrush(Colors.Blue);
                                Lbl_TagID.Content = $"TagID:{dao.TagID}";
                                Console.Beep(3500, 500);

                                MessageBox.Show("列印成功，但無法將資料加入白名單！請檢查資料庫連接或資料格式。", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            _q.Clear();
                        }
                        
                        obj.UpdatePrintState(dao);

                        Txt_SID.Text = "";
                        Lbl_Request.Foreground = new SolidColorBrush(Colors.Blue);
                        Lbl_Request.Content = $"RequestNo:{dao.RequestNo}";

                        Lbl_SID.Foreground = new SolidColorBrush(Colors.Blue);
                        Lbl_SID.Content = $"SID:{SID}";

                        Lbl_TagID.Foreground = new SolidColorBrush(Colors.Blue);
                        Lbl_TagID.Content = $"TagID:{dao.TagID}";
                        Console.Beep();
                    }
                    Utility.Log($"SID:{SID}, TagID:{dao.TagID}, RequestNo:{dao.RequestNo}");
                }
            }
        }

        private void Txt_SID_TextInput(object sender, TextCompositionEventArgs e)
        {
            string SID = e.Text;
        }
    }
}
