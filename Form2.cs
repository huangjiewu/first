using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            userlogin.Text = LogManager.ReadTXT();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strUserID = userlogin.Text.Trim();
            if (strUserID.IndexOf(",") > 0)
            {
                MessageBox.Show("请用多商户登陆");
                return;
            }
            try
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@ID", SqlDbType.VarChar)
                };
                parameters[0].Value = strUserID;
                //parameters[1].Value = Cryptography.MD5(strPassword);
                DataTable dt = SQLHelper.ExecuteDt("select id from userbase where id=@ID", parameters);
                if (dt.Rows.Count==1)
                {
                    Hide();
                    Form1 form1 = new Form1();
                    form1.Show();
                    form1.userid.Text = dt.Rows[0][0].ToString();
                    LogManager.writeTXT(strUserID);
                }
                else
                {
                    MessageBox.Show("ID不存在");
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("出错", "数据库:" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string strUserID = userlogin.Text.Trim();
            string[] UserID = strUserID.Split(',');
            if (UserID.Length == 1)
            {
                MessageBox.Show("请用单商户登陆");
                return;
            }
            if(IsRepeat(UserID))
            {
                MessageBox.Show("商户出现重复");
                return;
            }            
            try
            {
                for (int i = 0; i < UserID.Length; i++)
                {
                    SqlParameter[] parameters = {new SqlParameter("@ID", SqlDbType.Int)};
                    parameters[0].Value = Convert.ToInt32(UserID[i]);
                    //parameters[1].Value = Cryptography.MD5(strPassword);
                    DataTable dt = SQLHelper.ExecuteDt("select id from userbase where id=@ID", parameters);
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("商户："+UserID[i].ToString()+"不存在");
                        return;
                    }                        
                }     
                Hide();
                LogManager.writeTXT(strUserID);
                Form3 form1 = new Form3();
                form1.Show();
                form1.userid.Text = strUserID;                                                 
     
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("出错", "数据库:" + ex.Message);
            }
        }


        /// <summary>
        /// Hashtable 方法
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsRepeat(string[] array)
        {
            Hashtable ht = new Hashtable();
            for (int i = 0; i < array.Length; i++)
            {
                if (ht.Contains(array[i]))
                {
                    return true;
                }
                else
                {
                    ht.Add(array[i], array[i]);
                }
            }
            return false;
        }
    }
}
