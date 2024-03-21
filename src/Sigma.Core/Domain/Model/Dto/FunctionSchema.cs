using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.Domain.Model.Dto
{
    public class FunctionSchema
    {
        public string  Function { get; set; }
        public string Intention { get; set; }
        public Dictionary<string,object> Arguments { get; set; }
    }
}
