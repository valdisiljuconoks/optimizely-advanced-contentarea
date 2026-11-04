# AlloySampleSite

Alloy on Optimizely CMS 13 / .NET 10, wired up with `TechFellow.Optimizely.AdvancedContentArea` (and the
`.Forms` add-on) as the content area renderer. It is the manual test bed for the two libraries - `StartPage`
renders one content area per feature (rows, default display options, block index, skipped wrappers, start-tag
callback).

## Run it

One-time: create the (empty) database. CMS creates the schema itself on first start, and imports
`App_Data/DefaultSiteContent.episerverdata` on the first request.

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "IF DB_ID('AlloySampleSite') IS NULL CREATE DATABASE [AlloySampleSite];"
dotnet run
```

Then open http://localhost:8000 - the first request redirects to `/Util/Register` to create the admin user.

The last sections of `Views/StartPage/Index.cshtml` render the same content area four ways -
`Html.PropertyFor`, the CMS 13 `epi-property` tag helper, the tag helper with render settings as attributes,
and the tag helper with a hand written `epi-property-item` template - so the markup can be compared directly.

Use a different SQL Server by overriding `ConnectionStrings:EPiServerDB` in `appsettings.Development.json`.

To start over, drop the database and delete `App_Data/blobs`.
