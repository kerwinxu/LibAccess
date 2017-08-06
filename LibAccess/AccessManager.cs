using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Xuhengxiao.LibAccess
{
    public partial class AccessManager : UserControl
    {
        #region 各种属性，变量等。
        private DataTable  _dt;
        /// <summary>
        /// 保存的是DataTable
        /// </summary>
        public DataTable  DT
        {
            get { return _dt; }
            set {
                _dt = value;//先保存
                //然后设置啦
                //首先清空原先的
                this.clear();
                //然后设置新的
                //得判断是否是空值
                if (_dt==null)
                {
                    return;
                }
                foreach (DataRow item in _dt.Rows)
                {
                    getFreeAccessItem().DR = item;

                }
            }
        }

        /// <summary>
        /// 保存的是 AccessItem，每个添加进去的AccessItem，都要监听事件。
        /// 用这个属性的好处是，当多次查询的时候，有多次 AccessItem 频发的构造析构，会浪费性能。
        /// 所以这里用保存的方式来多次循环利用
        /// </summary>
        List<AccessItem> LstAccessItem = new List<AccessItem>();

        private string _sql_select;
        /// <summary>
        /// 保存的是select语句。
        /// </summary>
        public string SQL_Select
        {
            get { return _sql_select; }
            set {
                _sql_select = value;
                //得判断是否是空值
                if (_sql_select==null)
                {
                    clear();//如果查询为空，就清空吧
                    return;
                }
                select_sql(_sql_select);
            }
        }


        #endregion

        #region AccessItem操作相关，包括清空LstAccessItem，取得下一个空闲的 AccessItem，以及设置AccessItem事件等。

        /// <summary>
        /// 这个方法用来清空LstAccessItem
        /// </summary>
        public void clear()
        {
            //首先判断是否是空值
            if (LstAccessItem == null)
            {
                LstAccessItem = new List<AccessItem>();
                return;
            }
            //然后迭代
            foreach (AccessItem item in LstAccessItem)
            {
                item.clear();
                item.Visible = false;
            }

            flowLayoutPanel1.Controls.Clear();//这个布局也清空。
        }

        /// <summary>
        /// 取得一个空闲的AccessItem，如果没有，就新建，且设置好这个事件监听。
        /// </summary>
        /// <returns></returns>
        protected  AccessItem getFreeAccessItem()
        {
            //首先判断LstAccessItem中是否有空闲的。
            for (int i = 0; i < LstAccessItem.Count; i++)
            {
                //如果是空闲的，就取得这个
                if (LstAccessItem[i].Visible==false)
                {
                    LstAccessItem[i].Visible = true;//设置可见
                    flowLayoutPanel1.Controls.Add(LstAccessItem[i]);//将这个AccessItem加到布局中，以便显示啊。
                    return LstAccessItem[i];
                }
            }

            //到这类就表示，得新建一个 AccessItem ，然后设置各种事件了吧。
            AccessItem AccessItem_Retrun = getAccessItem();
            AccessItem_Retrun.Visible = true;
            LstAccessItem.Add(AccessItem_Retrun);
            init_event_AccessItem(AccessItem_Retrun);

            flowLayoutPanel1.Controls.Add(AccessItem_Retrun);//将这个AccessItem加到布局中，以便显示啊。

            return AccessItem_Retrun;
        }

        /// <summary>
        /// 子类中实现这个，会返回AccessItem的子类。并做初始化之类的。
        /// </summary>
        /// <returns></returns>
        public virtual AccessItem getAccessItem()
        {
            return new AccessItem();
        }

        /// <summary>
        /// 初始化单个 AccessItem的事件
        /// </summary>
        /// <param name="item"></param>
        public virtual  void init_event_AccessItem(AccessItem item)
        {
            item.DataUpdate += Item_DataUpdate;
            item.DataDelete += Item_DataDelete;
            item.DataCommmand += Item_DataCommmand;
        }

        /// <summary>
        /// 处理命令事件的。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual  void Item_DataCommmand(object sender, AccessItem.DataCommmandEventArgs e)
        {
            //所有的命令在这里处理
            throw new NotImplementedException();
        }

        /// <summary>
        /// 数据删除部分，如果要同步到数据库，要重写这个方法。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  void Item_DataDelete(object sender, AccessItem.DataDeleteEventArgs e)
        {
            //删除这一个项目的数据
            ((AccessItem)sender).DR.Delete();//这个Delete的作用仅仅是标记这个是要删除的。
            //更新到数据库
            update();
            DT.AcceptChanges();//这一步好像多此一举，不过还是加上吧。
            //然后这个控件不可见
            ((AccessItem)sender).Visible = false;
            //然后把这个控件从布局中删除
            flowLayoutPanel1.Controls.Remove((Control)sender);

            //最后清空这个值
            ((AccessItem)sender).DR = null;

            //throw new NotImplementedException();
        }

        /// <summary>
        /// 数据更改部分，DataTable中的更改，已经在AccessItem中修改了，如果要同步到数据库，要重写这个方法。
        /// </summary>
        /// <param name="sender"></param
        /// <param name="e"></param>
        public virtual  void Item_DataUpdate(object sender, AccessItem.DataUpdateEventArgs e)
        {
            //一般是先判断是否是主键进行更新，如果是，得自己提交修改
            //如果不是，那么就可以用DataAdapter的UpdateCommand进行修改。

            //throw new NotImplementedException();
            //
            update();
        }


        #endregion

        #region 构造函数，初始化部分。

        public AccessManager()
        {
            InitializeComponent();
        }

        #endregion


        #region 修改、插入部分

        /// <summary>
        /// 这个操作DataAdapter与数据库进行同步，这个得用异常，因为存在添加或者更新数据的时候，通不过。
        /// </summary>
        public virtual void update()
        {
            
        }


        /// <summary>
        /// 插入的话，应该不需要参数吧。
        /// </summary>
        public  void DataInsert()
        {
            DataRow dr = DT.NewRow();//新建一行
            default_DataRow(dr);//设置默认的
            DT.Rows.Add(dr);//然后将这行加到DataTable中。
            AccessItem item = getFreeAccessItem();//取得一个空的AccessItem
            item.DR = dr;//设置这些数据

            //并且设置取得焦点。
            item.getFocus();

            update();
        }

        /// <summary>
        /// 查询语句，要子类重写的。
        /// </summary>
        /// <param name="sql"></param>
        public virtual void select_sql(string sql)
        {

        }



        /// <summary>
        /// 设置默认的DataRow
        /// </summary>
        /// <param name="_dr"></param>
        public virtual void default_DataRow(DataRow _dr)
        {

        }


        #endregion
    }
}
