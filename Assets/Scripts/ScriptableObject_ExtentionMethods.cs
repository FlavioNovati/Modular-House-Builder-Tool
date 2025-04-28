using UnityEditor;
using UnityEngine;

public static class ScriptableObject_ExtentionMethods
{
    public static void Save(this ScriptableObject obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        ScriptableObject scToSave = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject));
        EditorUtility.SetDirty(scToSave);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
