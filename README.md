Copied from https://archive.codeplex.com/?p=avaloncontrolslib

# AvalonControlsLibrary
AvalonControlsLibrary is a set of WPF controls that can help WPF developers while developing applications. The library includes a WPF Date picker, Time Picker and loads of other stuff...

What controls can I find in this Library?

### DateTimePicker
DateTimePicker is DatePicker and TimePicker combined together in one control. For more info visit http://marlongrech.wordpress.com/2008/02/06/wpf-date-time-picker/

### Docky
Docky is a control that lets you Dock panels just like a normal dock panel. The difference with Docky is that you can undock and drag around the controls that you docke. [For more info](http://marlongrech.wordpress.com/2008/01/29/create-blend-like-uis-using-docky/)

### RatingSelector
RatingSelector is a very simple control that lets the user select a rating. The rating that the user can select can be within a specified range. This control also lets you change the way it looks by supporting control templates. For more info have a look at this [blog post](http://marlongrech.wordpress.com/2007/12/17/rating-selector/)

### MaskedTextBox
This control allows you to set a mask for a textbox to validate the user entry. For more info visit the [blog post](http://marlongrech.wordpress.com/2007/10/28/masked-textbox/).

### DataGridView
DataGridView is a maybe a misleading name for this control. This control is far from being the same as the WinForms DataGridView (maybe someday it will J). Basically this control is a WPF ListView control but it is capable of auto generate the GridViewColumns for you. It generates the columns by looking at the objects’ properties. You can also specify how you generate the columns by decorating your properties with a custom attribute. For more information have a look at [this post](http://marlongrech.wordpress.com/2007/09/01/listview-with-auto-generation-of-column-enable-disable-columns/).

### TimePicker
Time Picker is a very basic control that lets the user selects a specific time. The user can enter the time in 3 textboxes supplied. he can also increase/decrease the time by using the 2 buttons supplied in the control. The increase/decrease time commands are also linked to the Key up / key down so that the user can use the key up and key down to increase/decrease the time.
http://marlongrech.wordpress.com/2007/11/18/time-picker/

### DatePicker
I guess that this is the most popular control (at least that’s what my BlogStats say!). As the name suggests, DatePicker is used to pick a specific date. This control supports ControlTemplates to change the way it looks. It was a very nice control to develop, in fact this was my first ‘lookless’ control J. You can find a blog post for this control [here](http://marlongrech.wordpress.com/2007/09/11/wpf-datepicker/).

### RangeSlider

The RangeSlider is a control that lets the user select a range. Again this control supports ControlTemplates to change how it looks. You can find a blog post for this control [here](http://marlongrech.wordpress.com/2007/09/06/wpf-range-slider-control/).

### TreeListBox
This is my favourite control. This is basically a WPF ListBox yet it looks and behaves like a WPF TreeView. The question is why do you want a ListBox to look like a TreeView? The answer is very simple ‘Virtualization’. The WPF TreeView does not use the VirtualizingStackPanel (not even if you explicitly specify this in the ItemsPanel property) because VistualizingStackPanel does not understand hierarchies yet luckily the ListBox does. So basically this control takes care of placing the children of the root nodes in the proper index and indent the children by the level in the hierarchy. You can also use this control to support multi select, since the TreeView does not support this feature. When you have a lot of items for the TreeView I would suggest using this control since this will for sure benefit your application in terms of memory and also performance at start-up, since it only populates the items that can fit on screen. For more details on this control visit my [blog post here](http://marlongrech.wordpress.com/2007/10/01/virtualizing-treeview-aka-treelistbox-v20/).

### Magnifier
This is a very unusual control. Basically it is a slider that notifies listeners if the thumb is being dragged to one of the sides. It raises an event at specified intervals. I personal used this control to create a zoom in/out effect. I think that you can fairly say that the Magnifier is the brother of RangeSlider. Why? Imagine you have a specific date range and you want to decrease or increase the range. You can do this by using the RangeSlider but the RangeSlider has one limitation the user can increase the range from one side at a time. Yet by using the Magnifier increase/decrease the range selected from both ends (from and to at the same time). You can have a look at [this blog post](http://marlongrech.wordpress.com/2007/09/06/wpf-range-slider-control/) for more information.
