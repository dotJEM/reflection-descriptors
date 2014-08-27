using DotJEM.Reflection.Descriptors.Descriptors;
using DotJEM.Reflection.Descriptors.Descriptors.Instance;
using NUnit.Framework;

namespace Jeme.Reflection.Test
{
    [TestFixture]
    public class ObjectDescriptorTest
    {
        [Test]
        public void ObjectDescriptor_SimpleObject_ValuesMapped()
        {
            Simple simple = new Simple();
            simple.stringField = "string field";
            simple.intField = 50;
            simple.StringProperty = "string property";
            simple.IntProperty = -50;

            dynamic objectDescriptor = new ObjectDescriptor(simple);

            Assert.AreEqual(simple.stringField, objectDescriptor.stringField);
            Assert.AreEqual(simple.intField, objectDescriptor.intField);
            Assert.AreEqual(simple.StringProperty, objectDescriptor.StringProperty);
            Assert.AreEqual(simple.IntProperty, objectDescriptor.IntProperty);

            TypeDescriptor typeDescriptor = objectDescriptor.GetTypeDescriptor();
        }

        [Test]
        public void ObjectDescriptor_NestedObject_ValuesMapped()
        {
            Simple simple = new Simple();
            simple.stringField = "string field";
            simple.intField = 50;
            simple.StringProperty = "string property";
            simple.IntProperty = -50;

            Simple simple2 = new Simple();
            simple2.stringField = "string field";
            simple2.intField = 50;
            simple2.StringProperty = "string property";
            simple2.IntProperty = -50;

            Nested nested= new Nested();
            nested.simpleField = simple;
            nested.SimpleProperty = simple2;

            dynamic descriptor = new ObjectDescriptor(nested);

            Assert.AreEqual(nested.simpleField.stringField, descriptor.simpleField.stringField);
            Assert.AreEqual(nested.simpleField.intField, descriptor.simpleField.intField);
            Assert.AreEqual(nested.simpleField.StringProperty, descriptor.simpleField.StringProperty);
            Assert.AreEqual(nested.simpleField.IntProperty, descriptor.simpleField.IntProperty);

            Assert.AreEqual(nested.SimpleProperty.stringField, descriptor.SimpleProperty.stringField);
            Assert.AreEqual(nested.SimpleProperty.intField, descriptor.SimpleProperty.intField);
            Assert.AreEqual(nested.SimpleProperty.StringProperty, descriptor.SimpleProperty.StringProperty);
            Assert.AreEqual(nested.SimpleProperty.IntProperty, descriptor.SimpleProperty.IntProperty);
        }
    }

    public class Simple
    {
        public string stringField;
        public int intField;

        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
    }

    public class Nested
    {
        public Simple simpleField;
        public Simple SimpleProperty { get; set; }
    }
}