# Community.PowerToys.Run.Plugin.Lint

[![build](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.Lint/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.Lint/actions/workflows/build.yml)
[![Snyk Security Scan](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.Lint/actions/workflows/snyk.yml/badge.svg)](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.Lint/actions/workflows/snyk.yml)
[![Community.PowerToys.Run.Plugin.Lint](https://img.shields.io/nuget/v/Community.PowerToys.Run.Plugin.Lint.svg?label=Community.PowerToys.Run.Plugin.Lint)](https://www.nuget.org/packages/Community.PowerToys.Run.Plugin.Lint)
[![Mentioned in Awesome PowerToys Run Plugins](https://awesome.re/mentioned-badge.svg)](https://github.com/hlaueriksson/awesome-powertoys-run-plugins)

> Linter for PowerToys Run community plugins

This is a `dotnet` tool that requires:

- [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## Installation

Install:

```cmd
dotnet tool install -g Community.PowerToys.Run.Plugin.Lint
```

The tool is installed as `ptrun-lint.exe` in `%UserProfile%\.dotnet\tools`.

Update:

```cmd
dotnet tool update -g Community.PowerToys.Run.Plugin.Lint
```

Uninstall:

```cmd
dotnet tool uninstall -g Community.PowerToys.Run.Plugin.Lint
```

## Usage

```cmd
ptrun-lint <url>
ptrun-lint <path>
ptrun-lint <pat>
```

Arguments:

- `<url>` - An URL to a GitHub repo
- `<path>` - A path to a `zip` package or `dotnet` project on the filesystem
- `<pat>` - A GitHub Personal Access Token

Logs are written to a file, `ptrun-lint.log`, in the same directory as the tool was executed.

### Repository

Validates a GitHub repository.

Example:

```cmd
ptrun-lint https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.Install
```

During linting, GitHub release assets are downloaded to `%LocalAppData%\Temp`.
After linting is completed, downloaded files are deleted.

### Package

Validates a package on the local filesystem.

Examples:

```cmd
ptrun-lint c:\Users\Henrik\Downloads\Install-0.1.0-x64.zip
ptrun-lint Install-0.1.0-x64.zip
```

### Project

Validates a project on the local filesystem.

Examples:

```cmd
ptrun-lint c:\work\GitHub\Community.PowerToys.Run.Plugin.Install\src\Community.PowerToys.Run.Plugin.Install\
ptrun-lint .
```

### PersonalAccessToken

Saves a GitHub Personal Access Token to the `appsettings.json` file.

Examples:

```cmd
ptrun-lint github_pat_FOOBAR
ptrun-lint ghp_FOOBAR
```

This will increase the GitHub REST API rate limit.

Location:

- `%UserProfile%\.dotnet\tools\.store\community.powertoys.run.plugin.lint\<version>\community.powertoys.run.plugin.lint\<version>\tools\net9.0\any\appsettings.json`

Documentation:

- [Creating a fine-grained personal access token](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-fine-grained-personal-access-token)
- [Rate limits for the REST API](https://docs.github.com/en/rest/using-the-rest-api/rate-limits-for-the-rest-api?apiVersion=2022-11-28)

## Rules

| Code | Description |
| --- | --- |
| `PTRUN0001` | Args should be valid |
| | Args missing |
| | Args missing: GitHubRepo \| Path \| PersonalAccessToken |
| `PTRUN1001` | Repo should be valid |
| | Repository missing |
| `PTRUN1002` | Repo details should be valid |
| | Repository missing |
| | Topic "`powertoys-run-plugin`" missing |
| | License missing |
| `PTRUN1101` | Readme should be valid |
| | Readme missing |
| | Installation instructions missing |
| | Usage instructions missing |
| `PTRUN1201` | Release should be valid |
| | Release missing |
| | Asset missing |
| | Asset "`.zip`" missing |
| | Asset "`arm64`" platform missing |
| | Asset "`x64`" platform missing |
| `PTRUN1202` | Release notes should be valid (`<package>`) |
| | Release notes missing |
| | Package missing |
| `PTRUN1301` | Package should be valid (`<package>`) |
| | Package missing |
| | Filename does not match "`<name>-<version>-<platform>.zip`" convention |
| `PTRUN1302` | Package content should be valid (`<package>`) |
| | Package missing |
| | Plugin folder missing |
| | Folder is empty |
| | Metadata "`plugin.json`" missing |
| | Assembly "`.dll`" missing |
| `PTRUN1303` | Package checksum should be valid (`<package>`) |
| | Release notes missing |
| | Package missing |
| | Hash "`<hash>`" missing |
| `PTRUN1401` | Plugin metadata should be valid (`<package>`) |
| | Package missing |
| | Repository missing |
| | User missing |
| | ID is invalid |
| | ActionKeyword is not unique |
| | Name does not match plugin folder |
| | Author does not match GitHub user |
| | Version is invalid |
| | Version does not match filename version |
| | Website does not match repo URL |
| | ExecuteFileName missing in package |
| | ExecuteFileName does not match "`Community.PowerToys.Run.Plugin.<Name>.dll`" convention |
| | IcoPathDark missing in package |
| | IcoPathLight missing in package |
| | DynamicLoading is unnecessary |
| | DynamicLoading is necessary for multiple assemblies |
| `PTRUN1402` | Package dependencies should be valid (`<package>`) |
| | Package missing |
| | Unnecessary dependency: `PowerToys.Common.UI.dll` |
| | Unnecessary dependency: `PowerToys.ManagedCommon.dll` |
| | Unnecessary dependency: `PowerToys.Settings.UI.Lib.dll` |
| | Unnecessary dependency: `Wox.Infrastructure.dll` |
| | Unnecessary dependency: `Wox.Plugin.dll` |
| | Unnecessary debug dependency: `PowerToys.Common.UI.pdb` |
| | Unnecessary debug dependency: `PowerToys.ManagedCommon.pdb` |
| | Unnecessary debug dependency: `PowerToys.Settings.UI.Lib.pdb` |
| | Unnecessary debug dependency: `Wox.Infrastructure.pdb` |
| | Unnecessary debug dependency: `Wox.Plugin.pdb` |
| | Unnecessary dependency: `Newtonsoft.Json`, consider using `System.Text.Json` |
| | Unnecessary dependency: `<dependency>`, already defined in Central Package Management (`Directory.Packages.props`) |
| | Unnecessary Windows dependency: `Microsoft.Windows.SDK.NET.dll` |
| | Unnecessary Windows dependency: `WinRT.Runtime.dll` |
| `PTRUN1501` | Plugin assembly should be valid (`<package>`) |
| | Assembly could not be validated |
| | Target framework should be "`net9.0`" |
| | Target platform should be "`windows`" |
| | Assembly version does not match metadata (`plugin.json`) Version |
| | Main.PluginID does not match metadata (`plugin.json`) ID |
| `PTRUN2001` | Project content should be valid (`<project>`) |
| | Project missing |
| | Metadata "`plugin.json`" missing |
| `PTRUN2101` | Project dependencies should be valid (`<project>`) |
| | Project missing |
| | Unnecessary dependency: `Newtonsoft.Json`, consider using `System.Text.Json` |
| | Inconstant dependency version: `<dependency>`, use version `<version>` as defined in Central Package Management (`Directory.Packages.props`) |
| `PTRUN2201` | Project metadata should be valid (`<project>`) |
| | Metadata missing |
| | Repository missing |
| | User missing |
| | ID is invalid |
| | ActionKeyword is not unique |
| | Author does not match GitHub user |
| | Version is invalid |
| | Website does not match repo URL |
| | ExecuteFileName missing in package |
| | ExecuteFileName does not match "`Community.PowerToys.Run.Plugin.<Name>.dll`" convention |
| | IcoPathDark missing in package |
| | IcoPathLight missing in package |
| `PTRUN2301` | Project should be valid (`<project>`) |
| | Project missing |
| | Symbols missing |
| | Target framework should be "`net9.0`" |
| | Target platform should be "`windows`" |
| | Main.PluginID does not match metadata (`plugin.json`) ID |

## Package Checksum

The rule `PTRUN1303` can fail with:

- Hash "`<hash>`" missing

This can be fixed by including:

- Installer hashes in the release notes
- A checksums file in the release assets

Generate markup snippets with installer hashes:

- Run the `releasenotes.ps1` script from [Community.PowerToys.Run.Plugin.Templates](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.Templates)

Example:

| Filename | SHA256 Hash
| --- | ---
| `Install-0.1.0-arm64.zip` | `CB03EF645F24248F9618AF18E82D7BA7127F209AB1BE77701FC6183FDC952037`
| `Install-0.1.0-x64.zip` | `E0DB7B8A98D0891B97AAF85A68D340EDDCDF09389537BA64401CF8D786746EC5`

Generate a checksums file and attach it to the release as an asset:

- Use the GitHub action from [wangzuo/action-release-checksums](https://github.com/wangzuo/action-release-checksums)

The checksums file must:

- Have filename: `checksums.txt`
- Use method: `SHA256`

Example:

```txt
CB03EF645F24248F9618AF18E82D7BA7127F209AB1BE77701FC6183FDC952037 Install-0.1.0-arm64.zip
E0DB7B8A98D0891B97AAF85A68D340EDDCDF09389537BA64401CF8D786746EC5 Install-0.1.0-x64.zip
```

## Disclaimer

This is not an official Microsoft PowerToys tool.
