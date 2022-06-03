using System;
using Microsoft.EntityFrameworkCore;

namespace UserAssignmentRedone.Models
{
	public class UserDbContext : DbContext
	{
		public UserDbContext(DbContextOptions<UserDbContext> options)
			:base(options)
		{
		}

		public DbSet<User> Users { get; set; }


	}
}

