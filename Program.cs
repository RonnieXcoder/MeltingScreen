
using System.Runtime.InteropServices;

class Program
{
    static int screenWidth;
    static int screenHeight;
    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;
    const uint WS_EX_TOPMOST = 0x00000008;
    const uint WS_POPUP = 0x80000000;
    const uint WM_CREATE = 0x0001;
    const uint WM_CLOSE = 0x0010;
    const uint WM_PAINT = 0x000F;
    const uint WM_DESTROY = 0x0002;
    const uint SRCCOPY = 0x00CC0020;

    static readonly IntPtr HWND_DESKTOP = IntPtr.Zero;

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct WNDCLASS
    {
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern ushort RegisterClassW([In] ref WNDCLASS lpWndClass);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern IntPtr LoadCursorW(IntPtr hInstance, IntPtr lpCursorName);

    [DllImport("user32.dll")]
    static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern void PostQuitMessage(int nExitCode);

    [DllImport("user32.dll")]
    static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr CreateWindowExW(
        uint dwExStyle,
        [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
        [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);
    delegate void TimerProc(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool TranslateMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll")]
    static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool ValidateRect(IntPtr hWnd, IntPtr lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);

    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
        int nXSrc, int nYSrc, uint dwRop);

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    delegate IntPtr WndProcDelegate(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    static void TimerCallback(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime)
    {
        IntPtr Wnd = GetDC(hWnd);
        Random random = new Random();
        int x = random.Next(0, screenWidth) - (200 / 2);
        int y = random.Next(0, 15);
        int width = random.Next(0, 200);
        BitBlt(Wnd, x, y, width, screenHeight, Wnd, x, 0, SRCCOPY);
        ReleaseDC(hWnd, Wnd);
    }

    static IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
    {
        switch (uMsg)
        {
            case WM_CREATE:
                IntPtr Desktop = GetDC(HWND_DESKTOP);
                IntPtr Window = GetDC(hWnd);
                BitBlt(Window, 0, 0, screenWidth, screenHeight, Desktop, 0, 0, SRCCOPY);
                ReleaseDC(hWnd, Window);
                ReleaseDC(HWND_DESKTOP, Desktop);
                SetTimer(hWnd, IntPtr.Zero, 100, TimerCallback);
                ShowWindow(hWnd, 5);
                break;
            case WM_PAINT:
                ValidateRect(hWnd, IntPtr.Zero);
                break;
            case WM_DESTROY:
                KillTimer(hWnd, IntPtr.Zero);
                PostQuitMessage(0);
                break;
            case WM_CLOSE:
                KillTimer(hWnd, IntPtr.Zero);
                PostQuitMessage(0);
                break;

            default:
                return DefWindowProc(hWnd, uMsg, wParam, lParam);
        }

        return IntPtr.Zero;
    }
    static void Main(string[] args)
    {
        IntPtr hWndConsole = GetConsoleWindow();
        //ShowWindow(hWndConsole, 6);


        screenWidth = GetSystemMetrics(SM_CXSCREEN);

        screenHeight = GetSystemMetrics(SM_CYSCREEN);

        IntPtr hInstance = Marshal.GetHINSTANCE(typeof(Program).Module);

        WNDCLASS wndClass = new WNDCLASS
        {
            style = 0,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate<WndProcDelegate>(WndProc),
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = hInstance,
            hIcon = IntPtr.Zero,
            hCursor = LoadCursorW(IntPtr.Zero, new IntPtr(32512)), // IDC_ARROW
            hbrBackground = IntPtr.Zero,
            lpszMenuName = "",
            lpszClassName = "MeltingScreen"
        };

        ushort classAtom = RegisterClassW(ref wndClass);

        if (classAtom == 0)
        {
            int error = Marshal.GetLastWin32Error();
            Console.WriteLine("Error: " + error);
        }
        else
        {

            IntPtr hWnd = CreateWindowExW(
                WS_EX_TOPMOST,
                wndClass.lpszClassName,
                "MyWindow",
                WS_POPUP,
                0,
                0,
                screenWidth,
                screenHeight,
                HWND_DESKTOP,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero);

            MSG msg;
            while (GetMessage(out msg, IntPtr.Zero, 0, 0) != 0)
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }
    }
}

