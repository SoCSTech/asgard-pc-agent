using Microsoft.Win32;

namespace asgard_pc_agent
{
    internal class DeepFreeze
    {
        private ILogger _logger;
        public DeepFreeze(ILogger logger)
        {
            _logger = logger;
        }

        static readonly String DF_INSTALL_LOCATION = @"C:\Program Files (x86)\Faronics\Deep Freeze";
        static readonly String DF_STATUS_REGISTRY_KEY = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Faronics\Deep Freeze 6";
        static readonly String DF_STATUS_REGISTRY_VALUE = @"DF Status";
        public enum DeepFreezeStatus
        {
            Unknown,
            Frozen,
            Thawed,
            NotInstalled
        }

        public DeepFreezeStatus Status { get; set; } = DeepFreezeStatus.Unknown;

        public DeepFreezeStatus GetStatus()
        {
            // Check if DF is even installed?
            if (!Directory.Exists(DF_INSTALL_LOCATION))
                return DeepFreezeStatus.NotInstalled;

            // Check registry for status and convert it to a string.
            string dfStatus = "TBC"; // Set Placeholder Value
            try
            {
                object? dfRegistryItem = Registry.GetValue(DF_STATUS_REGISTRY_KEY, DF_STATUS_REGISTRY_VALUE, "");
                dfStatus = dfRegistryItem?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to convert registry value {DF_STATUS_REGISTRY_KEY} -> {DF_STATUS_REGISTRY_VALUE}");
                _logger.LogError(ex.Message);
            }

            // Thawed
            if (dfStatus == "Thawed")
            {
                return DeepFreezeStatus.Thawed;
            }

            // Frozen
            else if (dfStatus == "Frozen")
            {
                return DeepFreezeStatus.Frozen;
            }

            // Return Default State as Unknown
            return DeepFreezeStatus.Unknown;
        }
    }
}