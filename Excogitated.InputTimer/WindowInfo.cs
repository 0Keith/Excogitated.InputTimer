using Excogitated.Common.Atomic.Collections;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Excogitated.InputTimer
{
    internal static class WindowInfo
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        private static readonly AtomicDictionary<long, string> _names = new AtomicDictionary<long, string>();

        public static string GetActiveWindowName()
        {
            var id = GetForegroundWindow().ToInt64();
            if (_names.TryGetValue(id, out var name))
                return name;

            _names.Clear();
            foreach (var p in Process.GetProcesses())
                using (p)
                    _names[p.MainWindowHandle.ToInt64()] = p.MainWindowTitle;

            if (_names.TryGetValue(id, out name))
                return name;
            return string.Empty;
        }
    }
}
