using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miki.Framework.Commands.Scopes.Models
{
    [Table("Scopes")]
	public class Scope
	{
        [Column("UserId")]
		public long UserId { get; set; }

        [Column("ScopeId")]
        public string ScopeId { get; set; }
	}
}
