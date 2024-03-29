using Sigma.Core.Repositories.Base;
using Sigma.Core.Domain.Model.Enum;
using Sigma.Core.Repositories.AI.Plugin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sigma.Core.Repositories
{
    public partial class Plugin : EntityBase
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        [Required]
        public PluginType Type { get; set; } = PluginType.OpenAPI;

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