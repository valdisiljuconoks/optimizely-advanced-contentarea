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

Registered display modes are stored in Dynamic Data Store under `EPiBootstrapArea.DisplayModeFallback` type.

## Display Option Fallbacks
For every display modes there are 4 fallback width for various screen sizes based on Bootstrap grid system. According to Bootstrap v3 [specification](http://getbootstrap.com/css/#grid-options) following screen sizes are defined:
* Large screen (>= 1200px)
* Medium devices (>= 992px && < 1200px)
* Small devices (>= 768px && < 992px)
* Extra small devices (< 768px)

### Display Modes Width Fallbacks
These numbers are added at the end of Bootstrap grid system class (for instance 12 for Large screen -> 'col-lg-12')

| Display Mode Name   | Extra small devices | Small devices | Medium devices | Large screen |
|---------------------|---------------------|---------------|----------------|--------------|
|Full width           |12                   |12             |12              |12            |
|Half width           |12                   |12             |6               |6             |
|One third            |12                   |12             |6               |3             |
|Two thirds           |12                   |12             |6               |8             |
|One quater           |12                   |12             |6               |3             |
|Three quaters        |12                   |12             |6               |9             |
