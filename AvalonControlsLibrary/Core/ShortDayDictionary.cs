using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.AvalonControlsLibrary.Core {
  public class ShortDayDictionary {

    public string this[int dayIndex] {
      get {
        var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        string name = culture.DateTimeFormat.ShortestDayNames[dayIndex - 1];
        return name;
      }
    }
  }
}
