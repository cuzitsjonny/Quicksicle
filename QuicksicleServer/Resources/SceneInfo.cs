using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksicle.Resources
{
    public class SceneInfo
    {
        public SceneInfo(string name, string fileName)
        {
            this.Name = name;
            this.FileName = FileName;
        }

        public string Name
        {
            get;
        }

        public string FileName
        {
            get;
        }
    }
}
