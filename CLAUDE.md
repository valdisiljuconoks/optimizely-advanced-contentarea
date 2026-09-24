# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

Two NuGet libraries that replace Optimizely CMS `ContentAreaRenderer` with a Bootstrap-aware one:
`TechFellow.Optimizely.AdvancedContentArea` (`src/AdvancedContentArea`) and its EPiServer.Forms add-on
(`src/AdvancedContentArea.Forms`). `samples/AlloySampleSite` is the manual test bed.

## Commands

```powershell
dotnet build
dotnet test tests/AdvancedContentArea.Tests/AdvancedContentArea.Tests.csproj
./build-packages.ps1   # builds + packs both libs (nupkg + snupkg) into .nuget/
```

Running the sample site (DB create step + first-run flow): `samples/AlloySampleSite/README.md`.

`tests/AdvancedContentArea.SampleWeb` is a dead .NET Framework 4.6.1 leftover from the EPiBootstrapArea era.
It is not in the solution — never build, edit, or reference it.

## Language ceiling

net10.0, no multi-targeting, no `Directory.Build.props`. `LangVersion` is the net10.0 default (**C# 14**).
Nullable reference types and implicit usings are **off** — write explicit `using` blocks and no `?` annotations.
The existing code is still written to C# 10 idioms; don't churn it, but new code may use newer features.

## Style (what differs from defaults)

- Allman braces (`csharp_new_line_before_open_brace = all`), 4-space indent.
- `var` everywhere, including built-in types. No `this.` qualification.
- Block bodies for methods and constructors; expression bodies are fine for properties and accessors.
- Instance fields `_camelCase`; file-scoped namespaces.
- Every source file opens with the two-line Apache-2.0 header (`// Copyright (c) Valdis Iljuconoks. All rights reserved.`).
- Nothing is build-enforced: no analyzers, no `TreatWarningsAsErrors`, `.editorconfig` rules are `suggestion` severity.

## Integration points (easy to get wrong)

- There is **no `IInitializableModule`**. The single entry point is `services.AddAdvancedContentArea(...)` in
  `src/AdvancedContentArea/Initialization/IServiceCollectionExtensions.cs`. Setting `ItemStartRenderCallback`
  switches that registration from a type mapping to a factory lambda — keep both branches in sync.
- Consumers replacing the renderer must use EPiServer's `.Intercept<ContentAreaRenderer>(...)`; a plain
  re-registration is silently overridden.
- `AdvancedContentArea.Forms` resolves `ContentAreaRenderer` from `ServiceLocator.Current` and **silently falls
  back** to a default renderer when it isn't ours — misconfiguration shows up as lost customization, not an
  exception (`src/AdvancedContentArea.Forms/FormsHtmlHelperExtensions.cs`).
- `AdvancedContentAreaRenderer` buffers each block's HTML and re-parses it with HtmlAgilityPack to inject the
  wrapper element. That whole path is **skipped in edit mode** (`IsInEditMode()`), so edit view and public
  rendering differ by design.
- CMS 13 made `ContentArea.FilteredItems` a no-op passthrough for `Items`; filtering (access rights,
  personalization) now lives in `IContentAreaItemsRenderingFilter`. `Render` resolves those filters from
  `HttpContext.RequestServices` and applies them itself — the base renderer keeps its own copy private.
- `ContentAreaItem.GetContent()` is gone in CMS 13; use `ContentAreaItem.LoadContent()`, which returns
  `IContentData` (an inline block is not an `IContent`).
- CMS 13's base renderer wraps rendering in an internal `NewViewDataScope` that resets `tag`/`cssclass`/
  `customtag`/`childrencssclass`/`childrencustomtagname` per property. Our `Render` override does not — it reads
  `htmlHelper.ViewData["tag"]` directly, as it did on CMS 12, so nested content areas can still inherit the
  outer area's tag.
- `EPiServer.Forms` 6.x no longer self-registers; consumers must call `services.AddForms()`.
- CMS 13 tag helper views (`<div epi-property="@Model.Area">`) never reach `ContentAreaRenderer` - they go
  through `IPropertyRenderer`. `AdvancedContentAreaPropertyRenderer` (registered by `AddAdvancedContentArea`,
  `[PropertyRenderer(typeof(ContentArea), 100)]`) takes that path over and delegates to the renderer by
  contextualizing an `IHtmlHelper` on the current `ViewContext`. Sort order must stay non-zero and unique -
  the built-in renderer is 0 and the resolver throws on a tie.
- In tag helper mode the host element is the wrapper, so the property renderer forces `hascontainer = false`
  and lifts render settings (`rowsupport`, `rowcssclass`, ..) off the element's attributes into view data.
  An `epi-property-item` child template makes it bow out to the stock CMS 13 pipeline.
- Row support is per-call opt-in: `@Html.PropertyFor(x => x.Area, new { rowsupport = true })`. The global
  `RowSupportEnabled` option alone does nothing.
- `GetCssClassesForItem` is `internal static` so tests can call it without DI; `InternalsVisibleTo` for the
  Tests and Forms assemblies is declared in `AdvancedContentArea.csproj`.
- `docs/episerver-forms.md` is historical (manual patching of shipped Forms templates). It is superseded by the
  `AdvancedContentArea.Forms` package — do not follow it.

## Releases

Version lives only in each library's `.csproj` (`<Version>`, `<PackageVersion>`, `<AssemblyVersion>`,
`<FileVersion>`) — no GitVersion, and git tags stopped at the pre-rename 4.1. Bump both libraries in lockstep,
run `build-packages.ps1`, then push manually; `/release-package` does everything up to the push.
`appveyor.yml` and the README CI badge still point at the old `epibootstraparea` slug and are not a live pipeline.

## Git

Feature work on `feature/<name>`; small changes may go straight to `master`. Conventional commit subjects
(`feat:`, `fix:`, …).

## Current state

`master` targets Optimizely CMS 12 / net6.0 (package v6.x). The active branch `feature/cms13` targets
**Optimizely CMS 13.1.3 on net10.0** (package v7.0.0) — libraries, tests and the Alloy sample all build, the
test suite passes, and the sample has been run against LocalDB with the renderer verified in the output.
