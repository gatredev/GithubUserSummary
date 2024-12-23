using GithubUserSummary.Data;
using GithubUserSummary.Data.Entity;
using Microsoft.Extensions.Logging;
using Octokit;

namespace GithubUserSummary.Services;

internal class GithubSynchronizationService
{
    private readonly ILogger<GithubSynchronizationService> _logger;
    private readonly GithubApiService _githubApiService;
    private readonly GithubDbContext _dbContext;

    public GithubSynchronizationService(ILogger<GithubSynchronizationService> logger,
        GithubApiService githubApiService,
        GithubDbContext dbContext)
    {
        _logger = logger;
        _githubApiService = githubApiService;
        _dbContext = dbContext;
    }

    public async Task SynchronizeCommitsAsync(string username, string repository)
    {
        try
        {
            _logger.LogInformation($"Synchronizing commits for username: {username} repository: {repository}");
            var fetchedCommits = await _githubApiService.GetAllCommitsAsync(username, repository);

            var userRepoCommits = _dbContext.Commits
                .Where(i => i.Username == username)
                .Where(i => i.Repository == repository)
                .ToList();

            var commitsToAdd = fetchedCommits
                .ExceptBy(userRepoCommits.Select(i => i.Sha), i => i.Sha)
                .Select(i => new Data.Entity.Commit
                {
                    Committer = i.Committer?.Login,
                    Message = i.Commit.Message,
                    Repository = repository,
                    Sha = i.Sha,
                    Username = username,
                });

            await _dbContext.AddRangeAsync(commitsToAdd);

            _logger.LogInformation($"Saving commits for username: {username} repository: {repository}");
            await _dbContext.SaveChangesAsync();
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
