EPiBootstrapArea
================

Bootstrap aware EPiServer content area renderer. Provides easy way to register display options used to customize look and feel of the blocks inside a EPiServer content area.

## EPiServer versions Support

For EPiServer v10.x support please use `master` branch.
For EPiServer v9.x support please use `epi9` branch.

## List of Topics

* [Getting Started](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#getting-started)
* [Available Built-in Display Options](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#available-built-in-display-options)
* [Display Option Fallbacks](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#display-option-fallbacks)
* [Setup (Configuration)](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#available-configuration-options)
* [Support for EPiServer.Forms](https://github.com/valdisiljuconoks/EPiBootstrapArea/wiki/Support-for-EPiServer.Forms)
* [Advanced Features](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#advanced-features)
   * [Bootstrap Row Support](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#bootstrap-row-support)
   * [Validate Item Count to Match Bootstrap Columns](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#validate-item-count)
   * [Default DisplayOption for Block](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#default-displayoption-for-block)
   * [Default DisplayOption for Content Area](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#default-displayoption-for-content-area)
   * [Get Block Index in the ContentArea](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#get-block-index-in-the-contentarea)
* [Customize Bootstrap Content Area](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#customize-bootstrap-content-area)
    * [Provider Model](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#provider-model)
    * [Register Custom Provider](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#register-custom-provider)
    * [Customize Generated Css Classes](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#customize-generated-css-classes)
    * [Additional Styles](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#additional-styles)
    * [Localized Display Option Names](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#localized-display-option-names)
    * [Modify Block Start Element](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#modify-block-start-element)
    * [Skip Item Wrapper Element](https://github.com/valdisiljuconoks/EPiBootstrapArea/blob/master/README.md#skip-item-wrapper-element)

## Getting Started

You would need to install package from EPiServer's NuGet [feed](http://nuget.episerver.com/) to start using Twitter's Bootstrap aware EPiServer Content Area renderer:

```
PM> Install-Package EPiBootstrapArea
```

Default built-in display options are regsitered automatically. Below is described some built-in features.



## Available Built-in Display Options
Following display options are registered by default:
* Full width (`displaymode-full`)
* Half width (`displaymode-half`)
* One-third width (`displaymode-one-third`)
* Two-thirds width (`displaymode-two-thirds`)
* One-quarter width (`displaymode-one-quarter`)
* Three-quarter width (`displaymode-three-quarters`)

![](https://ruiorq.dm2304.livefilestore.com/y2pJ4-y8MWiBSk3Gmk_-7grHj7anXZMfEc6oyw9kbs_lZjjnXJiVWZGQRduzg25S0AblsZgDAXNdlfzlcZRd6KZtAiRtbhHT3GktV2osP8vD44/display-modes.png?psid=1)

### Display Option Fallbacks
For every display option there are 4 fallback width for various screen sizes based on Bootstrap grid system. According to Bootstrap v3 [specification](http://getbootstrap.com/css/#grid-options) following screen sizes are defined:
* Large screen (>= 1200px)
* Medium devices (>= 992px && < 1200px)
* Small devices (>= 768px && < 992px)
* Extra small devices (< 768px)

These numbers are added at the end of Bootstrap grid system class (for instance 12 for Large screen -> `'col-lg-12'`)

| Display Mode Name   | Extra small devices | Small devices | Medium devices | Large screen |
|---------------------|---------------------|---------------|----------------|--------------|
|Full width           |12                   |12             |12              |12            |
|Half width           |12                   |12             |6               |6             |
|One third            |12                   |12             |6               |4             |
|Two thirds           |12                   |12             |6               |8             |
|One quarter          |12                   |12             |6               |3             |
|Three quarters       |12                   |12             |6               |9             |


Eventually if you choose `Half-width` display option for a block of type `EditorialBlockWithHeader` following markup will be generated:

```xml
<div class="block editorialblockwithheader col-lg-6 col-md-6 col-sm-12 col-xs-12 displaymode-half">
    ...
</div>
```

Breakdown of added classes:
* `block` : generic class added to identify a block
* `{block-name}` : name of the block type is added (in this case `EditorialBlockWithHeader`)
* `col-xs-12` : block will occupy whole width of the screen on extra small devices
* `col-sm-12` : block will occupy whole width of the screen on small devices
* `col-md-6` : block will occupy one half of the screen on medium devices
* `col-lg-6` : block will occupy one half of the screen on desktop
* `displaymode-half` : chosen display option name is added

### Example
Let's take a look at `One quarter width` block.
This is a block layout in EPiServer content area on-page edit mode (desktop view - large screen `col-lg-3`):

![](https://ruiorq.dm2302.livefilestore.com/y2paqID-F3S4jadXB8_rQQ5mAquDiV7r0lPVUE8kv5FwBC_WeiqCoQkXnXf95tsH4yztnP-ab8BrrFvRAIAQwQ6Qk1P-hYQlN4weaKHYqLwUH4/one-qrt-1.png?psid=1)

This is a block layout in EPiServer content area on medium devices - `col-md-6`:

![](https://ruiorq.dm2304.livefilestore.com/y2pU33Adod14lNBoUHiRDA0AuXX21lElN5qpFE29wctS-xekkRWRPCU6gNN51JzuuZ7W6OU2V7iyUPlr2719g_MRCjrjwDV2Cp7OyLYQ828Pjw/one-qrt-2.PNG?psid=1)

This is a block layout in EPiServer content area on small and extra small devices - `col-sm-12` and `col-xs-12`:

![](https://ruiorq.dm2304.livefilestore.com/y2pbM5w0jTT9Y2yWK4-NPWjvlhMtPwvpwfpHN_GhuQknVqE77TooZp87lA5nfQ6n8Muz-aMQTcNxqGXnQdTOhZH96MG0l3Dbjtg_USObfEjrPo/one-qrt-3.png?psid=1)


### Available Configuration Options

There are few options you can set to content area renderer to customize its behavior:
* `AutoAddRow` - setting this to `true` will add `class='row'` to the content area wrapping element. Disabled by default;
* `RowSupportEnabled` - will add additional wrapping element (`<div class='row'>`) to wrap around blocks occupying whole (12 columns altogether) row. Disabled by default;
* `DisableBuiltinDisplayOptions` - built-in display options will not be registered. Instead - consumer application can register whatever and however display options needed.

You can customize content area renderer and set settings by instructing it via ConfigurationContext:

```csharp
namespace EPiBootstrapArea.SampleWeb.Business.Initialization
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CustomizedRenderingInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            ConfigurationContext.Setup(ctx =>
                                       {
                                           ctx.RowSupportEnabled = true;
                                           ctx.AutoAddRow = true;
                                           ctx.DisableBuiltinDisplayOptions = true;
                                       });
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}
```

## Advanced Features

### Bootstrap Row Support

If you need to support Boostrap row elements in Content Area, you can just render that area with `"rowsupport"` parameter:

```
@Html.PropertyFor(m => m.MainContentArea, new { rowsupport = true })
```

For every collection of elements that fill up 12 columns - additional element (`<div>`) will be wrapped around with `class="row"`.

If you need to add custom Css class to your `row` element, it's possible via `ViewData` object. Pass in `rowcssclass` parameter with desired class name:

```
    @Html.PropertyFor(x => x.CurrentPage.MainContentArea, 
                      new
                      {
                          rowsupport = true,
                          rowcssclass = "special-row"
                      })
```

### Validate Item Count

Thanks to [Jon Jones](http://www.jondjones.com/learn-episerver-cms/episerver-developers-guide/episerver-content-areas/how-to-add-bootstrap-row-validation-within-your-episerver-content-areas) for copyright! If you have Content Area with single row and want to validate item count inside to match single Bootstrap row (12 columns), you just need to add `[BootstrapRowValidation]` attribute:

```
public class StartPage : SitePageData
{
    ...
    [BootstrapRowValidation]
    public virtual ContentArea MainContentArea { get; set; }
```


### Default DisplayOption for Block

So now with latest version you can specify which display option to use if block is dropped inside content area:

```csharp
using EPiBootstrapArea;

public static Class ContentAreaTags  
{
    public const string HalfWidth = "half-width";
}

[DefaultDisplayOption(ContentAreaTags.HalfWidth)]
public class SomeBlock : BlockData  
{
    ...
}
```

This attribute will make sure that if block is dropped inside content area - display option registered with tag "half-width" is used.

Also "tagged" blocks are supported:

```csharp
using EPiBootstrapArea;

[DefaultDisplayOptionForTag("ca-tag", ContentAreaTags.OneThirdWidth)]
public class SomeBlock : BlockData  
{
    ...
}
```

### Default DisplayOption for Content Area

The same attribute can be used in ContentArea property definition:

```csharp
using EPiBootstrapArea;

[ContentType(DisplayName...]
public class StandardPage : PageData  
{
    [DefaultDisplayOption(ContentAreaTags.HalfWidth)]
    public virtual ContentArea MainContentArea { get; set; }
    ...
}
```

### Get Block Index in the ContentArea

If you need to get index of the current block in the ContentArea, you are able to write just following line:


```
<div>
    Index: @Html.BlockIndex()
</div>
```

## Customize Bootstrap Content Area
In order to customize available display options you need to add new ones through provider model.

### Provider Model
There is a tiny provider model inside this library to control how list of supported display modes is found. By default `DisplayModeFallbackDefaultProvider` provider is registered within `SetupBootstrapRenderer` module:

### Register Custom Provider

You can for instance create new module and register your own new custom provider:

```csharp
[ModuleDependency(typeof(SetupBootstrapRenderer))]
[InitializableModule]
public class CustomDisplayModeFallbackProviderInitModule : IConfigurableModule
{
    void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
    {
        context.Container.Configure(x => x.For<IDisplayModeFallbackProvider>()
                                          .Use<DisplayModeFallbackCustomProvider>());
    }

    public void Initialize(InitializationEngine context)
    {
    }

    public void Uninitialize(InitializationEngine context)
    {
    }
}
```

**NB!** In order to run *after* built-in initializable module you will need to add dependency to it in your module.

```csharp
...
[ModuleDependency(typeof(SetupBootstrapRenderer))]
public class CustomDisplayModeFallbackProviderInitModule : IConfigurableModule
{
```

And then in your custom provider you need to specify list of available display modes by overridding `GetAll()` method.

```csharp
public class DisplayModeFallbackCustomProvider : DisplayModeFallbackDefaultProvider
{
    public override List<DisplayModeFallback> GetAll()
    {
        var original = base.GetAll();

        original.Add(new DisplayModeFallback
        {
            Name = "This is from code (1/12)",
            Tag = "one-12th-from-code",
            LargeScreenWidth = 1,
            MediumScreenWidth = 1,
            SmallScreenWidth = 1,
            ExtraSmallScreenWidth = 1
        });

        return original;
    }
}
```

There is also backward compatibility with DDS storage. You will need to switch to that provider manually:

```csharp
    ...
    context.Container.Configure(x => x.For<IDisplayModeFallbackProvider>()
                                      .Use<DisplayModeDdsFallbackProvider>());
```

Registered display options will be stored in Dynamic Data Store under `EPiBootstrapArea.DisplayModeFallback` store type. Currently there is no built-in support for editing DisplayOptions on fly from EPiServer UI. For this reason you can choose for instance [Geta.DDSAdmin](https://github.com/Geta/DdsAdmin) plugin.

### Customize Generated Css Classes

Somtimes it's required to use totally different classes (not ones coming from Twitter Bootstrap - `col-lg-*`, `col-md-*`, etc).
This is now available. All you need to do is to provide your own custom pattern for Css class for each screen size.

```csharp
original.Add(new DisplayModeFallback
             {
                 Name = "This is from code (1/12)",
                 Tag = "one-12th-from-code",
                 LargeScreenWidth = 1,
                 LargeScreenCssClassPattern = "large-{0}",
                 MediumScreenWidth = 2,
                 MediumScreenCssClassPattern="medium-{0}-the-size",
                 SmallScreenWidth = 3,
                 SmallScreenCssClassPattern = "small-{0}",
                 ExtraSmallScreenWidth = 4,
                 ExtraSmallScreenCssClassPattern = "xs"
             });
```

If you will choose this `DisplayOption` for your block, following classes will be generated for wrapping element:

```
<div class="block <name-of-the-block> large-1 medium-2-the-size small-3 xs one-12th-from-code">
```

If you don't specify any of custom classes, Bootstrap default ones will be used.


### Additional Styles
Similar to EPiServer AlloyTech sample site it's possible to define custom styles for block. You have to implement `EPiBootstrapArea.ICustomCssInContentArea` interface.

```csharp
[ContentType(GUID = "EED33EA7-D118-4D3D-BD7F-88C012DFA1F8", GroupName = SystemTabNames.Content)]
public class Divider : BaseBlockData, EPiBootstrapArea.ICustomCssInContentArea
{
    public string ContentAreaCssClass
    {
        get
        {
            return "block-with-round-borders";
        }
    }
}
```

### Localized Display Option Names
You will need to add few localization resource entries in order to get localized DisplayOptions. Following entry has to be added to get localized names for default display options:

```xml
<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<languages>
  <language name="English" id="en">
    <displayoptions>
      <displaymode-full>Full (1/1)</displaymode-full>
      <displaymode-half>Half (1/2)</displaymode-half>
      <displaymode-one-third>One third (1/3)</displaymode-one-third>
      <displaymode-two-thirds>Two thirds (2/3)</displaymode-two-thirds>
      <displaymode-one-quarter>One quarter (1/4)</displaymode-one-quarter>
      <displaymode-three-quarters>Three quarters (3/4)</displaymode-three-quarters>
    </displayoptions>
  </language>
</languages>
```


### Modify Block Start Element
If there is requirement to modify start element tag for the block (i.e. [add `id` attribute to element](http://blog.tech-fellow.net/2015/09/07/create-episerver-site-menu-out-of-block-driven-content/)) you can inherit from this Bootstrap `ContentAreaRenderer` and set element start tag renderer callback:

```csharp
[ModuleDependency(typeof (SwapRendererInitModule))]
[InitializableModule]
public class SwapBootstrapRendererInitModule : IConfigurableModule
{
    public void ConfigureContainer(ServiceConfigurationContext context)
    {
        context.Container.Configure(container => container
                                        .For<ContentAreaRenderer>()
                                        .Use<AnotherBootstrapAwareContentAreaRenderer>());
    }

    public void Initialize(InitializationEngine context) {}

    public void Uninitialize(InitializationEngine context) {}
}


public class AnotherBootstrapAwareContentAreaRenderer : BootstrapAwareContentAreaRenderer
{
    public AnotherBootstrapAwareContentAreaRenderer()
    {
        SetElementStartTagRenderCallback(ModifyBlockElement);
    }

    private void ModifyBlockElement(HtmlNode blockElement, ContentAreaItem contentAreaItem, IContent content)
    {
        // modification logic here...
    }
}
```

### Skip Item Wrapper Element
By default EPiServer will generate wrapping element around content area (`div` tag name is actually controllable as well, more info [here](http://blog.tech-fellow.net/2015/06/11/content-area-under-the-hood-part-3/)):

```
@Html.PropertyFor(m => m.PageHeaderArea)
```

Resulting in:

```html
<div>                 <!-- CA wrapper element -->
    <div ...>         <!-- Block element -->
        <...>         <!-- Actual content of the block -->
    </div>
</div>
```

EPiServer gives you an option to skip wrapper element generation - resulting only in set of blocks added to the content area.

```
@Html.PropertyFor(m => m.PageHeaderArea, new { HasContainer = false })
```

Resulting in:

```html
<div ...>         <!-- Block element -->
    <...>         <!-- Actual content of the block -->
</div>
```

However, we still see that wrapping `<div>` element is not desired in `<head>` area.

Looking for the best place to add feature to skip even further - not to generate block wrapping element, but only content of the block itself.. Content area renderer is perfect candidate for this functionality.

So from version >=3.3.4 you can now write markup something like this:


```
@Html.PropertyFor(m => m.PageHeaderArea,
                  new { HasContainer = false, HasItemContainer = false })
```

Resulting in:

```html
<...>         <!-- Actual content of the block -->
```

If you use this approach to render elements for instance in [head section](http://blog.tech-fellow.net/2016/01/26/head-driven-by-content-area/), you might run into problems ending with invalid markup and EPiServer is adding edit container if property is rendered inside Edit Mode. To avoid this, you need to include additional parameter - `HasEditContainer = false`

```
@Html.PropertyFor(m => m.PageHeaderArea,
                  new
                  {
                        HasContainer = false,
                        HasItemContainer = false,
                        HasEditContainer = false
                  })
```
