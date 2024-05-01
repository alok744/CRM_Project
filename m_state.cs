using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Loginpage_project.Models
{
    public class m_state
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int state_id { get; set; }
        public int country_id { get; set; }
        public string state_name { get; set; }
        [ForeignKey("country_id")]
        public m_country m_country { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public bool del_status { get; set; }

    }
}
