using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.AvalonControlsLibrary.Core {
  public class MidDayDictionary {
    public string this[int dayIndex] {
      get {
        var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        string name = culture.DateTimeFormat.AbbreviatedDayNames[dayIndex - 1];
        return name;
      }
    }

  }
}
