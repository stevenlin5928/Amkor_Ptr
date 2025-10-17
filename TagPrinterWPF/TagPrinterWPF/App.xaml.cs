using System.Configuration;
using System.Data;
using System.Windows;

namespace TagPrinterWPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static string PTR_IP = "";
    public static string SQLConnection = "";
    public static bool enablePTR = false;
    public static string login_user = "";
    public static bool isPermission = false; //是否有權限操作
    public static string UploadInvDataUserID = "";
    public static string UploadInvDataSerialNo = "";

    public enum xResult
    {
        OK = 0,
        ERROR_DB_Connect_Fail,
        ERROR_DB_Access_Fail,
        ERROR_DB_Query_Fail,
        ERROR_DB_Unknow_Fail,
        ERROR_ADD_SCAN_TAG_FAIL,
        ERROR_Add_SystemConfig_Fail,
        ERROR_ADD_AMSSTOCK_FAIL,
        ERROR_Unknow_Fail,
        ERROR_LOAD_WHITELIST_FAIL,
        ERROR_LOAD_WHITELISTHEAD_FAIL,
        ERROR_DELETE_TEMPDATA_FAIL,
        ERROR_ADD_AMS_STOCK_IN_FAIL,
        ERROR_LOAD_AMSStock_FAIL,
        ERROR_AMS_QuerySIDOrder_Fail,
        ERROR_Update_SystemConfig_Fail,
        ERROR_RFID_SETUP_FAIL,
        ERROR_LOAD_RFIDCONFIG_FAIL,
        ERROR_RFID_COM_ISNOT_SETUP,
        ERROR_ADD_MRS_WHITELIST_FAIL,
        ERROR_LOAD_MRS_WHITELIST_FAIL,
        ERROR_ADD_WHITELIST_FAIL,
        ERROR_QUERY_SCAN_TAG_FAIL,
        ERROR_NULL_FROM_QuerySIDOrder,
        ERROR_NULL_FROM_QueryMRSInfo,
        ERROR_UPDATE_WHITELISTHEAD_FAIL,
        ERROR_PRINT_FAIL,
        ERROR_CREATE_PRINTLIST_FAIL,
        ERROR_MOVE_TAGS_FAIL,
        ERROR_ADD_TEMPDATA_FAIL,
        ERROR_SET_STATE_FAIL,
        ERROR_DELETE_TEST_DATA_FAIL,
        ERROR_UPDATE_WHITELIST_FAIL,
        ERROR_SCAN_TAGS_MISSING                  //盤點沒有讀取全部
    }

}

