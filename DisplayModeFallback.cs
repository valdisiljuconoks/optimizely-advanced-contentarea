using System;
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

        public Identity Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int LargeScreenWidth { get; set; }
        public int MediumScreenWidth { get; set; }
        public int SmallScreenWidth { get; set; }
        public int ExtraSmallScreenWidth { get; set; }
        public string Icon { get; set; }
    }
}
