using UpdateControls.Fields;
using System.Collections.Generic;

namespace FactualLive.Models
{
    public class FactualSession
    {
        private Independent<string> _factual = new Independent<string>();
        private Dependent<FactualDefinition> _depParse;

        public FactualSession()
        {
            _depParse = new Dependent<FactualDefinition>(() => FactualDefinition.Parse(_factual));
        }

        public string Factual
        {
            get { return _factual; }
            set { _factual.Value = value; }
        }

        public IEnumerable<FactualError> Errors
        {
            get { return _depParse.Value.Errors; }
        }
    }
}
