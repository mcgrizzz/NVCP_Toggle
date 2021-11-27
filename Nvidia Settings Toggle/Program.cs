using System;
using System.Configuration;
using System.IO;
using System.Linq;
using NvAPIWrapper;
using WindowsDisplayAPI;

namespace NVCP_Toggle
{
    class Program
    {
        static int defaultVibrance = 50;
        static int defaultHue = 0;

        static double defaultBrightness = .50;
        static double defaultContrast = .50;
        static double defaultGamma = 1.0;

        static void Main(string[] args)
        {
            NVIDIA.Initialize();
            setupConfig();

            bool allDisplays = Boolean.Parse(ConfigurationManager.AppSettings["toggleAllDisplays"]);
            int[] colorSettings = loadCustomColorSettings();
            double[] gammaRamp = loadCustomGammaRamp();

            if (allDisplays)
            {
                Console.WriteLine("Toggling all displays...\n");
                Display[] windowsDisplays = Display.GetDisplays().ToArray();
                NvAPIWrapper.Display.Display[] nvDisplays = NvAPIWrapper.Display.Display.GetDisplays();

                for(int i = 0; i < windowsDisplays.Length; i++)
                {
                    Display windowsDisplay = windowsDisplays[i];
                    NvAPIWrapper.Display.Display nvDisplay = nvDisplays[i];
                    toggleDisplay(nvDisplay, windowsDisplay, colorSettings, gammaRamp);
                    Console.WriteLine("");
                }
            }
            else
            {
                Console.WriteLine("Toggling primary display...\n");
                NvAPIWrapper.Display.Display nvDisplay = GetNvidiaMainDisplay();
                Display windowsDisplay = GetWindowsDisplay();
                toggleDisplay(nvDisplay, windowsDisplay, colorSettings, gammaRamp);
            }

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }

        private static void toggleDisplay(NvAPIWrapper.Display.Display nvDisplay, Display windowsDisplay, int[] colorSettings, double[] gammaRamp) 
        {
            int currentVibrance = nvDisplay.DigitalVibranceControl.CurrentLevel;
            int currentHue = nvDisplay.HUEControl.CurrentAngle;
            Console.WriteLine("Display: " + windowsDisplay.ToPathDisplayTarget().FriendlyName);
            if (currentVibrance == defaultVibrance && currentHue == defaultHue)
            {
                //Toggle on
                Console.WriteLine("Toggling Custom Settings:");
                Console.WriteLine("Vibrance: " + colorSettings[0] + " Hue: " + colorSettings[1]);
                Console.WriteLine("Brightness: " + gammaRamp[0] + " Contrast: " + gammaRamp[1] + " Gamma: " + gammaRamp[2]);

                nvDisplay.DigitalVibranceControl.CurrentLevel = colorSettings[0];
                nvDisplay.HUEControl.CurrentAngle = colorSettings[1];

                windowsDisplay.GammaRamp = new DisplayGammaRamp(gammaRamp[0], gammaRamp[1], gammaRamp[2]);
            }
            else
            {
                //Toggle off
                Console.WriteLine("Resetting to default settings...");
                nvDisplay.DigitalVibranceControl.CurrentLevel = defaultVibrance;
                nvDisplay.HUEControl.CurrentAngle = defaultHue;

                windowsDisplay.GammaRamp = new DisplayGammaRamp(defaultBrightness, defaultContrast, defaultGamma);
            }
        }

        //From: https://stackoverflow.com/a/62330624
        private static void setupConfig()
        {
            string programName = "NVCP Toggle";
            var sourceHostFile = Directory.GetCurrentDirectory() + @"\" + programName + @".dll.config";
            // to load yourProgram.dll.config
            // With Single-file executables, all files are bundled in a single host file with the .exe extension. 
            // When that file runs for the first time, it unpacks its contents to AppData\Local\Temp\.net\, in a new folder named for the application
            var targetHostFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            // ignore when in debug mode in vs ide
            if (sourceHostFile != targetHostFile)
            {
                File.Copy(sourceHostFile, targetHostFile, true);
            }
        }

        private static int[] loadCustomColorSettings()
        {
            int[] colors = new int[2];
            colors[0] = Int32.Parse(ConfigurationManager.AppSettings["vibrance"]);
            colors[1] = Int32.Parse(ConfigurationManager.AppSettings["hue"]);
            return colors;
        }

        private static double[] loadCustomGammaRamp()
        {
            double[] gamma = new double[3];
            gamma[0] = Double.Parse(ConfigurationManager.AppSettings["brightness"]);
            gamma[1] = Double.Parse(ConfigurationManager.AppSettings["contrast"]);
            gamma[2] = Double.Parse(ConfigurationManager.AppSettings["gamma"]);
            return gamma;
        }

        private static Display GetWindowsDisplay()
        {
            Display[] displays = Display.GetDisplays().ToArray();
            foreach(Display display in displays)
            {
                if (display.DisplayScreen.IsPrimary)
                {
                    return display;
                }
            }
            
            
            return null;
        }


        private static NvAPIWrapper.Display.Display GetNvidiaMainDisplay()
        {
            NvAPIWrapper.Display.Display[] allDisplays = NvAPIWrapper.Display.Display.GetDisplays();
            for(int i = 0; i < NvAPIWrapper.Display.PathInfo.GetDisplaysConfig().Length; i++)
            {
                NvAPIWrapper.Display.PathInfo info = NvAPIWrapper.Display.PathInfo.GetDisplaysConfig()[i];
                if(info.IsGDIPrimary)
                {
                    return allDisplays[i];
                }
            }

            return null;
            
        }
    }
}
