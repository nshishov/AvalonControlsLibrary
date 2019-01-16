using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AC.AvalonControlsLibrary.Core;
using System.Windows.Controls;
using System.Windows.Input;

namespace AC.AvalonControlsLibrary.Controls {

  /// <summary>
  /// TimeSpan picker which allow to edit all components of TimeSpan instance.
  /// </summary>
  public class TimeSpanPicker : TimePicker {

    private TextBox days;

    static TimeSpanPicker() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSpanPicker),
        new FrameworkPropertyMetadata(typeof(TimeSpanPicker)));

      MaxTimeProperty.OverrideMetadata(typeof(TimeSpanPicker),
        new UIPropertyMetadata(TimeSpan.MaxValue));
    }

    /// <summary>
    /// Gets or sets the selected days
    /// </summary>
    public int SelectedDays {
      get { return (int)GetValue(SelectedDaysProperty); }
      set { SetValue(SelectedDaysProperty, value); }
    }

    /// <summary>
    /// Backing store for the selected hour
    /// </summary>
    public static readonly DependencyProperty SelectedDaysProperty =
        DependencyProperty.Register("SelectedDays", typeof(int), typeof(TimeSpanPicker), new UIPropertyMetadata(0,
            delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
              TimeSpanPicker timePicker = (TimeSpanPicker)sender;

              //validate the hour set
              int days = MathUtil.ValidateNumber(timePicker.SelectedDays,
                  timePicker.MinTime.Days, timePicker.MaxTime.Days);
              if (days != timePicker.SelectedDays)
                timePicker.SelectedDays = days;

              //set the new timespan
              timePicker.SetNewTime();

            }));

    /// <summary>
    /// Override BuildTimeSpanFromComponents to include SelectedDays in it.
    /// </summary>
    /// <returns></returns>
    protected override TimeSpan BuildTimeSpanFromComponents() {
      TimeSpan ts = new TimeSpan(
        SelectedDays, SelectedHour, SelectedMinute, SelectedSecond);
      return ts;
    }

    public override void OnApplyTemplate() {
      base.OnApplyTemplate();

      //get the hours textbox and hook the events to it
      days = Template.FindName("PART_Days", this) as TextBox;
      days.PreviewTextInput += DaysTextChanged;
      days.KeyUp += DaysKeyUp;
      days.PreviewKeyDown += HandlePreviewKeyUp;
      days.GotFocus += TextGotFocus;
      days.GotMouseCapture += TextGotFocus;

      textBoxes.Insert(0, days);
    }

    //event handler for the Minute TextBox
    private void DaysTextChanged(object sender, TextCompositionEventArgs e) {
      //delete the text that is highlight(selected)
      TrimSelectedText(days);

      //Adjust the text according to the carrot index
      string newText = AdjustDaysText(days, e.Text);

      //validates that the minute is correct if not set a valid value (0 or 59)
      int minNum = ValidateAndSetDays(newText);

      //moves the carrot index or focus the neighbour
      AdjustDaysCarretIndexOrMoveToNeighbour(days);

      //handle the event so that it does not set the text, since we do it manually
      e.Handled = true;
    }

    protected static string AdjustDaysText(TextBox textBox, string newText) {
      //replace the new text with the old text if there are already 2 char in the textbox
      StringBuilder sb = new StringBuilder(textBox.Text);

      if (textBox.Text.Length == 3) {
        sb.Remove(textBox.CaretIndex, 1);
        sb.Insert(textBox.CaretIndex, newText);
      }
      else {
        sb.Insert(textBox.CaretIndex, newText);
      }
      return sb.ToString();
    }

    //moves the carrot for the textbox and if the carrot is at the end it will focus the neighbour
    protected void AdjustDaysCarretIndexOrMoveToNeighbour(TextBox current) {
      TextBox neighbour = GetRightTextBoxOf(current);

      //if the current is near the end move to neighbour
      if (current.CaretIndex == 3 && neighbour != null)
        neighbour.Focus();

          //if the carrot is in the first index move the caret one index
      else if (current.CaretIndex == 0)
        current.CaretIndex++;
    }


    private int ValidateAndSetDays(string text) {
      int minNum = MathUtil.ValidateNumber(text, MinTime.Days, MaxTime.Days);
      SelectedDays = minNum;
      return minNum;
    }

    private void DaysKeyUp(object sender, KeyEventArgs e) {
      //focus the next control
      TryFocusNeighbourControl(days, e.Key);

      if (!IncrementDecrementTime(e.Key))
        ValidateAndSetHour(hours.Text);
    }

    protected override TimeSpan GetIncermentDecrementSpan() {
      if (days == currentlySelectedTextBox) {
        return TimeSpan.FromDays(1);
      }
      else {
        return base.GetIncermentDecrementSpan();
      }
    }

    protected override void UpdateTimeComponents(TimeSpan newTimeValue) {
      base.UpdateTimeComponents(newTimeValue);

      if (SelectedDays != newTimeValue.Days)
        SelectedDays = newTimeValue.Days;

    }

  }
}
