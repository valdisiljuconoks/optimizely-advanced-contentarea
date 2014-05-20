using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace EPiBootstrapArea
{
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

        [Required]
        [Range(1, 12)]
        public int MediumScreenWidth { get; set; }

        [Required]
        [Range(1, 12)]
        public int SmallScreenWidth { get; set; }

        [Required]
        [Range(1, 12)]
        public int ExtraSmallScreenWidth { get; set; }

        public string Icon { get; set; }
    }
}
