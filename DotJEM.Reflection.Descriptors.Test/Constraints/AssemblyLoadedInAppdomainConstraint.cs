using System;
using System.Linq;
using System.Reflection;

namespace DotJEM.Reflection.Descriptors.Test.Constraints
{
    public class AssemblyLoadedInAppdomainConstraint : BasicConstraint
    {
        private readonly string fullName;

        public AssemblyLoadedInAppdomainConstraint(string fullName)
        {
            this.fullName = fullName;
        }

        public override bool Matches(object actual)
        {
            AppDomain testDomain = actual as AppDomain;
            if (testDomain == null)
            {
                AppendDescription("Object was not of type AppDomain.");
                return false;
            }

            Assembly test = (from assembly in testDomain.GetAssemblies()
                             where assembly.FullName.Equals(fullName)
                             select assembly).FirstOrDefault();
            if (test == null)
            {
                AppendDescription("Assembly was not loaded in appdomain.");
                return false;
            }
            return true;
        }
    }

    public class AssemblyNotLoadedInAppdomainConstraint : BasicConstraint
    {
        private readonly string fullName;

        public AssemblyNotLoadedInAppdomainConstraint(string fullName)
        {
            this.fullName = fullName;
        }

        public override bool Matches(object actual)
        {
            AppDomain testDomain = actual as AppDomain;
            if (testDomain == null)
            {
                AppendDescription("Object was not of type AppDomain.");
                return false;
            }

            Assembly test = (from assembly in testDomain.GetAssemblies()
                             where assembly.FullName.Equals(fullName)
                             select assembly).FirstOrDefault();
            if (test != null)
            {
                AppendDescription("Assembly was loaded in appdomain.");
                return false;
            }
            return true;
        }
    }
}