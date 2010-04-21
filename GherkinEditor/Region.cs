using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GherkinEditor
{
  internal class Region
  {
    public string Text { get; set; }
    public string Hover { get; set; }
    public int StartLine { get; set; }
    public int EndLine { get; set; }
  }
}
