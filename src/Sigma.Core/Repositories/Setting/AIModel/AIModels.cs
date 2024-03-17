using AntSK.Core.Repositories.Base;
using AntSK.Domain.Domain.Model.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntSK.Domain.Repositories
{
    [Table("AIModels")]
    public partial class AIModels : EntityBase
    {
        /// <summary>
        /// AI类型
        /// </summary>
        [Required]
        public AIType AIType { get; set; } = AIType.OpenAI;

        /// <summary>
        /// 模型类型
        /// </summary>
        [Required]
        public AIModelType AIModelType { get; set; } = AIModelType.Chat;

        /// <summary>
        /// 模型地址
        /// </summary>
        public string? EndPoint { get; set; } = "";

        /// <summary>
        /// 模型名称
        /// </summary>
        public string? ModelName { get; set; } = "";

        /// <summary>
        /// 模型秘钥
        /// </summary>
        public string? ModelKey { get; set; } = "";

        /// <summary>
        /// 部署名，azure需要使用
        /// </summary>
        public string? ModelDescription { get; set; }
    }
}