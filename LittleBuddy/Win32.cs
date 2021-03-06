using System;
using System.Runtime.InteropServices;
using System.Text;

public static class Win32 {
    [DllImport("user32.dll")]
    public static extern IntPtr GetFocus ();
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow ();
	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetForegroundWindow(IntPtr hWnd);
	[DllImport("user32.dll")]
	public static extern IntPtr GetActiveWindow();
	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr FindWindow (string lpClassName, string lpWindowName);
	[DllImport("user32.dll")]
	public static extern int GetWindowText (IntPtr hWnd, StringBuilder text, int count);
	[DllImport("user32.dll", SetLastError = true)]
	public static extern int GetClassName (IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    public const int KEY_DOWN    = 0x8000;
    public const int KEY_TOGGLED = 0x0001;

    [DllImport("user32.dll")]
    public static extern short GetKeyState (int nVirtKey);
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState (int nVirtKey);

	public static bool IsKeyDown(int key)
	{
		return KEY_DOWN == (GetAsyncKeyState(key) & KEY_DOWN);
	}

	public static bool IsKeyToggle(int key)
	{
		return KEY_TOGGLED == (GetAsyncKeyState(key) & KEY_TOGGLED);
	}
}
