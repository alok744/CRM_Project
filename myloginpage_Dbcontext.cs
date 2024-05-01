using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Loginpage_project.Models
{
    public class myloginpage_Dbcontext : DbContext
    {
        public myloginpage_Dbcontext(DbContextOptions<myloginpage_Dbcontext> options): base(options)
        {

        }
        public DbSet<Loginpage> Loginpage { get; set; }
        public DbSet<m_country> m_country { get; set; }
        public DbSet<m_state> m_state { get; set; }
        public DbSet<m_city> m_city { get; set; }
           
    }
}
