using System;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence
{
    class AttributeTypeStrategy : ITypeStrategy
    {
        public CorrespondenceFactType GetTypeOfFact(CorrespondenceFact fact)
        {
            // Get the type of the correspondence object.
            return GetTypeFromCLRType(fact.GetType());
        }

        public static CorrespondenceFactType GetTypeFromCLRType(Type type)
        {
            CorrespondenceTypeAttribute attribute = (CorrespondenceTypeAttribute)type
                .GetCustomAttributes(typeof(CorrespondenceTypeAttribute), false).FirstOrDefault();

            if (attribute == null)
                throw new CorrespondenceException(string.Format(
                    "Please add the CorrespondenceType attribute to the class {0}.", type.FullName));

            if (attribute.TypeName == null)
                return new CorrespondenceFactType(type.FullName, attribute.Version);
            else
                return new CorrespondenceFactType(attribute.TypeName, attribute.Version);
        }
    }
}
