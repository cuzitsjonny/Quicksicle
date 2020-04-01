using System;
using System.IO;

namespace Quicksicle.Resources
{
    public class PredefinedNameCache
    {
        private string[] first;
        private string[] middle;
        private string[] last;

        public PredefinedNameCache(string resourcesDirectoryPath)
        {
            this.first = File.ReadAllLines(Path.Combine(resourcesDirectoryPath, "names/minifigname_first.txt"));
            this.middle = File.ReadAllLines(Path.Combine(resourcesDirectoryPath, "names/minifigname_middle.txt"));
            this.last = File.ReadAllLines(Path.Combine(resourcesDirectoryPath, "names/minifigname_last.txt"));
        }

        public string GetFirstName(uint index)
        {
            try
            {
                return first[index];
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public string GetMiddleName(uint index)
        {
            try
            {
                return middle[index];
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public string GetLastName(uint index)
        {
            try
            {
                return last[index];
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
    }
}
