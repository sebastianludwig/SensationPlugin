using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace Sensation {

[CustomEditor(typeof(Probe))]
[CanEditMultipleObjects]
public class ProbeEditor : Editor {
    private Probe probe;
    
    void OnEnable() {
        probe = (Probe)target;
    }
    
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
    }
    
    [DrawGizmo(GizmoType.Selected)]
    static void SelectedGizmos(Probe probe, GizmoType gizmoType) {
        Vector3 worldOrigin = probe.transform.TransformPoint(probe.origin);
        Vector3 worldDirection = probe.transform.localToWorldMatrix * (probe.direction.normalized * probe.reach);
        Vector3 worldTarget = worldOrigin + worldDirection;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(worldOrigin, worldTarget);
        
        Handles.color = Color.cyan;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, worldDirection.normalized);
        float size = HandleUtility.GetHandleSize(worldTarget) * 0.15f;
        Handles.ConeCap(0, worldTarget - size * 0.5f * worldDirection.normalized, rotation, size);

        if (Event.current.shift) {
            Color color = Handles.color;
            color.a = 0.6f;
            var oldMatrix = Gizmos.matrix;
            Gizmos.color = color;
            Gizmos.matrix = Matrix4x4.TRS(worldOrigin, rotation, Vector3.one);
            Gizmos.DrawCube(Vector3.zero, new Vector3(0.3f, 0.3f, 0.005f));
            Gizmos.matrix = oldMatrix;
        }
    }
    
    void OnSceneGUI() {
        Vector3 worldOrigin = probe.transform.TransformPoint(probe.origin);
        Vector3 worldDirection = probe.transform.localToWorldMatrix * (probe.direction.normalized * probe.reach);
        Vector3 worldTarget = worldOrigin + worldDirection;

        Handles.color = Color.cyan;

        bool altPressed = Event.current.alt;

        if (!altPressed) {
            EditorGUI.BeginChangeCheck();
            Vector3 newTarget = Handles.PositionHandle(worldTarget, Quaternion.identity);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(probe, "move probe target");

                SetDirection(probe, worldOrigin, newTarget);

                EditorUtility.SetDirty(probe);
            }
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newOrigin = Handles.PositionHandle(worldOrigin, Quaternion.identity);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(probe, "move probe origin");

            if (!altPressed) {
                SetDirection(probe, newOrigin, worldTarget);
            }

            probe.origin = probe.transform.InverseTransformPoint(newOrigin);

            EditorUtility.SetDirty(probe);
        }
    }

    private void SetDirection(Probe probe, Vector3 worldOrigin, Vector3 worldTarget) {
        Vector3 newDirection = worldTarget - worldOrigin;
        newDirection = probe.transform.worldToLocalMatrix * newDirection;
        
        probe.reach = newDirection.magnitude;
        
        newDirection.Normalize();
        probe.direction = new Vector3((float)Math.Round(newDirection.x, 4), (float)Math.Round(newDirection.y, 4), (float)Math.Round(newDirection.z, 4));
    }
}

}
