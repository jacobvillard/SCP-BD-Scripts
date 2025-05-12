
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This attribute is used to mark methods that should be displayed as buttons in the inspector.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Method)]
public class ButtonAttribute : PropertyAttribute { }


#if UNITY_EDITOR

namespace Editor {
    /// <summary>
    /// This class is used to create a custom editor for MonoBehaviour scripts with buttons. 
    /// </summary>
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
