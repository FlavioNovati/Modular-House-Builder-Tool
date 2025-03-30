using UnityEngine;
using UnityEditor;

using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AssetAdjuster : EditorWindow
{
    [MenuItem("Tools/Adjuster")]
    public static void OpenScatterer() => GetWindow<AssetAdjuster>();

    private SerializedObject _so;

    private string _initial = string.Empty;
    private string _folderPath = string.Empty;
    private string _filter = string.Empty;
    private Vector3 _rotation = Vector3.zero;

    public Material MaterialToApply;
    private SerializedProperty _propMaterial;

    private bool _rename;
    private bool _assignMaterial;

    private void OnEnable()
    {
        _so = new SerializedObject(this);
        _propMaterial = _so.FindProperty("MaterialToApply");
        _rename = false;
        _assignMaterial = false;
    }

    private void OnDisable()
    {
        
    }

    private void OnGUI()
    {
        GUILayout.Label("Path");
        _folderPath = GUILayout.TextField(_folderPath, GUILayout.ExpandWidth(true));
        GUILayout.Label("Filter");
        _filter = GUILayout.TextField(_filter, GUILayout.ExpandWidth(true));

        _rename = GUILayout.Toggle(_rename, "Fix Asset Name", GUILayout.ExpandWidth(true));
        if(_rename)
        {
            GUILayout.Label("Before Name");
            _initial = GUILayout.TextField(_initial, GUILayout.ExpandWidth(true));
        }

        _assignMaterial = GUILayout.Toggle(_assignMaterial, "Assign Material", GUILayout.ExpandWidth(true));
        if (_assignMaterial)
        {
            GUILayout.Label("Material To Add");
            EditorGUILayout.PropertyField(_propMaterial);
        }

        _rotation = EditorGUILayout.Vector3Field("Rotation", _rotation, GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Rename Assets", GUILayout.ExpandWidth(true)))
        {
            //Rename assets
            string[] guids = AssetDatabase.FindAssets(_filter, new[] { _folderPath });
            IEnumerable<string> paths = guids.Select(AssetDatabase.GUIDToAssetPath);
            string[] pathsArray = paths.ToArray();

            Object[] objects = new Object[paths.Count()];
            for (int i = 0; i < pathsArray.Length; i++)
                objects[i] = AssetDatabase.LoadAssetAtPath(pathsArray[i], typeof(object));

            if(_rename)
                for(int i=0; i < objects.Length; i++)
                    RenameAsset(objects[i], pathsArray[i]);
            
            if (MaterialToApply != null && _assignMaterial)
                foreach(Object obj in objects)
                    ApplyMaterial(obj, MaterialToApply);
        }

        if(_so.ApplyModifiedProperties())
        {

        }
    }

    private void RenameAsset(Object obj, string path)
    {
        Undo.RecordObject(obj, $"{obj.name} renamed");

        //Get name
        string assetName = obj.name;

        //Replace spaces with underscore
        assetName.Replace(' ', '_');

        //Get chars array
        List<char> chars = assetName.ToCharArray().ToList();
        chars[0] = char.ToUpper(chars[0]);
        
        for (int i = 1; i < assetName.Length; i++)
        {
            if(chars[i] == '_')
                continue;

            //Preceded by Number Case
            if ((int)chars[i-1] >= 48 && (int)chars[i-1] <= 57)
            {
                //Is not a number
                if(!(chars[i] >= 48 && chars[i] <= 57) && chars[i] != 'x')
                {
                    chars.Insert(i, '_');
                    continue;
                }
            }

            //Preced by underscore Case
            if(assetName[i-1] == '_')
            {
                //'_' followed by number
                if ((int)chars[i] >= 48 && (int)chars[i] <= 57)
                    continue;
                else
                {
                    //If Followed by '_' replace with uppercase char
                    chars[i] = char.ToUpper(chars[i]);
                    continue;
                }
            }
        }

        //if last char is a number
        if(chars[^1] >= 48 && chars[^1] <= 57)
            if (!(chars[^2] >= 48 && chars[^2] <= 57))
                if (chars[^3] != '_')
                    chars.Insert(chars.Count - 1, '_');

        chars.InsertRange(0, _initial);

        assetName = new string(chars.ToArray());
        AssetDatabase.RenameAsset(path, assetName);
    }

    private void ApplyMaterial(Object obj, Material material)
    {
        //If material -> change material
        if ((GameObject)obj)
        {
            GameObject gameObject = (GameObject)obj;
            if (gameObject.TryGetComponent<Renderer>(out Renderer renderer))
                renderer.sharedMaterial = material;
        }
    }
}
