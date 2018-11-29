using System.Collections.Generic;

namespace SFA.DAS.Testing.UnitTests.Builder
{
    public class ObjectWithPrivateProperties
    {
        public int IntProperty { get; private set; }
        public string ReadOnlyStringProperty { get; }
        public IEnumerable<string> PropertyListOfObjects => ListOfObjects;
        public IEnumerable<string> ListOfObjects = new List<string>();
        public string StringField = "Field";

        public ObjectWithPrivateProperties(int intProperty)
        {
            IntProperty = intProperty;
            ReadOnlyStringProperty = "CannotBeChanged";
        }

        private ObjectWithPrivateProperties()
        {

        }
    }

    public class ObjectWithNoParameterlessConstructor
    {
        public int IntProperty { get; private set; }

        public string StringProperty { get; private set; }

        public ObjectWithNoParameterlessConstructor(int intProperty, string stringProperty)
        {
            IntProperty = intProperty;
            StringProperty = stringProperty;
        }
    }

}