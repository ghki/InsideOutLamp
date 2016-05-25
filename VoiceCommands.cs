using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace LampModule3
{
    public class VoiceCommands
    {
        private static bool onStateChangeRequested = false;
        private enum Emotion { NEUTRAL, HAPPY, SAD, ANGRY, SCARED};
        private static uint bright;
        private static uint hue;
        private static uint sat;


        public async static void RegisterVoiceCommands()
        {
            StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///LampVoiceCommands.xml"));
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);
        }

        public static void ProcessVoiceCommand(VoiceCommandActivatedEventArgs eventArgs)
        {
            String voice_input = eventArgs.Result.Text;  //SpeechRecognitionResult.text to get the FULL text result.
            Emotion current_emotion = ProcessEmotion(voice_input);
            
            switch (current_emotion)
            {
                case Emotion.NEUTRAL:
                    bright = 1314475648;
                    sat = 215616736;
                    hue = 430512544;
                    break;
                case Emotion.HAPPY:
                    bright = 2695536896;
                    sat = 3611612928;
                    hue = 647571072;
                    break;
                case Emotion.SAD:
                    bright = 223270497;
                    sat = 3611612928;
                    hue = 2659819264;
                    break;
                case Emotion.ANGRY:
                    bright = 277872639;
                    sat = 3611612928;
                    hue = 0;
                    break;
                case Emotion.SCARED:
                    bright = 2068656512;
                    sat = 4060738048;
                    hue = 3213475840;
                    break;
            }

            LampHelper lampHelper = new LampHelper();
            lampHelper.LampFound += LampHelper_LampFound;
        }

        private static void LampHelper_LampFound(object sender, EventArgs e)
        {
            LampHelper lampHelper = sender as LampHelper;
            lampHelper.SetBrightnessAsync(bright);
            lampHelper.SetSaturationAsync(sat);
            lampHelper.SetHueAsync(hue);
        }

        private static Emotion ProcessEmotion(String voice_command)
        {
            int overall_score = 0;
            int happy_score = 0;
            int sad_score = 0;
            int angry_score = 0;
            int scared_score = 0;

            String pattern = @"\s+";
            String[] command_words = Regex.Split(voice_command, pattern);

            happy_score = GetEmotionScore("happy.txt", Emotion.HAPPY, command_words);
            sad_score = GetEmotionScore("sad.txt", Emotion.SAD, command_words);
            angry_score = GetEmotionScore("angry.txt", Emotion.ANGRY, command_words);
            scared_score = GetEmotionScore("scared.txt", Emotion.SCARED, command_words);

            overall_score = happy_score + sad_score + angry_score + scared_score;

            int[] scores = { happy_score, Math.Abs(sad_score), Math.Abs(angry_score), Math.Abs(scared_score) };
            int max_score = scores.Max();

            if (overall_score == 0)
                return Emotion.NEUTRAL;
            else if (happy_score == max_score)
                return Emotion.HAPPY;
            else if (Math.Abs(sad_score) == max_score)
                return Emotion.SAD;
            else if (Math.Abs(angry_score) == max_score)
                return Emotion.ANGRY;
            else if (Math.Abs(scared_score) == max_score)
                return Emotion.SCARED;
            else
                return Emotion.NEUTRAL;
        }

        private static int GetEmotionScore(String filename, Emotion emotion, String[] command_words)
        {
            int emo_score = 0;

            String pattern = @"\s+";

            StreamReader reader = File.OpenText(filename);

            String current_line = reader.ReadLine();

            while (current_line != null)
            {
                String[] line = Regex.Split(current_line, pattern);
                if (line.Length == 2)
                {
                    for (int i = 0; i < command_words.Length; i++)
                    {
                        if (line[0].Equals(command_words[i]))
                        {
                            emo_score += Int32.Parse(line[1]);
                        }
                    }
                }
                current_line = reader.ReadLine();
            }

            return emo_score;
        }
    }
}
