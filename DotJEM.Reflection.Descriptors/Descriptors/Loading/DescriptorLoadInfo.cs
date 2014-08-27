using System.Collections.Generic;

namespace DotJEM.Reflection.Descriptors.Descriptors.Loading
{
    internal class DescriptorLoadInfo
    {
        public bool ShadowCopy { get; private set; }
        public string WorkingDirectory { get; private set; }
        public List<string> Locations { get; private set; }

        public DescriptorLoadInfo(string workingDirectory, bool shadowCopy)
        {
            WorkingDirectory = workingDirectory;
            ShadowCopy = shadowCopy;

            Locations = new List<string>();
        }

        public void AddDependencyLocation(string searchPath)
        {
            Locations.Add(searchPath);
        }
    }
}