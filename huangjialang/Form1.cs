using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace huangjialang
{
    public partial class Form1 : Form
    {
        string server = "localhost";
        string database = "hjl";
        string uid = "root";
        string password = "123456";
        List<string> TableNames;//表格名单
        List<string> ColumnNames;
        List<string> MemberList;//员工名单

        //DataSet ds;
        string connectionString;//= "datasource=" + server + ";"+"PORT=3306;"+ "DATABASE=" + database + ";" + "username=" + uid + ";" + "PASSWORD=" + password + ";";
        MySqlConnection conn;
        string OnLoadTable;


        public Form1()
        {
            InitializeComponent();
            connectionString = "datasource=" + server + ";" + "PORT=3306;" + "DATABASE=" + database + ";" + "username=" + uid + ";" + "PASSWORD=" + password + ";";
            conn = new MySqlConnection(connectionString);
            dateTimePicker3.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePicker1.Value = DateTime.Now;
        }

        private void CarNametextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CarNametextBox1.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            //load all table from DB
            GetTableList();
            loadTableList();
            loadMemberList();


            DataSet ds = new DataSet();
            //connect to DB
            
            conn.Open();

            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM hjl." + "维修记录", conn);
            OnLoadTable = TableNames[TableNames.IndexOf("维修记录")];
            
            adapter.Fill(ds, TableNames[TableNames.IndexOf("维修记录")]);
            dataGridView1.DataSource = ds.Tables[TableNames[TableNames.IndexOf("维修记录")]];
            conn.Close();
            comboBox1.SelectedItem = TableNames[TableNames.IndexOf("维修记录")];


        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            conn.Open();
            if (comboBox1.SelectedItem.ToString()!=null)
            {
                
                OnLoadTable = comboBox1.SelectedItem.ToString();
                MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM hjl." + OnLoadTable, conn);
                //conn.Open();
                DataSet ds = new DataSet();
                adapter.Fill(ds, OnLoadTable);
                dataGridView1.DataSource = ds.Tables[OnLoadTable];
                
            }
            conn.Close();
        }


        public void GetTableList()
        {
            TableNames = new List<string>();
            TableNames.Add("维修记录");
            TableNames.Add("单号信息");
            /*string query = "show tables from hjl";
            MySqlCommand command = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    TableNames.Add(reader.GetString(0));

                }
            }
            */


        }

        public void loadTableList()
        {
            comboBox1.Items.Clear();
            foreach (String dt in TableNames)
            {
                comboBox1.Items.Add(dt);
            }

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
            MemberListcomboBox2.Items.Clear();
            MemberListcomboBox3.Items.Clear();
            MemberListcomboBox4.Items.Clear();


            MemberListcomboBox1.Items.Add("");
            MemberListcomboBox2.Items.Add("");
            MemberListcomboBox3.Items.Add("");
            MemberListcomboBox4.Items.Add("");


            string query = "SELECT `员工姓名` FROM hjl.`员工名单` where `是否离职` = 0";
            MySqlCommand command = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    MemberListcomboBox1.Items.Add(reader.GetString(0));
                    MemberListcomboBox2.Items.Add(reader.GetString(0));
                    MemberListcomboBox3.Items.Add(reader.GetString(0));
                    MemberListcomboBox4.Items.Add(reader.GetString(0));

                }
            }

            MemberListcomboBox1.SelectedItem = "";
            MemberListcomboBox2.SelectedItem = "";
            MemberListcomboBox3.SelectedItem = "";
            MemberListcomboBox4.SelectedItem = "";

            conn.Close();
        }


        //open member info form
        private void button2_Click(object sender, EventArgs e)
        {
            var form2 = new Form2();
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.ShowDialog(this);
            //form2.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {   /*
            DialogResult dialog = MessageBox.Show("确定关闭？","关闭",MessageBoxButtons.YesNo);
           if (dialog != DialogResult.Yes)
            {

                e.Cancel = true;

            }   */

        }

        private void AddCarInfobutton1_Click(object sender, EventArgs e)
        {
            
            //GetMemberList();
            string carname = CarNametextBox1.Text.Replace(" ", "");
            string carmodel = CarModeltextBox2.Text.Replace(" ", "");
            string recodedate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string ordernumber = OrederNumbrtTextBox1.Text.Replace(" ", "");
            string handler = HandlerTextBox1.Text.Replace(" ", "");
            string ownername = OwnerNameTextBox1.Text.Replace(" ", "");
            string ownerphone = OwnerPhoneTextbox1.Text.Replace(" ", "");


            //to check the ordernumber is exist or not 
            conn.Open();
            string sendcomm = "SELECT COUNT(*) FROM hjl.单号信息 where `单号` = @ordernumber";
            MySqlCommand checkCar = new MySqlCommand(sendcomm, conn);
            checkCar.Parameters.AddWithValue("@ordernumber", ordernumber);
            int carcount = Convert.ToInt32(checkCar.ExecuteScalar());
            conn.Close();

            if (carcount == 1)
            { MessageBox.Show("单号已登记"); }
            else if (carcount == 0)
            {
                if (carmodel == "")
                {
                    MessageBox.Show("请输入车型号");
                }

                else {

                    conn.Open();
                    string query = "INSERT INTO `hjl`.`单号信息` (`单号`,`车牌`, `型号`,`日期`,`接单人`,`车主姓名`,`车主电话`) " + "VALUES('" + ordernumber + "', '" + carname + "', '"
                                                + carmodel + "', '" + recodedate + "', '" + handler + "', '" + ownername + "', '" + ownerphone + "')";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    MessageBox.Show("登记成功");
                    conn.Close();
                    GetTableList();
                    loadTableList();
                    updateOrderNumber();
                    changeDataGridView("单号信息");

                    dateTimePicker2.Value = dateTimePicker1.Value;
                    OrderNumberTextBox2.Text = ordernumber;
                    carmodeltextBox1.Text = carmodel;
                    CarNametextBox2.Text = carname;
                    
                    
                    
                    //reset the all text box
                    CarNametextBox1.Text = "";
                    CarModeltextBox2.Text = "";
                    HandlerTextBox1.Text = "";
                    OwnerNameTextBox1.Text = "";
                    OwnerPhoneTextbox1.Text = "";
                }

            }



            conn.Close();

          
        }

        private void CarNametextBox2_TextChanged(object sender, EventArgs e)
        {/*
            comboBox1.SelectedItem = "维修记录";
            conn.Open();
            
            string search = CarNametextBox2.Text;
            string pass;

            pass = "SELECT * FROM hjl.`维修记录` where 车牌 like '%" + search + "%'";
            if (search == "")
            { //MessageBox.Show("请输入关键字"); 
            }
            else
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(pass, conn);


                DataSet ds = new DataSet();
                adapter.Fill(ds, search);

                dataGridView1.DataSource = ds.Tables[search];
            }
            conn.Close();
            */
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e) 
        {
            
            int result = dateTimePicker4.Value.CompareTo(dateTimePicker3.Value);

            if (result<0)
            {
                dateTimePicker4.Value = dateTimePicker3.Value.AddDays(30);

            }
            dateTimePicker4.MinDate = dateTimePicker3.Value;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
            //set up vaule
            string member1="";
            string member2="";
            string member3="";
            string member4="";
            string member1wage = "";
            string member2wage = "";
            string member3wage = "";
            string member4wage = "";


            //get all info
            string date = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            string ordernumber = OrderNumberTextBox2.Text.Replace(" ", "");
            string carname = CarNametextBox2.Text.Replace(" ", "");
            string carmodel = carmodeltextBox1.Text.Replace(" ", "");
            string item = textBox1.Text.Replace(" ", "");
            if (MemberListcomboBox1.SelectedItem.ToString() != null)
            { member1 = MemberListcomboBox1.SelectedItem.ToString(); }
            if (MemberListcomboBox2.SelectedItem.ToString() != null)
            { member2 = MemberListcomboBox2.SelectedItem.ToString(); }
            if (MemberListcomboBox3.SelectedItem.ToString() != null)
            { member3 = MemberListcomboBox3.SelectedItem.ToString(); }
            if (MemberListcomboBox4.SelectedItem.ToString() != null)
            { member4 = MemberListcomboBox4.SelectedItem.ToString(); }


            if (WagetextBox1.Text != null)
            { member1wage = WagetextBox1.Text.Replace(" ", ""); }
            if (WagetextBox2.Text != null)
            { member2wage = WagetextBox2.Text.Replace(" ", ""); }
            if (WagetextBox3.Text != null)
            { member3wage = WagetextBox3.Text.Replace(" ", ""); }
            if (WagetextBox4.Text != null)
            { member4wage = WagetextBox4.Text.Replace(" ", ""); }



            string members = member1 + " " + member2 + " " + member3 + " " + member4;





            //to check the ordernumber is exist or not 
            conn.Open();
            string send = "SELECT COUNT(*) FROM hjl.单号信息 where `单号` = @ordernumber and `车牌` = @carname";
            MySqlCommand checkCar = new MySqlCommand(send, conn);
            checkCar.Parameters.AddWithValue("@ordernumber", ordernumber);
            checkCar.Parameters.AddWithValue("@carname", carname);
            int carcount = Convert.ToInt32(checkCar.ExecuteScalar());
            conn.Close();




            if (carcount == 0)
            { MessageBox.Show("单号未已登记或单号与车牌不符"); }
            
            else
            {
                if ((member1 == "" && member1wage != "") || (member1 != "" && member1wage == ""))
                { MessageBox.Show("组员或金额不能为空1"); }
                else if ((member2 == "" && member2wage != "") || (member2 != "" && member2wage == ""))
                { MessageBox.Show("组员或金额不能为空2"); }
                else if ((member3 == "" && member3wage != "") || (member3 != "" && member3wage == ""))
                { MessageBox.Show("组员或金额不能为空3"); }
                else if ((member4 == "" && member4wage != "") || (member4 != "" && member4wage == ""))
                { MessageBox.Show("组员或金额不能为空4"); }

                else {

                    //add record to 维修记录 table
                    conn.Open();
                    string sendcomm = "INSERT INTO `hjl`.`维修记录` (`日期`,`单号`,`车牌`, `型号`,`项目`,`维修人员`) " + "VALUES('" + date + "', '" + ordernumber
                                        + "', '" + carname + "', '" + carmodel + "', '" + item + "', '" + members + "')";
                    MySqlCommand command = new MySqlCommand(sendcomm, conn);
                    command.ExecuteNonQuery();
                    conn.Close();

                    //add record to member1
                    if (member1 != "")
                    {
                        conn.Open();
                        string sendcomm1 = "INSERT INTO `hjl`.`" + member1 + "` (`日期`,`单号`,`车牌`, `型号`, `项目`,`成员`,`金额`) " + "VALUES('" + date + "', '" + ordernumber
                                            + "', '" + carname + "', '" + carmodel + "', '" + item + "', '" + members + "', '" + member1wage + "')";
                        MySqlCommand command1 = new MySqlCommand(sendcomm1, conn);
                        command1.ExecuteNonQuery();
                        conn.Close();
                    }

                    //add record to member2
                    if (member2 != "")
                    {
                        conn.Open();
                        string sendcomm2 = "INSERT INTO `hjl`.`" + member2 + "` (`日期`,`单号`,`车牌`, `型号`, `项目`,`成员`,`金额`) " + "VALUES('" + date + "', '" + ordernumber
                                            + "', '" + carname + "', '" + carmodel + "', '" + item + "', '" + members + "', '" + member2wage + "')";
                        MySqlCommand command2 = new MySqlCommand(sendcomm2, conn);
                        command2.ExecuteNonQuery();
                        conn.Close();
                    }

                    //add record to member3
                    if (member3 != "")
                    {
                        conn.Open();
                        string sendcomm3 = "INSERT INTO `hjl`.`" + member3 + "` (`日期`,`单号`,`车牌`, `型号`, `项目`,`成员`,`金额`) " + "VALUES('" + date + "', '" + ordernumber
                                            + "', '" + carname + "', '" + carmodel + "', '" + item + "', '" + members + "', '" + member3wage + "')";
                        MySqlCommand command3 = new MySqlCommand(sendcomm3, conn);
                        command3.ExecuteNonQuery();
                        conn.Close();
                    }

                    //add record to member4
                    if (member4 != "")
                    {
                        conn.Open();
                        string sendcomm4 = "INSERT INTO `hjl`.`" + member4 + "` (`日期`,`单号`,`车牌`, `型号`, `项目`,`成员`,`金额`) " + "VALUES('" + date + "', '" + ordernumber
                                            + "', '" + carname + "', '" + carmodel + "', '" + item + "', '" + members + "', '" + member4wage + "')";
                        MySqlCommand command4 = new MySqlCommand(sendcomm4, conn);
                        command4.ExecuteNonQuery();
                        conn.Close();
                    }
                    MessageBox.Show("增加成功");


                    MemberListcomboBox1.SelectedItem = "";
                    MemberListcomboBox2.SelectedItem = "";
                    MemberListcomboBox3.SelectedItem = "";
                    MemberListcomboBox4.SelectedItem = "";

                    WagetextBox1.Text = "";
                    WagetextBox2.Text = "";
                    WagetextBox3.Text = "";
                    WagetextBox4.Text = "";

                    textBox1.Text = "";
                }

            }
            
           
            changeDataGridView("维修记录");
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];

               
                dateTimePicker2.Value = Convert.ToDateTime(row.Cells["日期"].Value.ToString());
                OrderNumberTextBox2.Text = row.Cells["单号"].Value.ToString();
                CarNametextBox2.Text = row.Cells["车牌"].Value.ToString();
                carmodeltextBox1.Text = row.Cells["型号"].Value.ToString();
               
              

            }
        }


        //open wage calculation form 
        private void button3_Click(object sender, EventArgs e)
        {
            var form3 = new Form3();
            form3.StartPosition = FormStartPosition.CenterParent;
            form3.ShowDialog(this);
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {
            loadMemberList();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
            //updateOrderNumber();
        }

        private void OrderNumberTextBox2_TextChanged(object sender, EventArgs e)
        {/*
            comboBox1.SelectedItem = "单号信息";
            conn.Open();
            
            string search = OrderNumberTextBox2.Text;
            string pass;

            pass = "SELECT * FROM hjl.`维修记录` where 单号 like '%" + search + "%'";
            if (search == "")
            { //MessageBox.Show("请输入关键字"); 
            }
            else
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(pass, conn);


                DataSet ds = new DataSet();
                adapter.Fill(ds, search);

                dataGridView1.DataSource = ds.Tables[search];
                
            }
            conn.Close();
            */
        }


        //function update the ordernumber
        public void updateOrderNumber()
        {
            conn.Open();
            //get the max ordernumber
            int test = dateTimePicker1.Value.Year * 100000 + dateTimePicker1.Value.Month * 1000;
            string search = dateTimePicker1.Value.ToString("yyyyMM");
            string sendcomm = "SELECT max(`单号`) as 单号 FROM hjl.单号信息 where `单号` like '"+search+"%'";
            MySqlCommand command = new MySqlCommand(sendcomm, conn);

            MySqlDataReader reader = command.ExecuteReader();
            var max =0;
           
                while (reader.Read())
                {
                if (!reader.IsDBNull(0))
                { max = reader.GetInt32("单号"); }

                }
            
           

           
           
            if (max >= test)
            {
                OrederNumbrtTextBox1.Text = Convert.ToString(Convert.ToInt32(max) + 1);

            }
            else if(max < test)
            {

                OrederNumbrtTextBox1.Text = Convert.ToString(test);
            }


            reader.Close();
            conn.Close();

        }


        public void changeDataGridView(string loadTablename)
        {
            conn.Open();
            DataSet ds = new DataSet();
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM hjl." + loadTablename, conn);
            OnLoadTable = TableNames[TableNames.IndexOf(loadTablename)];
            
            adapter.Fill(ds, TableNames[TableNames.IndexOf(loadTablename)]);
            dataGridView1.DataSource = ds.Tables[TableNames[TableNames.IndexOf(loadTablename)]];

            conn.Close();
            comboBox1.SelectedItem = TableNames[TableNames.IndexOf(loadTablename)];
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void WagetextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        private void WagetextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        private void WagetextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        private void WagetextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            updateOrderNumber();
            dateTimePicker2.Value = dateTimePicker1.Value;
        }

        private void MemberListcomboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void OrederNumbrtTextBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void OwnerPhoneTextbox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void WagetextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void WagetextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void OrederNumbrtTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        private void OwnerPhoneTextbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        private void OrderNumberTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            conn.Open();
            string startdate = dateTimePicker3.Value.ToString("yyyy-MM-dd");
            string enddate = dateTimePicker4.Value.ToString("yyyy-MM-dd");

            if (comboBox1.SelectedItem != null)
            {   
                OnLoadTable = comboBox1.SelectedItem.ToString();
                string sendcommand = "SELECT * FROM hjl." + OnLoadTable + " where `日期` >= '" + startdate + "' and " + "`日期` <= '" + enddate + "'";
                MySqlDataAdapter adapter = new MySqlDataAdapter(sendcommand, conn);
                //conn.Open();
                DataSet ds = new DataSet();
                adapter.Fill(ds, OnLoadTable);
                dataGridView1.DataSource = ds.Tables[OnLoadTable];
            }
            conn.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            conn.Open();
            string searchtable = comboBox1.SelectedItem.ToString();


             
            string search = textBox2.Text;
            string pass;

            pass = "SELECT * FROM hjl.`"+searchtable+"` where 车牌 like '%" + search + "%'"+ "OR 型号 like '%" + search + "%'"+ "OR 单号 like '%" + search + "%'";
            if (search == "")
            { //MessageBox.Show("请输入关键字"); 
            }
            else
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(pass, conn);


                DataSet ds = new DataSet();
                adapter.Fill(ds, search);

                dataGridView1.DataSource = ds.Tables[search];
            }
            conn.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var form4 = new Form4();
            form4.StartPosition = FormStartPosition.CenterParent;
            form4.ShowDialog(this);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var form5 = new Form5();
            form5.StartPosition = FormStartPosition.CenterParent;
            form5.ShowDialog(this);
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {
            OnLoadTable = comboBox1.SelectedItem.ToString();
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM hjl." + OnLoadTable, conn);
            //conn.Open();
            DataSet ds = new DataSet();
            adapter.Fill(ds, OnLoadTable);
            dataGridView1.DataSource = ds.Tables[OnLoadTable];
        }
    }
}
