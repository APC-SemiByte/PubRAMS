using Microsoft.EntityFrameworkCore;
using webapp.Helpers;
using webapp.Models;

namespace webapp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public virtual DbSet<Course> Course { get; set; } = null!;
    public virtual DbSet<Group> Group { get; set; } = null!;
    public virtual DbSet<Project> Project { get; set; } = null!;
    public virtual DbSet<ProjectTag> ProjectTag { get; set; } = null!;
    public virtual DbSet<Role> Role { get; set; } = null!;
    public virtual DbSet<School> School { get; set; } = null!;
    public virtual DbSet<Staff> Staff { get; set; } = null!;
    public virtual DbSet<StaffRole> StaffRole { get; set; } = null!;
    public virtual DbSet<State> State { get; set; } = null!;
    public virtual DbSet<Student> Student { get; set; } = null!;
    public virtual DbSet<StudentGroup> StudentGroup { get; set; } = null!;
    public virtual DbSet<Subject> Subject { get; set; } = null!;
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
            _ = optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Disable cascading delete
        // Required bc we have constants that refer to each other
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        base.OnModelCreating(modelBuilder);
        SeedHelper.Seed(modelBuilder, developerMode: true);
    }
}

