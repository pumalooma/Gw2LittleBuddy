using System;
using System.Runtime.InteropServices;
using System.Text;

public static class Win32 {
    [DllImport("user32.dll")]
    public static extern IntPtr GetFocus ();
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow ();
    [DllImport("user32.dll")]
	public static extern IntPtr GetActiveWindow();
	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr FindWindow (string lpClassName, string lpWindowName);
	[DllImport("user32.dll")]
	public static extern int GetWindowText (IntPtr hWnd, StringBuilder text, int count);
	[DllImport("user32.dll", SetLastError = true)]
	public static extern int GetClassName (IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
}
