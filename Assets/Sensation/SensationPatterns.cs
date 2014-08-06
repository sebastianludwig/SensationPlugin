using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Animation))]
public class SensationPatterns : MonoBehaviour {
	public float leftHand_thumb;
	public float leftHand_indexFinger;
	public float leftHand_middleFinger;
	public float leftHand_ringFinger;
	public float leftHand_pinkie;
	public float leftHand_palm;

	public AnimationCurve demoCurve;

	private Dictionary<string, bool> patternSelections = new Dictionary<string, bool>();

	private List<AnimationClip> GetPatterns() {
		var patterns = new List<AnimationClip>();
		foreach (AnimationState state in animation) {
			if (!state.clip) {
				continue;
			}
			patterns.Add(state.clip);
		}
		return patterns;
	}

	public List<string> GetPatternNames() {
		return GetPatterns().ConvertAll<string>(clip => clip.name);
	}

	public List<AnimationClip> GetActivePatterns() {
		var patterns = GetPatterns();
		patterns.RemoveAll(clip => !IsPatternActive(clip.name));
		return patterns;
	}

	public void AddPattern(AnimationClip clip, string name) {
		animation.AddClip(clip, name);
		patternSelections[name] = true;
	}

	public void SetPatternActive(string name, bool active) {
		patternSelections[name] = active;
	}

	public bool IsPatternActive(string name) {
		bool selected;
		if (patternSelections.TryGetValue(name, out selected)) {
			return selected;
		}
		return true;
	}
}
