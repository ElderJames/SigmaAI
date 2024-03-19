using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Model.Enum
{
    public enum AppType
    {
        [Display(Name ="会话应用")]
        Chat = 1,

        [Display(Name = "知识库")]
        Kms = 2
    }
}
