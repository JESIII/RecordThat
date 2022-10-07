using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Recognition;
using WindowsInput;

namespace XboxRecordThat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechRecognitionEngine speechRecognitionEngine = new SpeechRecognitionEngine();
        InputSimulator inputSimulator = new InputSimulator();
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
            listBox.Items.Add("Clip that");
            Choices commands = new Choices();
            commands.Add(listBox.Items.OfType<string>().ToArray());
            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);
            Grammar grammar = new Grammar(gBuilder);
            speechRecognitionEngine.LoadGrammarAsync(grammar);
            speechRecognitionEngine.SetInputToDefaultAudioDevice();
            speechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
        }

        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.LWIN);
            inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.MENU);
            inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_G);
            inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_G);
            inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.MENU);
            inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.LWIN);
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
            speechRecognitionEngine.LoadGrammarAsync(grammar);
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
                speechRecognitionEngine.LoadGrammarAsync(grammar);
            }
        }
    }
}
