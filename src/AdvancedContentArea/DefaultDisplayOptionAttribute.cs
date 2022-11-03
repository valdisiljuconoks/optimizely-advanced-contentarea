using System;

namespace TechFellow.Optimizely.AdvancedContentArea
{
    public class DefaultDisplayOptionAttribute : Attribute
    {
        public DefaultDisplayOptionAttribute(string displayOption)
        {
            if(string.IsNullOrWhiteSpace(displayOption))
            {
                throw new ArgumentNullException(nameof(displayOption));
            }

            DisplayOption = displayOption;
        }

        public string DisplayOption { get; private set; }
    }
}
