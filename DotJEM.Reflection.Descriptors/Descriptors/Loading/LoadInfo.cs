using System.Collections.Generic;

namespace DotJEM.Reflection.Descriptors.Descriptors.Loading
{
    public class LoadInfo
    {
        public bool ShadowCopy { get; }
        public string WorkingDirectory { get; }
        public List<string> Locations { get; }

        public LoadInfo(string workingDirectory, bool shadowCopy)
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