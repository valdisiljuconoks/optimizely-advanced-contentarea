---
name: release-package
description: Bump the NuGet version of both AdvancedContentArea libraries in lockstep and pack them into .nuget/. Stops before the manual push.
disable-model-invocation: true
---

Target version: `$ARGUMENTS` (e.g. `6.0.2`). If empty, ask for it before doing anything.

Run every command from the repository root.

1. **Check the tree is clean** — `git status --short`. If anything is uncommitted, stop and report instead of packing a dirty build.

2. **Bump both libraries in lockstep.** The Forms package takes a NuGet dependency on the main package at the
   same version, so a mismatch ships a broken graph. Set all four properties in each file:

   | File | Properties |
   |---|---|
   | `src/AdvancedContentArea/AdvancedContentArea.csproj` | `<Version>`, `<PackageVersion>` = `X.Y.Z`; `<AssemblyVersion>`, `<FileVersion>` = `X.Y.Z.0` |
   | `src/AdvancedContentArea.Forms/AdvancedContentArea.Forms.csproj` | same four, same values |

   Do not touch the Tests project — it is not packed.

3. **Run the tests** — `dotnet test tests/AdvancedContentArea.Tests/AdvancedContentArea.Tests.csproj`. Stop on failure.

4. **Clear stale packages** — `build-packages.ps1` copies `bin\Release\*.nupkg` wholesale, so old versions left in
   `bin\Release` get dragged into `.nuget\` too:

   ```powershell
   Remove-Item src\AdvancedContentArea\bin\Release\*.nupkg, src\AdvancedContentArea\bin\Release\*.snupkg, `
               src\AdvancedContentArea.Forms\bin\Release\*.nupkg, src\AdvancedContentArea.Forms\bin\Release\*.snupkg `
               -ErrorAction SilentlyContinue
   ```

5. **Pack** — `./build-packages.ps1` (it `cd`s around relative to the root and returns there).

6. **Verify** exactly these four files appeared in `.nuget\`:
   - `TechFellow.Optimizely.AdvancedContentArea.X.Y.Z.nupkg` + `.snupkg`
   - `TechFellow.Optimizely.AdvancedContentArea.Forms.X.Y.Z.nupkg` + `.snupkg`

   If any is missing or carries the wrong version, stop and report — do not "fix" it by hand.

7. **Commit** the two csproj changes as `chore: release X.Y.Z`. Leave the `.nuget/` artifacts staged or not per
   what the repo already does for the previous release — check `git log -- .nuget` before deciding.

8. **Stop here.** Pushing to nuget.org / the Optimizely feed is manual and out of scope for this skill. Report the
   four file paths and say the push has not been done.
