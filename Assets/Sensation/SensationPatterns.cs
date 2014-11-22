using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Animation))]
public class SensationPatterns : MonoBehaviour {
	public float chest_upperLeftCenter;
	public float chest_upperFarLeft;
	public float chest_middleLeftCenter;
	public float chest_middleFarLeft;
	public float chest_lowerLeftCenter;
	public float chest_lowerFarLeft;
	public float chest_upperFarRight;
	public float chest_upperRightCenter;
	public float chest_middleFarRight;
	public float chest_middleRightCenter;
	public float chest_lowerFarRight;
	public float chest_lowerRightCenter;
	public float back_upperFarLeft;
	public float back_upperLeftCenter;
	public float back_middleFarLeft;
	public float back_middleLeftCenter;
	public float back_lowerFarLeft;
	public float back_lowerLeftCenter;
	public float back_upperRightCenter;
	public float back_upperFarRight;
	public float back_middleRightCenter;
	public float back_middleFarRight;
	public float back_lowerRightCenter;
	public float back_lowerFarRight;
	public float leftArm_shoulderFront;
	public float leftArm_shoulderSide;
	public float leftArm_insideUpperArm;
	public float leftArm_outsideUpperArm;
	public float leftArm_upperForearmBack;
	public float leftArm_lowerForearmFront;
	public float leftArm_lowerForearmBack;
	public float leftHand_thumb;
	public float leftHand_indexFinger;
	public float leftHand_middleFinger;
	public float leftHand_ringFinger;
	public float leftHand_pinkie;
	public float leftHand_palm;
	public float leftHand_backOfHand;
	public float rightArm_shoulderFront;
	public float rightArm_shoulderSide;
	public float rightArm_insideUpperArm;
	public float rightArm_outsideUpperArm;
	public float rightArm_upperForearmBack;
	public float rightArm_lowerForearmFront;
	public float rightArm_lowerForearmBack;
	public float rightHand_thumb;
	public float rightHand_indexFinger;
	public float rightHand_middleFinger;
	public float rightHand_ringFinger;
	public float rightHand_pinkie;
	public float rightHand_palm;
	public float rightHand_backOfHand;
	public float leftLeg_hip;
	public float leftLeg_buttock;
	public float leftLeg_upperThighOutside;
	public float leftLeg_upperThighFront;
	public float leftLeg_upperThighInside;
	public float leftLeg_upperThighBack;
	public float leftLeg_lowerThighOutside;
	public float leftLeg_lowerTighFront;
	public float leftLeg_lowerThighInside;
	public float leftLeg_lowerTighBack;
	public float leftLeg_upperShankOutside;
	public float leftLeg_upperShankFront;
	public float leftLeg_upperShankInside;
	public float leftLeg_upperShankBack;
	public float leftLeg_lowerShankOutside;
	public float leftLeg_lowerShankFront;
	public float leftLeg_lowerShankInside;
	public float leftLeg_lowerShankBack;
	public float rightLeg_hip;
	public float rightLeg_buttock;
	public float rightLeg_upperThighOutside;
	public float rightLeg_upperThighFront;
	public float rightLeg_upperThighInside;
	public float rightLeg_upperThighBack;
	public float rightLeg_lowerThighOutside;
	public float rightLeg_lowerThighFront;
	public float rightLeg_lowerThighInside;
	public float rightLeg_lowerTighBack;
	public float rightLeg_upperShankOutside;
	public float rightLeg_upperShankFront;
	public float rightLeg_upperShankInside;
	public float rightLeg_upperShankBack;
	public float rightLeg_lowerShankOutside;
	public float rightLeg_lowerShankFront;
	public float rightLeg_lowerShankInside;
	public float rightLeg_lowerShankBack;

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
