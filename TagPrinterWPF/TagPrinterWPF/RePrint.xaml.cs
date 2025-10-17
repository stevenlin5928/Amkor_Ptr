using System.Windows;
using System.Windows.Controls;

namespace TagPrinterWPF
{
    /// <summary>
    /// RePrint.xaml 的互動邏輯
    /// </summary>
    public partial class RePrint : Window
    {
        List<daoWhiteList> _list;
        public RePrint()
        {
            InitializeComponent();

            Txt_OPID.Text = App.login_user;

            objWhiteList wlist = new objWhiteList();
            _list = wlist.Load("");

            foreach (var item in _list)
            {
                if(!Combo_Request.Items.Contains(item.RequestNo))
                    Combo_Request.Items.Add(item.RequestNo);
            }

        }

        private void Btn_Print_Click(object sender, RoutedEventArgs e)
        {
            if (Combo_Request.SelectedItem == null)
            {
                MessageBox.Show("請選擇Request No.");
                return;
            }
            if (Combo_SID.SelectedItem == null)
            {
                MessageBox.Show("請選擇SID");
                return;
            }
            if (Combo_TagID.SelectedItem == null)
            {
                MessageBox.Show("請選擇Tag ID");
                return;
            }
            if (Txt_OPID.Text == "")
            {
                MessageBox.Show("請輸入操作人員ID");
                return;
            }

            string ?_requestno = Combo_Request.SelectedItem.ToString();
            string ?_sid = Combo_SID.SelectedItem.ToString();
            string ?_tagid = Combo_TagID.SelectedItem.ToString();
            string ?_opid = Txt_OPID.Text;



            //UtilityPrt prt = new UtilityPrt();
            
            UtilityPrt.ConnectToPrt(App.PTR_IP);
            UtilityPrt.SendPrintString(_sid, _tagid, _requestno, _opid);

            objWhiteList wlist = new objWhiteList();
            wlist.UpdateEmpid(_sid,_tagid,_opid);

        }

        private void Combo_Request_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Combo_SID.Items.Clear();

            foreach (var item in _list)
            {
                if (item.RequestNo == Combo_Request.SelectedItem.ToString())
                {
                    if (!Combo_SID.Items.Contains(item.SID))
                        Combo_SID.Items.Add(item.SID);
                    
                }
            }
        }

        private void Combo_SID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Combo_TagID.Items.Clear();

            foreach (var item in _list)
            {
                if (item.SID == Combo_SID.SelectedItem.ToString() && item.RequestNo == Combo_Request.SelectedItem.ToString())
                {
                    
                    if (!Combo_TagID.Items.Contains(item.TagID))
                        Combo_TagID.Items.Add(item.TagID);
                }
            }
        }

        private void Combo_TagID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
