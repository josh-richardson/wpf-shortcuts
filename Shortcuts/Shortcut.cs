using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Shortcuts {
    class Shortcut {
        private readonly MainWindow _window;
        private readonly String _methodName;
        private readonly string _description;
        public List<ShortcutKey> ShortcutKeys { get; set; }

        public Shortcut(MainWindow window, string methodName, List<ShortcutKey> shortcuts, string description) {
            ShortcutKeys = shortcuts;
            _description = description;
            _window = window;
            _methodName = methodName;
        }

        public void Invoke() {
            var t = _window.GetType();
            var method = t.GetMethod(_methodName);
            method.Invoke(_window, null);
        }

        public JObject Serialize() {
            return new JObject(new JProperty("methodName", _methodName), new JProperty("description", _description), new JProperty("shortcuts", new JArray(from i in ShortcutKeys select i.Serialize())));
        }

        public static Shortcut Deserialize(MainWindow window, string json) {
            var obj = JObject.Parse(json);
            return new Shortcut(window, obj["methodName"].ToString(), JArray.Parse(obj["shortcuts"].ToString()).Select(jToken => ShortcutKey.Deserialize(jToken.Value<JObject>())).ToList(), obj["description"].ToString());
        }

        public string GetMethodName() {
            return _methodName;
        }

        public override string ToString() {
            return _description;
        }

    }
}
