A simple program which toggles specific desktop color settings on or off in Nvidia Control Panel. 
- Changes to Brightness, Contrast, and Gamma will not show up in nvidia control panel because these values are set through the Windows API and there is no way at the moment to change them through NVAPI. They DO actually apply to your desktop.
- This program only toggles color settings on your primary display. 
- I made this for myself, so I'm not guaranteeing support. But reasonable issues may be addressed

## Usage
- Download the bundled version if you do not have .NET core runtime installed
- Edit the values in NVCP appSettings.json to change what values the program will toggle
- Run NVCP Toggle.exe to toggle. It's easiest to put a shortcut on your desktop or task bar for quick toggling

## Known Issues and Caveats
- The base program will not run without .NET Core 3.1 Runtime installed. If you do not want to install this, you can use the bundled version. In this case it will be slower to toggle on and off +/- a few seconds depending on your computer speed.
- DisplayCal profile loader will override the gammaramp color settings (brightness, contrast, gamma). I'm looking into potential fixes to get it to behave.
