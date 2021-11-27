using System;
using System.Configuration;
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
            NvAPIWrapper.Display.Display mainDisplay = GetNvidiaMainDisplay();
            int currentVibrance = mainDisplay.DigitalVibranceControl.CurrentLevel;
            int currentHue = mainDisplay.HUEControl.CurrentAngle;
            Display windowsDisplay = GetWindowsDisplay();

            int[] colorSettings = loadCustomColorSettings();
            double[] gammaRamp = loadCustomGammaRamp();

            if (currentVibrance == defaultVibrance && currentHue == defaultHue)
            {
                //Toggle on
                mainDisplay.DigitalVibranceControl.CurrentLevel = colorSettings[0];
                mainDisplay.HUEControl.CurrentAngle = colorSettings[1];

                windowsDisplay.GammaRamp = new DisplayGammaRamp(gammaRamp[0], gammaRamp[1], gammaRamp[2]);

                Console.WriteLine("Toggling ON");
            }
            else
            {
                //Toggle off
                mainDisplay.DigitalVibranceControl.CurrentLevel = defaultVibrance;
                mainDisplay.HUEControl.CurrentAngle = defaultHue;
                
                windowsDisplay.GammaRamp = new DisplayGammaRamp(defaultBrightness, defaultContrast, defaultGamma );
                Console.WriteLine("Toggling OFF");
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
