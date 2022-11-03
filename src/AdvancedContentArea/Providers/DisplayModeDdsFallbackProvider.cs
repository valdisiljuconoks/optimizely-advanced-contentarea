// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data.Dynamic;

namespace TechFellow.Optimizely.AdvancedContentArea.Providers;

public class DisplayModeDdsFallbackProvider : IDisplayModeFallbackProvider
{
    private DynamicDataStore _store;

    private DynamicDataStore Store => _store ?? (_store = typeof(DisplayModeFallback).GetStore());

    public virtual void Initialize()
    {
        var initialData = GetInitialData();
        var registeredModes = Store.LoadAll<DisplayModeFallback>().ToList();

        ValidateInitialData(initialData);

        foreach (var mode in initialData.Where(newMode => registeredModes.All(originalMode => newMode.Tag != originalMode.Tag)))
        {
            Store.Save(mode);
        }
    }

    public List<DisplayModeFallback> GetAll()
    {
        return Store.LoadAll<DisplayModeFallback>().ToList();
    }

    protected virtual List<DisplayModeFallback> GetInitialData()
    {
        return new DisplayModeFallbackDefaultProvider().GetAll();
    }

    private static void ValidateInitialData(IEnumerable<DisplayModeFallback> initialData)
    {
        var duplicateTagsRegistered = initialData.GroupBy(x => x.Tag)
            .Select(g => new { Value = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count);

        foreach (var tagGroup in duplicateTagsRegistered.Where(tagGroup => tagGroup.Count > 1))
        {
            throw new ArgumentException("Multiple DisplayFallback options are registered with tag = " + tagGroup.Value);
        }
    }
}
