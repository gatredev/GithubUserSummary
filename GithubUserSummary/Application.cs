using GithubUserSummary.Data;
using GithubUserSummary.Data.Entity;
using GithubUserSummary.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GithubUserSummary;

internal class Application
{
    private readonly ILogger<Application> _logger;
    private readonly IDistributedCache _cache;
    private readonly GithubSynchronizationService _githubSynchronizationService;
    private readonly GithubDbContext _dbContext;

    public Application(ILogger<Application> logger,
        IDistributedCache cache,
        GithubSynchronizationService githubSynchronizationService,
        GithubDbContext dbContext)
    {
        _logger = logger;
        _cache = cache;
        _githubSynchronizationService = githubSynchronizationService;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Main method for console app
    /// </summary>
    public async Task RunAsync()
    {
        var args = Environment.GetCommandLineArgs();
        if (args?.Count() != 3)
        {
            _logger.LogError("Usage: dotnet run <username> <repo>");
            return;
        }

        var username = args[1];
        var repo = args[2];

        await SynchronizeCommitsAsync(username, repo);

        var commitsToDisplay = _dbContext.Commits
            .AsNoTracking()
            .Where(c => c.Username == username)
            .Where(i => i.Repository == repo)
            .ToList();

        DisplayList(commitsToDisplay);
    }

    private async Task SynchronizeCommitsAsync(string username, string repository)
    {
        var cacheKey = $"AllGithubCommits:{username}:{repository}";
        var dataSynced = _cache.GetString(cacheKey);
        if (dataSynced is null)
        {
            await _githubSynchronizationService.SynchronizeCommitsAsync(username, repository);
            _cache.SetString(cacheKey, true.ToString(), new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) });
        }
    }

    private void DisplayList(List<Commit> commits)
    {
        foreach (var commit in commits)
        {
            Console.WriteLine($"{commit.Repository}/{commit.Sha}: {commit.Message} [{commit.Committer ?? commit.Username}]");
        }
    }
}
