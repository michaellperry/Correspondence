using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Correspondence.Mementos;

namespace Correspondence.Strategy
{
    public class SaveResult
    {
        public bool WasSaved { get; set; }
        public FactID Id { get; set; }
    }
}
