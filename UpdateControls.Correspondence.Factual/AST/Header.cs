
namespace UpdateControls.Correspondence.Factual.AST
{
    public class Header
    {
        private readonly string _name;
        private readonly string _parameter;

        public Header(string name, string parameter)
        {
            _name = name;
            _parameter = parameter;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Parameter
        {
            get { return _parameter; }
        }
    }
}
