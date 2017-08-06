using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Xuhengxiao.LibAccess;
using Xuhengxiao.DbHelper;
using MySql.Data.MySqlClient;

namespace Xuhengxiao.LibAccess.LibAccessMysql
{
    public partial class AccessItemMySQL : AccessItem
    {
        public AccessItemMySQL()
        {
            InitializeComponent();
        }

        #region 增加的属性
        /// <summary>
        /// 因为自动补全需要链接数据库，这里就用这个属性来保存数据库连接吧。
        /// </summary>
        public DbHelperMySQL2 DB { get; set; }
        #endregion

        #region 增加的方法

        #region 设置组合框的

        /// <summary>
        /// 这个方法用指定的查询字符串来获得相关数据来填充组合框
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="sql"></param>
        /// <param name="par"></param>
        public void setComboBoxItem(ComboBox combo,string sql, MySqlParameter[] par )
        {
            //清空组合框
            combo.Items.Clear();
            //执行查询
            var reader = DB.ExecuteReader(sql, par);
            //如果有数据
            if (reader.HasRows)
            {
                while (reader.Read() != false)
                {
                    combo.Items.Add(reader[0].ToString());//默认只是加第一列
                }
            }

            reader.Close();//DataReader必须加上关闭。否则会异常。

            string str_old = combo.Text;//保存原先的文本
            int start_old = combo.SelectionStart;//保存原先的输入位置
            combo.DroppedDown = true;//显示下拉框，会自动设置第一项为Text属性，所以这里要保存原先的Text属性。
            combo.Text = str_old;//设置成原先的文本
            if (combo.Text.Length > 0)//要判断是否有文字的，不然SelectionStart会引发异常。
            {
                combo.SelectionStart = start_old;//设置成原先的文本位置
            }
            Cursor = System.Windows.Forms.Cursors.Default;
        }

        #endregion

        #region 设置列表框的
        /// <summary>
        /// 设置列表框的，只是设置数据，并不自动打开。
        /// </summary>
        /// <param name="lstBox"></param>
        /// <param name="sql"></param>
        /// <param name="par"></param>
        public void setListBoxItem(ListBox lstBox, string sql, MySqlParameter[] par)
        {
            //清空列表框
            lstBox.Items.Clear();
            //执行查询
            var reader = DB.ExecuteReader(sql, par);
            //如果有数据
            if (reader.HasRows)
            {
                while (reader.Read() != false)
                {
                    lstBox.Items.Add(reader[0].ToString());//默认只是加第一列
                }
            }

            reader.Close();//DataReader必须加上关闭。否则会异常。

        }

            #endregion

            #endregion
        }
}
