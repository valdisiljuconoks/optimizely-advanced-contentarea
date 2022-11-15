// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;

namespace TechFellow.Optimizely.AdvancedContentArea;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class BootstrapRowValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        var contentArea = value as ContentArea;
        var noItems = contentArea?.Items == null;

        if (noItems)
        {
            return true;
        }

        var count = 0;
        foreach (var item in contentArea.Items)
        {
            var displayOption = item.LoadDisplayOption();

            if (displayOption == null)
            {
                continue;
            }

            var optionAsEnum = GetDisplayOptionTag(displayOption.Tag);
            count = count + optionAsEnum;

            if (count > 12)
            {
                return false;
            }
        }

        return true;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var result = base.IsValid(value, validationContext);

        if (!string.IsNullOrWhiteSpace(result?.ErrorMessage))
        {
            result.ErrorMessage = "Items exceed all 12 Bootstrap columns";
        }

        return result;
    }

    public static int GetDisplayOptionTag(string tag)
    {
        // I love DI
        var renderer = ServiceLocator.Current.GetInstance<ContentAreaRenderer>();

        if (renderer is AdvancedContentAreaRenderer areaRenderer)
        {
            return areaRenderer.GetColumnWidth(tag);
        }

        return 12;
    }
}
