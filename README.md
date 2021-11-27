A simple program which toggles specific desktop color settings on or off in Nvidia Control Panel. 
- Changes to Brightness, Contrast, and Gamma will not show up in nvidia control panel because these values are set through the Windows API and there is no way at the moment to change them through NVAPI. They DO actually apply to your desktop.
- This program only toggles color settings on your primary display. 
- I made this for myself, so I'm not guaranteeing support. But reasonable issues may be addressed

## Usage
- Edit the values in NVCP Toggle.dll.config to change what values the program will toggle
- Run NVCP Toggle.exe to toggle. It's easiest to put a shortcut on your desktop or task bar for quick toggling