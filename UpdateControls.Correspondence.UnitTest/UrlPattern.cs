using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.UnitTest
{
    public static class UrlPattern
    {
        public static UrlPatternImpl<TFact> Create<TFact>(string pattern, Func<IMessageRepository, List<string>, TFact> factFromValues)
            where TFact : CorrespondenceFact
        {
            return new UrlPatternImpl<TFact>(pattern, factFromValues);
        }
    }
    public class UrlPatternImpl<TFact>
        where TFact : CorrespondenceFact
    {
        private Regex _pattern;
        private Func<IMessageRepository, List<string>, TFact> _factFromValues;

        public UrlPatternImpl(string pattern, Func<IMessageRepository, List<string>, TFact> factFromValues)
        {
            _pattern = new Regex(pattern);
            _factFromValues = factFromValues;
        }

        public TFact UrlToFact(IMessageRepository repository, string url)
        {
            Match match = _pattern.Match(url);
            if (!match.Success)
                return null;

            List<string> values = new List<string>(match.Groups.Count - 1);
            for (int i = 0; i < match.Groups.Count - 1; i++)
                values.Add(match.Groups[i + 1].Value);

            return _factFromValues(repository, values);
        }
    }
}
