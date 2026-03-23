# Validate NuGet CPM in Opplat

## When to use this
Use this when Opplat switches package versions to central management and restore/build starts failing or warning because project files still carry versions or the local machine has extra package feeds.

## Pattern
1. Keep ManagePackageVersionsCentrally and all PackageVersion items in Directory.Packages.props.
2. Remove Version= attributes from every .csproj PackageReference that should participate in CPM.
3. Add a repo-local NuGet.Config with only the package sources the repo actually needs so machine-level feeds do not trigger NU1507.
4. Validate with both dotnet build .\opplat.slnx -m:1 -v minimal and dotnet build .\opplat.slnx -t:Rebuild -m:1 -v minimal because incremental builds can hide the real remaining warning set.
5. Re-run dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj -m:1 -v minimal and update source-contract tests if the maintenance wave intentionally moved project ownership.

## Why it matters
CPM mistakes fail restore early with NU1008, while inherited user feeds create noisy NU1507 warnings that are unrelated to repository code. The rebuild step is important because it reveals the true residual warning debt after CPM is fixed, which is what Bishop needs to report during maintenance validation.
