using System;

namespace EPiBootstrapArea
{
    public class ConfigurationContext
    {
        public bool RowSupportEnabled { get; set; }

        public bool AutoAddRow { get; set; }

        public bool DisableBuiltinDisplayOptions { get; set; }

        public static ConfigurationContext Current { get; } = new ConfigurationContext();

        public static void Setup(Action<ConfigurationContext> configCallback)
        {
            configCallback?.Invoke(Current);
        }
    }
}
