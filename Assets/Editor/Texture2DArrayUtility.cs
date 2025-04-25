using UnityEditor;
using UnityEngine;

public class Texture2DArrayUtility {
    [MenuItem("Assets/Create/Splatmap Texture Array")]
    static void MakeSplatArray() {
        // assume you¡¯ve selected multiple textures in the Project window
        var sources = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
        if (sources.Length == 0) {
            Debug.LogError("Select at least one Texture2D in the Project view.");
            return;
        }

        int w = sources[0].width, h = sources[0].height;
        var fmt = sources[0].format;
        var arr = new Texture2DArray(w, h, sources.Length, fmt, true);

        // copy each slice
        for (int i = 0; i < sources.Length; i++) {
            Graphics.CopyTexture(sources[i], 0, 0, arr, i, 0);
        }

        // save as asset
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Texture2DArray",
            "NewArray",
            "asset",
            "Choose save location"
        );
        if (!string.IsNullOrEmpty(path)) {
            AssetDatabase.CreateAsset(arr, path);
            AssetDatabase.SaveAssets();
        }
    }
}
