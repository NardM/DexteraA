using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DexteraA
{
    [Serializable()]
    public struct ToFileSave
    {
        public List<Cell> Cells { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

}
