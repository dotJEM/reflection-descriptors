using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Constraints;

namespace DotJEM.Reflection.Descriptors.Test.Constraints
{
    public static class Does
    {
        public static Constraint HaveAssemblyLoaded(string fullName)
        {
            return new AssemblyLoadedInAppdomainConstraint(fullName);
        }

        public static Constraint NotHaveAssemblyLoaded(string fullName)
        {
            return new AssemblyNotLoadedInAppdomainConstraint(fullName);
        }

        //public static NegativeConstraint Not
        //{
        //    get { return new NegativeConstraint(); }
        //}

        //public static HaveConstraint Have
        //{
        //    get { return new PositiveConstraint().Have; }
        //}

        // ReSharper disable UnusedMember.Local
        private static void Usage()
        {
          //  Does.Have.AssemblyLoaded("");
        }
        // ReSharper restore UnusedMember.Local
    }

    public abstract class BasicConstraint : Constraint
    {
        private readonly BasicConstraint inner;
        private readonly StringBuilder description = new StringBuilder();
        private readonly List<string> information = new List<string>();

        protected BasicConstraint(BasicConstraint inner = null)
        {
            this.inner = inner;
        }

        protected BasicConstraint AppendDescription(string format, params object[] args)
        {
            description.AppendFormat(format, args);
            return this;
        }

        protected BasicConstraint AppendInformationLine(string format, params object[] args)
        {
            information.Add(string.Format(format, args));
            return this;
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            writer.WriteLine(description.ToString());
            foreach (string line in information)
                writer.WriteLine("  " + line);
        }

        public override bool Matches(object actual)
        {
            this.actual = actual;
            return inner != null ? inner.Matches(actual) : DoesMatch(actual);
        }

        public virtual bool DoesMatch(object actual)
        {
            return inner.DoesMatch(actual);
        }

        public override void WriteDescriptionTo(MessageWriter writer) { }
    }

    public class HaveConstraint : BasicConstraint
    {
        public HaveConstraint(BasicConstraint basicConstraint)
            : base(basicConstraint)
        {
        }

        public override bool DoesMatch(object actual)
        {
            return false;
        }
    }

    public class PositiveConstraint : BasicConstraint
    {

    }

    public class NegativeConstraint : BasicConstraint
    {
    }
}