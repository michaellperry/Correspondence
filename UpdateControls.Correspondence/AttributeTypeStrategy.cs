using System;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System.Collections.Generic;

namespace UpdateControls.Correspondence
{
    class AttributeTypeStrategy : ITypeStrategy
    {
        public CorrespondenceFactType GetTypeOfFact(CorrespondenceFact fact)
        {
            // Get the type of the correspondence object.
            return GetTypeFromCLRType(fact.GetType());
        }

        public IEnumerable<CorrespondenceFactType> GetAllTypesOfFact(CorrespondenceFact fact)
        {
            // Traverse the base class hierarchy.
            Type type = fact.GetType();
            while (type != null)
            {
                // Traverse the CorrespondenceType attributes.
                foreach (CorrespondenceTypeAttribute attribute in type
                    .GetCustomAttributes(typeof(CorrespondenceTypeAttribute), false))
                {
                    yield return FactTypeFromAttribute(type, attribute);
                }

                type = type.BaseType;
            }
        }

        public static CorrespondenceFactType GetTypeFromCLRType(Type type)
        {
            // Find the CorrespondenceType attribute.
            CorrespondenceTypeAttribute attribute = (CorrespondenceTypeAttribute)type
                .GetCustomAttributes(typeof(CorrespondenceTypeAttribute), false).FirstOrDefault();

            if (attribute != null)
                return FactTypeFromAttribute(type, attribute);
            else
                throw new CorrespondenceException(string.Format(
                    "Please add the CorrespondenceType attribute to the class {0}.", type.FullName));
        }

        private static CorrespondenceFactType FactTypeFromAttribute(Type type, CorrespondenceTypeAttribute attribute)
        {
            if (attribute.TypeName == null)
                return new CorrespondenceFactType(type.FullName, attribute.Version);
            else
                return new CorrespondenceFactType(attribute.TypeName, attribute.Version);
        }
    }
}
