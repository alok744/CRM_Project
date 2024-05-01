using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Loginpage_project.Models
{
    public class m_city
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int city_id { get; set; }
        public int state_id { get; set; }
        public string city_name { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public bool del_status { get; set; }
        [ForeignKey("state_id")]
        public m_state m_state { get; set; }
    }
}
