using System;

namespace EPiBootstrapArea
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DefaultDisplayOptionForTagAttribute : Attribute
    {
        public DefaultDisplayOptionForTagAttribute(string tag, string displayOption)
        {
            if(string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            Tag = tag;
            DisplayOption = displayOption;
        }

        public string Tag { get; private set; }

        public string DisplayOption { get; private set; }
    }
}
