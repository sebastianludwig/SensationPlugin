using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

[CustomEditor(typeof(SensationPatterns))]
public class SensationPatternsEditor : Editor {
	private class ActorLocation {
		public Vibration.Region Region;
		public int Index;
		public ActorLocation(Vibration.Region region, int index) {
			this.Region = region;
			this.Index = index;
		}
	}

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

	private static string UpcaseFirstChar(string s) {
		return char.ToUpper(s[0]) + s.Substring(1).ToLower();
	}

	private static Dictionary<string, ActorLocation> BuildPropertyMappings() {
		Func<string, Vibration.Region> toRegion = delegate(string name) {
			var parts = Array.ConvertAll<string, string>(name.Split('_'), part => UpcaseFirstChar(part));

			string regionName = string.Join("", parts);
			return (Vibration.Region)Enum.Parse(typeof(Vibration.Region), regionName);
		};
		Func<string, string> toName = delegate(string name) {
			var parts = Array.ConvertAll<string, string>(name.Split(new char[] {'_', ' '}), part => UpcaseFirstChar(part));
			parts[0] = parts[0].ToLower();
			return string.Join("", parts);
		};

		// load actor_conf.json
		string[] guids = AssetDatabase.FindAssets("actor_conf");
		if (guids.Length == 0) {
			throw new FileNotFoundException("Could not find 'actor_conf' in asset database");
		}

		string actorConfPath = Path.Combine(ProjectRootPath(), AssetDatabase.GUIDToAssetPath(guids[0]));
		if (guids.Length > 1) {
			Debug.Log("Found more than one 'actor_conf' file - using the one at " + actorConfPath);
		}

		var actorConf = MiniJSON.Json.Deserialize(File.ReadAllText(actorConfPath)) as Dictionary<string, object>;

		// build propertyMappings
		var propertyMappings = new Dictionary<string, ActorLocation>();

		var vibration = (Dictionary<string, object>)actorConf["vibration"];
		var regions = (List<object>)vibration["regions"];
		foreach (Dictionary<string, object> regionProperties in regions) {
			string regionName = toName(regionProperties["name"].ToString());
			Vibration.Region region;
			try {
				region = toRegion(regionProperties["name"].ToString());
			} catch (ArgumentException) {
				Debug.LogError("Could not map region " + regionProperties["name"] + " - skipping region");
				continue;
			}

			var actors = (List<object>)regionProperties["actors"];
			foreach (Dictionary<string, object> actor in actors) {
				string propertyName = regionName + "_" + toName(actor["position"].ToString());
				propertyMappings[propertyName] = new ActorLocation(region, Convert.ToInt32(actor["index"]));
			}
		}

		return propertyMappings;
	}
	
	private SensationPatterns patterns;

	private string newPatternName;

	void OnEnable() {
		patterns = (SensationPatterns)target;
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

	private Track SerializeCurve(AnimationClipCurveData curve, Dictionary<string, ActorLocation> propertyMappings) {
		ActorLocation location = null;
		if (!propertyMappings.TryGetValue(curve.propertyName, out location)) {
			throw new ArgumentException("No actor mapping found for " + curve.propertyName);
		}

		Track track = new Track();
		track.TargetRegion = location.Region;
		track.ActorIndex = location.Index;

		var keyframes = new List<Track.Keyframe>();

		AnimationCurve curveKeys = curve.curve;
		for (int i = 1; i < curveKeys.length; ++i) {
			Track.Keyframe start;
			if (keyframes.Any()) {
				start = keyframes.Last();
			} else {
				start = new Track.Keyframe();
				start.ControlPoint = new Track.Keyframe.Point();
				start.ControlPoint.Time = curveKeys[0].time;
				start.ControlPoint.Value = curveKeys[0].value;

				keyframes.Add(start);
			}

			Track.Keyframe end = new Track.Keyframe();
			end.ControlPoint = new Track.Keyframe.Point();;
			end.ControlPoint.Time = curveKeys[i].time;
			end.ControlPoint.Value = curveKeys[i].value;

			float tangentLength = Mathf.Abs(end.ControlPoint.Time - start.ControlPoint.Time) / 3f;

			start.OutTangentEnd = new Track.Keyframe.Point();
			start.OutTangentEnd.Time = start.ControlPoint.Time + tangentLength;
			start.OutTangentEnd.Value = start.ControlPoint.Value + tangentLength * curveKeys[i - 1].outTangent;

			end.InTangentStart = new Track.Keyframe.Point();
			end.InTangentStart.Time = end.ControlPoint.Time - tangentLength;
			end.InTangentStart.Value = end.ControlPoint.Value - tangentLength * curveKeys[i].inTangent;

			keyframes.Add(end);
		}

		track.Keyframes = keyframes.ToArray();

		return track;
	}

	private LoadPattern Serialize(AnimationClip pattern) {
		var propertyMappings = BuildPropertyMappings();
		var tracks = new List<Track>();
		AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(pattern);
		foreach (var curve in curves) {
			tracks.Add(SerializeCurve(curve, propertyMappings));
		}

		var message = new LoadPattern();
		message.Identifier = pattern.name;
		message.Tracks = tracks.ToArray();
		return message;
	}
	
	private void Export() {
		var path = EditorUtility.OpenFolderPanel("Destination folder", ProjectRootPath(), "");
		if (path == "") {
			return;
		}

		foreach (var pattern in patterns.GetActivePatterns()) {
			LoadPattern message = Serialize(pattern);
//			Debug.Log(message.Identifier);
//			foreach (var track in message.Tracks) {
//				Debug.Log(track.Region + " - " + track.ActorIndex);
//				foreach (var keyframe in track.Keyframes) {
//					Debug.Log("-----------------");
//					if (keyframe.InTangentStart != null) {
//						Debug.Log("InT: " + keyframe.InTangentStart.Time + " - " + keyframe.InTangentStart.Value);
//					}
//					Debug.Log(keyframe.ControlPoint.Time + " - " + keyframe.ControlPoint.Value);
//					if (keyframe.OutTangentEnd != null) {
//						Debug.Log("OutT: " + keyframe.OutTangentEnd.Time + " - " + keyframe.OutTangentEnd.Value);
//					}
//				}
//			}
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
