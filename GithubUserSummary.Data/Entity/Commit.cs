namespace GithubUserSummary.Data.Entity;

public class Commit
{
    public int Id { get; set; }
    public string Sha { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Repository { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Committer { get; set; }
}
