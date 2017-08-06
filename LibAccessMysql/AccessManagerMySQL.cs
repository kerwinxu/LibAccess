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
    public partial class AccessManagerMySQL : AccessManager
    {
        public AccessManagerMySQL()
        {
            InitializeComponent();
        }

        #region 如下是增加的属性或者字段
        /// <summary>
        /// 保存数据库连接
        /// </summary>
        public DbHelperMySQL2 DB { get; set; }

        /// <summary>
        /// 保存这个主要是为了同步到数据库
        /// </summary>
        protected  MySqlDataAdapter Adapter;//这个不能当属性，只能当字段，因为不用用out参数。

        #region  需要重写如下的方法

        /// <summary>
        /// 获得单个item
        /// </summary>
        /// <returns></returns>
        public override AccessItem getAccessItem()
        {
            var item = new AccessItemMySQL();
            item.DB = DB;//设置成一个连接，你可以重写这个方法。
            return item;
            //return base.getAccessItem();
        }

        /// <summary>
        /// 更新方法。用DataAdapter更新。
        /// </summary>
        public override void update()
        {
            try
            {
                Adapter.Update(this.DT);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //throw;
            }

            //base.update();
        }

        /// <summary>
        /// 默认的查询方法，如果有主键，这个会自动生成InsertCommand,UpdateCommand,DeleteCommand，你可以在子类中重写这个方法。
        /// </summary>
        /// <param name="sql"></param>
        /// 
        public override void select_sql(string sql)
        {
            this.DT = DB.ExecuteDataTable(sql, out Adapter);
            MySqlCommandBuilder cb = new MySqlCommandBuilder(Adapter);//增加了这个，就会自动生成UpdateCommand,InsertCommand,DeleteCommand，前提是表有主键。
            //base.select_sql(sql);
        }
        #endregion
        #endregion
    }
}
