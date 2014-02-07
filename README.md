EPiBootstrapArea
================

Bootstrap aware EPiServer content area renderer. Provides easy way to register display modes used to customize look and feel of the blocks inside EPiServer content area.

## Available Display Options
Following display options are regsitered by default:
* Full width
* Half width
* One-third width
* Two-thirds width
* One-quarter width
* Three-quarter width

![](https://ruiorq.dm2304.livefilestore.com/y2pJ4-y8MWiBSk3Gmk_-7grHj7anXZMfEc6oyw9kbs_lZjjnXJiVWZGQRduzg25S0AblsZgDAXNdlfzlcZRd6KZtAiRtbhHT3GktV2osP8vD44/display-modes.png?psid=1)

Registered display modes are stored in Dynamic Data Store under `EPiBootstrapArea.DisplayModeFallback` type. Currently there is no built-in support for editing DisplayOptions on fly from EPiServer UI. For this reason you can choose for isntance [Geta.DDSAdmin](https://github.com/Geta/DdsAdmin) plugin.

## Display Option Fallbacks
For every display modes there are 4 fallback width for various screen sizes based on Bootstrap grid system. According to Bootstrap v3 [specification](http://getbootstrap.com/css/#grid-options) following screen sizes are defined:
* Large screen (>= 1200px)
* Medium devices (>= 992px && < 1200px)
* Small devices (>= 768px && < 992px)
* Extra small devices (< 768px)

### Display Modes Width Fallbacks
These numbers are added at the end of Bootstrap grid system class (for instance 12 for Large screen -> `'col-lg-12'`)

| Display Mode Name   | Extra small devices | Small devices | Medium devices | Large screen |
|---------------------|---------------------|---------------|----------------|--------------|
|Full width           |12                   |12             |12              |12            |
|Half width           |12                   |12             |6               |6             |
|One third            |12                   |12             |6               |3             |
|Two thirds           |12                   |12             |6               |8             |
|One quater           |12                   |12             |6               |3             |
|Three quaters        |12                   |12             |6               |9             |


Eventually for instance if you choose Half-width display option for a block of type `EditorialBlockWithHeader` following markup will be generated:

```
<div class="block editorialblockwithheader col-lg-6 col-md-6 col-sm-12 col-xs-12 displaymode-half">
    ...
</div>
```

## Additional Styles
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

## Localized Display Option Names
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
