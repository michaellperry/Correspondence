﻿using System;
using System.Collections.Generic;
using UpdateControls.Correspondence.Factual.Compiler;
using System.IO;
using UpdateControls.Correspondence.Factual.AST;
using System.Linq;

namespace FactualLive.Models
{
    public class FactualDefinition
    {
        private readonly List<FactualError> _errors;

        public FactualDefinition(IEnumerable<FactualError> errors)
        {
            _errors = errors.ToList();
        }

        public static FactualDefinition Parse(string factual)
        {
            using (StringReader reader = new StringReader("namespace com.sample.test; " + factual))
            {
                FactualParser parser = new FactualParser(reader);
                Namespace parsedNamespace = parser.Parse();
                return new FactualDefinition(
                    from error in parser.Errors
                    select new FactualError(error.Message));
            }
        }

        public IEnumerable<FactualError> Errors
        {
            get { return _errors; }
        }
    }
}
