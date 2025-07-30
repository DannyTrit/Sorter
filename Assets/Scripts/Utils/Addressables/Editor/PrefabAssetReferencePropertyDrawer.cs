using UnityEditor;
using UnityEngine;

namespace Sorter.Addressables.Editor
{
    [CustomPropertyDrawer(typeof(PrefabAssetReference<>), true)]
    public class PrefabAssetReferencePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = default;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_reference"), GUIContent.none);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}