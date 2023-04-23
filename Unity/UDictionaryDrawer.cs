using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//The drawer for UDictionary, a dictionary class
//that can be serialized by Unity

/// Authors: TCGM
/// Assistants: CGPT4, CGPT3
/// <summary>
/// The drawer for UDictionary, a dictionary class
/// that can be serialized by Unity
/// </summary>
/// Unity Location: Goes in Editor folder
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
[CustomPropertyDrawer(typeof(UDictionary<,>), true)]
public class UDictionaryDrawer : PropertyDrawer
{
    // Constants for the height of a single line and the padding between lines
    private const float lineHeight = 18f;
    private const float padding = 2f;
    private const float removeButtonOffset = 16f;

    // OnGUI is called by Unity when it needs to draw the custom property in the inspector
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // BeginProperty and EndProperty are used to correctly handle multiple properties being drawn together
        EditorGUI.BeginProperty(position, label, property);

        // Get the SerializedProperty objects for the dictionary keys and values
        var keysProperty = property.FindPropertyRelative("keys");
        var valuesProperty = property.FindPropertyRelative("values");

        // Draw the label for the dictionary
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, lineHeight), label);
        // Move the position down by one line so that the keys and values are drawn below the label
        position.y += lineHeight;

        // Increase the indent level so that the keys and values are indented in the inspector
        EditorGUI.indentLevel++;

        // Iterate over each key-value pair in the dictionary
        for (int i = 0; i < keysProperty.arraySize; i++)
        {
            // Draw the key property field on the left side of the inspector
            Rect keyRect = new Rect(position.x, position.y, position.width / 2 - padding * 3f, lineHeight);
            EditorGUI.PropertyField(keyRect, keysProperty.GetArrayElementAtIndex(i), GUIContent.none);

            // Draw the value property field on the right side of the inspector
            Rect valueRect = new Rect(position.x + position.width / 2 + padding - removeButtonOffset, position.y, position.width / 2 - padding * 3f - removeButtonOffset, lineHeight);
            EditorGUI.PropertyField(valueRect, valuesProperty.GetArrayElementAtIndex(i), GUIContent.none);

            // Draw a "Remove" button to the right of the value field, which will remove the entry when clicked
            Rect removeButtonRect = new Rect(position.x + position.width - padding * 3f - removeButtonOffset, position.y, padding * 2f + removeButtonOffset, lineHeight);
            if (GUI.Button(removeButtonRect, "X"))
            {
                keysProperty.DeleteArrayElementAtIndex(i);
                valuesProperty.DeleteArrayElementAtIndex(i);
                break;
            }

            // Move the position down by one line for the next key-value pair
            position.y += lineHeight;
        }

        // Draw an "Add" button at the bottom of the inspector, which will add a new entry when clicked
        Rect addButtonRect = new Rect(position.x + position.width / 4f, position.y, position.width / 2f, lineHeight);
        
        if (GUI.Button(addButtonRect, "Add"))
        {
            // Generate a new unique key
            string newKey = Guid.NewGuid().ToString().Substring(0, 8);

            // Add a new key-value pair to the dictionary
            keysProperty.InsertArrayElementAtIndex(keysProperty.arraySize);
            valuesProperty.InsertArrayElementAtIndex(valuesProperty.arraySize);
            keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1).stringValue = newKey;
        }

        // Decrease the indent level after all key-value pairs have been drawn
        EditorGUI.indentLevel--;

        // EndProperty is used to correctly handle multiple properties being drawn together
        EditorGUI.EndProperty();
    }

    // GetPropertyHeight returns the height of the property in the inspector based on the number of key-value pairs in the dictionary
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var keysProperty = property.FindPropertyRelative("keys");
        // The height of the property is the height of the label plus the height of each key-value pair, plus the height of the "Add" button at the bottom of the inspector
        return lineHeight * (keysProperty.arraySize + 2);
    }
}