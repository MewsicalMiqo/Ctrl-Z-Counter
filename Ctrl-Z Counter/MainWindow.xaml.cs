using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Ctrl_Z_Counter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GlobalKeyboardHook _globalKeyboardHook;
        private const string path = "count.txt";
        private int count = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RegisterHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed -= OnKeyPressed;
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
        }

        private void UnregisterHooks()
        {
            _globalKeyboardHook?.Dispose();
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            Keys loggedKey = e.KeyboardData.Key;

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown || e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown)
            {
                if (GlobalKeyboardHook.ModifierKeys.Contains(loggedKey) && !_globalKeyboardHook.IsModifierDown(loggedKey))
                    _globalKeyboardHook.AddModifier(loggedKey);

                if (loggedKey == Keys.Z && (_globalKeyboardHook.IsModifierDown(Keys.LControlKey) || _globalKeyboardHook.IsModifierDown(Keys.RControlKey)))
                {
                    count++;
                    textBox.Text = count.ToString();
                    WriteToFile();
                }
            }
            else if (GlobalKeyboardHook.ModifierKeys.Contains(loggedKey) && _globalKeyboardHook.IsModifierDown(loggedKey))
                _globalKeyboardHook.RemoveModifier(loggedKey);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterHooks();
            WriteToFile();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnregisterHooks();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            count = 0;
            WriteToFile();
        }

        private void WriteToFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Ctrl-Z count:");
            sb.AppendLine(count.ToString());
            File.WriteAllText(path, sb.ToString());
        }
    }
}
