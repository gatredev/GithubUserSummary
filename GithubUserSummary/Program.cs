using GithubUserSummary;
using GithubUserSummary.Configuration;
using GithubUserSummary.Data;
using GithubUserSummary.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeoSmart.Caching.Sqlite;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTransient<Application>();
builder.Services.AddOptions<GithubOptions>().BindConfiguration("Github");
builder.Services.AddScoped<GithubApiService>();
builder.Services.AddScoped<GithubSynchronizationService>();
builder.Services.AddDbContext<GithubDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
// TODO: Replace with redis if required
builder.Services.AddSqliteCache(options =>
{
    options.CachePath = builder.Configuration.GetConnectionString("Cache");
}, default);

using var host = builder.Build();

await RunOnStartup(host);

static async Task RunOnStartup(IHost host)
{
    using var scope = host.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;

    ApplyMigrations(serviceProvider);

    var app = serviceProvider.GetRequiredService<Application>();
    await app.RunAsync();
}

static void ApplyMigrations(IServiceProvider serviceProvider)
{
    var dbContext = serviceProvider.GetRequiredService<GithubDbContext>();
    dbContext.Database.Migrate();
}