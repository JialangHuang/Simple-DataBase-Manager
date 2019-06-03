using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace huangjialang
{
    public partial class Form2 : Form
    {
        string server = "localhost";
        string database = "hjl";
        string uid = "root";
        string password = "123456";

        List<string> MemberList;//员工名单

        //DataSet ds;
        string connectionString;//= "datasource=" + server + ";"+"PORT=3306;"+ "DATABASE=" + database + ";" + "username=" + uid + ";" + "PASSWORD=" + password + ";";
        MySqlConnection conn;
        

        public Form2()
        {
            InitializeComponent();
            connectionString = "datasource=" + server + ";" + "PORT=3306;" + "DATABASE=" + database + ";" + "username=" + uid + ";" + "PASSWORD=" + password + ";";
            conn = new MySqlConnection(connectionString);
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            conn.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
            //conn.Open();

            loadMemberList();
            loadMemberTable();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            GetMemberList();
            string name = textBox1.Text.Replace(" ", "");
            string phone = textBox2.Text.Replace(" ", "");
            //INSERT INTO `hjl`.`员工名单` (`员工姓名`, `电话`) VALUES('黄轩朗', '123');
            if (MemberList.Contains(name))
            {
                MessageBox.Show("员工已登记");

            }
            else if (phone.Length != 11 || !phone.All(char.IsDigit))
            {
                MessageBox.Show("电话号码格式不正确");

            }
            else if (name =="")
            {
                MessageBox.Show("请输入姓名");

            }
            else if (phone == "")
            {
                MessageBox.Show("请输入电话");

            }
            else
            {
                conn.Open();
                string query = "INSERT INTO `hjl`.`员工名单` (`员工姓名`, `电话`) " + "VALUES('" + name + "', '" + phone + "')";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("登记成功");
                textBox1.Text = "";
                textBox2.Text = "";
                //CREATE TABLE `hjl`.`黄嘉朗` (`日期` DATE NULL,`编号` INT(255) NULL,`车牌` VARCHAR(255) NULL,`项目` VARCHAR(255) NULL,`成员` VARCHAR(255) NULL,`金额` INT(255) NULL);

               

                //new wage table for new member
                conn.Open();
                string com = "CREATE TABLE `hjl`.`" + name + "` (`ID` INT NOT NULL AUTO_INCREMENT,`日期` DATE NULL,`单号` VARCHAR(255) NULL,`车牌` VARCHAR(255) NULL,`型号` VARCHAR(255) NULL,`项目` VARCHAR(255) NULL,`成员` VARCHAR(255) NULL,`金额` INT(255) NULL,PRIMARY KEY (`ID`))";
                 MySqlCommand commandsecond = new MySqlCommand(com, conn);
                commandsecond.ExecuteNonQuery();
                conn.Close();
                GetMemberList();
                loadMemberTable();
                loadMemberList();

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
            MemberListcomboBox1.Items.Clear();
            MemberListcomboBox1.Items.Add("");
            conn.Open();
            string query = "SELECT `员工姓名` FROM hjl.`员工名单` where `是否离职` = 0";
            MySqlCommand command = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    MemberListcomboBox1.Items.Add(reader.GetString(0));

                }
            }

            MemberListcomboBox1.SelectedItem = "" ;
            conn.Close();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            //UPDATE `hjl`.`员工名单` SET `电话` = '13923212200' WHERE(`id` = '33');
            GetMemberList();
            string name = MemberListcomboBox1.SelectedItem.ToString();
            string phone = textBox4.Text.Replace(" ", "");

            if (name == "")
            {
                MessageBox.Show("请选择员工");
            }
            else if (phone.Length != 11 || !phone.All(char.IsDigit))
            {
                MessageBox.Show("电话号码不正确");

            }
            else

            {
                conn.Open();
                string setphone = "`电话` = '" + phone + "'";
                string query = "UPDATE `hjl`.`员工名单` SET " + setphone + " WHERE `员工姓名` = '" + name +"'";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("修改成功");
                loadMemberTable();

            }
        }

        private void deletebutton1_Click(object sender, EventArgs e)
        {
            //DELETE FROM `hjl`.`员工名单` WHERE(`id` = '34');
            string name = MemberListcomboBox1.SelectedItem.ToString();
            if (name == "")
            {  MessageBox.Show("请选择员工"); }
            else
            {
                conn.Open();
                string query = "UPDATE `hjl`.`员工名单` SET `是否离职` = '1' WHERE `员工姓名` = '" + name + "'";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();

                conn.Open();
                string query2 = "RENAME TABLE `"+name+"` TO `"+name+"离职`";
                MySqlCommand command2 = new MySqlCommand(query2, conn);
                command2.ExecuteNonQuery();
                conn.Close();


                MessageBox.Show("删除成功");
                loadMemberList();
                MemberListcomboBox1.SelectedItem = "";
                loadMemberTable();
                textBox4.Text = "";
            }
        }


        public void loadMemberTable()
        {

            DataSet ds = new DataSet();
            conn.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT `员工姓名`, `电话` FROM hjl." + "员工名单 where `是否离职` = '0'", conn);
            adapter.Fill(ds, "员工名单");
            dataGridView1.DataSource = ds.Tables[0];
            conn.Close();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string name = MemberListcomboBox1.SelectedItem.ToString();
            string phone = textBox4.Text.Replace(" ", "");
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];

                MemberListcomboBox1.SelectedItem = row.Cells["员工姓名"].Value.ToString();
               textBox4.Text = row.Cells["电话"].Value.ToString();


            }
            //Thread.Sleep(200);
        }

        private void MemberListcomboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
