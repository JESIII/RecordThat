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
            speechRecognitionEngine.RecognizeAsync(RecognizeMode.Single);
            activeIndicator.Fill = new SolidColorBrush(Colors.LimeGreen);
        }
        private void button_Disable_Click(object sender, RoutedEventArgs e)
        {
            speechRecognitionEngine.RecognizeAsyncStop();
            activeIndicator.Fill = new SolidColorBrush(Colors.Red);
        }
        private void MainWindow_Load(object sedner, EventArgs e)
        {
            Choices commands = new Choices();
            commands.Add(new string[] { "Xbox record that" });
            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);
            Grammar grammar = new Grammar(gBuilder);
            speechRecognitionEngine.LoadGrammarAsync(grammar);
            speechRecognitionEngine.SetInputToDefaultAudioDevice();
            speechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
        }

        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "Xbox record that":
                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.LWIN);
                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.MENU);
                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_G);
                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.LWIN);
                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.MENU);
                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_G);
                    break;
            }
        }
    }
}
