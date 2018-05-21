using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;



namespace CPrint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           // this.WindowState = FormWindowState.Maximized;
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            this.txt_fh.Text = "";
            this.label4.Text = "";
            this.txt_fh.Focus();
         
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            try
            {
                this.label4.Text = "";
                if (!string.IsNullOrEmpty(this.cmb_ph.Text))
                {
                    if (!string.IsNullOrEmpty(this.txt_fh.Text))
                    {

                        #region 简单根据第一码是否是%判断条码扫描是否正确
                        if (!txt_fh.Text.StartsWith("%"))
                        {
                          // MessageBox.Show("条码错误，请确认第一码是否是%");

                           #region 弹窗
                           //Form_Message f = new Form_Message();

                           //f.Show();
                            this.label4.Text = "条码第一码不为%";
                           #endregion

                           txt_fh.SelectAll();
                            return;
                            
                        }

                        #endregion


                        #region  判断条码长度是否不为12码，如果是提示；

                        if (this.txt_fh.Text.Trim().Length != 10)
                        {
                        
                           // MessageBox.Show("条码长度不正确！");
                            this.label4.Text = "条码长度不正确！";
                           // this.btn_print.Enabled = false;
                            FormLogin f = new FormLogin();
                            f.StartPosition = FormStartPosition.CenterScreen;
                            if (f.ShowDialog()!=DialogResult.OK)
                            {
                                return;
                            }

                        }

                                #region 比对是否已经打过，已经存在于wps文件中。
                        if (heckisexit(this.txt_fh.Text.Trim().Substring(1).ToString(), this.cmb_ph.Text))
                        {
                            this.label4.Text = "条码重复，已经打印，请联系主管确认！！";

                            FormLogin f = new FormLogin();
                            f.StartPosition = FormStartPosition.CenterScreen;
                          
                            if (f.ShowDialog()!=DialogResult.OK)
                            {
                                return;
                            }

                        }
                                #endregion

                        #endregion
                        this.label4.Text = "";
                        #region print
                        LabelManager2.Application labelapp = new LabelManager2.Application(); //创建lppa.exe进程
                        string strPath = System.Windows.Forms.Application.StartupPath + "\\PrinteModel1.lab";
                        

                        labelapp.Documents.Open(strPath, false);
                        LabelManager2.Document labeldoc = labelapp.ActiveDocument;

                        labeldoc.Variables.FormVariables.Item("p1").Value = cmb_ph.Text.Trim();
                        string fh = txt_fh.Text.Trim().Substring(1);
                        labeldoc.Variables.FormVariables.Item("p2").Value = fh;
                        //MessageBox.Show(fh);

                        labeldoc.PrintDocument(); //打印一次
                        labeldoc.FormFeed(); //结束打印

                        labeldoc.Close(true);
                        labelapp.Application.Quit();

                        #endregion

                        #region SaveToCsv
                        SaveDataToCsv();
                        #endregion

                        #region bindgrid

                        //BindGrid();
                        //改为一次只绑定一笔
                        BindDataManu();
                        #endregion

                        #region clear
                        // btn_clear_Click(null, null);
                        #endregion
                        this.txt_fh.Text = "";
                        this.txt_fh.Focus();
                    }
                    else
                    {
                        MessageBox.Show("请扫描番号条码");
                    }
                }
                else
                {
                    MessageBox.Show("请先选择品号！");
                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            
         
        }


        
        private void txt_fh_TextChanged(object sender, EventArgs e)
        {
            this.label4.Text = "";
            if (this.txt_fh.TextLength >=12)
            {
                this.btn_print_Click(sender, e);
            }
            //else
            //{
            //    MessageBox.Show("条码长度不正确，不为12码！");
            //    this.btn_print.Enabled = false;
            //    FormLogin f = new FormLogin();
            //    f.Show();
            //}
            
        }

        private bool heckisexit(string barcode,string cmb)
        {
            
            string sfilename = AppDomain.CurrentDomain.BaseDirectory;
            string cnstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + sfilename + ";Extended Properties=\"text;HDR=YES;FMT=Delimited\";";
            //string cnstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\txtFilesFolder\\;Extended Properties=\"text;HDR=Yes;FMT=Delimited\";";
            //HDR=Yes表示有列头，No表示无列头
            OleDbConnection cn = new OleDbConnection(cnstring);
            string aSQL =string.Format("select count(*) as cn from DVR保证书数据.csv where FH='{0}' and ZZFH='{1}' ",cmb,barcode);
            cn.Open();
            OleDbDataAdapter da = new OleDbDataAdapter(aSQL, cn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (Convert.ToInt32(dt.Rows[0][0])>0)
            {
                return true;
            }
            else
            {
                return false;
            }
          

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            #region 注册
            //RegistryKey RootKey, RegKey;

            ////项名为：HKEY_CURRENT_USER\Software
            //RootKey = Registry.CurrentUser.OpenSubKey("Software", true);

            ////打开子项：HKEY_CURRENT_USER\Software\MyRegDataApp1
            //if ((RegKey = RootKey.OpenSubKey("MyRegDataApp1", true)) == null)
            //{
            //    RootKey.CreateSubKey("MyRegDataApp1");//不存在，则创建子项
            //    RegKey = RootKey.OpenSubKey("MyRegDataApp1", true);
            //    RegKey.SetValue("UseTime", (object)19);  //创建键值，存储可使用次数
            //    MessageBox.Show("您可以免费使用本软件20次！", "感谢您首次使用");
            //    return;
            //}

            //try
            //{
            //    object usetime = RegKey.GetValue("UseTime");//读取键值，可使用次数
            //    MessageBox.Show("你还可以使用本软件 :" + usetime.ToString() + "次！", "确认", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    int newtime = Int32.Parse(usetime.ToString()) - 1;

            //    if (newtime < 0)
            //    {
            //        if (MessageBox.Show("继续使用，请购买本软件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
            //        {
            //            // Application.Exit();
            //            System.Environment.Exit(0);
            //        }
            //    }
            //    else
            //    {
            //        RegKey.SetValue("UseTime", (object)newtime);//更新键值，可使用次数减1
            //    }
            //}
            //catch
            //{
            //    RegKey.SetValue("UseTime", (object)10); //创建键值，存储可使用次数
            //    MessageBox.Show("您可以免费使用本软件10次！", "感谢您首次使用");
            //    return;
            //}
            #endregion

            #region Fill Combobox
            ReadPCodeConfig();

            #endregion

           

        }

        private void btn_save_Click(object sender, EventArgs e)
        {
           SaveDataToCsv();
        }

        private void SaveDataToCsv()
        {
            try
            {
                StreamWriter sw;
                string data = string.Empty;
               
                if (!File.Exists("DVR保证书数据.csv"))
                {
                    sw = new StreamWriter("DVR保证书数据.csv", true);
                    data = "品番,制造番号,打印时间";
                    sw.WriteLine(data);
                    sw.Close();
                }

                sw = new StreamWriter("DVR保证书数据.csv", true);
                DateTime dt = DateTime.Now;

                data = this.cmb_ph.Text + "," + this.txt_fh.Text.Trim().Substring(1).ToString() + "," + dt.GetDateTimeFormats('g')[0].ToString();
                sw.WriteLine(data);
           
                sw.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


        }

        #region 读取CSV到DataGridView

        private void BindGrid() 
        {
            string sfilename = AppDomain.CurrentDomain.BaseDirectory;
            //string sfilename = AppDomain.CurrentDomain.BaseDirectory + "\\DVR保证书数据.csv";
            string cnstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + sfilename + ";Extended Properties=\"text;HDR=YES;FMT=Delimited\";";
            //string cnstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\txtFilesFolder\\;Extended Properties=\"text;HDR=Yes;FMT=Delimited\";";
            //HDR=Yes表示有列头，No表示无列头
            OleDbConnection cn = new OleDbConnection(cnstring);
           // string aSQL = "select * from DVR保证书数据.csv ";
           string aSQL = "select top 3 * from DVR保证书数据.csv order by printdate desc";
            cn.Open();
            OleDbDataAdapter da = new OleDbDataAdapter(aSQL, cn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cn.Close();
            dataGridView1.DataSource = dt;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.GreenYellow;
            //行Header的背景色为橙色
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.Lime;

          

        
        }


        #endregion


        #region 手工绑定一行数据到DataGridView
        private void BindDataManu() 
        {
            dataGridView1.DataSource = null; //先清掉之前的绑定信息  
            dataGridView1.Refresh();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("品番"));
            dt.Columns.Add(new DataColumn("制造番号"));
            dt.Columns.Add(new DataColumn("打印时间"));
            DateTime dtime = DateTime.Now;

            dt.Rows.Add(new object[] { cmb_ph.Text, txt_fh.Text.Replace("%",""), dtime.GetDateTimeFormats('g')[0].ToString() });
           

            //dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dt;
            dataGridView1.Refresh();
            //dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.GreenYellow;

            dataGridView1.Rows[0].DefaultCellStyle.BackColor = Color.GreenYellow; 
            
            //行Header的背景色为橙色
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.Lime;
        
        }


        #endregion



        private void ReadPCodeConfig() 
        {
            string XmlFileString = "PCode.Xml";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(XmlFileString);
            XmlNodeList nodelist = xmldoc.SelectSingleNode("Config").ChildNodes;
            for (int i = 0; i < nodelist.Item(0).ChildNodes.Count; i++)
            {
                //this.cmb_ph.Items.Add(xmldoc.SelectSingleNode("Config").ChildNodes[i].InnerText);
                this.cmb_ph.Items.Add(nodelist.Item(0).ChildNodes[i].InnerText);
                
            }
        
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex <= 0)
            {
                dataGridView1.Rows[e.RowIndex].HeaderCell.Value = (e.RowIndex + 1).ToString();

            }
        }



    }
}
