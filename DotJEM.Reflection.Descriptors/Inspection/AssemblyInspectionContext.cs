using System;
using System.Reflection;
using System.Threading;
using DotJEM.Reflection.Descriptors.Cache;
using DotJEM.Reflection.Descriptors.Descriptors;
using DotJEM.Reflection.Descriptors.Descriptors.Loading;

namespace DotJEM.Reflection.Descriptors.Inspection
{
    /// <summary>
    /// Represents a context of an <see cref="Assembly"/> inspection.
    /// </summary>
    /// <remarks>
    /// For as long as the context exists, a dedicated <see cref="AppDomain"/> will exists where the <see cref="Assembly"/> will be inspected in.
    /// <p>
    /// When disposing an <see cref="AssemblyInspectionContext"/> the <see cref="AppDomain"/> is released causing further calls
    /// inspection of the descriptors will cause a new <see cref="AppDomain"/> to be created and unloaded for each call.
    /// </p>
    /// 
    /// </remarks>
    public class AssemblyInspectionContext : IAssemblyInspectionContext
    {
        private readonly InspectionDomain domain;
        private readonly DescriptorCache cache;
        private readonly LoadInfo loadInfo;

        internal AssemblyLoader Loader { get; }

        public AssemblyInspectionContext()
            : this(Environment.CurrentDirectory, true)
        {
        }
        
        public AssemblyInspectionContext(string workingDirectory)
            : this(workingDirectory, true)
        {
        }

        public AssemblyInspectionContext(string workingDirectory, bool shadowCopy)
            : this(workingDirectory, shadowCopy, new DescriptorCache())
        {
        }

        internal AssemblyInspectionContext(string workingDirectory, bool shadowCopy, DescriptorCache cache)
        {
            domain = new InspectionDomain(workingDirectory, shadowCopy);
            loadInfo = new LoadInfo(workingDirectory, shadowCopy);

            AddDependencyLocation(Environment.CurrentDirectory);
            AddDependencyLocation(workingDirectory);

            cache.Context = this;

            Loader = domain.Create<AssemblyLoader>();

            this.cache = cache;
        }


        public void AddDependencyLocation(string searchPath)
        {
            domain.AddDependencyLocation(searchPath);
            loadInfo.AddDependencyLocation(searchPath);
        }

        public AssemblyDescriptor LoadAssembly(string assemblyPath)
        {
            return cache.LoadAssembly(assemblyPath, loadInfo);
        }

        public void Dispose()
        {
            cache.Context = null;
            domain.Dispose();
        }
    }
}