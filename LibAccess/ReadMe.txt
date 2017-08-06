这个库是做类似access的，做到如下功能
	1、 显示数据：完成
	2、 修改数据，包括主键也能修改：完成
	3、 插入数据，且要取得焦点，以便输入：完成
	4、 删除数据。完成
	5、 自动补全，比如组合框，输入后，下拉列表会显示匹配的。这个在AccessItem子类中自己实现吧。完成
	6、 发出事件消息，比如点击按钮，就打开另一个查询。完成。



这个分2个部分：
	AccessManager ; 保存的是数据库相关信息，将DataTable的内容，然后对于AccessItem的更改做出相应。
		属性：
			DT ： DataTable类型，保存的是一个表。
			LstAccessItem ：类似一个池子。
			SQL_Select ：sql语句，设置sql语句的时候，会更新最新的DataTable，并且设置相关的AccessItem。

		方法：
			clear ： 清空数据操作，无非是调用AccessItem的数据清空操作，另外清空布局。
			getFreeAccessItem ：返回一个没有在使用的AccessItem，
			getAccessItem ：返回AccessItem的子类，毕竟AccessItem的布局只是空白的，
			init_event_AccessItem ： 设置监听AccessItem的事件，
				DataUpdate ： 数据更改事件
				DataDelete ： 数据删除事件
				           ： 数据查询事件，有些组合框会查询，用这个来调用。
			Item_DataDelete ：删除某项，无非是这个控件数据清空，不可见，从布局中删除，然后更新数据库。
			Item_DataUpdate ：更新某项，简单的只是直接调用更新数据库就可以了。
			update ：更新到数据库，实际操作在子类中实现。
			select_sql ：查询sql，子类中要实现怎么查询，且设置DataAdapter已经相关的InsertCommand、UpdateCommand、DeleteCommand
			select_list ：查询sql，返回list，用在类似组合框的智能输入中。
			DataInsert ：插入数据
			default_DataRow ：设置一个默认的数据。
			Item_DataUpdate ： 数据更新监听者。通常要判断更改的是否是主键，如果是主键，得自己写更新sql，其他的，一般UpdateCommand会处理。

		用flowLayoutPanel布局来管理，也可以用其他布局。
	AccessItem : 单个项目显示一行数据的。
		属性：
			DR ： DataRow 类型，
			isSettingValue ： 是否是程序在设置数据，因为我根据DataRow来设置控件的值，或者清空控件的值，都会改变控件的值，会引发ValueChanged之类的事件，而有些控件的ValueChanged事件会更改数据库，会造成异常。所以这里用这个来设置。
		方法：
			INIT ：我自己写的初始化函数，得在子类中显示调用，里边有如下2个方法，
			init_controlColl ： 取得所有的设置tag属性的控件
			init_event ：设置所有的tag属性的控件的事件，一般是，当某个控件失去焦点，就发出同步数据库事件。其他没有设置tag属性的事件，得自己编写事件处理方法，且一定要注意判断isSettingValue的值。
				LostFocus ：失去焦点的事件，这里设置TextBox、ComboBox、ListBox、CheckBox控件监听这个事件。
				LocationChanged ： 这里设置PictureBox控件监听这个事件
				TextChanged ： 这里设置LinkLabel控件来监听这个事件，因为LinkLabel不可像TextBox那样编辑，只能有别的方法来设置。
				ValueChanged ： 这里设置DateTimePicker 控件监听这个事件，其实这个也是可以用LostFocus来的。
			各种事件方法：
				数据更新事件，我的层次是，当控件的值被用户更改的时候，首先在控件的相关事件中将值同步到DataRow中，然后调用OnDataUpdate发出更改事件，一般由AccessManager监听到，然后AccessManager会同步到数据库中。
					DataUpdateHandler ： 事件委托
					DataUpdate ：事件变量，一般由AccessManager的子类进行监听。
					OnDataUpdate ：发出事件信息，都有控件的值被更改的时候，调用这个，发出通知，然后由调用者做出相应的更改。
				数据删除事件，数据删除事件，最方便的就是做一个按钮。
					DataDeleteHandler ：事件委托
					DataDelete ：事件变量，
					OnDataDelete ： 发出事件信息，一般由AccessManager的子类进行监听，删除的实际操作是有AccessManager来处理的。
			clear ： 数据清空方法，
			updateDR ：根据控件的值来设置DataRow的值。
			setDR    ：根据DaraRow的值来设置控件的值。

AccessItem的子类，我将AccessItem设置成自动处理设置tag属性的控件，其他控件得重写如下方法。
	INIT ：可以不写。无非是调用如下2个方法。
	init_controlColl ： 取得所有的控件。
	init_event ： 设置所有控件的事件。
	clear ： 数据清空方法，
	updateDR ：根据控件的值来设置DataRow的值。
	setDR    ：根据DaraRow的值来设置控件的值。
AccessManager的子类
	getAccessItem ：必须实现，这里会返回具体需要AccessItem的哪个子类。
	select_sql ：查询sql，子类中要实现怎么查询，且设置DataAdapter已经相关的InsertCommand、UpdateCommand、DeleteCommand
	default_DataRow ：设置一个默认的数据。
	update ：怎么将DataTable中的数据同步到数据库。