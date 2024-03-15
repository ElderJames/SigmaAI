using AntSK.Domain.Domain.Model.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntSK.Domain.Repositories
{
    [Table("AIModels")]
    public partial class AIModels
    {
        public string Id { get; set; }
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
        [Required]
        public string EndPoint { get; set; } = "";
        /// <summary>
        /// 模型名称
        /// </summary>
        [Required]
        public string ModelName { get; set; } = "";
        /// <summary>
        /// 模型秘钥
        /// </summary>
        [Required]
        public string ModelKey { get; set; } = "";
        /// <summary>
        /// 部署名，azure需要使用
        /// </summary>

        [Required]
        public string ModelDescription { get; set; }
    }
}
