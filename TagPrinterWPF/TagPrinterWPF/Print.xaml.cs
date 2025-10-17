using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UniPRT.Sdk.LabelMaker.Interfaces;
using static TagPrinterWPF.App;

namespace TagPrinterWPF
{
    /// <summary>
    /// Print.xaml 的互動邏輯
    /// </summary>
    public partial class Print : Window
    {
        ObservableCollection<daoMRS> MRSDataset = new ObservableCollection<daoMRS>();
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        int timeout = 30;
        int timeCount = 30;
        public Print()
        {
            InitializeComponent();
            login();
            if(App.login_user == "")
            {
                Lbl_Empid.Foreground = new SolidColorBrush(Colors.Red);
                Lbl_Empid.Content = $"Not logged in!";

                Btn_CreatePrintList.IsEnabled = false;
                Btn_MRS_Refresh.IsEnabled = false;
                Btn_rePrint.IsEnabled = false;
                Btn_StartPrint.IsEnabled = false;
                return;
            }
            

            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 10);


            List_MRS.ItemsSource = MRSDataset;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (timeout > 0)
            {
                timeout--;

            }
            else
            {
                App.login_user = "";
                Lbl_Empid.Foreground = new SolidColorBrush(Colors.Red);
                Lbl_Empid.Content = $"Not logged in!";

                //timeout = timeCount;
            }
        }

        public void login()
        {
         
        
            string empid = "";
            string opid = Interaction.InputBox("PLEASE INPUT OP ID：", "OP ID", "");
            if(opid == "")
            {
                Console.Beep(3500, 500);
                return;
            }

            AmkorMRSServices _myService = new AmkorMRSServices();
            bool result = _myService.SOAP_CheckOpPermissions(opid, out empid);

            if (result == true)
            {
                App.login_user = empid;
                Lbl_Empid.Foreground = new SolidColorBrush(Colors.Blue);
                Lbl_Empid.Content = $"EmpID: {App.login_user}";
            }
            else
            {
                Console.Beep(3500, 500);
            }

        

            timeout = timeCount;

            Timer.Start();
        }
        private void Btn_StartPrint_Click(object sender, RoutedEventArgs e)
        {
            if(App.login_user == "")
            {
                login();
            }

            timeout = timeCount;

 //           UtilityPrt prt = new UtilityPrt();
 //           prt.ConnectToPrt(App.PTR_IP);
            
            Queue<daoMRS> ptrItem = new Queue<daoMRS>();
            int count = 0;
            foreach(var item in MRSDataset)
            {
                if(item.Checked == true)
                {
                    if (item.Remark.Contains("PTR"))
                    {
                        //如果有PTR字樣，則不列印
                        Utility.Log($"RequestNo: {item.RequestNo}, SID:{item.SID}, Qty：{item.Qty} has PTR, skip print.");
                        MessageBox.Show($"RequestNo: {item.RequestNo}, SID:{item.SID} has PTR, skip print.");
                        Console.Beep();
                        return;
                    }

                    count = count + item.Qty;
                    ptrItem.Enqueue(item);

                    Utility.Log($"RequestNo: {item.RequestNo}, SID:{item.SID}, Qty：{item.Qty}");

                    
                }
            }
            //int a = 64730;
            //string s = $"-->{a:X6}";
            //Utility.Log(s);
            objSystemSerialNo serial = new objSystemSerialNo();
            int startSerial = serial.getSerialNo("TAGID");
            Utility.Log($"startSerial:{startSerial}, Hex:{startSerial:X6}");
            bool result = serial.UpdateSerialNo("TAGID", startSerial+count);
            Utility.Log($"UpdateSerialNo result: {result}");

            objMRS mrs = new objMRS();
            daoWhiteList wl;
            Queue<daoWhiteList> _q = new Queue<daoWhiteList>();

            foreach (var item in ptrItem)
            {
                for(int index = 0; index < item.Qty; index++)
                {
                    startSerial++;
                    Utility.Log($"SID: {item.SID}, TagID:{startSerial}-->{startSerial:X6}");

                    string str_serial = $"{startSerial:X6}";
  //                  prt.SendPrintString(item.SID.TrimStart('0'), str_serial, item.RequestNo, App.login_user);

                    wl = new daoWhiteList(Utility.getDatetime2());
                    wl.SID = item.SID.TrimStart('0');
                    wl.TagID = str_serial;
                    wl.RequestNo = item.RequestNo;
                    wl.Empid = App.login_user;

                    _q.Enqueue(wl);
                }
                objWhiteList obj = new objWhiteList();
                obj.Add(_q);
                _q.Clear();

                mrs.MarkAlreadyPtr(item.RequestNo, item.SID, item.Remark);
            }

            //prt.ClosePrt();

            MRSDataset.Clear();
            mrs.Load();
            while (mrs._mrs.Count > 0)
            {
                daoMRS m = mrs._mrs.Dequeue();
                MRSDataset.Add(m);
            }

        }

        private void Btn_MRS_Refresh_Click(object sender, RoutedEventArgs e)
        {
            MRSDataset.Clear();
            timeout = timeCount;

            objMRS mrs = new objMRS();
            mrs.Load();
            while (mrs._mrs.Count > 0)
            {
                daoMRS m = mrs._mrs.Dequeue();
                MRSDataset.Add(m);
            }

        }

        private void List_MRS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //daoMRS p = (daoMRS)e.AddedItems[0];
            ////MessageBox.Show(p.RequestNo);
            //int index = p.index;
            //if(p.Checked == false)
            //{
            //    MRSDataset[index - 1].Checked = true;
            //}
            //else
            //{
            //    MRSDataset[index - 1].Checked = false;
            //}

                List_MRS.Items.Refresh();
        }

        private void Btn_rePrint_Click(object sender, RoutedEventArgs e)
        {
            if (App.login_user == "")
            {
                login();
            }
            timeout = timeCount;

            RePrint rePrint = new RePrint();
            rePrint.Top = this.Top + (this.Height-rePrint.Height)/2;
            rePrint.WindowStyle = WindowStyle.ToolWindow;
            rePrint.Left = this.Left+(this.Width-rePrint.Width)/2;
            rePrint.Topmost = true;
            rePrint.Show();
        }

        private void Btn_CreatePrintList_Click(object sender, RoutedEventArgs e)
        {
            if(App.login_user == "")
            {
                login();
            }
            timeout = timeCount;
            objMRS mrs = new objMRS();

            foreach (var item in MRSDataset)
            {
                if (item.Checked == true)
                {
                    if (item.Memo.Contains("PTR"))
                    {
                        Console.Beep();
                        //如果有PTR字樣，則不列印
                        Utility.Log($"RequestNo: {item.RequestNo}, SID:{item.SID}, Qty：{item.Qty} has PTR, skip print.");
                        MessageBox.Show($"RequestNo: {item.RequestNo}, SID:{item.SID} Print list has been generated. Please skip this requestID!");
                        
                        return;
                    }

                    daoPrintList ptr = new daoPrintList();
                    ptr.RequestNo = item.RequestNo;
                    ptr.SID = item.SID;
                    ptr.PrintState = 0; // 0:未列印, 1:已列印
                    ptr.TagID = "";
                    ptr.Remark = item.Remark;
                    ptr.Count = item.Qty;
                    
                    objPrintList objPrint = new objPrintList();
                    xResult r = objPrint.CreatePrintList(ptr);
                    if(r == xResult.OK)
                    {
                        Utility.Log($"RequestNo: {ptr.RequestNo}, SID:{ptr.SID}, Count：{ptr.Count} added to print list.");
                        
                        mrs.MarkAlreadyPtr(ptr.RequestNo, ptr.SID, "PTR");
                        
                    }
                    else
                    {
                        Utility.Log($"Failed to add RequestNo: {ptr.RequestNo}, SID:{ptr.SID} to print list. Error: {r}");
                        MessageBox.Show($"Failed to add RequestNo: {ptr.RequestNo}, SID:{ptr.SID} to print list. Error: {r}");
                    }

                    

                }
            }

            MRSDataset.Clear();
            mrs.Load();
            while (mrs._mrs.Count > 0)
            {
                daoMRS m = mrs._mrs.Dequeue();
                MRSDataset.Add(m);
            }

        }

        private void Btn_Barcode2Print_Click(object sender, RoutedEventArgs e)
        {
            if (App.login_user == "")
            {
                login();
            }
            timeout = timeCount;
            Timer.Stop();

            Barcode2Print barcode2Print = new Barcode2Print();
            barcode2Print.Top = this.Top + (this.Height - barcode2Print.Height) / 2;
            barcode2Print.WindowStyle = WindowStyle.ToolWindow;
            barcode2Print.Left = this.Left + (this.Width - barcode2Print.Width) / 2;
            barcode2Print.Topmost = true;
            barcode2Print.Show();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.login_user = "";
        }
    }
}
