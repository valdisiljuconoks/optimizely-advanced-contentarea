using System.Collections.Generic;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace EPiBootstrapArea.Initialization
{
    [ModuleDependency(typeof(DataInitialization))]
    public class DefaultDisplayModesRegistrationInitModule : IInitializableModule
    {
        private DynamicDataStore store;

        private DynamicDataStore Store
        {
            get
            {
                return this.store ?? (this.store = typeof(DisplayModeFallback).GetStore());
            }
        }

        public void Initialize(InitializationEngine context)
        {
            foreach (var item in Store.Items<DisplayModeFallback>())
            {
                Store.Delete(item);
            }

            var initialData = new List<DisplayModeFallback>
                              {
                                      new DisplayModeFallback
                                      {
                                              Name = "Full width (1/1)",
                                              Tag = ContentAreaTags.FullWidth,
                                              LargeScreenWidth = 12,
                                              MediumScreenWidth = 12,
                                              SmallScreenWidth = 12,
                                              ExtraSmallScreenWidth = 12
                                      },
                                      new DisplayModeFallback
                                      {
                                              Name = "Half width (1/2)",
                                              Tag = ContentAreaTags.HalfWidth,
                                              LargeScreenWidth = 6,
                                              MediumScreenWidth = 6,
                                              SmallScreenWidth = 12,
                                              ExtraSmallScreenWidth = 12
                                      },
                                      new DisplayModeFallback
                                      {
                                              Name = "One third width (1/3)",
                                              Tag = ContentAreaTags.OneThirdWidth,
                                              LargeScreenWidth = 4,
                                              MediumScreenWidth = 6,
                                              SmallScreenWidth = 12,
                                              ExtraSmallScreenWidth = 12
                                      },
                                      new DisplayModeFallback
                                      {
                                              Name = "Two thirds width (2/3)",
                                              Tag = ContentAreaTags.TwoThirdsWidth,
                                              LargeScreenWidth = 8,
                                              MediumScreenWidth = 6,
                                              SmallScreenWidth = 12,
                                              ExtraSmallScreenWidth = 12
                                      },
                                      new DisplayModeFallback
                                      {
                                              Name = "One quarter width (1/4)",
                                              Tag = ContentAreaTags.OneQuarterWidth,
                                              LargeScreenWidth = 3,
                                              MediumScreenWidth = 6,
                                              SmallScreenWidth = 12,
                                              ExtraSmallScreenWidth = 12
                                      },
                                      new DisplayModeFallback
                                      {
                                              Name = "Three quarters width (3/4)",
                                              Tag = ContentAreaTags.ThreeQuartersWidth,
                                              LargeScreenWidth = 9,
                                              MediumScreenWidth = 6,
                                              SmallScreenWidth = 12,
                                              ExtraSmallScreenWidth = 12
                                      },
                              };

            foreach (var item in initialData)
            {
                Store.Save(item);
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
