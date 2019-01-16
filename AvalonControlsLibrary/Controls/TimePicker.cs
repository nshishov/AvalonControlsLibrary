using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using AC.AvalonControlsLibrary.Core;
using System.Collections.Generic;

namespace AC.AvalonControlsLibrary.Controls {
  /// <summary>
  /// Time Picker as a control that lets the user select a specific time
  /// </summary>
  [TemplatePart(Name = "PART_Hours", Type = typeof(TextBox)),
  TemplatePart(Name = "PART_Minutes", Type = typeof(TextBox)),
  TemplatePart(Name = "PART_Seconds", Type = typeof(TextBox)),
  TemplatePart(Name = "PART_IncreaseTime", Type = typeof(ButtonBase)),
  TemplatePart(Name = "PART_DecrementTime", Type = typeof(ButtonBase))]
  public class TimePicker : Control {

    protected List<TextBox> textBoxes = new List<TextBox>();

    //data memebers to store the textboxes for hours, minutes and seconds
    protected internal TextBox hours, minutes, seconds;

    //the textbox that is selected
    protected TextBox currentlySelectedTextBox;

    #region Properties

    /// <summary>
    /// Gets or sets the minimum time that can be selected
    /// </summary>
    public TimeSpan MinTime {
      get { return (TimeSpan)GetValue(MinTimeProperty); }
      set { SetValue(MinTimeProperty, value); }
    }

    /// <summary>
    /// Gets or sets the minimum time selected
    /// </summary>
    public static readonly DependencyProperty MinTimeProperty =
        DependencyProperty.Register("MinTime", typeof(TimeSpan), typeof(TimePicker), new UIPropertyMetadata(TimeSpan.Zero,
            delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
              TimePicker picker = (TimePicker)sender;
              picker.CoerceValue(SelectedTimeProperty);//make sure to update the time if appropiate
            }));

    /// <summary>
    /// Gets or sets the maximum time that can be selected
    /// </summary>
    public TimeSpan MaxTime {
      get { return (TimeSpan)GetValue(MaxTimeProperty); }
      set { SetValue(MaxTimeProperty, value); }
    }

    /// <summary>
    /// Gets or sets the maximum time that can be selected
    /// </summary>
    public static readonly DependencyProperty MaxTimeProperty =
        DependencyProperty.Register("MaxTime", typeof(TimeSpan), typeof(TimePicker), 
          new UIPropertyMetadata(new TimeSpan(23,59,59),
            delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
              TimePicker picker = (TimePicker)sender;
              picker.CoerceValue(SelectedTimeProperty);//make sure to update the time if appropiate
            }));


    /// <summary>
    /// Gets or sets the selected timestamp 
    /// </summary>
    public Nullable<TimeSpan> SelectedTime {
      get { return (Nullable<TimeSpan>)GetValue(SelectedTimeProperty); }
      set { SetValue(SelectedTimeProperty, value); }
    }

    /// <summary>
    /// Backing store for the selected timestamp 
    /// </summary>
    public static readonly DependencyProperty SelectedTimeProperty =
        DependencyProperty.Register("SelectedTime", typeof(Nullable<TimeSpan>),
        typeof(TimePicker), new FrameworkPropertyMetadata(null, 
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            SelectedTimePropertyChanged,
            ForceValidSelectedTime));

    //make sure tha the proper time is set
    private static object ForceValidSelectedTime(DependencyObject sender, object value) {
      TimePicker picker = (TimePicker)sender;
      Nullable<TimeSpan> time = (Nullable<TimeSpan>)value;
      if (time.HasValue) {
        time = picker.ValidateNewTime(time.Value);
      }
      return time;
    }

    private static void SelectedTimePropertyChanged(DependencyObject sender,
        DependencyPropertyChangedEventArgs e) {

      TimePicker timePicker = (TimePicker)sender;
      Nullable<TimeSpan> newTime = (Nullable<TimeSpan>)e.NewValue;
      Nullable<TimeSpan> oldTime = (Nullable<TimeSpan>)e.OldValue;

      if (!timePicker.isUpdatingTime) {
        timePicker.BeginUpdateSelectedTime();//signal that the selected time is being updated

        if (newTime == null) {
          // Update to null
        }
        else {
          TimeSpan newTimeValue = newTime.Value;

          timePicker.UpdateTimeComponents(newTimeValue);
        }

        timePicker.IsNull = (e.NewValue == null);
        timePicker.EndUpdateSelectedTime();//signal that the selected time has been updated
        timePicker.OnTimeSelectedChanged(timePicker.SelectedTime, oldTime);
      }
    }

    protected virtual void UpdateTimeComponents(TimeSpan newTimeValue) {
      if (SelectedHour != newTimeValue.Hours)
        SelectedHour = newTimeValue.Hours;

      if (SelectedMinute != newTimeValue.Minutes)
        SelectedMinute = newTimeValue.Minutes;

      if (SelectedSecond != newTimeValue.Seconds)
        SelectedSecond = newTimeValue.Seconds;
    }

    /// <summary>
    /// Gets or sets the selected Hour
    /// </summary>
    public int SelectedHour {
      get { return (int)GetValue(SelectedHourProperty); }
      set { SetValue(SelectedHourProperty, value); }
    }

    /// <summary>
    /// Backing store for the selected hour
    /// </summary>
    public static readonly DependencyProperty SelectedHourProperty =
        DependencyProperty.Register("SelectedHour", typeof(int), typeof(TimePicker), new UIPropertyMetadata(0,
            delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
              TimePicker timePicker = (TimePicker)sender;
              int hourToSet = MathUtil.ValidateNumber((int)e.NewValue, 0, 23);

              //validate the hour set
              TimeSpan newTime = timePicker.BuildTimeSpanFromComponents();
              newTime = newTime.Subtract(TimeSpan.FromHours(newTime.Hours));
              newTime = newTime.Add(TimeSpan.FromHours(hourToSet));
              TimeSpan validTime = timePicker.ValidateNewTime(newTime);

              if (validTime.Hours != timePicker.SelectedHour)
                timePicker.SelectedHour = validTime.Hours;

              //set the new timespan
              timePicker.SetNewTime();

            }));

    /// <summary>
    /// Validate the newTime is between MinTime and MaxTime.
    /// </summary>
    /// <param name="newTime"></param>
    /// <returns></returns>
    protected TimeSpan ValidateNewTime(TimeSpan newTime) {
      if (newTime > MaxTime) newTime = MaxTime;
      if (newTime < MinTime) newTime = MinTime;

      return newTime;
    }

    /// <summary>
    /// Gets or sets the selected minutes
    /// </summary>
    public int SelectedMinute {
      get { return (int)GetValue(SelectedMinuteProperty); }
      set { SetValue(SelectedMinuteProperty, value); }
    }

    /// <summary>
    /// Backing store for the selected minsutes
    /// </summary>
    public static readonly DependencyProperty SelectedMinuteProperty =
        DependencyProperty.Register("SelectedMinute", typeof(int), typeof(TimePicker), new UIPropertyMetadata(0,
            delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
              TimePicker timePicker = (TimePicker)sender;
              int minToSet = MathUtil.ValidateNumber((int)e.NewValue, 0, 59);

              //validate the minute set
              TimeSpan newTime = timePicker.BuildTimeSpanFromComponents();
              newTime = newTime.Subtract(TimeSpan.FromMinutes(newTime.Minutes));
              newTime = newTime.Add(TimeSpan.FromMinutes(minToSet));
              TimeSpan validTime = timePicker.ValidateNewTime(newTime);

              if (validTime.Minutes != timePicker.SelectedMinute)
                timePicker.SelectedMinute = validTime.Minutes;

              //set the new timespan
              timePicker.SetNewTime();

            }));

    /// <summary>
    /// Gets or sets the selected second
    /// </summary>
    public int SelectedSecond {
      get { return (int)GetValue(SelectedSecondProperty); }
      set { SetValue(SelectedSecondProperty, value); }
    }

    /// <summary>
    /// Backing store for the selected second
    /// </summary>
    public static readonly DependencyProperty SelectedSecondProperty =
        DependencyProperty.Register("SelectedSecond", typeof(int), typeof(TimePicker), new UIPropertyMetadata(0,
            delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
              TimePicker timePicker = (TimePicker)sender;
              int secToSet = MathUtil.ValidateNumber((int)e.NewValue, 0, 59);

              //validate the minute set
              TimeSpan newTime = timePicker.BuildTimeSpanFromComponents();
              newTime = newTime.Subtract(TimeSpan.FromSeconds(newTime.Seconds));
              newTime = newTime.Add(TimeSpan.FromSeconds(secToSet));
              TimeSpan validTime = timePicker.ValidateNewTime(newTime);

              if (validTime.Seconds != timePicker.SelectedSecond)
                timePicker.SelectedSecond = validTime.Seconds;

              //set the new timespan
              timePicker.SetNewTime();

            }));


    private Nullable<TimeSpan> savedTime;

    public bool IsNull {
      get { return (bool)GetValue(IsNullProperty); }
      set { SetValue(IsNullProperty, value); }
    }

    // Using a DependencyProperty as the backing store for IsNull.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsNullProperty =
        DependencyProperty.Register("IsNull", typeof(bool), typeof(TimePicker),
        new UIPropertyMetadata(true, IsNullChangedCallback));

    private static void IsNullChangedCallback(object sender, DependencyPropertyChangedEventArgs e) {
      TimePicker dateTimePicker = (TimePicker)sender;
      if (dateTimePicker.isUpdatingTime) return;

      bool turnToNull = (bool)e.NewValue;
      if (turnToNull) {
        dateTimePicker.savedTime = dateTimePicker.SelectedTime;
        dateTimePicker.SelectedTime = null;
      }
      else {
        dateTimePicker.SelectedTime = dateTimePicker.savedTime;
      }
    }


    public object IsNullContent {
      get { return (object)GetValue(IsNullContentProperty); }
      set { SetValue(IsNullContentProperty, value); }
    }

    // Using a DependencyProperty as the backing store for IsNullContent.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsNullContentProperty =
        DependencyProperty.Register("IsNullContent", typeof(object), typeof(TimePicker), new UIPropertyMetadata("No date set"));


    #endregion

    #region Events

    /// <summary>
    /// 
    /// </summary>
    public static readonly RoutedEvent SelectedTimeChangedEvent = EventManager.RegisterRoutedEvent("SelectedTimeChanged",
        RoutingStrategy.Bubble, typeof(TimeSelectedChangedEventHandler), typeof(TimePicker));

    public event TimeSelectedChangedEventHandler SelectedTimeChanged {
      add { AddHandler(SelectedTimeChangedEvent, value); }
      remove { RemoveHandler(SelectedTimeChangedEvent, value); }
    }

    #endregion

    /// <summary>
    /// Default constructor
    /// </summary>
    public TimePicker() {
    }

    /// <summary>
    /// Static constructor
    /// </summary>
    static TimePicker() {
      DefaultStyleKeyProperty.OverrideMetadata(
          typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)
              ));
    }

    /// <summary>
    /// override to hook to the Control template elements
    /// </summary>
    public override void OnApplyTemplate() {
      //get the hours textbox and hook the events to it
      hours = GetTemplateChild("PART_Hours") as TextBox;
      hours.PreviewTextInput += HoursTextChanged;
      hours.KeyUp += HoursKeyUp;
      hours.PreviewKeyDown += HandlePreviewKeyUp;
      hours.GotFocus += TextGotFocus;
      hours.GotMouseCapture += TextGotFocus;

      //get the minutes textbox and hook the events to it
      minutes = GetTemplateChild("PART_Minutes") as TextBox;
      minutes.PreviewTextInput += MinutesTextChanged;
      minutes.KeyUp += MinutesKeyUp;
      minutes.PreviewKeyDown += HandlePreviewKeyUp;
      minutes.GotFocus += TextGotFocus;
      minutes.GotMouseCapture += TextGotFocus;

      //get the seconds textbox and hook the events to it
      seconds = GetTemplateChild("PART_Seconds") as TextBox;
      seconds.PreviewTextInput += SecondsTextChanged;
      seconds.KeyUp += SecondsKeyUp;
      seconds.PreviewKeyDown += HandlePreviewKeyUp;
      seconds.GotFocus += TextGotFocus;
      seconds.GotMouseCapture += TextGotFocus;

      textBoxes.Add(hours);
      textBoxes.Add(minutes);
      textBoxes.Add(seconds);

      //Get the increase button and hook to the click event
      ButtonBase increaseButton = GetTemplateChild("PART_IncreaseTime") as ButtonBase;
      increaseButton.Click += IncreaseTime;
      //Get the decrease button and hook to the click event
      ButtonBase decrementButton = GetTemplateChild("PART_DecrementTime") as ButtonBase;
      decrementButton.Click += DecrementTime;
    }

    protected void HandlePreviewKeyUp(object sender, KeyEventArgs e) {
      TextBox tx = (TextBox)sender;
      previousCaretIndex = tx.CaretIndex;
    }

    //event handler for the textboxes (hours, minutes, seconds)
    protected void TextGotFocus(object sender, RoutedEventArgs e) {
      TextBox selectedBox = (TextBox)sender;
      //set the currently selected textbox. 
      //This field is used to check which entity(hour/minute/second) to increment/decrement when user clicks the buttuns
      currentlySelectedTextBox = selectedBox;

      //highlight all code so that it is easier to the user to enter new info in the text box
      selectedBox.SelectAll();
    }

    #region preview input handler
    //handle the preview event so that we validate the text before it is set in the textbox's text

    //event handler for the Hour TextBox
    private void HoursTextChanged(object sender, TextCompositionEventArgs e) {
      //delete the text that is highlight(selected)
      TrimSelectedText(hours);

      //Adjust the text according to the carrot index
      string newText = AdjustText(hours, e.Text);

      //validates that the hour is correct if not set a valid value (0 or 24)
      int hourNum = ValidateAndSetHour(newText);

      //moves the carrot index or focus the neighbour
      AdjustCarretIndexOrMoveToNeighbour(hours);

      //handle the event so that it does not set the text, since we do it manually
      e.Handled = true;
    }

    //event handler for the Minute TextBox
    private void MinutesTextChanged(object sender, TextCompositionEventArgs e) {
      //delete the text that is highlight(selected)
      TrimSelectedText(minutes);

      //Adjust the text according to the carrot index
      string newText = AdjustText(minutes, e.Text);

      //validates that the minute is correct if not set a valid value (0 or 59)
      int minNum = ValidateAndSetMinute(newText);

      //moves the carrot index or focus the neighbour
      AdjustCarretIndexOrMoveToNeighbour(minutes);

      //handle the event so that it does not set the text, since we do it manually
      e.Handled = true;
    }

    //event handler for the Second TextBox
    private void SecondsTextChanged(object sender, TextCompositionEventArgs e) {
      //delete the text that is highlight(selected)
      TrimSelectedText(seconds);

      //Adjust the text according to the carrot index
      string newText = AdjustText(seconds, e.Text);

      //validates that the second is correct if not set a valid value (0 or 59)
      int secNum = ValidateAndSetSeconds(newText);

      //moves the carrot index or focus the neighbour
      AdjustCarretIndexOrMoveToNeighbour(seconds);

      //handle the event so that it does not set the text, since we do it manually
      e.Handled = true;
    }

    #endregion

    #region key up handlers

    //increments/decrement the selected time accordingly to the selected control
    protected bool IncrementDecrementTime(Key selectedKey) {
      if (selectedKey == Key.Up)
        IncrementDecrementTime(true);
      else if (selectedKey == Key.Down)
        IncrementDecrementTime(false);
      else
        return false;
      return true;
    }

    protected virtual TextBox GetLeftTextBoxOf(TextBox current) {
      int index = textBoxes.IndexOf(current);
      if (index == 0) return null;
      return textBoxes[index - 1];
    }

    protected virtual TextBox GetRightTextBoxOf(TextBox current) {
      int index = textBoxes.IndexOf(current);
      if (index + 1 == textBoxes.Count) return null;
      return textBoxes[index + 1];
    }

    private void HoursKeyUp(object sender, KeyEventArgs e) {
      //focus the next control
      TryFocusNeighbourControl(hours, e.Key);

      if (!IncrementDecrementTime(e.Key))
        ValidateAndSetHour(hours.Text);
    }

    private void MinutesKeyUp(object sender, KeyEventArgs e) {
      //focus the next control
      TryFocusNeighbourControl(minutes, e.Key);

      if (!IncrementDecrementTime(e.Key))
        ValidateAndSetMinute(minutes.Text);
    }

    private void SecondsKeyUp(object sender, KeyEventArgs e) {
      //focus the next control
      TryFocusNeighbourControl(seconds, e.Key);

      if (!IncrementDecrementTime(e.Key))
        ValidateAndSetSeconds(seconds.Text);
    }
    #endregion

    #region increase decrease button handlers

    //event handler for the decrease button click
    private void DecrementTime(object sender, RoutedEventArgs e) {
      IncrementDecrementTime(false);
    }

    private void IncreaseTime(object sender, RoutedEventArgs e) {
      IncrementDecrementTime(true);
    }

    #endregion

    #region Helper methods

    //increment or decrement the time (hour/minute/second) currently in selected (determined by the currentlySelectedTextBox that is set in the GotFocus event of the textboxes)
    protected void IncrementDecrementTime(bool increment) {
      TimeSpan change = GetIncermentDecrementSpan();
      if (!increment) {
        change = change.Negate();
      }

      IncrementDecrementTime(change);
    }

    protected virtual TimeSpan GetIncermentDecrementSpan() {
      TimeSpan change = TimeSpan.Zero;
      //check if hour is selected if yes set it
      if (hours == currentlySelectedTextBox) {
        change = TimeSpan.FromHours(1);
      }

      //check if minute is selected if yes set it
      else if (minutes == currentlySelectedTextBox) {
        change = TimeSpan.FromMinutes(1);
      }

      //if non of the above are selected assume that the seconds is selected
      else if (seconds == currentlySelectedTextBox) {
        change = TimeSpan.FromSeconds(1);
      }

      return change;
    }

    protected void IncrementDecrementTime(TimeSpan change) {
      Nullable<TimeSpan> currTime = SelectedTime ?? savedTime;
      if (currTime.HasValue) {
        TimeSpan newTime = currTime.Value.Add(change);
        newTime = ValidateNewTime(newTime);
        SelectedTime = newTime;
      }
    }

    #region Validate and set properties
    //validates the hour passed as text and sets it to the SelectedHour property
    protected int ValidateAndSetHour(string text) {
      int hourNum = MathUtil.ValidateNumber(text, 0, 23);
      SelectedHour = hourNum;
      return hourNum;
    }

    //validates the minute passed as text and sets it to the SelectedMinute property
    private int ValidateAndSetMinute(string text) {
      int minNum = MathUtil.ValidateNumber(text, 0, 59);
      SelectedMinute = minNum;
      return minNum;
    }

    //validates the second passed as text and sets it to the SelectedSecond property
    private int ValidateAndSetSeconds(string text) {
      int secNum = MathUtil.ValidateNumber(text, 0, 59);
      SelectedSecond = secNum;
      return secNum;
    }
    #endregion

    protected int previousCaretIndex = -1;

    //focuses the left/right control accordingly to the key passed. Pass null if there is not a neighbour control
    protected void TryFocusNeighbourControl(TextBox currentControl,  Key keyPressed) {

      TextBox leftControl = GetLeftTextBoxOf(currentControl);
      TextBox rightControl = GetRightTextBoxOf(currentControl);

      if (keyPressed == Key.Left &&
          leftControl != null &&
          previousCaretIndex == currentControl.CaretIndex &&
          currentControl.CaretIndex == 0) {
        leftControl.Focus();
        leftControl.CaretIndex = leftControl.Text.Length;
      }

      else if (keyPressed == Key.Right &&
           rightControl != null &&
        //if the caret index is the same as the length of the text and the user clicks right key it means that he wants to go to the next textbox
          previousCaretIndex == currentControl.CaretIndex &&
           currentControl.CaretIndex == currentControl.Text.Length) {

        previousCaretIndex = -1;
        rightControl.Focus();
        rightControl.CaretIndex = 0;
      }
    }

    //remove the left hand side number if the carrot index is 0 if the carrot index is 1 it removes the right hand side text
    protected static string AdjustText(TextBox textBox, string newText) {
      //replace the new text with the old text if there are already 2 char in the textbox
      if (textBox.Text.Length == 2) {
        if (textBox.CaretIndex == 0)
          return newText + textBox.Text[1];
        else
          return textBox.Text[0] + newText;
      }
      else {
        return textBox.CaretIndex == 0 ?
            newText + textBox.Text //if the carrot is in front the text append the new text infront
            : textBox.Text + newText; //else put it in behind the existing text
      }
    }

    //moves the carrot for the textbox and if the carrot is at the end it will focus the neighbour
    protected void AdjustCarretIndexOrMoveToNeighbour(TextBox current) {
      TextBox neighbour = GetRightTextBoxOf(current);

      //if the current is near the end move to neighbour
      if (current.CaretIndex == 1 && neighbour != null)
        neighbour.Focus();

          //if the carrot is in the first index move the caret one index
      else if (current.CaretIndex == 0)
        current.CaretIndex++;
    }

    //Removes the selected text
    protected static void TrimSelectedText(TextBox textBox) {
      if (textBox.SelectionLength > 0)
        textBox.Text = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
    }

    /// <summary>
    /// sets the selectedTime with the selectedhour, selectedminute and selectedsecond
    /// </summary>
    protected void SetNewTime() {
      if (!isUpdatingTime) {
        TimeSpan newTime = BuildTimeSpanFromComponents();
        //check if the time is the same
        if (SelectedTime != newTime)
          SelectedTime = newTime;
      }
    }

    /// <summary>
    /// Build TimeSpan instance from Selected* properties.
    /// </summary>
    /// <returns></returns>
    protected virtual TimeSpan BuildTimeSpanFromComponents() {
      TimeSpan newTime = new TimeSpan(
          SelectedHour,
          SelectedMinute,
          SelectedSecond);
      return newTime;
    }
    private bool isUpdatingTime = false;
    //call this method while updating the SelectedTimeProperty from the control itself only
    private void BeginUpdateSelectedTime() {
      isUpdatingTime = true;
    }
    private void EndUpdateSelectedTime() {
      isUpdatingTime = false;
    }

    private void OnTimeSelectedChanged(Nullable<TimeSpan> newTime, Nullable<TimeSpan> oldTime) {
      TimeSelectedChangedRoutedEventArgs args = new TimeSelectedChangedRoutedEventArgs(SelectedTimeChangedEvent);
      args.NewTime = newTime;
      args.OldTime = oldTime;
      RaiseEvent(args);
    }
    #endregion

    #region Unit Tests
    /// <summary>
    /// Exposes TryFocusNeighbourControl
    /// </summary>
    /// <param name="currentControl"></param>
    /// <param name="leftControl"></param>
    /// <param name="rightControl"></param>
    /// <param name="keyPressed"></param>
    [System.Diagnostics.Conditional(Globals.UnitTestSymbol)]
    public void ExposeTryFocusNeighbourControl(TextBox currentControl, Key keyPressed) {
      TryFocusNeighbourControl(currentControl, keyPressed);
    }
    /// <summary>
    /// Exposes the AdjustCarretIndexOrMoveToNeighbour
    /// </summary>
    /// <param name="current"></param>
    /// <param name="neighbour"></param>
    [System.Diagnostics.Conditional(Globals.UnitTestSymbol)]
    public void ExposeAdjustCarretIndexOrMoveToNeighbour(TextBox current) {
      AdjustCarretIndexOrMoveToNeighbour(current);
    }

    /// <summary>
    /// Exposes the TrimSelectedText method
    /// </summary>
    /// <param name="textBox"></param>
    [System.Diagnostics.Conditional(Globals.UnitTestSymbol)]
    public static void ExposeTrimSelectedText(TextBox textBox) {
      TrimSelectedText(textBox);
    }

    #endregion
  }

  #region Routed Event

  /// <summary>
  /// Delegate for the TimeSelectedChanged event
  /// </summary>
  /// <param name="sender">The object raising the event</param>
  /// <param name="e">The routed event arguments</param>
  public delegate void TimeSelectedChangedEventHandler(object sender, TimeSelectedChangedRoutedEventArgs e);

  /// <summary>
  /// Routed event arguments for the TimeSelectedChanged event
  /// </summary>
  public class TimeSelectedChangedRoutedEventArgs : RoutedEventArgs {
    /// <summary>
    /// Gets or sets the new time
    /// </summary>
    public Nullable<TimeSpan> NewTime { get; set; }

    /// <summary>
    /// Gets or sets the old time
    /// </summary>
    public Nullable<TimeSpan> OldTime { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="routedEvent">The event that is raised </param>
    public TimeSelectedChangedRoutedEventArgs(RoutedEvent routedEvent)
      : base(routedEvent) { }
  }
  #endregion
}
