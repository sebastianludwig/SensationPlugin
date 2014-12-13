using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;


namespace Sensation {

[CustomEditor(typeof(Patterns))]
public class PatternEditor : Editor {
    private static String MakeRelativePath(string fromPath, string toPath) {
        if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
        if (string.IsNullOrEmpty(toPath))   throw new ArgumentNullException("toPath");
        
        Uri fromUri = new Uri(fromPath);
        Uri toUri = new Uri(toPath);
        
        if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.
        
        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
        String relativePath = Uri.UnescapeDataString(relativeUri.ToString());
        
        if (toUri.Scheme.ToUpperInvariant() == "FILE")
        {
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
        
        return relativePath;
    }

    private static string ProjectRootPath() {
        return Path.GetDirectoryName(Application.dataPath);
    }

    private Patterns patterns;

    private string newPatternName;

    void OnEnable() {
        patterns = (Patterns)target;
    }

    private void CreateNewPattern(string name) {
        AnimationClip pattern = new AnimationClip();

        string path = EditorUtility.SaveFilePanel("Save pattern", Application.dataPath, name + ".anim", "anim");
        if (path.Length == 0) {
            return;
        }

        string relativePath = MakeRelativePath(ProjectRootPath() + '/', path);

        AssetDatabase.CreateAsset(pattern, relativePath);
        AssetDatabase.SaveAssets();

        patterns.AddPattern(pattern, name);
    }

    private LoadPattern Serialize(AnimationClip pattern) {
        PatternBuilder patternBuilder = new PatternBuilder(pattern.name);

        foreach (var curve in AnimationUtility.GetAllCurves(pattern)) {
            Patterns.ActorLocation location;
            if (!Patterns.propertyMappings.TryGetValue(curve.propertyName, out location)) {
                throw new ArgumentException("No actor mapping found for " + curve.propertyName);
            }

            var track = patternBuilder.AddTrack(location.Region, location.Index);

            AnimationCurve curveKeys = curve.curve;
            for (int i = 0; i < curveKeys.length; ++i) {
                track.AddKeyframe(curveKeys[i].time, curveKeys[i].value, curveKeys[i].inTangent, curveKeys[i].outTangent);

            }
        }

        return patternBuilder.Build();
    }
    
    private void Export() {
        var path = EditorUtility.OpenFolderPanel("Destination folder", ProjectRootPath(), "");
        if (path == "") {
            return;
        }

        foreach (var pattern in patterns.GetActivePatterns()) {
            LoadPattern message = Serialize(pattern);

            using (var outputStream = File.Create(Path.Combine(path, pattern.name + ".bytes"))) {
                Serializer.Serialize(outputStream, message);
            }
        }
    }
    
    public override void OnInspectorGUI() {
        GUILayout.BeginHorizontal();
        {
            newPatternName = EditorGUILayout.TextField("New Pattern", newPatternName, GUILayout.MinWidth(30));
            if (GUILayout.Button("Create", GUILayout.ExpandWidth(false))) {
                CreateNewPattern(newPatternName);

                newPatternName = "";
            }
        }
        GUILayout.EndHorizontal();

        foreach (var patternName in patterns.GetPatternNames()) {
            GUILayout.BeginHorizontal();
            {
                var active = EditorGUILayout.ToggleLeft(patternName, patterns.IsPatternActive(patternName));
                patterns.SetPatternActive(patternName, active);
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Save", GUILayout.ExpandWidth(true))) {
            Export();
        }
    }
}

}
