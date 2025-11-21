using Microsoft.EntityFrameworkCore;
using CMCSProject.Models;

namespace CMCSProject.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
			: base(options) { }

		public DbSet<Claim> Claims { get; set; }
	}
}
