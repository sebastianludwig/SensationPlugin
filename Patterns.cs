using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Sensation {

[RequireComponent(typeof(Animation))]
public class Patterns : MonoBehaviour {
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


    static Patterns() {
        propertyMappings["chest_upperLeftCenter"] = new ActorLocation(Vibration.Region.Chest, 2);
        propertyMappings["chest_upperFarLeft"] = new ActorLocation(Vibration.Region.Chest, 3);
        propertyMappings["chest_middleLeftCenter"] = new ActorLocation(Vibration.Region.Chest, 6);
        propertyMappings["chest_middleFarLeft"] = new ActorLocation(Vibration.Region.Chest, 7);
        propertyMappings["chest_lowerLeftCenter"] = new ActorLocation(Vibration.Region.Chest, 10);
        propertyMappings["chest_lowerFarLeft"] = new ActorLocation(Vibration.Region.Chest, 11);
        propertyMappings["chest_upperFarRight"] = new ActorLocation(Vibration.Region.Chest, 0);
        propertyMappings["chest_upperRightCenter"] = new ActorLocation(Vibration.Region.Chest, 1);
        propertyMappings["chest_middleFarRight"] = new ActorLocation(Vibration.Region.Chest, 4);
        propertyMappings["chest_middleRightCenter"] = new ActorLocation(Vibration.Region.Chest, 5);
        propertyMappings["chest_lowerFarRight"] = new ActorLocation(Vibration.Region.Chest, 8);
        propertyMappings["chest_lowerRightCenter"] = new ActorLocation(Vibration.Region.Chest, 9);
        propertyMappings["back_upperFarLeft"] = new ActorLocation(Vibration.Region.Back, 0);
        propertyMappings["back_upperLeftCenter"] = new ActorLocation(Vibration.Region.Back, 1);
        propertyMappings["back_middleFarLeft"] = new ActorLocation(Vibration.Region.Back, 4);
        propertyMappings["back_middleLeftCenter"] = new ActorLocation(Vibration.Region.Back, 5);
        propertyMappings["back_lowerFarLeft"] = new ActorLocation(Vibration.Region.Back, 8);
        propertyMappings["back_lowerLeftCenter"] = new ActorLocation(Vibration.Region.Back, 9);
        propertyMappings["back_upperRightCenter"] = new ActorLocation(Vibration.Region.Back, 2);
        propertyMappings["back_upperFarRight"] = new ActorLocation(Vibration.Region.Back, 3);
        propertyMappings["back_middleRightCenter"] = new ActorLocation(Vibration.Region.Back, 6);
        propertyMappings["back_middleFarRight"] = new ActorLocation(Vibration.Region.Back, 7);
        propertyMappings["back_lowerRightCenter"] = new ActorLocation(Vibration.Region.Back, 10);
        propertyMappings["back_lowerFarRight"] = new ActorLocation(Vibration.Region.Back, 11);
        propertyMappings["leftArm_shoulderFront"] = new ActorLocation(Vibration.Region.LeftArm, 0);
        propertyMappings["leftArm_shoulderSide"] = new ActorLocation(Vibration.Region.LeftArm, 1);
        propertyMappings["leftArm_insideUpperArm"] = new ActorLocation(Vibration.Region.LeftArm, 2);
        propertyMappings["leftArm_outsideUpperArm"] = new ActorLocation(Vibration.Region.LeftArm, 3);
        propertyMappings["leftArm_upperForearmBack"] = new ActorLocation(Vibration.Region.LeftArm, 4);
        propertyMappings["leftArm_lowerForearmFront"] = new ActorLocation(Vibration.Region.LeftArm, 5);
        propertyMappings["leftArm_lowerForearmBack"] = new ActorLocation(Vibration.Region.LeftArm, 6);
        propertyMappings["leftHand_thumb"] = new ActorLocation(Vibration.Region.LeftHand, 0);
        propertyMappings["leftHand_indexFinger"] = new ActorLocation(Vibration.Region.LeftHand, 1);
        propertyMappings["leftHand_middleFinger"] = new ActorLocation(Vibration.Region.LeftHand, 2);
        propertyMappings["leftHand_ringFinger"] = new ActorLocation(Vibration.Region.LeftHand, 3);
        propertyMappings["leftHand_pinkie"] = new ActorLocation(Vibration.Region.LeftHand, 4);
        propertyMappings["leftHand_palm"] = new ActorLocation(Vibration.Region.LeftHand, 5);
        propertyMappings["leftHand_backOfHand"] = new ActorLocation(Vibration.Region.LeftHand, 6);
        propertyMappings["rightArm_shoulderFront"] = new ActorLocation(Vibration.Region.RightArm, 0);
        propertyMappings["rightArm_shoulderSide"] = new ActorLocation(Vibration.Region.RightArm, 1);
        propertyMappings["rightArm_insideUpperArm"] = new ActorLocation(Vibration.Region.RightArm, 2);
        propertyMappings["rightArm_outsideUpperArm"] = new ActorLocation(Vibration.Region.RightArm, 3);
        propertyMappings["rightArm_upperForearmBack"] = new ActorLocation(Vibration.Region.RightArm, 4);
        propertyMappings["rightArm_lowerForearmFront"] = new ActorLocation(Vibration.Region.RightArm, 5);
        propertyMappings["rightArm_lowerForearmBack"] = new ActorLocation(Vibration.Region.RightArm, 6);
        propertyMappings["rightHand_thumb"] = new ActorLocation(Vibration.Region.RightHand, 0);
        propertyMappings["rightHand_indexFinger"] = new ActorLocation(Vibration.Region.RightHand, 1);
        propertyMappings["rightHand_middleFinger"] = new ActorLocation(Vibration.Region.RightHand, 2);
        propertyMappings["rightHand_ringFinger"] = new ActorLocation(Vibration.Region.RightHand, 3);
        propertyMappings["rightHand_pinkie"] = new ActorLocation(Vibration.Region.RightHand, 4);
        propertyMappings["rightHand_palm"] = new ActorLocation(Vibration.Region.RightHand, 5);
        propertyMappings["rightHand_backOfHand"] = new ActorLocation(Vibration.Region.RightHand, 6);
        propertyMappings["leftLeg_hip"] = new ActorLocation(Vibration.Region.LeftLeg, 0);
        propertyMappings["leftLeg_buttock"] = new ActorLocation(Vibration.Region.LeftLeg, 1);
        propertyMappings["leftLeg_upperThighOutside"] = new ActorLocation(Vibration.Region.LeftLeg, 2);
        propertyMappings["leftLeg_upperThighFront"] = new ActorLocation(Vibration.Region.LeftLeg, 3);
        propertyMappings["leftLeg_upperThighInside"] = new ActorLocation(Vibration.Region.LeftLeg, 4);
        propertyMappings["leftLeg_upperThighBack"] = new ActorLocation(Vibration.Region.LeftLeg, 5);
        propertyMappings["leftLeg_lowerThighOutside"] = new ActorLocation(Vibration.Region.LeftLeg, 6);
        propertyMappings["leftLeg_lowerTighFront"] = new ActorLocation(Vibration.Region.LeftLeg, 7);
        propertyMappings["leftLeg_lowerThighInside"] = new ActorLocation(Vibration.Region.LeftLeg, 8);
        propertyMappings["leftLeg_lowerTighBack"] = new ActorLocation(Vibration.Region.LeftLeg, 9);
        propertyMappings["leftLeg_upperShankOutside"] = new ActorLocation(Vibration.Region.LeftLeg, 10);
        propertyMappings["leftLeg_upperShankFront"] = new ActorLocation(Vibration.Region.LeftLeg, 11);
        propertyMappings["leftLeg_upperShankInside"] = new ActorLocation(Vibration.Region.LeftLeg, 12);
        propertyMappings["leftLeg_upperShankBack"] = new ActorLocation(Vibration.Region.LeftLeg, 13);
        propertyMappings["leftLeg_lowerShankOutside"] = new ActorLocation(Vibration.Region.LeftLeg, 14);
        propertyMappings["leftLeg_lowerShankFront"] = new ActorLocation(Vibration.Region.LeftLeg, 15);
        propertyMappings["leftLeg_lowerShankInside"] = new ActorLocation(Vibration.Region.LeftLeg, 16);
        propertyMappings["leftLeg_lowerShankBack"] = new ActorLocation(Vibration.Region.LeftLeg, 17);
        propertyMappings["rightLeg_hip"] = new ActorLocation(Vibration.Region.RightLeg, 0);
        propertyMappings["rightLeg_buttock"] = new ActorLocation(Vibration.Region.RightLeg, 1);
        propertyMappings["rightLeg_upperThighOutside"] = new ActorLocation(Vibration.Region.RightLeg, 2);
        propertyMappings["rightLeg_upperThighFront"] = new ActorLocation(Vibration.Region.RightLeg, 3);
        propertyMappings["rightLeg_upperThighInside"] = new ActorLocation(Vibration.Region.RightLeg, 4);
        propertyMappings["rightLeg_upperThighBack"] = new ActorLocation(Vibration.Region.RightLeg, 5);
        propertyMappings["rightLeg_lowerThighOutside"] = new ActorLocation(Vibration.Region.RightLeg, 6);
        propertyMappings["rightLeg_lowerThighFront"] = new ActorLocation(Vibration.Region.RightLeg, 7);
        propertyMappings["rightLeg_lowerThighInside"] = new ActorLocation(Vibration.Region.RightLeg, 8);
        propertyMappings["rightLeg_lowerTighBack"] = new ActorLocation(Vibration.Region.RightLeg, 9);
        propertyMappings["rightLeg_upperShankOutside"] = new ActorLocation(Vibration.Region.RightLeg, 10);
        propertyMappings["rightLeg_upperShankFront"] = new ActorLocation(Vibration.Region.RightLeg, 11);
        propertyMappings["rightLeg_upperShankInside"] = new ActorLocation(Vibration.Region.RightLeg, 12);
        propertyMappings["rightLeg_upperShankBack"] = new ActorLocation(Vibration.Region.RightLeg, 13);
        propertyMappings["rightLeg_lowerShankOutside"] = new ActorLocation(Vibration.Region.RightLeg, 14);
        propertyMappings["rightLeg_lowerShankFront"] = new ActorLocation(Vibration.Region.RightLeg, 15);
        propertyMappings["rightLeg_lowerShankInside"] = new ActorLocation(Vibration.Region.RightLeg, 16);
        propertyMappings["rightLeg_lowerShankBack"] = new ActorLocation(Vibration.Region.RightLeg, 17);
    }

    public class ActorLocation {
        public Vibration.Region Region;
        public int Index;
        public ActorLocation(Vibration.Region region, int index) {
            this.Region = region;
            this.Index = index;
        }
    }

    public static Dictionary<string, ActorLocation> propertyMappings = new Dictionary<string, ActorLocation>();

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

}
