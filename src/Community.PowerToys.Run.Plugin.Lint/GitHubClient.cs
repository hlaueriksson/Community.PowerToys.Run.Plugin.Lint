using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Community.PowerToys.Run.Plugin.Lint;

public interface IGitHubClient
{
    Task<User?> GetUserAsync();
    Task<Repository?> GetRepositoryAsync();
    Task<Readme?> GetReadmeAsync();
    Task<Release?> GetLatestReleaseAsync();
}

public class GitHubClient : IGitHubClient
{
    public GitHubClient(GitHubOptions? options, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(options);

        Options = options;
        Logger = logger;
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.github.com"),
            Timeout = TimeSpan.FromSeconds(5),
        };
        HttpClient.DefaultRequestHeaders.Add("User-Agent", "Community.PowerToys.Run.Plugin.Lint");

        if (!string.IsNullOrEmpty(options.PersonalAccessToken))
        {
            // https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-fine-grained-personal-access-token
            HttpClient.DefaultRequestHeaders.Add("Authorization", $"token {options.PersonalAccessToken}");
        }
    }

    private GitHubOptions Options { get; }
    private HttpClient HttpClient { get; }
    private ILogger Logger { get; }

    public async Task<User?> GetUserAsync()
    {
        try
        {
            // https://docs.github.com/en/rest/users/users?apiVersion=2022-11-28#get-a-user
            return await HttpClient.GetFromJsonAsync<User>($"/users/{Options.Owner}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GetUserAsync failed.");
            return null;
        }
    }

    public async Task<Repository?> GetRepositoryAsync()
    {
        try
        {
            // https://docs.github.com/en/rest/repos/repos?apiVersion=2022-11-28#get-a-repository
            return await HttpClient.GetFromJsonAsync<Repository>($"/repos/{Options.Owner}/{Options.Repo}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GetRepositoryAsync failed.");
            return null;
        }
    }

    public async Task<Readme?> GetReadmeAsync()
    {
        try
        {
            // https://docs.github.com/en/rest/repos/contents?apiVersion=2022-11-28#get-a-repository-readme
            return await HttpClient.GetFromJsonAsync<Readme>($"/repos/{Options.Owner}/{Options.Repo}/readme");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GetReadmeAsync failed.");
            return null;
        }
    }

    public async Task<Release?> GetLatestReleaseAsync()
    {
        try
        {
            // https://docs.github.com/en/rest/releases/releases?apiVersion=2022-11-28#get-the-latest-release
            return await HttpClient.GetFromJsonAsync<Release>($"/repos/{Options.Owner}/{Options.Repo}/releases/latest");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GetLatestReleaseAsync failed.");
            return null;
        }
    }
}

public class GitHubOptions
{
    public string? PersonalAccessToken { get; set; }
    public string Owner { get; set; }
    public string Repo { get; set; }
}

/*
{
  "login": "octocat",
  "name": "monalisa octocat"
}
 */

public class User
{
    public string login { get; set; }
    public string name { get; set; }
}

/*
{
  "name": "Hello-World",
  "full_name": "octocat/Hello-World",
  "html_url": "https://github.com/octocat/Hello-World",
  "description": "This your first repo!",
  "topics": [
    "octocat",
    "atom",
    "electron",
    "api"
  ],
  "license": {
    "name": "MIT License"
  }
}
 */
public class Repository
{
    public string name { get; set; }
    public string full_name { get; set; }
    public string html_url { get; set; }
    public string description { get; set; }
    public string[] topics { get; set; }
    public License? license { get; set; }
}

public class License
{
    public string name { get; set; }
}

/*
{
  "type": "file",
  "encoding": "base64",
  "size": 5362,
  "name": "README.md",
  "path": "README.md",
  "content": "encoded content ...",
  "html_url": "https://github.com/octokit/octokit.rb/blob/master/README.md",
  "download_url": "https://raw.githubusercontent.com/octokit/octokit.rb/master/README.md"
}
 */

public class Readme
{
    public string type { get; set; }
    public string encoding { get; set; }
    public int size { get; set; }
    public string name { get; set; }
    public string path { get; set; }
    public string content { get; set; }
    public string html_url { get; set; }
    public string download_url { get; set; }
}

/*
{
  "html_url": "https://github.com/octocat/Hello-World/releases/v1.0.0",
  "id": 1,
  "tag_name": "v1.0.0",
  "name": "v1.0.0",
  "body": "Description of the release",
  "draft": false,
  "prerelease": false,
  "assets": [
    {
      "browser_download_url": "https://github.com/octocat/Hello-World/releases/download/v1.0.0/example.zip",
      "id": 1,
      "name": "example.zip",
      "label": "short description",
      "state": "uploaded",
      "content_type": "application/zip",
      "size": 1024
    }
  ]
}
 */

/// <summary>
/// Release of a GitHub repository.
/// </summary>
public class Release
{
    public string html_url { get; set; }
    public int id { get; set; }
    public string tag_name { get; set; }
    public string name { get; set; }
    public string body { get; set; }
    public bool draft { get; set; }
    public bool prerelease { get; set; }
    public Asset[] assets { get; set; }
}

/// <summary>
/// Release asset of a GitHub repository.
/// </summary>
public class Asset
{
    public string browser_download_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string label { get; set; }
    public string state { get; set; }
    public string content_type { get; set; }
    public int size { get; set; }
}
