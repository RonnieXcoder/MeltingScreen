# MeltingScreen
This code snippet demonstrates the usage of WinAPI to create a "floating" window above all others, implementing a "melting" effect on the screen. 

Key elements include:

- Definition of screen width and height variables using GetSystemMetrics function.
- Constants representing window styles and messages for window handling.
- Structure definitions for POINT, MSG, and WNDCLASS, detailing window attributes and message structures.
- Declaration of methods for WinAPI function calls using Platform Invocation Services (PInvoke).
- Window procedure handling various messages like WM_CREATE, WM_PAINT, WM_DESTROY, and WM_CLOSE.
- Utilization of BitBlt function to copy portions of images, creating the "melting" effect.
- Setup of a timer for periodic function calls, generating random parameters for image display.
- Window creation, registration of the window class, and message loop execution in the Main application entry point.
