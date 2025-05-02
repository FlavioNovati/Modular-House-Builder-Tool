using UnityEditor;
using UnityEngine;

public static class ScriptableObject_ExtentionMethods
{
    public static void Save(this ScriptableObject obj)
    {
        EditorUtility.SetDirty(obj);
        AssetDatabase.SaveAssetIfDirty(obj);
    }
}
