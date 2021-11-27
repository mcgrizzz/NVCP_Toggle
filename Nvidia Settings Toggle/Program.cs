using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NvAPIWrapper;
using WindowsDisplayAPI;

namespace NVCP_Toggle
{
    class Program
    {
        static int DefaultVibrance = 50;
        static int DefaultHue = 0;

        static double DefaultBrightness = .50;
        static double DefaultContrast = .50;
        static double DefaultGamma = 1.0;

        static void Main(string[] args)
        {
            try
            {
                NVIDIA.Initialize();
            }
            catch(Exception e)
            {
                Console.WriteLine("\nERROR: Unable to initialize nvidia api");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            IConfigurationRoot config;

            try
            {
                config = GetConfig();
            }catch(Exception e)
            {
                Console.WriteLine("\nERROR: Unable to find or load 'appSettings.json'");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }



            bool allDisplays = Boolean.Parse(config.GetSection("toggleAllDisplays").Value);
            bool keyPressToExit = Boolean.Parse(config.GetSection("keyPressToExit").Value);

            int[] colorSettings = LoadCustomColorSettings(config);
            float[] gammaRamp = LoadCustomGammaRamp(config);

            if (allDisplays)
            {
                Console.WriteLine("Toggling all displays...\n");
                Display[] windowsDisplays = Display.GetDisplays().ToArray();
                NvAPIWrapper.Display.Display[] nvDisplays = NvAPIWrapper.Display.Display.GetDisplays();

                for(int i = 0; i < windowsDisplays.Length; i++)
                {
                    Display windowsDisplay = windowsDisplays[i];
                    NvAPIWrapper.Display.Display nvDisplay = nvDisplays[i];
                    ToggleDisplay(nvDisplay, windowsDisplay, colorSettings, gammaRamp);
                    Console.WriteLine("");
                }
            }
            else
            {
                Console.WriteLine("Toggling primary display...\n");
                NvAPIWrapper.Display.Display nvDisplay = GetNvidiaMainDisplay();
                Display windowsDisplay = GetWindowsDisplay();
                ToggleDisplay(nvDisplay, windowsDisplay, colorSettings, gammaRamp);
            }

            if (keyPressToExit)
            {
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            
        }

        private static void ToggleDisplay(NvAPIWrapper.Display.Display nvDisplay, Display windowsDisplay, int[] colorSettings, float[] gammaRamp) 
        {
            int currentVibrance = nvDisplay.DigitalVibranceControl.CurrentLevel;
            int currentHue = nvDisplay.HUEControl.CurrentAngle;
            Console.WriteLine("Display: " + windowsDisplay.ToPathDisplayTarget().FriendlyName);
            if (currentVibrance == DefaultVibrance && currentHue == DefaultHue)
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
                nvDisplay.DigitalVibranceControl.CurrentLevel = DefaultVibrance;
                nvDisplay.HUEControl.CurrentAngle = DefaultHue;

                windowsDisplay.GammaRamp = new DisplayGammaRamp(DefaultBrightness, DefaultContrast, DefaultGamma);
            }
        }

        private static IConfigurationRoot GetConfig()
        {

            return new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appSettings.json", optional: false)
                .Build();
        }

        private static int[] LoadCustomColorSettings(IConfigurationRoot config)
        {
            int[] colors = new int[2];
            colors[0] = config.GetValue<int>("vibrance");
            colors[1] = config.GetValue<int>("hue");
            return colors;
        }

        private static float[] LoadCustomGammaRamp(IConfigurationRoot config)
        {
            float[] gamma = new float[3];
            gamma[0] = config.GetValue<float>("brightness");
            gamma[1] = config.GetValue<float>("contrast");
            gamma[2] = config.GetValue<float>("gamma");
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
