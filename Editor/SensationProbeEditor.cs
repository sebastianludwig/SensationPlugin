using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[CustomEditor(typeof(SensationProbe))]
[CanEditMultipleObjects]
public class SensationProbeEditor : Editor {
	private SensationProbe probe;
	
	void OnEnable() {
		probe = (SensationProbe)target;
	}
	
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
	}
	
	[DrawGizmo(GizmoType.Selected)]
	static void SelectedGizmos(SensationProbe probe, GizmoType gizmoType) {
		Vector3 worldOrigin = probe.transform.TransformPoint(probe.origin);
		Vector3 worldDirection = probe.transform.localToWorldMatrix * (probe.direction.normalized * probe.reach);
		Vector3 worldTarget = worldOrigin + worldDirection;

		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(worldOrigin, worldTarget);
		
		Handles.color = Color.cyan;
		Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, worldDirection.normalized);
		float size = HandleUtility.GetHandleSize(worldTarget) * 0.15f;
		Handles.ConeCap(0, worldTarget - size * 0.5f * worldDirection.normalized, rotation, size);
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

	private void SetDirection(SensationProbe probe, Vector3 worldOrigin, Vector3 worldTarget) {
		Vector3 newDirection = worldTarget - worldOrigin;
		newDirection = probe.transform.worldToLocalMatrix * newDirection;
		
		probe.reach = newDirection.magnitude;
		
		newDirection.Normalize();
		probe.direction = new Vector3((float)Math.Round(newDirection.x, 4), (float)Math.Round(newDirection.y, 4), (float)Math.Round(newDirection.z, 4));
	}
}
