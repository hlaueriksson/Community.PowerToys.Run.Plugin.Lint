using System.Text;
using FluentAssertions;
using Spectre.Console;

namespace Community.PowerToys.Run.Plugin.Lint.Tests
{
    public class RulesTests
    {
        [Test]
        public void ArgsRules_should_validate_Repository()
        {
            var subject = new ArgsRules(null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Args missing");

            subject = new ArgsRules([]);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Args missing");

            subject = new ArgsRules(["invalid"]);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Args missing: GitHubRepo | Path | PersonalAccessToken");

            subject = new ArgsRules(["github_pat_FOOBAR"]);
            subject.Validate().Clean().Should().BeEmpty();

            subject = new ArgsRules(["https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.Install"]);
            subject.Validate().Clean().Should().BeEmpty();

            subject = new ArgsRules([@"..\..\..\Packages\Valid-0.87.0-x64.zip"]);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public void RepoRules_should_validate_Repository()
        {
            var subject = new RepoRules(null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Repository missing");

            subject = new RepoRules(new());
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public void RepoDetailsRules_should_validate_Repository()
        {
            var subject = new RepoDetailsRules(null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Repository missing");

            subject = new RepoDetailsRules(new());
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Topic \"powertoys-run-plugin\" missing",
                "License missing");

            var repository = new Repository
            {
                topics = ["powertoys-run-plugin"],
                license = new License { name = "name" },
            };
            subject = new RepoDetailsRules(repository);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public void ReadmeRules_should_validate_Readme()
        {
            var subject = new ReadmeRules(null);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Readme missing");

            subject = new ReadmeRules(new());
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Installation instructions missing",
                "Usage instructions missing");

            var readme = new Readme
            {
                content = Encode("##Installation\n##Usage"),
            };
            subject = new ReadmeRules(readme);
            subject.Validate().Clean().Should().BeEmpty();

            static string Encode(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        [Test]
        public void ReleaseRules_should_validate_Release()
        {
            var subject = new ReleaseRules(null);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Release missing");

            subject = new ReleaseRules(new());
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Asset missing");

            subject = new ReleaseRules(new() { assets = [] });
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Asset \".zip\" missing");

            subject = new ReleaseRules(new() { assets = [new() { name = "plugin.json", content_type = "application/json" }] });
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Asset \".zip\" missing");

            subject = new ReleaseRules(new() { assets = [new() { name = "Valid-0.87.0.zip", content_type = "application/zip" }] });
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Asset \"arm64\" platform missing",
                "Asset \"x64\" platform missing");

            var release = new Release
            {
                assets =
                [
                    new() { name = "Valid-0.87.0-arm64.zip", content_type = "application/zip" },
                    new() { name = "Valid-0.87.0-x64.zip", content_type = "application/zip" },
                ],
            };
            subject = new ReleaseRules(release);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public void ReleaseNotesRules_should_validate_Release_and_Package()
        {
            var subject = new ReleaseNotesRules(null!, null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Release notes missing");

            subject = new ReleaseNotesRules(new(), null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Release notes missing");

            var release = new Release { body = "" };
            subject = new ReleaseNotesRules(release, null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Package missing");

            var package = new Package(@"..\..\..\Packages\Valid-0.87.0-x64.zip");
            subject = new ReleaseNotesRules(release, package);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Package missing");

            package.Load();
            subject = new ReleaseNotesRules(release, package);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public void PackageRules_should_validate_Package()
        {
            var subject = new PackageRules(null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Package missing");

            subject = new PackageRules(new(@"..\..\..\Community.PowerToys.Run.Plugin.Lint.Tests.csproj"));
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Filename does not match \"<name>-<version>-<platform>.zip\" convention");

            subject = new PackageRules(new(@"..\..\..\Packages\Valid-0.87.0-x64.zip"));
            subject.Validate().Clean().Should().BeEmpty();

            subject = new PackageRules(new(@"..\..\..\Packages\Valid.Name-0.82.1-X64.zip"));
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public void PackageContentRules_should_validate_Package()
        {
            var subject = new PackageContentRules(null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Package missing");

            subject = new PackageContentRules(new(@"..\..\..\Packages\Valid-0.87.0-x64.zip"));
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Package missing");

            var package = new Package(@"..\..\..\Packages\NoPluginFolder-0.82.1-x64.zip");
            package.Load();
            subject = new PackageContentRules(package);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Plugin folder missing");

            package = new Package(@"..\..\..\Packages\DupePluginFolders-0.82.1-x64.zip");
            package.Load();
            subject = new PackageContentRules(package);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Plugin folder missing",
                "Folder is empty");

            package = new Package(@"..\..\..\Packages\NoMetadata-0.82.1-x64.zip");
            package.Load();
            subject = new PackageContentRules(package);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Metadata \"plugin.json\" missing");

            package = new Package(@"..\..\..\Packages\NoAssembly-0.82.1-x64.zip");
            package.Load();
            subject = new PackageContentRules(package);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Assembly \".dll\" missing");

            package = new Package(@"..\..\..\Packages\Valid-0.87.0-x64.zip");
            package.Load();
            subject = new PackageContentRules(package);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public void PackageChecksumRules_should_validate_Package()
        {
            var subject = new PackageChecksumRules(null!, new(@"..\..\..\Packages\Valid-0.87.0-x64.zip"), []);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Release notes missing");

            subject = new PackageChecksumRules(new(), new(@"..\..\..\Packages\Valid-0.87.0-x64.zip"), []);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Release notes missing");

            subject = new PackageChecksumRules(new() { body = "" }, new(@"..\..\..\Packages\Valid-0.87.0-x64.zip"), []);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Package missing");

            var package = new Package(@"..\..\..\Packages\Valid-0.87.0-x64.zip");
            package.Load();
            subject = new PackageChecksumRules(new() { body = "" }, package, []);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Hash \"343F000FE24F6EB3A24E9D1A09D19280BC94204A65C640141007992D3C513CFD\" missing");

            subject = new PackageChecksumRules(new() { body = "" }, package, null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Hash \"343F000FE24F6EB3A24E9D1A09D19280BC94204A65C640141007992D3C513CFD\" missing");

            var release = new Release
            {
                body = "343F000FE24F6EB3A24E9D1A09D19280BC94204A65C640141007992D3C513CFD",
            };
            subject = new PackageChecksumRules(release, package, []);
            subject.Validate().Clean().Should().BeEmpty();

            release = new Release
            {
                body = "343f000fe24f6eb3a24e9d1a09d19280bc94204a65c640141007992d3c513cfd",
            };
            subject = new PackageChecksumRules(release, package, []);
            subject.Validate().Clean().Should().BeEmpty();

            var checksum = new Checksum("343F000FE24F6EB3A24E9D1A09D19280BC94204A65C640141007992D3C513CFD", "Valid-0.87.0-x64.zip");
            subject = new PackageChecksumRules(new() { body = "" }, package, [checksum]);
            subject.Validate().Clean().Should().BeEmpty();

            checksum = new Checksum("343f000fe24f6eb3a24e9d1a09d19280bc94204a65c640141007992d3c513cfd", "Valid-0.87.0-x64.zip");
            subject = new PackageChecksumRules(new() { body = "" }, package, [checksum]);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public void PluginMetadataRules_should_validate_Package()
        {
            var subject = new PluginMetadataRules(null!, new(), new());
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Package missing");

            var package = new Package(@"..\..\..\Packages\Valid-0.87.0-x64.zip");
            package.Load();
            subject = new PluginMetadataRules(package, null!, new());
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Repository missing");

            package = new Package(@"..\..\..\Packages\NoMetadata-0.82.1-x64.zip");
            package.Load();
            subject = new PluginMetadataRules(package, new(), new());
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Package missing");

            package = new Package(@"..\..\..\Packages\Valid-0.87.0-x64.zip");
            package.Load();
            SetMetadata(package, new Metadata
            {
                ID = "",
                ActionKeyword = "=",
                Name = "",
                Author = "",
                Version = "",
                Website = "",
                ExecuteFileName = "Invalid.dll",
                IcoPathDark = "Images\\invalid.dark.png",
                IcoPathLight = "Images\\invalid.light.png",
                DynamicLoading = true,
            });
            var repository = new Repository
            {
                html_url = "https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.Valid",
            };
            subject = new PluginMetadataRules(package, repository, new());
            subject.Validate().Clean().Should().BeEquivalentTo(
                "ID is invalid",
                "ActionKeyword is not unique",
                "Name does not match plugin folder",
                "Author does not match GitHub user",
                "Version is invalid",
                "Version does not match filename version",
                "Website does not match repo URL",
                "ExecuteFileName missing in package",
                "ExecuteFileName does not match \"Community.PowerToys.Run.Plugin.<Name>.dll\" convention",
                "IcoPathDark missing in package",
                "IcoPathLight missing in package",
                "DynamicLoading is unnecessary");

            package = new Package(@"..\..\..\Packages\Dependencies-0.82.1-x64.zip");
            package.Load();
            var user = new User { login = "hlaueriksson" };
            subject = new PluginMetadataRules(package, repository, user);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "DynamicLoading is necessary for multiple assemblies");

            package = new Package(@"..\..\..\Packages\Valid-0.87.0-x64.zip");
            package.Load();
            subject = new PluginMetadataRules(package, repository, user);
            subject.Validate().Clean().Should().BeEmpty();

            package = new Package(@"..\..\..\Packages\ValidZipPathsWithSlash-0.82.1-x64.zip");
            package.Load();
            subject = new PluginMetadataRules(package, repository, user);
            subject.Validate().Clean().Should().BeEmpty();

            static void SetMetadata(Package package, Metadata metadata)
            {
                var property = package.GetType().GetProperty(nameof(Package.Metadata));
                property?.SetValue(package, metadata);
            }
        }

        [Test]
        public void PluginDependenciesRules_should_validate_Package()
        {
            var subject = new PluginDependenciesRules(null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Package missing");

            var package = new Package(@"..\..\..\Packages\Dependencies-0.82.1-x64.zip");
            package.Load();
            subject = new PluginDependenciesRules(package);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Unnecessary dependency: PowerToys.Common.UI.dll",
                "Unnecessary dependency: PowerToys.ManagedCommon.dll",
                "Unnecessary dependency: PowerToys.Settings.UI.Lib.dll",
                "Unnecessary dependency: Wox.Infrastructure.dll",
                "Unnecessary dependency: Wox.Plugin.dll",
                "Unnecessary debug dependency: PowerToys.Common.UI.pdb",
                "Unnecessary debug dependency: PowerToys.ManagedCommon.pdb",
                "Unnecessary debug dependency: PowerToys.Settings.UI.Lib.pdb",
                "Unnecessary debug dependency: Wox.Infrastructure.pdb",
                "Unnecessary debug dependency: Wox.Plugin.pdb",
                "Unnecessary dependency: Newtonsoft.Json, consider using System.Text.Json",
                "Unnecessary dependency: LazyCache, already defined in Central Package Management (Directory.Packages.props)",
                "Unnecessary Windows dependency: Microsoft.Windows.SDK.NET.dll",
                "Unnecessary Windows dependency: WinRT.Runtime.dll");

            package = new Package(@"..\..\..\Packages\Valid-0.87.0-x64.zip");
            package.Load();
            subject = new PluginDependenciesRules(package);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public void AssemblyRules_should_validate_Package()
        {
            var subject = new AssemblyRules(null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Assembly could not be validated");

            var package = new Package(@"..\..\..\Packages\InvalidTarget-0.82.1-x64.zip");
            package.Load();
            subject = new AssemblyRules(package);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Target framework should be \"net9.0\"",
                "Target platform should be \"windows\"",
                "Assembly version does not match metadata (plugin.json) Version",
                "Main.PluginID does not match metadata (plugin.json) ID");

            package = new Package(@"..\..\..\Packages\Valid-0.87.0-x64.zip");
            package.Load();
            subject = new AssemblyRules(package);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public async Task ProjectContentRules_should_validate_Project()
        {
            var subject = new ProjectContentRules(null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Project missing");

            var project = new Project(@"..\..\..\Projects\Invalid");
            await project.LoadAsync();
            subject = new ProjectContentRules(project);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Metadata \"plugin.json\" missing");

            project = new Project(@"..\..\..\Projects\Valid");
            await project.LoadAsync();
            subject = new ProjectContentRules(project);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test, Explicit("Fails in CI")]
        public async Task ProjectDependenciesRules_should_validate_Project()
        {
            var subject = new ProjectDependenciesRules(null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Project missing");

            // NOTE: Fails in CI
            var project = new Project(@"..\..\..\Projects\Invalid");
            await project.LoadAsync();
            subject = new ProjectDependenciesRules(project);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Unnecessary dependency: Newtonsoft.Json, consider using System.Text.Json",
                "Inconstant dependency version: LazyCache, use version \"2.4.0\" as defined in Central Package Management (Directory.Packages.props)");

            project = new Project(@"..\..\..\Projects\Valid");
            await project.LoadAsync();
            subject = new ProjectDependenciesRules(project);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public async Task ProjectMetadataRules_should_validate_Project()
        {
            var subject = new ProjectMetadataRules(null!, null!, null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Metadata missing");

            var repository = new Repository
            {
                html_url = "https://github.com/hlaueriksson/Valid",
            };
            var user = new User { login = "hlaueriksson" };

            var project = new Project(@"..\..\..\Projects\Invalid");
            await project.LoadAsync();
            subject = new ProjectMetadataRules(project, repository, user);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Metadata missing");

            project = new Project(@"..\..\..\Projects\Valid");
            await project.LoadAsync();
            subject = new ProjectMetadataRules(project, repository, user);
            subject.Validate().Clean().Should().BeEmpty();
        }

        [Test]
        public async Task ProjectRules_should_validate_Project()
        {
            var subject = new ProjectRules(null!);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Project missing");

            var project = new Project(@"..\..\..\Projects\Invalid");
            await project.LoadAsync();
            subject = new ProjectRules(project);
            subject.Validate().Clean().Should().BeEquivalentTo(
                "Target framework should be \"net9.0\"",
                "Target platform should be \"windows\"");

            project = new Project(@"..\..\..\Projects\Valid");
            await project.LoadAsync();
            subject = new ProjectRules(project);
            subject.Validate().Clean().Should().BeEmpty();
        }
    }

    file static class RulesTestsExtensions
    {
        public static IEnumerable<string> Clean(this IEnumerable<string> messages) => messages.Select(Markup.Remove);
    }
}
