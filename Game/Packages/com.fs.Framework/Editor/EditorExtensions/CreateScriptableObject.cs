using UnityEditor;
using UnityEngine;

public class ScriptableObjectCreator : UnityEditor.Editor
{
    [MenuItem("Assets/Create/ThisScriptableObject", false, 1)]
    public static void CreateManager()
    {
        ScriptableObject asset = ScriptableObject.CreateInstance(Selection.activeObject.name);
        AssetDatabase.CreateAsset(asset, AssetDatabase.GetAssetPath(Selection.activeObject).Replace(".cs", ".asset"));
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}