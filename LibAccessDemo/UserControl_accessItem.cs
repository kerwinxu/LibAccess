using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Xuhengxiao.LibAccess;
using Xuhengxiao.DbHelper;
using MySql.Data.MySqlClient;


namespace LibAccessDemo
{
    public partial class UserControl_accessItem : AccessItem
    {
        /// <summary>
        /// 因为自动补全需要链接数据库，这里就用这个属性来保存数据库连接吧。
        /// </summary>
        public DbHelperMySQL2 DB { get; set; }

        public UserControl_accessItem()
        {
            InitializeComponent();
            INIT();
        }

        protected override void init_event()
        {
            //因为有一个日期框没有设置tag属性，这里就要加上事件
            dateTimePicker1.ValueChanged += DateTimePicker1_ValueChanged;
            txt_ID.LostFocus += Txt_ID_LostFocus;
            base.init_event();
        }

        public override void clear()
        {
            //有2个控件没有设置tag属性，所以这里要设置
            isSettingValue = true;
            txt_ID.Text = string.Empty;
            dateTimePicker1.Value = DateTime.Now;
            isSettingValue = false;

            base.clear();
            
        }
        private void Txt_ID_LostFocus(object sender, EventArgs e)
        {
            //判断是否是设置数据
            if (isSettingValue)
            {
                return;//如果是，就直接返回吧。
            }
            //定义一个事件变量
            DataUpdateEventArgs ev = new DataUpdateEventArgs();
            //是什么列改变值
            ev.Column_name = "ID";

            //关键的一点，先看看这列的列名是否在Datarow的列名中
            if (DR.Table.Columns.Contains(ev.Column_name))
            {
                
                ev.Old_DR = DataRowCopy(DR);//
                updateDR(DR);
                ev.New_DR = DR;
                OnDataUpdate(ev);

            }
            //throw new NotImplementedException();
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            //判断是否是设置数据
            if (isSettingValue)
            {
                return;//如果是，就直接返回吧。
            }
            //定义一个事件变量
            DataUpdateEventArgs ev = new DataUpdateEventArgs();
            //是什么列改变值
            ev.Column_name = "日期";

            //关键的一点，先看看这列的列名是否在Datarow的列名中
            if (DR.Table.Columns.Contains(ev.Column_name))
            {
                ev.Old_DR = DR.Table.Copy().Rows[0];//先复制到DataTable，然后选择第一列
                updateDR(DR);
                ev.New_DR = DR;
                OnDataUpdate(ev);

            }
            //throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //选择图片啦。
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.ImageLocation = openFileDialog1.FileName;
            }
        }
        public override void updateDR(DataRow dr)
        {
            //因为日期框没有写tag属性，这里要写上。
            if (dr.Table.Columns.Contains("日期"))
            {
                dr["日期"] = dateTimePicker1.Value.ToShortDateString();
            }
            //因为ID的值是数字，而文本框的text属性是字符串，所以ID也不能用默认的tag
            if (dr.Table.Columns.Contains("ID"))
            {
                dr["ID"] = ulong.Parse(txt_ID.Text);
            }

            base.updateDR(dr);
        }
        public override void setDR(DataRow dr)
        {
            isSettingValue = true;
            //因为日期框没有写tag属性，这里要写上。
            if (dr.Table.Columns.Contains("日期"))
            {
                dateTimePicker1.Value = DateTime.Parse(dr["日期"].ToString());
            }
            //因为ID的值是数字，而文本框的text属性是字符串，所以ID也不能用默认的tag
            if (dr.Table.Columns.Contains("ID"))
            {
                txt_ID.Text = dr["ID"].ToString();
            }
            base.setDR(dr);
            isSettingValue = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //简单的调用这个方法，就会删除了。
            this.OnDataDelete();
        }
        /// <summary>
        /// 用DataReader查询快。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="column_name"></param>
        /// <returns></returns>
        private   void  getItems(string sql, MySqlParameter[] par,ComboBox combo)
        {
            //清空组合框
            combo.Items.Clear();
            //执行查询
            var reader = DB.ExecuteReader(sql,par);
            //如果有数据
            if (reader.HasRows)
            {
                while (reader.Read() != false)
                {
                    combo.Items.Add(reader[0].ToString());//默认只是加第一列
                }
            }
            reader.Close();//必须加上关闭。
            
            string str_old = combo.Text;
            comboBox1.DroppedDown = true;//显示下拉框，会自动设置第一项为Text属性，所以这里要保存原先的Text属性。
            combo.Text = str_old;
            if (combo.Text.Length>0)//要判断是否有文字的，不然SelectionStart会引发异常。
            {
                combo.SelectionStart = combo.Text.Length;
            }
            Cursor = System.Windows.Forms.Cursors.Default;
        }
        /// <summary>
        /// 这个组合框自动补全
        /// </summary>
        private void comboBox1_complete()
        {
            //所以得用ExecuteReader的方式来实现
            string str_select = "SELECT distinct 品名 FROM learnmysql.test100 where 品名 like @pinming;";//查询语句
            MySqlParameter par1 = new MySqlParameter("@pinming", MySqlDbType.VarChar);//参数
            par1.Value = "%" + comboBox1.Text + "%";//参数值
            MySqlParameter[] par = { par1 };//参数数组
            getItems(str_select, par, comboBox1);

        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            comboBox1_complete();
        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            comboBox1_complete();
        }
    }
}
