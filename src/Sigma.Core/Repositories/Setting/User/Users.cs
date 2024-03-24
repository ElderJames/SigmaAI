using Sigma.Core.Repositories.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sigma.Core.Repositories
{
    [Table("Users")]
    public partial class Users : EntityBase
    {
        /// <summary>
        /// 工号，用于登陆
        /// </summary>
        [Required]
        public string No { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Required]
        public string Describe { get; set; }

        /// <summary>
        /// 菜单权限
        /// </summary>
        [Required]
        public string MenuRole { get; set; }
    }
}