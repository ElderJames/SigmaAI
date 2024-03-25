using Sigma.Core.Repositories.Base;
using Sigma.Core.Domain.Model.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sigma.Core.Repositories
{
    [Table("Apps")]
    public partial class Apps : EntityBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Required]
        public string Describe { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Required]
        public string Icon { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Required]
        public AppType Type { get; set; } = AppType.Chat;

        /// <summary>
        /// 会话模型ID
        /// </summary>
        public string? ChatModelID { get; set; }

        /// <summary>
        /// 向量模型ID
        /// </summary>
        public string? EmbeddingModelID { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public double Temperature { get; set; } = 70f;

        /// <summary>
        /// 提示词
        /// </summary>
        public string? Prompt { get; set; }

        /// <summary>
        /// 插件列表
        /// </summary>
        [Column(TypeName = "varchar(1000)")]
        public string? ApiFunctionList { get; set; }

        /// <summary>
        /// 本地函数列表
        /// </summary>
        [Column(TypeName = "varchar(1000)")]
        public string? NativeFunctionList { get; set; }

        /// <summary>
        /// 知识库ID列表
        /// </summary>
        public string? KmsIdList { get; set; }

        /// <summary>
        /// API调用秘钥
        /// </summary>
        public string? SecretKey { get; set; }

        [NotMapped]
        public AIModels AIModel { get; set; }
    }
}