using System;
using FlubuCore.Context;
using FlubuCore.Context.Attributes.BuildProperties;
using FlubuCore.IO;
using FlubuCore.Scripting;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Versioning;

namespace BuildScript
{
    public class BuildScript : DefaultBuildScript
    {
        [FromArg("apiKey", "Nuget api key required for publishing nuget packages.")]
        public string NugetApiKey { get; set; }

        //// With attribute solution is stored in flubu session so it doesn't need to be defined in restore and build task.
        [SolutionFileName] 
        public string SolutionFileName { get; set; } = "{{SolutionFileName}}";

        //// BuildConfiguration is stored in flubu session so it doesn't need to be defined in build task and test tasks etc.
        [BuildConfiguration] 
        public string BuildConfiguration { get; set; } = "Release";

        //// Alternatively flubu supports fetching of build version out of the box with FetchBuildVersionFromFileTask or GitVersionTask. Just apply [FetchBuildVersionFromFile] or [GitVersion] attribute on property.
        [BuildVersion]
        public BuildVersion BuildVersion { get; set; } = new BuildVersion(new Version(1, 0, 0, 0));

        public FullPath OutputDir => RootDirectory.CombineWith("output");

        protected override void ConfigureTargets(ITaskContext context)
        {
            var clean = context.CreateTarget("Clean")
                .SetDescription("Clean's the solution.")
                .AddCoreTask(x => x.Clean()
                    .AddDirectoryToClean(OutputDir, true));

            var restore = context.CreateTarget("Restore")
                .SetDescription("Restore's nuget packages in all projects.")
                .AddCoreTask(x => x.Restore());

            var build = context.CreateTarget("Build")
                .SetDescription("Build's the solution.")
                .SetAsDefault()
                .DependsOn(clean)
                .DependsOn(restore)
                .AddCoreTask(x => x.Build()
                    .Version(BuildVersion.Version.ToString()));

            // Uncomment if your library contains tests otherwise remove this target
            //var tests = context.CreateTarget("Run.tests")
            //    .SetDescription("Run's all test's in the solution")
            //    .AddCoreTask(x => x.Test()
            //        .Project("Enter test project name.")
            //        .NoBuild());

            var pack = context.CreateTarget("Pack")
                .SetDescription("Prepare's nuget package.")
                .AddCoreTask(x => x.Pack()
                    .NoBuild()
                    .OutputDirectory(OutputDir));

            var nugetPackages = context.GetFiles(OutputDir, "*.nupkg");

            var nugetPush = context.CreateTarget("Nuget.publish")
                .SetDescription("Publishes nuget package.")
                .Must((c => !string.IsNullOrEmpty(NugetApiKey)))
                .DependsOn(pack)
                .ForEach(nugetPackages, (nugetPackagePath, target) =>
                {
                    target.AddCoreTask(x => x.NugetPush(nugetPackagePath)
                        .ServerUrl("https://www.nuget.org/api/v2/package")
                        .ApiKey(NugetApiKey));
                });

            context.CreateTarget("All")
                .SetDescription("Builds the solution and publishes nuget package.")
                .DependsOn(build) 
              // Uncomment if your library contains tests otherwise remove this dependency.
              //.DependsOn(tests)
                .DependsOn(nugetPush);
        }
    }
}