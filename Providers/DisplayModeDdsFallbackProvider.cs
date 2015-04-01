using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data.Dynamic;

namespace EPiBootstrapArea.Providers
{
    public class DisplayModeDdsFallbackProvider : IDisplayModeFallbackProvider
    {
        private DynamicDataStore _store;

        private DynamicDataStore Store
        {
            get
            {
                return _store ?? (_store = typeof(DisplayModeFallback).GetStore());
            }
        }

        public virtual void Initialize()
        {
            var initialData = new DisplayModeFallbackDefaultProvider().GetAll();
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
}
