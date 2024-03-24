using Sigma.Core.Repositories.Base;
using Sigma.Core.Domain.Model.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sigma.Core.Repositories
{
    [Table("KmsDetails")]
    public partial class KmsDetails : EntityBase
    {
        public string KmsId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; } = "";

        public string FileGuidName { get; set; } = "";

        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; } = "";

        /// <summary>
        /// 类型 file，url
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 数据数量
        /// </summary>
        public int? DataCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ImportKmsStatus? Status { get; set; } = ImportKmsStatus.Loadding;
    }
}