using System;
using System.IO;
using System.Linq;
using DotJEM.Reflection.Descriptors;
using DotJEM.Reflection.Descriptors.Descriptors;
using DotJEM.Reflection.Descriptors.Inspection;
using NUnit.Framework;
using Does = DotJEM.Reflection.Descriptors.Test.Constraints.Does;

namespace Jeme.Reflection.Test
{
    [TestFixture]
    [Category("Integration")]
    public class AssemblyInspectionContextTest
    {
        [Test]
        public void LoadAssemblyDescriptor_TestDataDll_ReturnsDescriptor()
        {
            string loadFrom = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data\\TestData.dll");

            using (IAssemblyInspectionContext context = new AssemblyInspectionContext(TestContext.CurrentContext.TestDirectory))
            {
                AssemblyDescriptor descriptor = context.LoadAssembly(loadFrom);

                Assert.That(descriptor.Location, Is.EqualTo(loadFrom));
                Assert.That(descriptor.GlobalAssemblyCache, Is.False);
                Assert.That(descriptor.IsFullyTrusted, Is.True);
                Assert.That(descriptor.FullName, Is.EqualTo("TestData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
            }
            Assert.That(AppDomain.CurrentDomain, Does.NotHaveAssemblyLoaded("TestData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
        }

        [Test]
        public void GetTypesByInterface_TestDataDll_ReturnsDescriptor()
        {
            string loadFrom = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data\\TestData.dll");
            using (IAssemblyInspectionContext context = new AssemblyInspectionContext(TestContext.CurrentContext.TestDirectory))
            {
                AssemblyDescriptor descriptor = context.LoadAssembly(loadFrom);

                TypeDescriptor[] types = descriptor.Types;

                TypeDescriptor[] implements = types.Where(td => td.IsSubclassOf(typeof(List))).ToArray();

                TypeDescriptor class1 = types.Single(t => t.Name == "Class1");

                Assert.That(descriptor.Location, Is.EqualTo(Environment.CurrentDirectory + "\\Data\\TestData.dll"));
                Assert.That(descriptor.GlobalAssemblyCache, Is.False);
                Assert.That(descriptor.IsFullyTrusted, Is.True);
                Assert.That(descriptor.FullName, Is.EqualTo("TestData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
            }
            Assert.That(AppDomain.CurrentDomain, Does.NotHaveAssemblyLoaded("TestData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
        }

        [Test]
        public void GetTypes_TestDataDll_ReturnsTypeDescriptors()
        {
            using (IAssemblyInspectionContext context = new AssemblyInspectionContext())
            {
                AssemblyDescriptor descriptor = context.LoadAssembly("Data\\TestData.dll");

                //Note: Descriptor Asserts on AssemblyDescriptors should always check that the parrent assembly is the correct one.
                //Assert.That(descriptor, Does.Contain.TypeDescriptors(TD.Named("TestData.Class1"), TD.Named("TestData.Class1")));

                TypeDescriptor[] types = descriptor.Types;

                TypeDescriptor class1 = types.Single(t => t.Name == "Class1");
                Assert.That(class1.Assembly, Is.SameAs(descriptor));
                Assert.That(class1.FullName, Is.EqualTo("TestData.Class1"));

                TypeDescriptor class2 = types.Single(t => t.Name == "Class2");
                Assert.That(class2.Assembly, Is.SameAs(descriptor));
                Assert.That(class2.FullName, Is.EqualTo("TestData.Class2"));
            }
            Assert.That(AppDomain.CurrentDomain, Does.NotHaveAssemblyLoaded("TestData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
        }

        [Test]
        public void GetCustomAttributes_TestDataDll_ReturnsDynamicAttributeDescriptors()
        {
            using (IAssemblyInspectionContext context = new AssemblyInspectionContext())
            {
                AssemblyDescriptor descriptor = context.LoadAssembly("Data\\TestData.dll");

                TypeDescriptor[] types = descriptor.Types;

                TypeDescriptor class1 = types.Single(t => t.Name == "Class1");
                dynamic category1 = class1.GetCustomAttributes(true).FirstOrDefault();

                Assert.That(category1.Category, Is.EqualTo("Category Class 1"));

                TypeDescriptor class2 = types.Single(t => t.Name == "Class2");
                dynamic category2 = class2.GetCustomAttributes(true).FirstOrDefault();
                
                Assert.That(category2.Category, Is.EqualTo("Category Class 2"));
            }
            Assert.That(AppDomain.CurrentDomain, Does.NotHaveAssemblyLoaded("TestData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
        }
        
        [Test]
        public void GetProperties_TestDataDll_ReturnsPropertyDescriptor()
        {
            using (IAssemblyInspectionContext context = new AssemblyInspectionContext("Data\\"))
            {
                AssemblyDescriptor descriptor = context.LoadAssembly("Data\\TestData.dll");

                TypeDescriptor[] types = descriptor.Types;
  
                TypeDescriptor class1 = types.Single(t => t.Name == "Class1");
                PropertyDescriptor property1 = class1.Properties.First();

                Assert.That(property1.Name, Is.EqualTo("StringProperty"));
                Assert.That(property1.PropertyType.FullName, Is.EqualTo("System.String"));

                TypeDescriptor class2 = types.Single(t => t.Name == "Class2");
                PropertyDescriptor property2 = class2.Properties.First();

                Assert.That(property2.Name, Is.EqualTo("StringProperty"));
                Assert.That(property2.PropertyType.FullName, Is.EqualTo("System.String"));
            }
            Assert.That(AppDomain.CurrentDomain, Does.NotHaveAssemblyLoaded("TestData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
        }

        [Test]
        public void GetTypes_TestDataDll_WorksAfterContextDispose()
        {
            AssemblyDescriptor descriptor;
            using (IAssemblyInspectionContext context = new AssemblyInspectionContext("Data\\"))
            {
                descriptor = context.LoadAssembly("Data\\TestData.dll");
            }

            TypeDescriptor[] types = descriptor.Types;
            Assert.That(AppDomain.CurrentDomain, Does.NotHaveAssemblyLoaded("TestData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
        }
    }
}