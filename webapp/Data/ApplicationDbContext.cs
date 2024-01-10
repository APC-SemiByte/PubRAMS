using Microsoft.EntityFrameworkCore;
using webapp.Helpers;
using webapp.Models;

namespace webapp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public virtual DbSet<Group> Group { get; set; } = null!;
    public virtual DbSet<LookupGroup> LookupGroup { get; set; } = null!;
    public virtual DbSet<LookupRole> LookupRole { get; set; } = null!;
    public virtual DbSet<LookupTag> LookupTag { get; set; } = null!;
    public virtual DbSet<ProjectInfo> ProjectInfo { get; set; } = null!;
    public virtual DbSet<ProjectState> ProjectState { get; set; } = null!;
    public virtual DbSet<Role> Role { get; set; } = null!;
    public virtual DbSet<Staff> Staff { get; set; } = null!;
    public virtual DbSet<Student> Student { get; set; } = null!;
    public virtual DbSet<Tag> Tag { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            _ = optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }
}


