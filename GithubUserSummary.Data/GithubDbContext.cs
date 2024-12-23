using GithubUserSummary.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace GithubUserSummary.Data;

public class GithubDbContext : DbContext
{
    public GithubDbContext(DbContextOptions<GithubDbContext> options) : base(options) { }

    public DbSet<Commit> Commits { get; set; } = null!;
}
