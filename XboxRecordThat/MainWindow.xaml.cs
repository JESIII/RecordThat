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
        List<string> commands = new List<string> { "Xbox record that", "Record that", "Clip that", "Clip it" };
        List<string> overlayTypes = new List<string> { "Gamebar", "Shadowplay" };
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
            
            listBox.ItemsSource = commands;
            overlayDropdown.ItemsSource = overlayTypes;
            overlayDropdown.SelectedIndex = 0;
            Grammar grammar = new Grammar(new GrammarBuilder(new Choices (commands.ToArray())));
            grammar.Name = "customGrammar";
            speechRecognitionEngine.LoadGrammarAsync(grammar);
            speechRecognitionEngine.LoadGrammarAsync(new DictationGrammar());
            speechRecognitionEngine.SetInputToDefaultAudioDevice();
            speechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
        }

        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > slider.Value/100 && e.Result.Grammar.Name == "customGrammar")
            {
                switch (overlayDropdown.SelectedItem.ToString())
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
            commands.Add(textBox.Text);
            Grammar grammar = new Grammar(new GrammarBuilder(new Choices(commands.ToArray())));
            grammar.Name = "customGrammar";
            speechRecognitionEngine.LoadGrammarAsync(grammar);
            speechRecognitionEngine.LoadGrammarAsync(new DictationGrammar());
        }

        private void removePhrase(object sender, RoutedEventArgs e)
        {
            commands.RemoveAt(listBox.SelectedIndex);
            speechRecognitionEngine.UnloadAllGrammars();
            if (commands.Count > 0)
            {
                Grammar grammar = new Grammar(new GrammarBuilder(new Choices(commands.ToArray())));
                grammar.Name = "customGrammar";
                speechRecognitionEngine.LoadGrammarAsync(grammar);
                speechRecognitionEngine.LoadGrammarAsync(new DictationGrammar());
            }
        }
    }
}
