using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFS_POC.Model.GitCommits
{

    public class GitCommits
    {
        public int count { get; set; }
        public Value[] Value { get; set; }
    }

    public class Value
    {
        public string commitId { get; set; }
        public Author author { get; set; }
        public Committer committer { get; set; }
        public string comment { get; set; }
        public Changecounts changeCounts { get; set; }
        public bool commentTruncated { get; set; }
    }

    public class Author
    {
        public string name { get; set; }
        public string email { get; set; }
        public DateTime date { get; set; }
    }

    public class Committer
    {
        public string name { get; set; }
        public string email { get; set; }
        public DateTime date { get; set; }
    }

    public class Changecounts
    {
        public int Edit { get; set; }
        public int Add { get; set; }
    }

}
