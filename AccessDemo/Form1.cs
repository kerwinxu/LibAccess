using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Xuhengxiao.DbHelper;
using MySql.Data.MySqlClient;

namespace AccessDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string datatable_name = "learnmysql";
            string table_name = "test100";
            //这个会创建一个新表
            DbHelperMySQL2 db = new DbHelperMySQL2("localhost", datatable_name, "learn", "123456");
            //要指定表的类型
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("日期", "DATE");
            dict.Add("图片", "TEXT");
            dict.Add("是否", "tinyint(1)");
            dict.Add("品名", "VARCHAR(200)");
            try
            {
                db.CreateDataTable(datatable_name, table_name, dict);

            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
                //throw;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //根据sql显示表
            userControl_access_manager1.SQL_Select = txt_select_sql.Text;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            userControl_access_manager1.DataInsert();
        }
    }
}
