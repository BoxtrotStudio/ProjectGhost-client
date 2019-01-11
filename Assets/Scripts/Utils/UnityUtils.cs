#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
using UnityEditor;
using System.IO;

public static class UnityUtils
{
    public static string GetSelectedPath()
    {
        string path = "Assets";
		
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if ( !string.IsNullOrEmpty(path) && File.Exists(path) ) 
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}
#endif