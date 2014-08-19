using System;
using DotJEM.Reflection.Descriptors.Descriptors;

namespace DotJEM.Reflection.Descriptors
{
    public interface IAssemblyInspectionContext : IDisposable
    {
        void AddDependencyLocation(string searchPath);

        AssemblyDescriptor LoadAssembly(string assemblyPath);
    }
}