using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WindowsInput;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using SpeechRecognizer = Microsoft.CognitiveServices.Speech.SpeechRecognizer;
using Microsoft.CognitiveServices.Speech.Intent;

namespace XboxRecordThat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> overlayTypes;
        private KeywordRecognitionModel keywordModel;
        private KeywordRecognizer keywordRecognizer;
        private SpeechRecognizer speechRecognizer;
        private AudioConfig audioConfig;
        private SpeechConfig speechConfig;
        private InputSimulator inputSimulator = new InputSimulator();
        private int selectedOverlay = 0;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void button_Enable_Click(object sender, RoutedEventArgs e)
        {
            speechRecognizer.StartKeywordRecognitionAsync(keywordModel);
            activeIndicator.Fill = new SolidColorBrush(Colors.LimeGreen);
        }
        private void button_Disable_Click(object sender, RoutedEventArgs e)
        {
            speechRecognizer.StopKeywordRecognitionAsync();
            activeIndicator.Fill = new SolidColorBrush(Colors.Red);
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            overlayTypes = new List<string> { "Gamebar", "Shadowplay" };
            overlayDropdown.ItemsSource = overlayTypes;
            overlayDropdown.SelectedIndex = 0;
            selectedOverlay = 0;
            audioConfig = AudioConfig.FromDefaultMicrophoneInput();

            var subscriptionKey = "{yourkey}";
            var region = "eastus";

            speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);

            // Set up the keyword recognition model
            keywordModel = KeywordRecognitionModel.FromFile("C:\\Users\\John\\source\\repos\\JESIII\\XboxRecordThat\\XboxRecordThat\\SpeechModels\\74f5662d-aee4-4035-abe4-fbdb083bf999.table");

            // Set up the keyword recognizer
            keywordRecognizer = new KeywordRecognizer(audioConfig);

            // Set up the continuous recognition
            speechRecognizer = new SpeechRecognizer(speechConfig);

            // Event handler for recognized speech
            speechRecognizer.Recognized += speechRecognizer_Recognized;
        }

        void speechRecognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
        {
            if (e.Result.Reason != ResultReason.RecognizedKeyword) return;
            Dispatcher.Invoke((() =>
            {
                selectedOverlay = overlayDropdown.SelectedIndex;
            }));
            switch (selectedOverlay)
            {
                case 0:
                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.LWIN);
                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.MENU);
                    inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_G);
                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_G);
                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.MENU);
                    inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.LWIN);
                    break;
                case 1:
                    inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.PAUSE);
                    break;
            }
        }
    }
}
