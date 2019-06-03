using Microsoft.Office.Interop.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace huangjialang
{
    public partial class Form3 : Form
    {
        string server = "localhost";
        string database = "hjl";
        string uid = "root";
        string password = "123456";

        List<string> MemberList;//员工名单
        string onloadMember;

        //DataSet ds;
        string connectionString;//= "datasource=" + server + ";"+"PORT=3306;"+ "DATABASE=" + database + ";" + "username=" + uid + ";" + "PASSWORD=" + password + ";";
        MySqlConnection conn;

        public Form3()
        {
            InitializeComponent();
            connectionString = "datasource=" + server + ";" + "PORT=3306;" + "DATABASE=" + database + ";" + "username=" + uid + ";" + "PASSWORD=" + password + ";";
            conn = new MySqlConnection(connectionString);
        }

        

        private void Form3_Load(object sender, EventArgs e)
        {
            conn = new MySqlConnection(connectionString);
           
            onloadMember = "";

            loadMemberList();
        }

        public void GetMemberList()
        {
            conn.Open();
            MemberList = new List<string>();
            string query = "SELECT `员工姓名` FROM hjl.`员工名单` where `是否离职` = 0";
            MySqlCommand command = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    MemberList.Add(reader.GetString(0));

                }
            }

            conn.Close();
        }

        public void loadMemberList()
        {
            conn.Open();
            MemberListcomboBox1.Items.Clear();
            //MemberListcomboBox1.Items.Add("");
            string query = "SELECT `员工姓名` FROM hjl.`员工名单` where `是否离职` = 0";
            MySqlCommand command = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    MemberListcomboBox1.Items.Add(reader.GetString(0));

                }
                reader.Close();
            }

            conn.Close();
            //MemberListcomboBox1.SelectedItem = "";

        }

        private void MemberListcomboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            conn.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            int result = dateTimePicker2.Value.CompareTo(dateTimePicker1.Value);

            if (result < 0)
            {
                dateTimePicker2.Value = dateTimePicker1.Value.AddDays(30);

            }
            dateTimePicker2.MinDate = dateTimePicker1.Value;
        }

        private void SearchButton1_Click(object sender, EventArgs e)
        {
            conn.Open();
            string startdate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string enddate = dateTimePicker2.Value.ToString("yyyy-MM-dd");
                
            if(MemberListcomboBox1.SelectedItem!=null)
            { 
                onloadMember = MemberListcomboBox1.SelectedItem.ToString();
                string sendcommand = "SELECT `日期`, `单号`, `车牌`, `型号`, `项目`, `成员` ,`金额` FROM hjl." + onloadMember + " where `日期` >= '" + startdate + "' and " + "`日期` <= '" + enddate + "'"
                                        + "UNION SELECT ' ' , ' ', ' ', ' ', ' ','总数' ,sum(`金额`) as `金额` FROM hjl." + onloadMember + " where `日期` >= '" + startdate + "' and " + "`日期` <= '" + enddate + "'";
                MySqlDataAdapter adapter = new MySqlDataAdapter(sendcommand, conn);
                //conn.Open();
                DataSet ds = new DataSet();
                adapter.Fill(ds, onloadMember);
                dataGridView1.DataSource = ds.Tables[onloadMember];
            }
            conn.Close();
        }

        private void exportbutton1_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "excel work|*.xlsx", ValidateNames = true })
            {

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                    Workbook wb = app.Workbooks.Add(XlSheetType.xlWorksheet);
                    //Worksheet ws = (Worksheet)app.ActiveSheet;
                    app.Visible = false;
                    /* ws.Name = "jialang";
                     
                     int i = 2;
                     ws.Cells[1, 1] = "日期";
                     ws.Cells[1, 2] = "单号";
                     ws.Cells[1, 3] = "车牌";
                     ws.Cells[1, 4] = "型号";
                     ws.Cells[1, 5] = "项目";
                     ws.Cells[1, 6] = "成员";
                     ws.Cells[1, 7] = "金额";
                     foreach (DataGridViewRow row in dataGridView1.Rows)
                     {

                         ws.Cells[i, 1] = row.Cells[0].Value.ToString();
                         ws.Cells[i, 2] = row.Cells[1].Value.ToString();
                         ws.Cells[i, 3] = row.Cells[2].Value.ToString();
                         ws.Cells[i, 4] = row.Cells[3].Value.ToString();
                         ws.Cells[i, 5] = row.Cells[4].Value.ToString();
                         ws.Cells[i, 6] = row.Cells[5].Value.ToString();
                         ws.Cells[i, 7] = row.Cells[6].Value.ToString();

                         i++;
                     }
                     */

                    DataSet dr = getMemberData();

                    foreach (string member in MemberList)
                    {
                        Worksheet ws = wb.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        ws = (Worksheet)app.ActiveSheet;
                        ws.Name = member;

                        int i = 2;
                        ws.Cells[1, 1] = "日期";
                        ws.Cells[1, 2] = "单号";
                        ws.Cells[1, 3] = "车牌";
                        ws.Cells[1, 4] = "型号";
                        ws.Cells[1, 5] = "项目";
                        ws.Cells[1, 6] = "成员";
                        ws.Cells[1, 7] = "金额";
                        
                        int j = 2;
                        foreach (DataRow row in dr.Tables[member].Rows)
                        {
                            ws.Cells[j, 1] = row[0].ToString();//.Value.ToString();
                            ws.Cells[j, 2] = row[1].ToString();
                            ws.Cells[j, 3] = row[2].ToString();
                            ws.Cells[j, 4] = row[3].ToString();
                            ws.Cells[j, 5] = row[4].ToString();
                            ws.Cells[j, 6] = row[5].ToString();
                            ws.Cells[j, 7] = row[6].ToString();

                            j++; 
                          }

                    }



                    wb.SaveAs(sfd.FileName, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,false, false, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
                    app.Quit();
                    MessageBox.Show("saved");
                    
                }

            }
        }




        public DataSet getMemberData()
        {
            GetMemberList();
            DataSet ds =new DataSet();

           


            foreach (string membername in MemberList)
            {
                string startdate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string enddate = dateTimePicker2.Value.ToString("yyyy-MM-dd");


                conn.Open();
                string sendcommand = "SELECT `日期`, `单号`, `车牌`, `型号`, `项目`, `成员` ,`金额` FROM hjl." + membername + " where `日期` >= '" + startdate + "' and " + "`日期` <= '" + enddate + "'"
                                       + "UNION SELECT ' ' , ' ', ' ', ' ', ' ','总数' ,sum(`金额`) as `金额` FROM hjl." + membername + " where `日期` >= '" + startdate + "' and " + "`日期` <= '" + enddate + "'";
                MySqlDataAdapter adapter = new MySqlDataAdapter(sendcommand, conn);
                adapter.Fill(ds, membername);

                conn.Close();
            }


            return ds;
        }










    }
}
