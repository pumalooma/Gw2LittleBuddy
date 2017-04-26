
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

public static class Win32 {
    [DllImport("user32.dll")]
    private static extern void mouse_event (uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);
    
    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    private const int MOUSEEVENTF_RIGHTUP = 0x10;

    public static void DoMouseClickLeft () {
        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }

    public static void DoMouseClickRight () {
        mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
    }

    [DllImport("user32.dll")]
    private static extern void keybd_event (byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    public const byte VK_LEFT = 0x25;
    public const byte VK_UP = 0x26;
    public const byte VK_RIGHT = 0x27;
    public const byte VK_DOWN = 0x28;

    public static void PressKey (byte key, int time) {
        keybd_event(key, 0, 0, 0);
        Thread.Sleep(time);
        keybd_event(key, 0, KEYEVENTF_KEYUP, 0);
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetFocus ();
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow ();
    [DllImport("user32.dll")]
    public static extern int GetWindowText (IntPtr hWnd, StringBuilder text, int count);
    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow ();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow (string lpClassName, string lpWindowName);

    
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetClassName (IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);


    // A Win32 constant
    const int WM_SETTEXT = 0x000C;
    public const int WM_KEYDOWN = 0x0100;
    public const int WM_KEYUP = 0x0101;
    public const int VK_RETURN  = 0x0D;

    // An overload of the SendMessage function, this time taking in a string as the lParam.
    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage (IntPtr hWnd, int Msg, int wParam, string lParam);

    // PostMessage is very similar to SendMessage. They both send a message to the given
    // handle / window, the difference being that SendMessage sends the message and waits
    // for the window to "handle" the message and return a return code. PostMessage on the
    // other hand simply posts the message and returns instantly, whether the window
    // handles the message or not, we don't care.
    [DllImport("user32.Dll")]
    public static extern IntPtr PostMessage (IntPtr hWnd, int msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern bool AttachThreadInput (uint idAttach, uint idAttachTo, bool fAttach);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId ();
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId (IntPtr hWnd, IntPtr ProcessId);
}