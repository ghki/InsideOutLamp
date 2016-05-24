using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace LampModule3
{
    public class VoiceCommands
    {
        private static uint bright;
        private static uint sat;
        private static uint hue;


        public async static void RegisterVoiceCommands()
        {
            StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///LampVoiceCommands.xml"));
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);
        }

        public static void ProcessVoiceCommand(VoiceCommandActivatedEventArgs eventArgs)
        {
            switch (eventArgs.Result.RulePath[0])
            {
                case "ToggleLamp":
                    string switchableStateChange = eventArgs.Result.SemanticInterpretation.Properties["switchableStateChange"][0];
                    string switchVerb = eventArgs.Result.SemanticInterpretation.Properties["switchVerb"][0];
                    if (string.Equals(switchableStateChange, "sad", StringComparison.OrdinalIgnoreCase) || string.Equals(switchVerb, "lost", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(switchVerb, "hurt", StringComparison.OrdinalIgnoreCase)) 
                    {
                        bright = 223270497;
                        sat = 3611612928;
                        hue = 2659819264;
                    }

                    else if (string.Equals(switchableStateChange, "mad", StringComparison.OrdinalIgnoreCase))
                    {
                        bright = 277872639;
                        sat = 3611612928;
                        hue = 0;
                    }

                    else if (string.Equals(switchableStateChange, "happy", StringComparison.OrdinalIgnoreCase))
                    {
                        bright = 2695536896;
                        sat = 3611612928;
                        hue = 647571072;
                    }

                    else if (string.Equals(switchableStateChange, "scared", StringComparison.OrdinalIgnoreCase))
                    {
                        bright = 2068656512;
                        sat = 4060738048;
                        hue = 3213475840;
                    }

                    else if (string.Equals(switchableStateChange, "disgusted", StringComparison.OrdinalIgnoreCase) || string.Equals(switchableStateChange, "eww", StringComparison.OrdinalIgnoreCase))
                    {
                        bright = 2773329152;
                        sat = 2683609088;
                        hue = 1423791360;
                    }



                    else
                    {
                        bright = 0;
                        sat = 0;
                        hue = 0;
                    }
                    LampHelper lampHelper = new LampHelper();
                    lampHelper.LampFound += LampHelper_LampFound;
                    break;
                default:
                    break;
            }
        }

        private static void LampHelper_LampFound(object sender, EventArgs e)
        {
            LampHelper lampHelper = sender as LampHelper;
            lampHelper.SetBrightnessAsync(bright);
            lampHelper.SetSaturationAsync(sat);
            lampHelper.SetHueAsync(hue);
        }
    }
}
