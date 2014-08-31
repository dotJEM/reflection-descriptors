using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace DotJEM.Reflection.Descriptors.Inspection
{
    public class InspectionDomain : IDisposable
    {
        private readonly AppDomain domain;
        private readonly DependencyResolverProxy resolver;

        public InspectionDomain(string workingDirectory, bool shadowCopy)
        {
            AppDomainSetup setup = new AppDomainSetup
                                       {
                                           ApplicationName = Guid.NewGuid().ToString("N"),
                                           ApplicationBase = workingDirectory,
                                           PrivateBinPath = Directory.GetCurrentDirectory(),
                                           ShadowCopyFiles = shadowCopy.ToString().ToLower(),
                                           ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                                       };

            Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
            PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);

            domain = AppDomain.CreateDomain(setup.ApplicationName, evidence, setup, permissions);
            //domain.UnhandledException += HandleException;
            resolver = Create<DependencyResolverProxy>();
        }



        internal void AddDependencyLocation(string directory)
        {
            resolver.AddLocation(directory);
        }

        public TProxy Create<TProxy>() where TProxy : MarshalByRefObject
        {
            Type proxy = typeof(TProxy);
            return (TProxy)domain.CreateInstanceFromAndUnwrap(proxy.Assembly.Location, proxy.FullName);
        }

        public void Dispose()
        {
            AppDomain.Unload(domain);
        }
    }
}