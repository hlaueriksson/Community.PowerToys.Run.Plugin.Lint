using System.Text.RegularExpressions;

namespace Community.PowerToys.Run.Plugin.Lint;

public static partial class Extensions
{
    public static GitHubOptions GetGitHubOptions(this string? url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        var result = new GitHubOptions();
        var match = GitHubRegex().Match(url);
        if (match.Success)
        {
            result.Owner = match.Groups[1].Value;
            result.Repo = match.Groups[2].Value;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(result.Owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(result.Repo);

        return result;
    }

    public static string? GetEmbeddedResourceContent(this string name)
    {
        using var stream = typeof(Extensions).Assembly.GetManifestResourceStream(typeof(Extensions).Namespace + "." + name);
        if (stream == null) return null;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    [GeneratedRegex(@"https:\/\/github.com\/([^\/]+)\/([^\/]+)$")]
    private static partial Regex GitHubRegex();
}
