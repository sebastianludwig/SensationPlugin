using ProtoBuf;
using System.ComponentModel;

// disabling unreachable code warnings and warnings about comparisons with null being always false
// both make ToSting() methods a lot easier to write
#pragma warning disable 0429
#pragma warning disable 0472

namespace Sensation {
    
[ProtoContract]
public class Vibration {
    public enum Region {
        Chest = 0,
        Back = 1,
        LeftArm = 2,
        RightArm = 3,
        LeftHand = 4,
        RightHand = 5,
        LeftLeg = 6,
        RightLeg = 7,
    }
    
    
    [ProtoMember(1, IsRequired = true)]
    public Region TargetRegion { get; set; }
    
    [ProtoMember(2, IsRequired = true)]
    public int ActorIndex { get; set; }
    
    [ProtoMember(3, IsRequired = true)]
    public float Intensity { get; set; }
    
    [ProtoMember(4), DefaultValue(100)]
    public int Priority { get; set; }
    
    
    public Vibration() {
        Priority = 100;
    }
    
    public override string ToString() {
        return "{ Vibration:" + " TargetRegion = " + (TargetRegion == null ? "null" : TargetRegion.ToString()) + ";" + " ActorIndex = " + (ActorIndex == null ? "null" : ActorIndex.ToString()) + ";" + " Intensity = " + (Intensity == null ? "null" : Intensity.ToString("G20")) + ";" + " Priority = " + (Priority == null ? "null" : Priority.ToString()) + ";" + " }";
    }
}

[ProtoContract]
public class MuscleStimulation {
    
    public MuscleStimulation() {
    }
    
    public override string ToString() {
        return "{ MuscleStimulation:" + " }";
    }
}

[ProtoContract]
public class Track {
    [ProtoContract]
    public class Keyframe {
        [ProtoContract]
        public class Point {
            [ProtoMember(1, IsRequired = true)]
            public float Time { get; set; }
            
            [ProtoMember(2, IsRequired = true)]
            public float Value { get; set; }
            
            
            public Point() {
            }
            
            public override string ToString() {
                return "{ Point:" + " Time = " + (Time == null ? "null" : Time.ToString()) + ";" + " Value = " + (Value == null ? "null" : Value.ToString()) + ";" + " }";
            }
        }
        
        
        [ProtoMember(1, IsRequired = true)]
        public Point ControlPoint { get; set; }
        
        [ProtoMember(2)]
        public Point InTangentStart { get; set; }
        
        [ProtoMember(3)]
        public Point OutTangentEnd { get; set; }
        
        
        public Keyframe() {
        }
        
        public override string ToString() {
            return "{ Keyframe:" + " ControlPoint = " + (ControlPoint == null ? "null" : ControlPoint.ToString()) + ";" + " InTangentStart = " + (InTangentStart == null ? "null" : InTangentStart.ToString()) + ";" + " OutTangentEnd = " + (OutTangentEnd == null ? "null" : OutTangentEnd.ToString()) + ";" + " }";
        }
    }
    
    
    [ProtoMember(1, IsRequired = true)]
    public Vibration.Region TargetRegion { get; set; }
    
    [ProtoMember(2, IsRequired = true)]
    public int ActorIndex { get; set; }
    
    [ProtoMember(3)]
    public Keyframe[] Keyframes { get; set; }
    
    
    public Track() {
    }
    
    public override string ToString() {
        return "{ Track:" + " TargetRegion = " + (TargetRegion == null ? "null" : TargetRegion.ToString()) + ";" + " ActorIndex = " + (ActorIndex == null ? "null" : ActorIndex.ToString()) + ";" + " Keyframes = " + (Keyframes == null ? "null" : Keyframes.ToString()) + ";" + " }";
    }
}

[ProtoContract]
public class LoadPattern {
    [ProtoMember(1, IsRequired = true)]
    public string Identifier { get; set; }
    
    [ProtoMember(2)]
    public Track[] Tracks { get; set; }
    
    
    public LoadPattern() {
    }
    
    public override string ToString() {
        return "{ LoadPattern:" + " Identifier = " + (Identifier == null ? "null" : Identifier.ToString()) + ";" + " Tracks = " + (Tracks == null ? "null" : Tracks.ToString()) + ";" + " }";
    }
}

[ProtoContract]
public class PlayPattern {
    [ProtoMember(1, IsRequired = true)]
    public string Identifier { get; set; }
    
    [ProtoMember(2), DefaultValue(80)]
    public int Priority { get; set; }
    
    
    public PlayPattern() {
        Priority = 80;
    }
    
    public override string ToString() {
        return "{ PlayPattern:" + " Identifier = " + (Identifier == null ? "null" : Identifier.ToString()) + ";" + " Priority = " + (Priority == null ? "null" : Priority.ToString()) + ";" + " }";
    }
}

[ProtoContract]
public class Message {
    public enum MessageType {
        Vibration = 0,
        MuscleStimulation = 1,
        LoadPattern = 2,
        PlayPattern = 3,
    }
    
    
    [ProtoMember(1, IsRequired = true)]
    public MessageType Type { get; set; }
    
    [ProtoMember(2)]
    public Vibration Vibration { get; set; }
    
    [ProtoMember(3)]
    public MuscleStimulation MuscleStimulation { get; set; }
    
    [ProtoMember(4)]
    public LoadPattern LoadPattern { get; set; }
    
    [ProtoMember(5)]
    public PlayPattern PlayPattern { get; set; }
    
    
    public Message() {
    }
    
    public override string ToString() {
        return "{ Message:" + " Type = " + (Type == null ? "null" : Type.ToString()) + ";" + " Vibration = " + (Vibration == null ? "null" : Vibration.ToString()) + ";" + " MuscleStimulation = " + (MuscleStimulation == null ? "null" : MuscleStimulation.ToString()) + ";" + " LoadPattern = " + (LoadPattern == null ? "null" : LoadPattern.ToString()) + ";" + " PlayPattern = " + (PlayPattern == null ? "null" : PlayPattern.ToString()) + ";" + " }";
    }
}

}


#pragma warning restore 0429
#pragma warning restore 0472
