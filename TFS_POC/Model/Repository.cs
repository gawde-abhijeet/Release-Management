using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFS_POC.Model
{
        public class Repository
        {
            public int Count { get; set; }
            public List<Value> Value { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string name { get; set; }
            public Project project { get; set; }
            public string defaultBranch { get; set; }
            public string remoteUrl { get; set; }
        }

        public class Project
        {
            public string id { get; set; }
            public string name { get; set; }
        }
}
