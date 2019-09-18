using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string alipayUrl = Common.alipayUrl;
        string TZalipayUrl = Common.TZalipayUrl;
        string strbank = Common.strbank;
        string struserid = "";

        /// <summary>
        /// 获取银行表单
        /// </summary>
        public string Delay1(int milliSecond)
        {
            string L500 = webBrowser2.Url.ToString();  
            string webBrowser2html="";
            if (L500.IndexOf("enterpriseSingleChannelDeposit") > 0)
            {
                webBrowser2html= webBrowser2.Document.Body.InnerHtml;
                return webBrowser2html;
            }
            if (L500.IndexOf("error") > 0)   //网址有错误时返回
            {
                webBrowser2.Url = new Uri(alipayUrl);
                LogManager.WriteLog("出错", "error:"+ L500);
            }
            Common.Sleep(milliSecond);
            return webBrowser2html;
        }

        /// <summary>
        /// 修改表单信息
        /// </summary>
        public void setbank(string instId,string apiCode, string money)
        {
            webBrowser2.Document.All["instId"].SetAttribute("value", instId);
            webBrowser2.Document.All["apiCode"].SetAttribute("value", apiCode);
            HtmlElement amount = webBrowser2.Document.All["amount"];
            HtmlElement J_depositForm = webBrowser2.Document.All["J_depositForm"];
            amount.SetAttribute("value", money);
            J_depositForm.SetAttribute("target", "");
            webBrowser2.Document.All["channelType"].SetAttribute("value", "B2C_EBANK");
            webBrowser2.Document.All["channelAccessType"].SetAttribute("value", "EBANK_B2C");

            HtmlElement btnSubmit = webBrowser2.Document.All["J_submit"];
            btnSubmit.InvokeMember("click"); //模拟点击登录按钮
        }

        #region 银行代码转换
        public void selectbank(string bank,string money)
        {
            switch (bank)
            {
                case "招商银行":
                    setbank("CMB", "cmb103", money);
                    break;
                case "工商银行":
                    setbank("ICBC", "icbc105", money);
                    break;
                case "建设银行":
                    setbank("CCB", "ccb103", money);
                    break;
                case "中国银行":
                    setbank("BOC", "boc102", money);
                    break;
                case "中国光大银行":
                    setbank("CEB", "cebnucc103", money);
                    break;
                case "兴业银行":
                    setbank("CIB", "cib102", money);
                    break;
                case "中信银行":
                    setbank("CITIC", "citicnucc103", money);
                    break;
                case "中国邮政储蓄银行":
                    setbank("PSBC", "psbcnucc103", money);
                    break;
                case "平安银行":
                    setbank("SPABANK", "spabanknucc103", money);
                    break;
                case "浦发银行":
                    setbank("SPDB", "spdbnucc103", money);
                    break;
                case "华夏银行":
                    setbank("HXBANK", "hxbanknucc103", money);
                    break;
                case "北京银行":
                    setbank("BJBANK", "bjbanknucc103", money);
                    break;
                case "杭州银行":
                    setbank("HZCB", "hzcbnucc103", money);
                    break;
                case "宁波银行":
                    setbank("NBBANK", "nbbanknucc103", money);
                    break;
                case "上海银行":
                    setbank("SHBANK", "shbanknucc103", money);
                    break;
                case "广东发展银行":
                    setbank("GDB", "gdbnucc103", money);
                    break;
            }
        }
        public string selectbank(int bank, string money)
        {
            switch (bank)
            {
                case 970:
                    setbank("CMB", "cmb103", money);
                    return "招商银行";
                case 967:
                    setbank("ICBC", "icbc105", money);
                    return "工商银行"; 
                case 965:
                    setbank("CCB", "ccb103", money);
                    return "建设银行"; 
                case 963:
                    setbank("BOC", "boc102", money);
                    return "中国银行"; 
                case 986:
                    setbank("CEB", "cebnucc103", money);
                    return "中国光大银行"; 
                case 972:
                    setbank("CIB", "cib102", money);
                    return "兴业银行"; 
                case 987:
                    setbank("CITIC", "citicnucc103", money);
                    return "中信银行"; 
                case 971:
                    setbank("PSBC", "psbcnucc103", money);
                    return "中国邮政储蓄银行"; 
                case 978:
                    setbank("SPABANK", "spabanknucc103", money);
                    return "平安银行"; 
                case 977:
                    setbank("SPDB", "spdbnucc103", money);
                    return "浦发银行"; 
                case 982:
                    setbank("HXBANK", "hxbanknucc103", money);
                    return "华夏银行";
                case 989:
                    setbank("BJBANK", "bjbanknucc103", money);
                    return "北京银行";
                case 983:
                    setbank("HZCB", "hzcbnucc103", money);
                    return "杭州银行";
                case 998:
                    setbank("NBBANK", "nbbanknucc103", money);
                    return "宁波银行";
                case 975:
                    setbank("SHBANK", "shbanknucc103", money);
                    return "上海银行";
                case 985:
                    setbank("GDB", "gdbnucc103", money);
                    return "广东发展银行";
            }
            return "";
        }
        #endregion

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (webBrowser2.Url.ToString().IndexOf("https://excashier.alipay.com") != 0)
            {
                MessageBox.Show("请先登陆支付宝！");
                webBrowser2.Url = new Uri(alipayUrl);
                return;
            } 
            startstatus.Text = "开始中...";
            startstatus.ForeColor = Color.Red;
            button2.Enabled = false;
            button4.Enabled = true;
            int i = 0;
            while (startstatus.Text == "开始中..."|| startstatus.Text == "切换中...")
            {
                #region  处理获取银行网址
                i++;
                if (startstatus.Text == "开始中...")
                {
                    GetBankParameter(i);
                } 
                refurbish(i);  //每二百秒刷新一次页面，防止登陆过期
                Common.Sleep(1000);
                #endregion

                #region 查询到帐信息

                if (TZstatus.Text == "开始中..." && i % 10 == 9)   //开始并10秒一次
                {
                    try
                    {
                        DataTable orderdt = SQLHelper.ExecuteDt("SELECT orderid,paymodeId,refervalue,userid,addtime,status FROM V_orderbankUrl where addtime > DATEADD(MINUTE, -10, GETDATE()) " + struserid + " and paystatus=0 and supplierId = 8203 and bankurl is not null" + strbank);
                        if (orderdt.Rows.Count > 0)
                        {
                            webBrowser1.Url = new Uri(TZalipayUrl);
                            DataTable dt = new DataTable();
                            if (dt.Rows.Count == 0)
                            {
                                #region 获取表格
                                HtmlDocument doc = webBrowser1.Document;
                                dt.Columns.Add("paytime", System.Type.GetType("System.DateTime"));
                                dt.Columns.Add("alipayorderid", System.Type.GetType("System.String"));
                                dt.Columns.Add("bankname", System.Type.GetType("System.String"));
                                dt.Columns.Add("bankorderid", System.Type.GetType("System.String"));
                                dt.Columns.Add("paymoney", System.Type.GetType("System.String"));
                                dt.Columns.Add("bankID", System.Type.GetType("System.Int32"));
                                dt.Columns.Add("parter", System.Type.GetType("System.Int32"));

                                HtmlElementCollection tbs = doc.GetElementsByTagName("TABLE");
                                foreach (HtmlElement tb in tbs)
                                {
                                    HtmlElementCollection trs = tb.GetElementsByTagName("TR");
                                    foreach (HtmlElement tr in trs)
                                    {
                                        HtmlElementCollection tds = tr.GetElementsByTagName("TD");
                                        if (tds.Count > 0)
                                        {
                                            DataRow dr = dt.NewRow();
                                            for (int y = 0; y < tds.Count; y++)
                                            {
                                                dr["paytime"] = Convert.ToDateTime(tds[0].InnerText.Insert(10, " "));
                                                dr["alipayorderid"] = tds[1].InnerText;
                                                dr["bankname"] = tds[2].InnerText;
                                                dr["bankorderid"] = tds[3].InnerText;
                                                dr["paymoney"] = tds[4].InnerText.Replace(",", "");
                                                dr["bankID"] = Common.getbankID(tds[2].InnerText);
                                                dr["parter"] = Convert.ToInt32(userid.Text.Trim());
                                            }
                                            dt.Rows.Add(dr);
                                        }
                                    }
                                }
                                #endregion
                                #region 支付宝后台充值列表写入数据库
                                if (dt.Rows.Count > 0)
                                {
                                    DateTime NewestPaytime = DateTime.Now.AddMinutes(-10);     //数据库最新的一条到帐信息记录
                                    string Lastalipayorderid = "0000";
                                    try
                                    {
                                        DataTable DTPaytime = SQLHelper.ExecuteDt("SELECT top 1 paytime,alipayorderid FROM alipayLog where parter=" + userid.Text + " order by paytime desc");
                                        if (DTPaytime.Rows.Count > 0)
                                        {
                                            NewestPaytime = Convert.ToDateTime(DTPaytime.Rows[0][0]);
                                            Lastalipayorderid = DTPaytime.Rows[0][1].ToString();     
                                        }
                                    }
                                    catch (Exception ex) { LogManager.WriteLog("出错", "数据库Paytime" + ex.Message); }

                                    DataRow[] paylist = dt.Select("paytime>'" + NewestPaytime.ToString() + "'");

                                    if (dt.Select("paytime='"+ NewestPaytime.ToString() + "'").Length > 1)
                                    {
                                        //解决支付时间相同BUG
                                        paylist = Common.GetSameTimeOrder(dt, NewestPaytime, Lastalipayorderid, paylist);
                                    }

                                    if (paylist.Length > 0)
                                    {
                                        DataTable newdt = dt.Clone();  //复制表结构                                  

                                        foreach (DataRow dr in paylist)
                                        {
                                            newdt.ImportRow(dr);
                                        }
                                        SqlParameter[] para = {
                                            new SqlParameter("@Table", SqlDbType.Structured)
                                        };
                                        para[0].Value = newdt;
                                        SQLHelper sqlhelp = new SQLHelper();
                                        SqlDataReader reader;
                                        sqlhelp.RunProc("proc_orderbank_alipayLog", para, out reader);
                                        while (reader.Read())
                                        {
                                            string strorder = DateTime.Now.ToString() + "，订单：" + reader[0].ToString() + "金额：" + reader[1].ToString() + ",支付宝订单：" + reader[2].ToString() + ",银行：" + reader[3].ToString();
                                            string ret =Common.Notify4F(reader[0].ToString(), reader[2].ToString(), reader[1].ToString());
                                            if (ret == "opstate=0")
                                            {
                                                AllTranAmt.Text = (Convert.ToDecimal(AllTranAmt.Text) + Convert.ToDecimal(reader[1])).ToString();
                                                strorder += ",OK" + Environment.NewLine;
                                            }
                                            else
                                            {
                                                strorder += ",失败" + Environment.NewLine;
                                            }
                                            textBox2.Text = strorder + textBox2.Text;

                                        }
                                        reader.Close();

                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog("出错", "查询充值记录:" + ex.Message);
                    }
                }
                #endregion
            }

        }

        /// <summary>
        /// 获取银行表单
        /// </summary>
        private void GetBankParameter(int i)
        {
            button2.Text = "启动时间(秒)：" + i.ToString();
            try
            {
                struserid = " and userid = " + userid.Text;
                DataTable dt = SQLHelper.ExecuteDt("SELECT orderid,paymodeId,refervalue,userid FROM V_orderbankUrl where addtime > DATEADD(MINUTE, -1, GETDATE()) " + struserid + " and supplierId = 8203 and bankurl is null" + strbank);
                if (dt.Rows.Count > 0)
                {
                    string orderid = dt.Rows[0][0].ToString();
                    int paymodeId = Convert.ToInt32(dt.Rows[0][1]);
                    string refervalue = decimal.Round(Convert.ToDecimal(dt.Rows[0][2].ToString()), 2).ToString();
                    string parter = dt.Rows[0][3].ToString();
                    string bankname = selectbank(paymodeId, refervalue);

                    string webBrowser2html = "";
                    int count = 0;
                    while (webBrowser2html == "" && count < 10)
                    {
                        count++;
                        webBrowser2html = Delay1(300);    //延时0.3秒
                    }
                    if (webBrowser2html != "")
                    {
                        textBox1.Text = DateTime.Now.ToString() + "，商户ID:" + parter + "，订单号:" + orderid + "，银行：" + bankname + ",金额：" + refervalue + Environment.NewLine + textBox1.Text;
                        string sql = "insert into orderbankUrl(orderid, bankurl)values(@orderid, @bankurl);";
                        SqlParameter[] parameters = {
                                new SqlParameter("@orderid", SqlDbType.VarChar),
                                new SqlParameter("@bankurl", SqlDbType.NVarChar)
                            };
                        parameters[0].Value = orderid;
                        parameters[1].Value = webBrowser2html;
                        SQLHelper.ExecuteSql(sql, parameters);
                        webBrowser2.Url = new Uri(alipayUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("对象") > 0)
                {
                    webBrowser2.Url = new Uri(alipayUrl);
                }
                LogManager.WriteLog("出错", "充值:" + ex.Message);
            }
        }

        /// <summary>
        /// 二百秒心跳检测
        /// </summary>
        private void refurbish(int i)
        {
            if (i % 200 == 199)    
            {
                string WebUrl = webBrowser2.Url.ToString();
                if (WebUrl.IndexOf("alipay.com") < 0 || WebUrl.IndexOf("error") > 0)
                {
                    LogManager.WriteLog("出错", "error:" + WebUrl);
                    webBrowser2.Url = new Uri(alipayUrl);
                }
                else
                {
                    webBrowser2.Refresh();
                }
            }
        }

        /// <summary>
        /// 充值启动
        /// </summary>
        private void button4_Click_1(object sender, EventArgs e)
        {
            if (TZstatus.Text == "开始中...")
            {
                startstatus.Text = "切换中...";
                MessageBox.Show("切换中，请在最后一笔订单10分钟后再关闭软件");
                return;
            }
            startstatus.Text = "停止中";
            button2.Text = "充值启动";
            startstatus.ForeColor = Color.Black;
            button2.Enabled = true;
            button4.Enabled = false;
        }
     

        /// <summary>
        /// 修改浏览器信息
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Common.SetIE(Common.IeVersion.强制ie10);
            MessageBox.Show("修改为：" + "强制ie10" + "，请重新启动程序！");
        }
          

        /// <summary>
        /// 到帐信息查询停止
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            TZstatus.Text = "停止中";
            button2.Text = "到帐通知启动";
            TZstatus.ForeColor = Color.Black;
            button6.Enabled = true;
            button5.Enabled = false;
        }

        /// <summary>
        /// 查询到帐信息
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            if (startstatus.Text != "开始中...")
            {
                MessageBox.Show("请先充值启动");
                return;
            }
            if (webBrowser1.Url==null||webBrowser1.Url.ToString().IndexOf(TZalipayUrl) != 0)
            {
                webBrowser1.Url = new Uri(TZalipayUrl);
            }

            TZstatus.Text = "开始中...";
            TZstatus.ForeColor = Color.Red;
            button6.Enabled = false;
            button5.Enabled = true;
             

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit(); 
        }

       


    }
}
