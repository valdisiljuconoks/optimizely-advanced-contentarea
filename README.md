EPiBootstrapArea
================

Bootstrap aware EPiServer content area renderer. Provides easy way to register display options used to customize look and feel of the blocks inside a EPiServer content area.

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


## Customize Bootstrap Content Area
In order to customize available display options you need to add new ones through provider model.

### Provider Model
There is a tiny provider model inside plugin to control how list of supported display modes is found. By default `DisplayModeFallbackDefaultProvider` provider is registered:

```csharp
[ModuleDependency(typeof(ServiceContainerInitialization))]
[InitializableModule]
public class DisplayModeFallbackProviderInitModule : IConfigurableModule
{
    void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
    {
        context.Container.Configure(x => x.For<IDisplayModeFallbackProvider>()
                                          .Use<DisplayModeFallbackDefaultProvider>());
    }

    public void Initialize(InitializationEngine context)
    {
    }

    public void Uninitialize(InitializationEngine context)
    {
    }
}
```

You can for instance create new module and register your own new custom provider:

```csharp
    context.Container.Configure(x => x.For<IDisplayModeFallbackProvider>()
                                      .Use<DisplayModeFallbackCustomProvider>());

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
            LargeScreenWidth = 12,
            MediumScreenWidth = 12,
            SmallScreenWidth = 12,
            ExtraSmallScreenWidth = 12
        });

        return original;
    }
}
```

There is also backward compatibility with DDS storage. You will need to switch to that provider manually:

```csharp
    context.Container.Configure(x => x.For<IDisplayModeFallbackProvider>()
                                      .Use<DisplayModeDdsFallbackProvider>());
```

Registered display options will be stored in Dynamic Data Store under `EPiBootstrapArea.DisplayModeFallback` store type. Currently there is no built-in support for editing DisplayOptions on fly from EPiServer UI. For this reason you can choose for instance [Geta.DDSAdmin](https://github.com/Geta/DdsAdmin) plugin.


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
