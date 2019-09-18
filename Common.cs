using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class Common
    {

        public static string alipayUrl = "https://bizfundprod.alipay.com/allocation/deposit/index.htm";
        public static string TZalipayUrl = "https://mbillexprod.alipay.com/enterprise/depositQuery.htm";
        public static string strbank = " and paymodeid in(970,967,965,963,986,972,987,981,971,978,977,982,989,983,998,975,985)";

        public static void Sleep(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }


        public static int getbankID(string bankname)
        {
            switch (bankname)
            {
                case "招行":
                    return 970;
                case "工行":
                    return 967;
                case "建行":
                    return 965;
                case "中行":
                    return 963;
                case "光大":
                    return 986;
                case "兴业":
                    return 972;
                case "中信":
                    return 987;
                case "邮储":
                    return 971;
                case "平安":
                    return 978;
                case "浦发":
                    return 977;
                case "华夏":
                    return 982;
                case "北京":
                    return 989;
                case "杭州":
                    return 983;
                case "宁波":
                    return 998;
                case "上海":
                    return 975;
                case "广发":
                    return 985;
            }
            return 0;
        }

        #region 设置浏览器
        /// <summary>
        /// 定义IE版本的枚举
        /// </summary>
        public enum IeVersion
        {
            强制ie10,//10001 (0x2711) Internet Explorer 10。网页以IE 10的标准模式展现，页面!DOCTYPE无效
            标准ie10,//10000 (0x02710) Internet Explorer 10。在IE 10标准模式中按照网页上!DOCTYPE指令来显示网页。Internet Explorer 10 默认值。
            强制ie9,//9999 (0x270F) Windows Internet Explorer 9. 强制IE9显示，忽略!DOCTYPE指令
            标准ie9,//9000 (0x2328) Internet Explorer 9. Internet Explorer 9默认值，在IE9标准模式中按照网页上!DOCTYPE指令来显示网页。
            强制ie8,//8888 (0x22B8) Internet Explorer 8，强制IE8标准模式显示，忽略!DOCTYPE指令
            标准ie8,//8000 (0x1F40) Internet Explorer 8默认设置，在IE8标准模式中按照网页上!DOCTYPE指令展示网页
            标准ie7//7000 (0x1B58) 使用WebBrowser Control控件的应用程序所使用的默认值，在IE7标准模式中按照网页上!DOCTYPE指令来展示网页
        }

        /// <summary>
        /// 设置WebBrowser的默认版本
        /// </summary>
        /// <param name="ver">IE版本</param>
        public static void SetIE(IeVersion ver)
        {
            string productName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;//获取程序名称

            object version;
            switch (ver)
            {
                case IeVersion.标准ie7:
                    version = 0x1B58;
                    break;
                case IeVersion.标准ie8:
                    version = 0x1F40;
                    break;
                case IeVersion.强制ie8:
                    version = 0x22B8;
                    break;
                case IeVersion.标准ie9:
                    version = 0x2328;
                    break;
                case IeVersion.强制ie9:
                    version = 0x270F;
                    break;
                case IeVersion.标准ie10:
                    version = 0x02710;
                    break;
                case IeVersion.强制ie10:
                    version = 0x2711;
                    break;
                default:
                    version = 0x1F40;
                    break;
            }

            RegistryKey key = Registry.CurrentUser;
            RegistryKey software =
                key.CreateSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION\" + productName);
            if (software != null)
            {
                software.Close();
                software.Dispose();
            }
            RegistryKey wwui =
                key.OpenSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            //该项必须已存在
            if (wwui != null) wwui.SetValue(productName, version, RegistryValueKind.DWord);
        }


        #endregion

        /// <summary>
        /// 获取支付时间相同的订单
        /// </summary>
        public static DataRow[] GetSameTimeOrder(DataTable dt, DateTime NewestPaytime, string Lastalipayorderid, DataRow[] paylist)
        {
            DataRow[] sameTimedrs = dt.Select("paytime='" + NewestPaytime.ToString() + "' and alipayorderid<>'" + Lastalipayorderid + "'");
            DataRow[] newdrs = new DataRow[paylist.Length + sameTimedrs.Length];
            paylist.CopyTo(newdrs, 0);
            sameTimedrs.CopyTo(newdrs, paylist.Length);
            paylist = newdrs;
            return paylist;
        }

        /// <summary>
        /// 向四方发送到帐信息
        /// </summary>
        public static string Notify4F(string outer_trade_no, string inner_trade_no, string trade_amount)
        {
            try
            {
                //创建HttpWebRequest对象
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://45.192.173.214:18082/Receive/alipayZC/Bank.aspx");//目标地址

                //模拟POST的数据
                string postData = string.Empty;

                postData += "outer_trade_no=" + outer_trade_no;
                postData += "&trade_status=" + "TRADE_SUCCESS";
                postData += "&inner_trade_no=" + inner_trade_no;
                postData += "&trade_amount=" + trade_amount;
                Encoding utf8 = Encoding.UTF8;
                byte[] data = utf8.GetBytes(postData);

                //设置请求头信息
                string cookieheader = string.Empty;
                CookieContainer cookieCon = new CookieContainer();
                request.Method = "POST";
                //设置cookie，若没有可以不设置
                request.CookieContainer = cookieCon;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                Stream newStream = request.GetRequestStream();
                //把请求数据写入请求流中
                newStream.Write(data, 0, data.Length);
                newStream.Close();


                //获得HttpWebResponse对象
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();


                //获得响应流
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                //输入响应流信息       

                string Ret = readStream.ReadToEnd();
                LogManager.WriteLog("发送通知", postData + Ret);

                response.Close();
                receiveStream.Close();
                readStream.Close();
                return Ret;
            }
            catch {
                LogManager.WriteLog("可能掉单", outer_trade_no + Environment.NewLine);                
            }
            return "";
        }
    }
}
