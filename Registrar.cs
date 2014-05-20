using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EPiServer.Data.Dynamic;

namespace EPiBootstrapArea
{
    public class Registrar
    {
        private DynamicDataStore store;

        private DynamicDataStore Store
        {
            get
            {
                return this.store ?? (this.store = typeof(DisplayModeFallback).GetStore());
            }
        }

        public static void Register(params DisplayModeFallback[] modes)
        {
            if (modes == null)
            {
                throw new ArgumentNullException("modes");
            }

            ValidateModes(modes);

            var instance = new Registrar();
            var comparer = new DisplayModeFallbackComparer();
            var allStores = instance.Store.LoadAll<DisplayModeFallback>();

            foreach (var displayModeFallback in modes)
            {
                if (!allStores.Contains(displayModeFallback, comparer))
                {
                    instance.Store.Save(displayModeFallback);
                }
            }
        }

        internal static void ValidateModes(params DisplayModeFallback[] modes)
        {
            // validate all modes
            foreach (var mode in modes)
            {
                Validator.ValidateObject(mode, new ValidationContext(mode, null, null), true);
            }
        }
    }
}
