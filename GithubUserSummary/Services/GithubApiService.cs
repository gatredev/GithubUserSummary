using GithubUserSummary.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Octokit;
using System.Text.Json;

namespace GithubUserSummary.Services;

internal class GithubApiService
{
    private readonly GithubOptions _githubOptions;

    public GithubApiService(IOptions<GithubOptions> githubOptions)
    {
        _githubOptions = githubOptions.Value;
    }

    public async Task<IReadOnlyList<GitHubCommit>> GetAllCommitsAsync(string username, string repository)
    {
        var githubClient = new GitHubClient(new ProductHeaderValue("CommitView"));
        if (!string.IsNullOrWhiteSpace(_githubOptions.Token))
        {
            githubClient.Credentials = new Credentials(_githubOptions.Token);
        }

        try
        {
            Console.WriteLine("Github data Synchronization. It may take a while...");

            var commits = await githubClient.Repository.Commit.GetAll(username, repository);
            return commits;
        }
        catch
        {
            var info = githubClient.GetLastApiInfo();
            Console.WriteLine($"RateLimit.Limit: {info?.RateLimit?.Limit}");
            Console.WriteLine($"RateLimit.Remaining: {info?.RateLimit?.Remaining}");
            Console.WriteLine($"RateLimit.Reset: {info?.RateLimit?.Reset}");

            throw;
        }
    }
}
