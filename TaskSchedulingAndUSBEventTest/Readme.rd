The Problem
----------------
After SWAP bit to Swap bit application update, for example RP4.2-TY11 to UUT 0.0.0.7, We saw an inconsistency of seeing the UsbChangeEvent() being called.
The Main page generates the UsbChangeEvent(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) when a USB device is attached.
We observed very inconsistent behavior when we had the logs, meanings these prints were not seen as expected. This caused many reconnection issues.
The purpose of creating this project is  to recreate and understand how andy why the UsbChangeEvent() is not called as expected in a clean project enviroment.

The USB event
----------------
The Main page generates the UsbChangeEvent(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) and prints
"Main - Device Changed | 331564 | 537 | 7 | 0" .
This method is executed in the Main Thread as you can see Thread view, call stack.


The Expected USB event behavior
---------------------------------
The UsbChangeEvent() should be called once when the USB device detaches, and UsbChangeEvent() should be called twice when attached.
Which should print the message as shown below


We Connected the USB device here
Main - Device Changed | 1511000 | 537 | 7 | 0 
[2022-07-19T22:28:02] DeviceManager: UsbChangeEvent: hwnd=1511000, msg:537, wParam=7, lParam=0, handle=False Connect event
 Main - Device Changed | 1511000 | 537 | 7 | 0 
[2022-07-19T22:28:02] DeviceManager: UsbChangeEvent: hwnd=1511000, msg:537, wParam=7, lParam=0, handle=False Connect event

We disconnected the USB device here Main - Device Changed | 1511000 | 537 | 7 | 0 
[2022-07-19T22:28:05] DeviceManager: UsbChangeEvent: hwnd=1511000, msg:537, wParam=7, lParam=0, handle=False Disconnect event


conculsion.
Once the background thread completes it calls  a function using Dispatcher.BeginInvoke, that function runs on the UI(Main thread) thread.
The mainpage UsbChangeEvent() also runs on the mainthread,which was verified by checking out the Thread view, call stack.
