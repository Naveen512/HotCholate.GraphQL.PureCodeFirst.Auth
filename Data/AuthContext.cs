using GraphQL.PureCodeFirst.Auth.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.PureCodeFirst.Auth.Data
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
    }
}