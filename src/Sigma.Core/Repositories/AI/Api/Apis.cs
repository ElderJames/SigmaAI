using AntSK.Core.Repositories.Base;
using AntSK.Domain.Domain.Model.Enum;
using Sigma.Core.Repositories.AI.Api;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntSK.Domain.Repositories
{
    [Table("Apis")]
    public partial class Apis : EntityBase
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        [Required]
        public ApiPluginType Type { get; set; } = ApiPluginType.OpenAPI;

        /// <summary>
        /// 接口描述
        /// </summary>
        [Required]
        public string Describe { get; set; }

        /// <summary>
        /// 接口地址
        /// </summary>
        [Required]
        public string Url { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        public HttpMethodType Method { get; set; } = HttpMethodType.Get;

        [Column(TypeName = "varchar(1000)")]
        public string? Header { get; set; }

        /// <summary>
        /// QueryString参数
        /// </summary>
        [Column(TypeName = "varchar(1000)")]
        public string? Query { get; set; }

        /// <summary>
        /// jsonBody 实体
        /// </summary>
        [Column(TypeName = "varchar(7000)")]
        public string? JsonBody { get; set; }

        /// <summary>
        /// 入参提示词
        /// </summary>
        [Column(TypeName = "varchar(1500)")]
        public string? InputPrompt { get; set; }

        /// <summary>
        /// 返回提示词
        /// </summary>
        [Column(TypeName = "varchar(1500)")]
        public string? OutputPrompt { get; set; }
    }
}