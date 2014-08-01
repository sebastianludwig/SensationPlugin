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
		Gizmos.color = Color.cyan;
		Vector3 target = probe.transform.position + probe.transform.rotation * probe.direction.normalized * probe.reach;
		Gizmos.DrawLine(probe.transform.position, target);
		
		Handles.color = Color.cyan;
		Vector3 direction = (target - probe.transform.position).normalized;
		Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, direction);
		float size = HandleUtility.GetHandleSize(target) * 0.15f;
		Handles.ConeCap(0, target - size * 0.5f * direction, rotation, size);
	}
	
	void OnSceneGUI() {
		Handles.color = Color.cyan;
		Vector3 target = probe.transform.position + probe.transform.rotation * probe.direction.normalized * probe.reach;

		EditorGUI.BeginChangeCheck();
		Vector3 newTarget = Handles.PositionHandle(target, Quaternion.identity);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(probe, "move probe target");
			Vector3 newDirection = newTarget - probe.transform.position;
			
			probe.reach = newDirection.magnitude;
			newDirection = Quaternion.Inverse(probe.transform.rotation) * newDirection.normalized * probe.direction.magnitude;
			probe.direction = new Vector3((float)Math.Round(newDirection.x, 4), (float)Math.Round(newDirection.y, 4), (float)Math.Round(newDirection.z, 4));
			

			EditorUtility.SetDirty(probe);
		}
	}
}