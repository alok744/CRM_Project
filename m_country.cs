using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loginpage_project.Models
{
    public class m_country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int country_id { get; set; }
        public string country_name { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public bool del_status { get; set; }
    }
}
