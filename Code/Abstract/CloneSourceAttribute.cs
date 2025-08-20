using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Abstract
{
    public class CloneSourceAttribute : Attribute
    {
        public readonly string clone_source_id;

        public CloneSourceAttribute(string id)
        {
            clone_source_id = id;
        }
    }
}
