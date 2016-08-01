using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace Shortcuts {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        readonly List<Shortcut> _shortcuts = new List<Shortcut>();
        private bool _shouldInvoke = true;
        public MainWindow() {
            InitializeComponent();
            if (File.Exists("shortcuts.json")) {
                btnLoad_Click(this, null);
            }

            if (_shortcuts.Count == 0) {
                _shortcuts.Add(new Shortcut(this, "ShowMessage", new List<ShortcutKey> { new ShortcutKey(Key.A, ModifierKeys.Control) }, "Show a messagebox"));
                _shortcuts.Add(new Shortcut(this, "ChangeColour", new List<ShortcutKey> { new ShortcutKey(Key.B, ModifierKeys.Control) }, "Change background colour"));
                _shortcuts.Add(new Shortcut(this, "ResetColour", new List<ShortcutKey> { new ShortcutKey(Key.Z, (ModifierKeys.Control | ModifierKeys.Alt)) }, "Reset background colour"));
            }
            
            lstActions.ItemsSource = _shortcuts;
        }

        public void ShowMessage() {
            MessageBox.Show("Hello there, this is a message!");
        }

        public void ChangeColour() {
            Background = Brushes.Red;
        }

        public void ResetColour() {
            Background = Brushes.White;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            base.OnPreviewKeyDown(e);
            if (!_shouldInvoke) return;
            foreach (var shortcut in from shortcut in _shortcuts from shortcutKey in shortcut.ShortcutKeys.Where(shortcutKey => Keyboard.Modifiers == shortcutKey.Modifiers && Keyboard.IsKeyDown(shortcutKey.Key)) select shortcut) {
                shortcut.Invoke();
            }
        }


        private void btnLoad_Click(object sender, RoutedEventArgs e) {
            var array = JObject.Parse(File.ReadAllText("shortcuts.json")).GetValue("shortcuts");
            foreach (var jToken in array) {
                _shortcuts.Add(Shortcut.Deserialize(this, jToken.ToString()));
            }
            lstActions.Items.Refresh();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            var arr = new JArray();
            foreach (var shortcut in _shortcuts) {
                arr.Add(shortcut.Serialize());
            }
            var toSave = new JObject(new JProperty("shortcuts", arr));
            File.WriteAllText("shortcuts.json", toSave.ToString());
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e) {
            e.Handled = true;
            if (Keyboard.Modifiers == ModifierKeys.None) return;
            _shouldInvoke = false;
            ((ShortcutKey)lstShortcuts.SelectedItem).Key = e.Key;
            ((ShortcutKey)lstShortcuts.SelectedItem).Modifiers = Keyboard.Modifiers;
            textBox.Text = ((ShortcutKey)lstShortcuts.SelectedItem).ToString();
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e) {
            new Thread(() => {
                Thread.Sleep(1000);
                _shouldInvoke = true;
            }).Start();
            lstShortcuts.Items.Refresh();
        }

        private void lstActions_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            lstShortcuts.ItemsSource = ((Shortcut) e.AddedItems[0]).ShortcutKeys;
            lstShortcuts.SelectedIndex = 0;
        }

    }
}
