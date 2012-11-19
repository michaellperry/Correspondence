using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public class SaveResult
    {
        public bool WasSaved { get; set; }
        public FactID Id { get; set; }
    }
}
