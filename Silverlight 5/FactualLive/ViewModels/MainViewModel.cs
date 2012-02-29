using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FactualLive.Models;

namespace FactualLive.ViewModels
{
    public class MainViewModel
    {
        private readonly FactualSession _session;

        public MainViewModel(FactualSession session)
        {
            _session = session;
        }

        public string Factual
        {
            get { return _session.Factual; }
            set { _session.Factual = value; }
        }

        public IEnumerable<FactualError> Errors
        {
            get { return _session.Errors; }
        }
    }
}
