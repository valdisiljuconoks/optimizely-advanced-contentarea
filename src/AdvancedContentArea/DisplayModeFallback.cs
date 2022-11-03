// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace TechFellow.Optimizely.AdvancedContentArea;

[EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
[Serializable]
public class DisplayModeFallback
{
    public DisplayModeFallback()
    {
        Id = Identity.NewIdentity();
    }

    public Identity Id { get; internal set; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Tag { get; set; }

    [Required]
    [Range(1, 12)]
    public int LargeScreenWidth { get; set; }

    public string LargeScreenCssClassPattern { get; set; }

    [Required]
    [Range(1, 12)]
    public int MediumScreenWidth { get; set; }

    public string MediumScreenCssClassPattern { get; set; }

    [Required]
    [Range(1, 12)]
    public int SmallScreenWidth { get; set; }

    public string SmallScreenCssClassPattern { get; set; }

    [Required]
    [Range(1, 12)]
    public int ExtraSmallScreenWidth { get; set; }

    public string ExtraSmallScreenCssClassPattern { get; set; }

    public string Icon { get; set; }

    public class None : DisplayModeFallback
    {
        public None()
        {
            Name = "None";
            Tag = ContentAreaTags.None;
            LargeScreenWidth = 0;
            MediumScreenWidth = 0;
            SmallScreenWidth = 0;
            ExtraSmallScreenWidth = 0;
        }
    }
}
