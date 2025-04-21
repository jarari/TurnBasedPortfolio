using UnityEngine;
using UnityEditor;
using System.Linq;

public class AnimationClipMerger : EditorWindow {
    private AnimationClip clipA;
    private AnimationClip clipB;
    private string newClipName = "MergedClip";

    [MenuItem("Assets/Animation/Merge Two Clips¡¦")]
    static void OpenWindow() {
        var wnd = GetWindow<AnimationClipMerger>();
        wnd.titleContent = new GUIContent("Merge Clips");
        wnd.Show();
    }

    void OnGUI() {
        EditorGUILayout.LabelField("Select two clips to merge:", EditorStyles.boldLabel);

        // Watch for changes to clipA
        EditorGUI.BeginChangeCheck();
        clipA = (AnimationClip)EditorGUILayout.ObjectField("Clip A", clipA, typeof(AnimationClip), false);
        if (EditorGUI.EndChangeCheck() && clipA != null) {
            // Initialize newClipName when the user picks clipA
            newClipName = clipA.name;
        }

        clipB = (AnimationClip)EditorGUILayout.ObjectField("Clip B", clipB, typeof(AnimationClip), false);
        newClipName = EditorGUILayout.TextField("New Clip Name", newClipName);

        GUI.enabled = clipA != null && clipB != null;
        if (GUILayout.Button("Create Merged Clip"))
            MergeClips();
        GUI.enabled = true;
    }

    void MergeClips() {
        // Create new asset
        var path = AssetDatabase.GetAssetPath(clipA);
        var folder = System.IO.Path.GetDirectoryName(path);
        var newPath = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{newClipName}.anim");
        var merged = new AnimationClip { frameRate = clipA.frameRate };

        // Helper: copy all curves from src into dest
        void CopyCurves(AnimationClip src) {
            foreach (var binding in AnimationUtility.GetCurveBindings(src)) {
                var curve = AnimationUtility.GetEditorCurve(src, binding);
                AnimationUtility.SetEditorCurve(merged, binding, curve);
            }
            // Copy events too
            var events = AnimationUtility.GetAnimationEvents(src);
            if (events != null && events.Length > 0)
                AnimationUtility.SetAnimationEvents(merged, events);
        }

        // Pull A then B; B's curves will overwrite any A curves on same paths/properties
        CopyCurves(clipA);
        CopyCurves(clipB);

        // Save the asset
        AssetDatabase.CreateAsset(merged, newPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = merged;
        Debug.Log($"Created merged clip at: {newPath}");
    }
}
