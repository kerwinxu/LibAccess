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
    public partial class UserControl_access_manager : AccessManager
    {
        public UserControl_access_manager()
        {
            InitializeComponent();
        }

        #region 要同步到数据库中，需要保存如下的字段
        //如下是一堆属性字段
        DbHelperMySQL2 db = new DbHelperMySQL2("localhost", "learnmysql", "learn", "123456");
        MySqlDataAdapter adapter;
        #endregion

        #region  需要重写如下的方法
        /// <summary>
        /// 用AccessItem的子类,且设置数据库连接
        /// </summary>
        /// <returns></returns>
        public override AccessItem getAccessItem()
        {
            UserControl_accessItem item = new UserControl_accessItem();
            item.DB = db;
            return item;
        }
        /// <summary>
        /// 查询方法，
        /// </summary>
        /// <param name="sql"></param>
        public override void select_sql(string sql)
        {
            this.DT = db.ExecuteDataTable(sql, out adapter);
            string str_insert_sql = "insert into test100 (ID,日期,图片,是否,品名) values(@ID,@riqi,@tupian,@shifou,@pinming);";
            adapter.InsertCommand = new MySqlCommand(str_insert_sql, db.Connection);
            adapter.InsertCommand.Parameters.Add("@ID", MySqlDbType.Int64, 20, "ID");
            adapter.InsertCommand.Parameters.Add("@riqi", MySqlDbType.Date, 20, "日期");
            adapter.InsertCommand.Parameters.Add("@tupian", MySqlDbType.Text, 1000, "图片");
            adapter.InsertCommand.Parameters.Add("@shifou", MySqlDbType.Bit, 1, "是否");
            adapter.InsertCommand.Parameters.Add("@pinming", MySqlDbType.VarChar, 200, "品名");


            //添加UpdateCommand
            string str_update_sql = "update test100 set ID=@NewId ,日期=@riqi ,图片=@tupian ,是否=@shifou,  品名=@pinming where ID=@OldID;";
            adapter.UpdateCommand = new MySqlCommand(str_update_sql, db.Connection);
            adapter.UpdateCommand.Parameters.Add("@NewId", MySqlDbType.Int64, 20, "ID");
            adapter.UpdateCommand.Parameters.Add("@riqi", MySqlDbType.Date, 20, "日期");
            adapter.UpdateCommand.Parameters.Add("@tupian", MySqlDbType.Text, 1000, "图片");
            adapter.UpdateCommand.Parameters.Add("@shifou", MySqlDbType.Bit, 1, "是否");
            adapter.UpdateCommand.Parameters.Add("@pinming", MySqlDbType.VarChar, 200, "品名");
            //针对修改主键的情况下，可以用这个来“.SourceVersion = DataRowVersion.Original”设置。
            adapter.UpdateCommand.Parameters.Add("@OldID", MySqlDbType.Int64, 20, "ID").SourceVersion = DataRowVersion.Original ;

            //添加DeleteCommand
            string str_delete_sql = "delete from test100 where ID=@ID;";
            adapter.DeleteCommand = new MySqlCommand(str_delete_sql, db.Connection);  
            adapter.DeleteCommand.Parameters.Add("@ID", MySqlDbType.Int64, 20, "ID");


            //还要写UpdateCommand等等。
            //base.select(sql);
        }
        /// <summary>
        /// 更新方法。用DataAdapter更新。
        /// </summary>
        public override void update()
        {
            try
            {
                adapter.Update(this.DT);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //throw;
            }
            
            //base.update();
        }

        public override void default_DataRow(DataRow _dr)
        {
            _dr["ID"] = 999;//先这样投机取巧吧
            _dr["日期"] = DateTime.Now.ToShortDateString();
            base.default_DataRow(_dr);
        }
        #endregion
    }
}
