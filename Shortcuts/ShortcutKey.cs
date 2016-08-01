using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json.Linq;


namespace Shortcuts {
    class ShortcutKey {
        public Key Key { get; set; }
        public ModifierKeys Modifiers { get; set; }

        public ShortcutKey(Key key, ModifierKeys modifiers) {
            Modifiers = modifiers;
            Key = key;
        }

        public JObject Serialize() {
            return new JObject(new JProperty("key", (int)Key), new JProperty("modifiers", (int)Modifiers));
        }

        public static ShortcutKey Deserialize(JObject json) {
            return new ShortcutKey((Key) int.Parse(json["key"].ToString()), (ModifierKeys) json["modifiers"].Value<int>());
        }

        public override string ToString() {
            return Modifiers + " + " + Key;
        }
    }
}
