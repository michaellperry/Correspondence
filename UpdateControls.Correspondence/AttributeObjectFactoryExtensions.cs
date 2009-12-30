using System;
using System.Linq;
using System.Reflection;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence
{
    public static class AttributeObjectFactoryExtensions
    {
        public static Community RegisterAssembly(this Community community, Type exampleType)
        {
            // Get the assembly from the example type.
            Assembly assembly = exampleType.Assembly;

            // Search the assembly for all types that have the CorrespondenceType attribute.
            foreach (Type type in assembly.GetTypes())
            {
                foreach (CorrespondenceTypeAttribute correspondenceAttribute in
                    type.GetCustomAttributes(typeof(CorrespondenceTypeAttribute), false))
                {
                    // Get the correspondence type from the attribute.
                    CorrespondenceFactType typeName = (correspondenceAttribute.TypeName == null) ?
                        new CorrespondenceFactType(type.FullName, correspondenceAttribute.Version) :
                        new CorrespondenceFactType(correspondenceAttribute.TypeName, correspondenceAttribute.Version);

                    // Get the memento constructor for this type.
                    ConstructorInfo mementoConstructor = GetMementoConstructor(type);

                    // Register a factory for this type.
                    community.AddType(
                        typeName,
                        new AttributeFactFactory(type, mementoConstructor, community.FieldSerializerByType),
                        new FactMetadata(type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                            .Select(field => field.GetValue(null))
                            .OfType<RoleBase>()
                            .Where(role => role.Metadata == RoleRelationship.Pivot)
                            .Select(role => role.RoleMemento)));

                    // Find all queries defined within the type.
                    foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                        .Where(field => typeof(Query).IsAssignableFrom(field.FieldType) && field.IsStatic))
                    {
                        community.AddQuery(
                            typeName,
                            ((Query)field.GetValue(null)).QueryDefinition);
                    }
                }
            }
            return community;
        }

        private static ConstructorInfo GetMementoConstructor(Type type)
        {
            ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { typeof(FactMemento) });
            if (constructorInfo != null)
                return constructorInfo;
            else
                // No appropriate constructor found.
                throw new CorrespondenceException(string.Format(
                    "Add a constructor to the class {0} taking a Memento as a parameter", type.FullName));
        }
    }
}
