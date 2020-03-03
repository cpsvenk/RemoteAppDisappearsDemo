# RemoteAppDisappearsDemo
This App demonstrates a problem with WPF apps using a custom window chrome when running via RemoteApp. Under certain circumstances described below,
the window disappears from the client machine when the title of the window is altered.
## Steps to reproduce:
- Start a long running operation ("Do work" button)
- While that process is running, `<Alt>+<Tab>` out of the remote app 
- Immediately use `<Alt>+<Tab>` to switch back to the remote app again
- Once "do work" is finished, try to change the window title using the text box and the "Change title" button.
## Effect
- The remote app disappears from the screen and even from the taskbar of the client machine
- It it still running normally on the server
- Usually, a single click on the task bar or desktop of the client machine brings back the remote app
- The window title is changed correctly
- Every subsequent attempt to change the window title will cause this behavior
## Remarks
The problem occurs only when the application\'s UI thread becomes unresponsive for more than 5 seconds.
Such behavior is certainly not ideal. But it cannot be avoided entirely in some scenarios.


The problem can be traced back to the WindowChromeWorker of WPF. It handles the WM_SETTEXT and WM_SETICON messages of the window it 
is applied to thusly:

```C#
        private IntPtr _HandleSetTextOrIcon(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            bool modified = _ModifyStyle(WS.VISIBLE, 0);

            // Setting the caption text and icon cause Windows to redraw the caption.
            // Letting the default WndProc handle the message without the WS_VISIBLE
            // style applied bypasses the redraw.
            IntPtr lRet = NativeMethods.DefWindowProc(_hwnd, uMsg, wParam, lParam);

            // Put back the style we removed.
            if (modified)
            {
                _ModifyStyle(0, WS.VISIBLE);
            }
            handled = true;
            return lRet;
        }
```
The "VISIBLE" style of the window is cleared temporarily in order to avoid a redraw. If you remove this optimization from the code,
the window will not disappear in the scenario described above. Also, both _ModifyStyle calls are executed. I.E. the "modified" flag is
calculated correctly.
