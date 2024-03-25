using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sigma.Core.Domain.Model.Dto
{
    public class FunctionSchema
    {
        public string  Function { get; set; }
        public string Intention { get; set; }
        public string Reason { get; set; }
        public JsonElement Arguments { get; set; }
    }
}
