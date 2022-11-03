// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using EPiServer.Core;
using EPiServer.Web;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TechFellow.Optimizely.AdvancedContentArea;

internal class ContentAreaItemContext : IDisposable
{
    private readonly ViewDataDictionary _viewData;

    public ContentAreaItemContext(ViewDataDictionary viewData, ContentAreaItem contentAreaItem)
    {
        _viewData = viewData;
        var displayOption = contentAreaItem.LoadDisplayOption()
                            ?? new DisplayOption { Id = Guid.NewGuid().ToString(), Name = "Unknown" };

        if (!_viewData.ContainsKey(Constants.CurrentDisplayOptionKey))
        {
            _viewData.Add(Constants.CurrentDisplayOptionKey, displayOption);
        }
        else
        {
            _viewData[Constants.CurrentDisplayOptionKey] = displayOption;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _viewData.Remove(Constants.CurrentDisplayOptionKey);
    }
}
