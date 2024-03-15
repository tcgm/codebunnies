using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using System.IO;
using System.Text.RegularExpressions;

[CustomEditor(typeof(MonoBehaviour), true)]
public class CustomInspectorMonoBehaviour : Editor
{
    private string regex = @"\bclass\s+\w+\s*:\s*\w+\s*\{";

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag Here");
        GUI.Label(dropArea, "Hold SHIFT when dropping to choose Components.");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        GameObject draggedGameObject = draggedObject as GameObject;
                        if (draggedGameObject != null)
                        {
                            string propName = draggedObject.name == null ?
                                draggedObject.GetType().Name : draggedObject.name;

                            // Create a new serialized property for the GameObject reference
                            SerializedProperty gameObjectProperty =
                                serializedObject.FindProperty(
                                    propName);

                            if (gameObjectProperty == null)
                            {
                                MonoBehaviour script
                                    = (MonoBehaviour)target;
                                MonoScript monoScript = MonoScript.FromMonoBehaviour(script);
                                string scriptPath = AssetDatabase.GetAssetPath(monoScript);

                                if (string.IsNullOrEmpty(scriptPath))
                                {
                                    Debug.LogError("Script file path not found.");
                                    return;
                                }

                                // Show context menu if shift key is held
                                if ((evt.modifiers & EventModifiers.Shift) != 0)
                                {
                                    // Create a menu to list all components attached to the dropped GameObject
                                    GenericMenu menu = new GenericMenu();

                                    Component[] components = draggedGameObject.GetComponents<Component>();
                                    foreach (Component component in components)
                                    {
                                        string compName = propName 
                                            + component.GetType().Name
                                            + DateTime.Now.Millisecond;

                                        // Add menu item for each component
                                        menu.AddItem(new GUIContent(component.GetType().Name), false, () =>
                                        {
                                            // Add the selected component to the MonoBehaviour script
                                            AddComponentToScript(scriptPath, component.GetType(), compName);
                                        });
                                    }

                                    // Show the context menu
                                    menu.ShowAsContext();
                                }
                                else
                                {
                                    string newVariableDeclaration = "public "
                                        + draggedObject.GetType().Name
                                        + " "
                                        + propName
                                        + DateTime.Now.Millisecond
                                        + ";";

                                    // Read the content of the script file
                                    string scriptContent = File.ReadAllText(scriptPath);

                                    // Find the index of the first opening curly bracket after the class declaration
                                    Match match = Regex.Match(scriptContent,
                                        regex);
                                    if (!match.Success)
                                    {
                                        Debug.LogError("Failed to find the insertion point in the script.");
                                        return;
                                    }
                                    int insertionIndex = match.Index + match.Length;

                                    // Insert the new variable declaration
                                    newVariableDeclaration =
                                        "\n\t// Your new variable declaration here\n\t"
                                        + newVariableDeclaration;

                                    scriptContent = scriptContent.Insert(insertionIndex, newVariableDeclaration);

                                    // Write the modified script content back to the file
                                    File.WriteAllText(scriptPath, scriptContent);

                                    Debug.Log("Script modified successfully.");

                                    // If the property doesn't exist, create a new one
                                    serializedObject.Update();
                                    gameObjectProperty = serializedObject.FindProperty(propName);
                                    serializedObject.ApplyModifiedProperties();
                                }
                            }

                            // Assign the dragged GameObject to the serialized property
                            if (gameObjectProperty != null) gameObjectProperty.objectReferenceValue = draggedGameObject;

                            // Apply modifications
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
                Event.current.Use();
                break;
        }

        serializedObject.ApplyModifiedProperties();

        // Draw default inspector properties
        DrawDefaultInspector();

        // Mark the target MonoBehaviour as dirty to refresh the inspector
        EditorUtility.SetDirty(target);
    }

    private void AddComponentToScript(string scriptPath, Type componentType, string propName)
    {
        if (!File.Exists(scriptPath))
        {
            Debug.LogError("Script file not found at path: " + scriptPath);
            return;
        }

        // Read the content of the script file
        string scriptContent = File.ReadAllText(scriptPath);

        // Find the index of the first opening curly bracket after the class declaration
        Match match = Regex.Match(scriptContent, regex);
        if (!match.Success)
        {
            Debug.LogError("Failed to find the insertion point in the script.");
            return;
        }
        int insertionIndex = match.Index + match.Length;

        // Insert the new variable declaration
        string newVariableDeclaration = $"\n\t// Your new variable declaration here\n\tpublic {componentType.Name} {propName};\n";
        scriptContent = scriptContent.Insert(insertionIndex, newVariableDeclaration);

        // Write the modified script content back to the file
        File.WriteAllText(scriptPath, scriptContent);

        // Mark the target MonoBehaviour as dirty to refresh the inspector
        EditorUtility.SetDirty(target);

        Debug.Log($"Component {componentType.Name} added to the script successfully.");
    }
}
