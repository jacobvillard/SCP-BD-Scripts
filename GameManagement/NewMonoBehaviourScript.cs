
using System.Reflection;
using UnityEditor;
using UnityEngine;


[System.AttributeUsage(System.AttributeTargets.Method)]
public class ButtonAttribute : PropertyAttribute { }


#if UNITY_EDITOR

namespace Editor {
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonDrawer : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods) {
                var attributes = method.GetCustomAttributes(typeof(ButtonAttribute), true);
                if (attributes.Length > 0) {
                    if (GUILayout.Button(method.Name)) {
                        method.Invoke(target, null);
                    }
                }
            }
        }
    }
}
#endif
