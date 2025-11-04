using System.ComponentModel.DataAnnotations.Schema;

namespace CosmosDbThirdPartySeed.Models
{
    public class Iswc
    {
        [Column("Iswc")]
        public string IswcCode { get; set; }
    }
}
