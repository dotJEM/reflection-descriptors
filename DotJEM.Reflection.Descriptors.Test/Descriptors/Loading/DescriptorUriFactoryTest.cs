using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotJEM.Reflection.Descriptors.Descriptors.Loading;
using NUnit.Framework;

namespace DotJEM.Reflection.Descriptors.Test.Descriptors.Loading
{
    [TestFixture]
    public class DescriptorUriFactoryTest
    {
        [Test]
        public void TestAssembly()
        {
            // assembly://F:\OSS\dotJEM Stack\reflection-descriptors\DotJEM.Reflection.Descriptors.Test\bin\Debug\DotJEM.Reflection.Descriptors.Test.DLL
            string uri = DescriptorUriFactory.CreateReference(Assembly.GetExecutingAssembly());
            Assert.That(uri, Is.EqualTo(""));
        }

        [Test]
        public void TestType()
        {
            // type://e3e508c0-e442-3b15-a686-5f49daf2e138@assembly://F:\OSS\dotJEM Stack\reflection-descriptors\DotJEM.Reflection.Descriptors.Test\bin\Debug\DotJEM.Reflection.Descriptors.Test.DLL
            string uri = DescriptorUriFactory.CreateReference(typeof(Fake));
            Assert.That(uri, Is.EqualTo(""));
        }

        [Test]
        public void TestField()
        {
            // field://System.String myField@type://e3e508c0-e442-3b15-a686-5f49daf2e138@assembly://F:\OSS\dotJEM Stack\reflection-descriptors\DotJEM.Reflection.Descriptors.Test\bin\Debug\DotJEM.Reflection.Descriptors.Test.DLL
            string uri = DescriptorUriFactory.CreateReference(typeof(Fake).GetField("myField"));
            Assert.That(uri, Is.EqualTo(""));
        }

        [Test]
        public void TestMethod()
        {
            // field://System.String myField@type://e3e508c0-e442-3b15-a686-5f49daf2e138@assembly://F:\OSS\dotJEM Stack\reflection-descriptors\DotJEM.Reflection.Descriptors.Test\bin\Debug\DotJEM.Reflection.Descriptors.Test.DLL
            string uri = DescriptorUriFactory.CreateReference(typeof(Fake).GetMethod("MyMethod"));

            

            Assert.That(uri, Is.EqualTo(""));
        }

        public class Fake
        {
            public static string myStaticField = String.Empty;

            public string myField = string.Empty;

            public static string MyStaticMethod(string param1, string param2) => "";
            public string MyMethod(string param1, string param2) => "";
            public string MyGenericMethod<T1, T2>(T1 param1, T2 param2) => "";
        }
    }

    [TestFixture]
    public class DescriptorUriTest
    {

        [Test]
        public void TestType()
        {
            // 
            string uri = "type://e3e508c0-e442-3b15-a686-5f49daf2e138@assembly://C:\\dummy space\\My.DLL";
            string uri2 = "method://System.String MyMethod(System.String, System.String)@type://e3e508c0-e442-3b15-a686-5f49daf2e138@assembly://C:\\dummy space\\My.DLL";
            DescriptorUri.TryParse(uri, out DescriptorUri parsed);

            Assert.That(parsed, Is.TypeOf<TypeDescriptorUri>());

        }

        [TestCase("type://e3e508c0-e442-3b15-a686-5f49daf2e138@assembly://C:\\dummy space\\My.DLL", typeof(TypeDescriptorUri))]
        [TestCase("method://System.String MyMethod(System.String, System.String)@type://e3e508c0-e442-3b15-a686-5f49daf2e138@assembly://C:\\dummy space\\My.DLL", typeof(MethodDescriptorUri))]
        public void TryParse_ValidUri_ReturnsExpectedType(string uri, Type expectedType)
        {
            DescriptorUri.TryParse(uri, out DescriptorUri parsed);
            Assert.That(parsed, Is.TypeOf(expectedType));
        }
    }
}
