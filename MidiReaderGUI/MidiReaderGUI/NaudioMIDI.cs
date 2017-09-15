using System;
using System.Runtime.InteropServices;
using NAudio.Midi;
using System.Windows.Input;
using WindowsInput;

namespace MidiReaderGUI
{

    public class NAudioMIDI
    {
        //[DllImport("coredll.dll")]
        [DllImport("user32.DLL")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.DLL")]
        static extern IntPtr GetForegroundWindow();

        public IntPtr hwnd;

        System.IDisposable Dispose() { midiIn.Close(); midiIn.Dispose(); return null; }
        public MidiIn midiIn;
        private bool monitoring;
        //private int midiInDevice;



        public string[] GetMIDIInDevices()
        {
            // Get a list of devices  
            string[] returnDevices = new string[MidiIn.NumberOfDevices];

            // Get the product name for each device found  
            for (int device = 0; device < MidiIn.NumberOfDevices; device++)
            {
                returnDevices[device] = MidiIn.DeviceInfo(device).ProductName;
            }
            return returnDevices;
        }

        public void StartMonitoring(int MIDIInDevice)
        {
            if (midiIn == null)
            {
                midiIn = new MidiIn(MIDIInDevice);
            }
            midiIn.Start();
            monitoring = true;
            midiIn.MessageReceived += new EventHandler<MidiInMessageEventArgs>(midiIn_MessageReceived);
        }

        [STAThread]
        public void midiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            //MainWindow mywindow = new MainWindow();
            //mywindow.changeMidiInfo(e.Timestamp + " : " + e.RawMessage + " : " + e.MidiEvent.CommandCode.ToString());
            //Console.WriteLine(e.Timestamp + " : " + e.RawMessage + " : " + e.MidiEvent.CommandCode.ToString());
            // Exit if the MidiEvent is null or is the AutoSensing command code  
            if (e.MidiEvent != null && e.MidiEvent.CommandCode == MidiCommandCode.AutoSensing)
            {
                return;
            }
            if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn)
            {
                // As the Command Code is a NoteOn then we need 
                // to cast the MidiEvent to the NoteOnEvent  

                NoteOnEvent ne;
                ne = (NoteOnEvent)e.MidiEvent;
            }
            if (e.MidiEvent.CommandCode == MidiCommandCode.ControlChange)
            {
                ControlChangeEvent cce;
                cce = (ControlChangeEvent)e.MidiEvent;

                //Console.WriteLine(cce.Controller.ToString() + " " + cce.ControllerValue.ToString());
                if (cce.Controller.ToString().Trim().ToLower() == "sostenuto" && cce.ControllerValue > 0)
                {
                    //THIS IS WHERE I NEED TO CHANGE THE PDF PAGE
                    try
                    {
                        IntPtr currentHwnd = GetForegroundWindow();
                        if (currentHwnd != hwnd && !(hwnd.ToInt32() == 0)) { SetForegroundWindow(hwnd); }
                    }
                    catch (Exception err) { }
                    WindowsInput.InputSimulator myInput = new WindowsInput.InputSimulator();
                    myInput.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.DOWN);

                }

                if (cce.Controller.ToString().Trim().ToLower() == "softpedal" && cce.ControllerValue > 0)
                {
                    //THIS IS WHERE I NEED TO CHANGE THE PDF PAGE
                    try
                    {
                        IntPtr currentHwnd = GetForegroundWindow();
                        if (currentHwnd != hwnd && !(hwnd.ToInt32() == 0)) { SetForegroundWindow(hwnd); }
                    }
                    catch (Exception err) { }
                    WindowsInput.InputSimulator myInput = new WindowsInput.InputSimulator();
                    myInput.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.UP);

                }
            }
            return;
        }
    }
}