using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    internal class TagData
    {
        public Guid Id { get; set; }
        public string Type { get; set; }

        public TagData(Guid id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}
