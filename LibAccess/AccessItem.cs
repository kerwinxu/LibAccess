using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Xuhengxiao.LibAccess
{


    public partial class AccessItem : UserControl
    {
        #region 这个类的属性，变量之类的。
        /// <summary>
        /// 这个控件所有的子控件，包括TextBox、ComboBox、ListBox、CheckBox、PictureBox、LinkLabel
        /// </summary>
        protected List<Control> controlColl = new List<Control>();

        private DataRow _DR;
        /// <summary>
        /// 保存的是这列的数据。
        /// </summary>
        public DataRow DR
        {
            get { return _DR; }
            set
            {
                _DR = value;//保存
                //如果数据为空，
                if (value==null)
                {
                    clear();//就清空
                }
                else
                {
                    setDR(_DR);//设置数据
                }
                
            }
        }
        /// <summary>
        /// 是否在设置数据，有时候设置数据的时候，并不希望引发事件
        /// 在事件中，这个要自己判断的。
        /// </summary>
        public bool isSettingValue;

        #endregion

        #region 构造函数，初始化部分
        public AccessItem()
        {
            InitializeComponent();

        }
        /// <summary>         
        /// 我自己的初始化函数，主要是搜索控件，添加事件等操作。请注意在子类中必须手动调用。
        /// 因为AccessItem的子类中，会优先调用父类的构造函数，但是意味着，在父类的构造函数中运行的话，相关的控件还没初始化，结果就是在父类运行，不会获得子类的控件的。
        /// </summary>
        public virtual void INIT()
        {
            //取得所有的子控件，包括文本框、列表框之类的。
            init_controlColl(this);
            //注册子控件的消息，包括文本框更改事件之类的。
            init_event();
        }

        /// <summary>
        /// 复制DataRow
        /// </summary>
        /// <param name="_dr_old"></param>
        /// <returns></returns>
        public DataRow DataRowCopy(DataRow _dr_old)
        {
            //首先建立一个
            DataTable dt = new DataTable();
            //然后复制列
            foreach (DataColumn item in _dr_old.Table.Columns)
            {
                dt.Columns.Add(item.ColumnName);
            }
            //创建一行
            DataRow dr_return = dt.NewRow();
            //然后复制所有的信息。
            foreach (DataColumn item in _dr_old.Table.Columns)
            {
                dr_return[item.ColumnName] = _dr_old[item.ColumnName];
            }
            return dr_return;
        }

        /// <summary>
        /// 取得子控件，这个只取得设置tag属性的子控件，保存到controlColl；
        /// </summary>
        /// <param name="_control"></param>
        public virtual void init_controlColl(Control _control)
        {
            //搜索控件包含3个地方，字段、属性和Controls

            #region 搜索属性
            //首先取得所有属性，然后判断是否是控件，如果是控件，就添加到
            Type t = _control.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            foreach (PropertyInfo item in PropertyList)
            {
                string s = item.Name;
                object value = item.GetValue(_control, null);
                //判断这个value是否是控件
                if (value is Control)
                {
                    //判断是否已经包含这个子控件以及是否有tag属性
                    if (!controlColl.Contains((Control)value) &&
                        ((Control)value).Tag != null)
                    {
                        //没包含则添加
                        controlColl.Add((Control)value);
                        //然后迭代这个
                        init_controlColl((Control)value);
                    }
                }
            }
            #endregion

            #region 搜索字段
            //获得字段
            var v = _control.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo fi in v)
            {
                string s = fi.Name;
                object value = fi.GetValue(_control);
                //判断这个value是否是控件
                if (value is Control)
                {
                    //判断是否已经包含这个子控件
                    if (!controlColl.Contains((Control)value) &&
                        ((Control)value).Tag != null)
                    {
                        //没包含则添加
                        controlColl.Add((Control)value);
                        //然后迭代这个
                        init_controlColl((Control)value);
                    }
                }
            }
            #endregion

            #region 搜索Controls属性，这个有子控件集合
            //判断Control这个控件是否有子控件
            if (_control.Controls.Count > 0)
            {
                //如果有子控件，就遍历
                foreach (Control item in _control.Controls)
                {
                    //判断是否已经包含这个子控件
                    if (!controlColl.Contains(item) &&
                        ((Control)item).Tag != null)
                    {
                        //没包含则添加
                        controlColl.Add(item);
                        //然后迭代这个
                        init_controlColl(item);
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 各种事件的初始化
        /// </summary>
        protected virtual void init_event()
        {
            if (controlColl.Count > 0)
            {
                foreach (Control item in controlColl)
                {
                    //不设置tag属性的控件，就不做事件监听。
                    if (item.Tag==null)
                    {
                        continue;
                    }

                    if (item is TextBox)//如果是文本框
                    {
                        ((TextBox)item).LostFocus += AccessItem_LostFocus; ;
                    }
                    else if (item is ComboBox)//如果是组合框
                    {
                        ((ComboBox)item).LostFocus += AccessItem_LostFocus; ;
                    }
                    else if (item is ListBox)//如果是列表框
                    {
                        ((ListBox)item).LostFocus += AccessItem_LostFocus; ;
                    }
                    else if (item is CheckBox)//单选框，暂时不用复选框吧。
                    {
                        ((CheckBox)item).LostFocus += AccessItem_LostFocus; ;
                    }
                    else if (item is PictureBox)//图像框
                    {
                        ((PictureBox)item).LocationChanged += AccessItem_LostFocus; ;

                    }
                    else if (item is LinkLabel)//link
                    {
                        ((LinkLabel)item).TextChanged += AccessItem_LostFocus;
                    }
                    else if (item is DateTimePicker)//日期框
                    {
                        ((DateTimePicker)item).ValueChanged += AccessItem_LostFocus; //value属性改变
                    }

                }
            }

        }

        /// <summary>
        /// 这个数据取得焦点，一般用在插入数据中。
        /// </summary>
        public virtual void getFocus()
        {

            //如果没有子控件，就只是这个取得焦点吧
            if (controlColl==null ||controlColl.Count==0)
            {
                this.Focus();
            }else
            {
                //如果有控件，先设置所有的控件取得焦点，然后第一个控件取得焦点
                isSettingValue = true;
                foreach (Control item in controlColl)
                {
                    item.Focus();
                }
                controlColl[0].Focus();
                isSettingValue = false;
            }
        }
        /// <summary>
        /// 焦点离开控件时发生。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AccessItem_LostFocus(object sender, EventArgs e)
        {
            //判断是否是设置数据
            if (isSettingValue)
            {
                return;//如果是，就直接返回吧。
            }

            if (((Control)sender).Tag == null)//如果没设置tag属性，就不会
            {
                return;//触发这个事件，得自己重写
            }

            //定义一个事件变量
            DataUpdateEventArgs ev = new DataUpdateEventArgs();
            //是什么列改变值
            ev.Column_name = ((Control)sender).Tag.ToString();

            //关键的一点，先看看这列的列名是否在Datarow的列名中
            if (DR.Table.Columns.Contains(ev.Column_name))
            {
                ev.Old_DR = DR.Table.Copy().Rows[0];//先复制到DataTable，然后选择第一列
                //然后更新
                updateDR(DR);
                ev.New_DR = DR;
                OnDataUpdate(ev);

            }

            //throw new NotImplementedException();

        }


        #endregion

        #region 各种事件

        #region 数据更改事件部分，一般是，比如在焦点离开文本框的时候发生。

        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DataUpdateHandler(Object sender, DataUpdateEventArgs e);
        /// <summary>
        /// 事件
        /// </summary>
        public event DataUpdateHandler DataUpdate;


        /// <summary>
        /// 调用事件
        /// </summary>
        /// <param name="e"></param>
        protected void OnDataUpdate(DataUpdateEventArgs e)
        {
            //如果有监听者
            if (DataUpdate != null)
            {
                DataUpdate(this, e);

            }
        }

        /// <summary>
        /// 数据更改的事件
        /// </summary>
        public class DataUpdateEventArgs : EventArgs
        {
            /// <summary>
            /// 列名，一般是保存在Control.tag属性中
            /// </summary>
            public string Column_name { get; set; }

            /// <summary>
            /// 保存这列的数据，因为c#中对象赋值的本质是引用，所以实际指向的是原先DataTable中的数据。
            /// </summary>
            public DataRow New_DR { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public DataRow Old_DR { get; set; }
        }

        #endregion

        #region 数据删除部分,一般是单独的一个删除按钮。
        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DataDeleteHandler(Object sender, DataDeleteEventArgs e);
        /// <summary>
        /// 事件
        /// </summary>
        public event DataDeleteHandler DataDelete;

        /// <summary>
        /// 删除这个的事件
        /// </summary>
        public class DataDeleteEventArgs : EventArgs
        {
            /// <summary>
            /// 保存这列的数据，因为c#中对象赋值的本质是引用，所以实际指向的是原先DataTable中的数据。
            /// </summary>
            public DataRow DR { get; set; }
        }
        /// <summary>
        /// 调用事件
        /// </summary>
        /// <param name="e"></param>
        protected void OnDataDelete(DataDeleteEventArgs e)
        {
            //如果有监听者
            if (DataDelete != null)
            {
                DataDelete(this, e);
            }

        }

        /// <summary>
        /// 方便的调用删除事件
        /// </summary>
        public void OnDataDelete()
        {
            DataDeleteEventArgs a = new DataDeleteEventArgs();
            a.DR = DR;
            OnDataDelete(a);
        }

        #endregion

        #region 命令事件。
        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DataCommandHandler(Object sender, DataCommmandEventArgs e);
        /// <summary>
        /// 事件
        /// </summary>
        public event DataCommandHandler DataCommmand;


        /// <summary>
        /// 调用事件
        /// </summary>
        /// <param name="e"></param>
        protected void OnDataCommand(DataCommmandEventArgs e)
        {
            //如果有监听者
            if (DataCommmand != null)
            {
                DataCommmand(this, e);

            }
        }

        /// <summary>
        /// 发出命令事件
        /// </summary>
        public class DataCommmandEventArgs : EventArgs
        {

        }


        #endregion

        #endregion


        #region 设置 清空数据部分,特别注意，只是清空有tag属性的
        /// <summary>
        /// 所有的数据清空的
        /// </summary>
        public virtual void clear()
        {
            isSettingValue = true;

            //将这个控件所有的信息全部清零
            foreach (Control item in controlColl)
            {
                if (item.Tag==null)
                {
                    continue;
                }

                if (item is TextBox)//如果是文本框
                {
                    ((TextBox)item).Text = string.Empty;//清空文本
                }
                else if (item is ComboBox)//如果是组合框
                {
                    ((ComboBox)item).Text = string.Empty;//清空文本
                    ((ComboBox)item).Items.Clear();//清空列表
                }
                else if (item is ListBox)//如果是列表框
                {
                    ((ListBox)item).Text = string.Empty;//清空文本
                    ((ListBox)item).Items.Clear();//清空列表
                }
                else if (item is CheckBox)//单选框
                {
                    ((CheckBox)item).Checked = false;//默认不选中

                }
                else if (item is LinkLabel)//链接
                {
                    ((LinkLabel)item).Text = string.Empty;//清空文本

                }
                else if (item is PictureBox)//图像框
                {
                    ((PictureBox)item).Image = null;//没有图像显示
                    ((PictureBox)item).ImageLocation = string.Empty;//设置图像路径为空
                }
                else if (item is DateTimePicker)//日期框
                {
                    //设置日期为当天的日期
                    ((DateTimePicker)item).Value = DateTime.Now;
                }

            }
            isSettingValue = false;
        }
        /// <summary>
        /// 根据数据来设置DataRow
        /// </summary>
        /// <param name="dr"></param>
        public virtual void updateDR(DataRow dr)
        {
            //只是设置所有子控件的值而已。
            foreach (Control item in controlColl)
            {
                string str_columns_name = (string)item.Tag;

                if (str_columns_name == null)
                {
                    continue;
                }
                //首先判断是否有这个值
                if (dr.Table.Columns.Contains(str_columns_name))
                {
                    if (item is TextBox)//如果是文本框
                    {
                        dr[str_columns_name] = item.Text;
                    }
                    else if (item is ComboBox)//如果是组合框
                    {
                        dr[str_columns_name] = item.Text;
                    }
                    else if (item is ListBox)//如果是列表框
                    {
                        dr[str_columns_name] = item.Text;
                    }
                    else if (item is LinkLabel)//如果是链接标签
                    {
                        dr[str_columns_name] = item.Text;
                    }
                    else if (item is CheckBox)//如果是单选框
                    {
                        if (((CheckBox)item).Checked)
                        {
                            dr[str_columns_name] = 1;//按照mysql而言，真就是1
                        }
                        else
                        {
                            dr[str_columns_name] = 0;//假就是0
                        }
                    }
                    else if (item is PictureBox)//图像框
                    {
                        dr[str_columns_name] = ((PictureBox)item).ImageLocation;
                    }
                    else if (item is DateTimePicker)
                    {
                        dr[str_columns_name] = ((DateTimePicker)item).Value;
                    }
                }
            }
        }

        /// <summary>
        /// 根据DataRow来设置所有的数据
        /// </summary>
        /// <param name="dr"></param>
        public virtual void setDR(DataRow dr)
        {
            isSettingValue = true;
            //只是设置所有子控件的值而已。
            foreach (Control item in controlColl)
            {
                if (item == null)
                {
                    continue;
                }
                string str_columns_name = (string)item.Tag;

                //首先判断是否有这个值
                if (str_columns_name != null &&
                    dr.Table.Columns.Contains(str_columns_name))
                {
                    if (item is TextBox)//如果是文本框
                    {
                        item.Text = dr[str_columns_name].ToString();
                    }
                    else if (item is ComboBox)//如果是组合框
                    {
                        item.Text = dr[str_columns_name].ToString();
                    }
                    else if (item is ListBox)//如果是列表框
                    {
                        item.Text = dr[str_columns_name].ToString();
                    }
                    else if (item is LinkLabel)//如果是链接标签
                    {
                        item.Text = dr[str_columns_name].ToString();
                    }
                    else if (item is CheckBox)//如果是单选框
                    {
                        if (dr[str_columns_name].ToString() == "1")
                        {
                            ((CheckBox)item).Checked = true;
                        }
                        else
                        {
                            ((CheckBox)item).Checked = false;
                        }
                    }
                    else if (item is PictureBox)//图像框
                    {
                        //默认只是保存路径的。
                        try
                        {
                            ((PictureBox)item).ImageLocation = dr[str_columns_name].ToString();
                        }
                        catch (Exception e)
                        {
                            isSettingValue = false;
                            //throw e;
                        }
                    }
                    else if (item is DateTimePicker)
                    {
                        ((DateTimePicker)item).Value = DateTime.Parse(dr[str_columns_name].ToString());
                    }
                }



            }

            isSettingValue = false;

        }

        #endregion

    }

}
