using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Speech.Recognition;
using WindowsInput;
using System.Windows.Documents;
using System.Collections.Generic;

namespace XboxRecordThat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechRecognitionEngine speechRecognitionEngine = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
        InputSimulator inputSimulator = new InputSimulator();
        string overlayType = "Gamebar";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Enable_Click(object sender, RoutedEventArgs e)
        {
            speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            activeIndicator.Fill = new SolidColorBrush(Colors.LimeGreen);
        }
        private void button_Disable_Click(object sender, RoutedEventArgs e)
        {
            speechRecognitionEngine.RecognizeAsyncStop();
            activeIndicator.Fill = new SolidColorBrush(Colors.Red);
        }
        private void MainWindow_Load(object sedner, EventArgs e)
        {
            listBox.Items.Add("Xbox record that");
            listBox.Items.Add("Record that");
            listBox.Items.Add("Clip that");
            listBox.Items.Add("Clip it");
            overlayDropdown.ItemsSource = new List<string> { "Gamebar", "Shadowplay"};
            overlayDropdown.SelectedIndex = 0;
            Choices commands = new Choices();
            commands.Add(listBox.Items.OfType<string>().ToArray());
            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);
            Grammar grammar = new Grammar(gBuilder);
            DictationGrammar dg = new DictationGrammar("grammar:dictation#pronunciation");
            dg.Name = "pronun";
            speechRecognitionEngine.LoadGrammarAsync(grammar);
            speechRecognitionEngine.LoadGrammarAsync(dg);
            speechRecognitionEngine.SetInputToDefaultAudioDevice();
            speechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
        }

        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > .70)
            {
                switch (overlayType)
                {
                    case "Gamebar":
                        inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.LWIN);
                        inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.MENU);
                        inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_G);
                        inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_G);
                        inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.MENU);
                        inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.LWIN);
                        break;
                    case "Shadowplay":
                        inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.MENU);
                        inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.F10);
                        inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.F10);
                        inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.MENU);
                        break;
                }
                
            }
        }

        private void addPhrase(object sender, RoutedEventArgs e)
        {
            speechRecognitionEngine.UnloadAllGrammars();
            listBox.Items.Add(textBox.Text);
            Choices commands = new Choices();
            commands.Add(listBox.Items.OfType<string>().ToArray());
            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);
            Grammar grammar = new Grammar(gBuilder);
            DictationGrammar dg = new DictationGrammar("grammar:dictation#pronunciation");
            dg.Name = "Random";
            speechRecognitionEngine.LoadGrammarAsync(grammar);
            speechRecognitionEngine.LoadGrammarAsync(dg);
        }

        private void removePhrase(object sender, RoutedEventArgs e)
        {
            listBox.Items.RemoveAt(listBox.Items.IndexOf(listBox.SelectedItem));
            speechRecognitionEngine.UnloadAllGrammars();
            if (listBox.Items.Count > 0)
            {
                Choices commands = new Choices();
                commands.Add(listBox.Items.OfType<string>().ToArray());
                GrammarBuilder gBuilder = new GrammarBuilder();
                gBuilder.Append(commands);
                Grammar grammar = new Grammar(gBuilder);
                DictationGrammar dg = new DictationGrammar("grammar:dictation#pronunciation");
                dg.Name = "Random";
                speechRecognitionEngine.LoadGrammarAsync(grammar);
                speechRecognitionEngine.LoadGrammarAsync(dg);
            }
        }

        private void overlayDropdown_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            overlayType = overlayDropdown.SelectedItem.ToString();
        }
    }
}
