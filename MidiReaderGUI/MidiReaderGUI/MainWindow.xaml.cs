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
using System.Diagnostics;

namespace MidiReaderGUI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 

    
    public partial class MainWindow : Window
    {
        static NAudioMIDI MIDI = new NAudioMIDI();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void hwnd_Initialized(object sender, EventArgs e)
        {

        }

        [STAThread]
        public void changeMidiInfo(string midiMessage)
        {
            lastMidiAction.Text = midiMessage;
        }

        private void btn_activateMIDI_Click(object sender, RoutedEventArgs e)
        {
            MIDI.StartMonitoring(MIDI_Devices.SelectedIndex);
            
            var btn_activate = sender as Button;
            btn_activate.IsEnabled = false;
        }

        private void Window_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            { 
                var Window_List = sender as ComboBox;
                //OpenWindowGetter windowGetter = new OpenWindowGetter();
                //foreach(KeyValuePair<IntPtr,string> window in windowGetter.GetOpenWindows())
                foreach (KeyValuePair<IntPtr, string> window in OpenWindowGetter.GetOpenWindows())
                {
                    if(window.Value == Window_List.SelectedValue.ToString())
                    {
                        hwnd.Text = window.Key.ToString();
                        MIDI.hwnd = window.Key;
                    }
                }
            }
            catch(Exception err) { hwnd.Text = err.Message; }

        }

        private void Window_List_DropDownOpened(object sender, EventArgs e)
        {
            List<string> windowList = new List<string>();
            var Window_List = sender as ComboBox;
            Window_List.ItemsSource = windowList;
            //OpenWindowGetter windowGetter = new OpenWindowGetter();
            //Window_List.ItemsSource = windowGetter.GetOpenWindows().Values;
            Window_List.ItemsSource = OpenWindowGetter.GetOpenWindows().Values;
        }

        private void MIDI_Devices_DropDownOpened(object sender, EventArgs e)
        {
            List<string> deviceNames = new List<string>();
            for (int i = 0; i < MIDI.GetMIDIInDevices().Count(); i++) { deviceNames.Add(MIDI.GetMIDIInDevices()[i]); }

            var MIDI_Devices = sender as ComboBox;
            MIDI_Devices.ItemsSource = deviceNames;
            MIDI_Devices.SelectedIndex = 0;
            
        }

        private void MIDI_Devices_Initialized(object sender, EventArgs e)
        {
            List<string> deviceNames = new List<string>();
            for (int i = 0; i < MIDI.GetMIDIInDevices().Count(); i++) { deviceNames.Add(MIDI.GetMIDIInDevices()[i]); }

            var MIDI_Devices = sender as ComboBox;
            MIDI_Devices.ItemsSource = deviceNames;
            MIDI_Devices.SelectedIndex = 0;
            if (MIDI_Devices.Items.Count == 1) { MIDI_Devices.IsEnabled = false; }
        }

        private void Window_List_Initialized(object sender, EventArgs e)
        {
            List<string> windowList = new List<string>();
            var Window_List = sender as ComboBox;
            Window_List.ItemsSource = windowList;
            Window_List.ItemsSource = OpenWindowGetter.GetOpenWindows().Values;
            for(int i=0;i<Window_List.Items.Count;i++)
            {
                if (Window_List.Items[i].ToString().Trim().ToLower().Contains("adobe"))
                {
                    Window_List.SelectedIndex = i;
                }
            }
        }


    }
}
