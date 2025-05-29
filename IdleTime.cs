using System.Runtime.InteropServices;

namespace asgard_pc_agent
{
    internal class IdleTime
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        // Import the GetLastInputInfo function from User32.dll
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        /// <summary>
        /// Gets the time since the last user input (keyboard or mouse) in milliseconds.
        /// </summary>
        public static TimeSpan GetIdleTime()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            if (GetLastInputInfo(ref lastInputInfo))
            {
                // Environment.TickCount gets the number of milliseconds elapsed since the system started.
                // dwTime is the tick count when the last input event was received.
                uint idleMilliseconds = (uint)Environment.TickCount - lastInputInfo.dwTime;
                return TimeSpan.FromMilliseconds(idleMilliseconds);
            }
            else
            {
                // Handle error, e.g., throw an exception or return TimeSpan.Zero
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "Failed to get last input info.");
            }
        }

    }
}
