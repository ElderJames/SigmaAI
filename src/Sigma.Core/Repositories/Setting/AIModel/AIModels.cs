using Sigma.Core.Repositories.Base;
using Sigma.Core.Domain.Model.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sigma.Core.Repositories
{
    [Table("AIModels")]
    public partial class AIModels : EntityBase
    {
        /// <summary>
        /// AI类型
        /// </summary>
        [Required]
        public AIType AIType { get; set; } = AIType.OpenAI;

        public bool IsChat { get; set; }

        public bool IsEmbedding { get; set; }

        /// <summary>
        /// 模型地址
        /// </summary>
        public string EndPoint { get; set; } = "";

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

        /// <summary>
        /// 启用意图识别
        /// </summary>
        public bool UseIntentionRecognition { get; set; }
    }
}