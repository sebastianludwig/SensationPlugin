using ProtoBuf;
using System.ComponentModel;

[ProtoContract]
public class Vibration {
	public enum Region {
		LeftHand = 0,
		LeftForearm = 1,
		LeftUpperArm = 2,
	}
	
	
	[ProtoMember(1, IsRequired = true)]
	public Region TargetRegion { get; set; }
	
	[ProtoMember(2, IsRequired = true)]
	public int ActorIndex { get; set; }
	
	[ProtoMember(3, IsRequired = true)]
	public float Intensity { get; set; }
	
	[ProtoMember(4) , DefaultValue(100)]
	public int Priority { get; set; }
	
}

[ProtoContract]
public class MuscleStimulation {
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
			
		}
		
		
		[ProtoMember(1, IsRequired = true)]
		public Point ControlPoint { get; set; }
		
		[ProtoMember(2)]
		public Point InTangentStart { get; set; }
		
		[ProtoMember(3)]
		public Point OutTangentEnd { get; set; }
		
	}
	
	
	[ProtoMember(1, IsRequired = true)]
	public Vibration.Region Region { get; set; }
	
	[ProtoMember(2, IsRequired = true)]
	public int ActorIndex { get; set; }
	
	[ProtoMember(3)]
	public Keyframe[] Keyframes { get; set; }
	
}

[ProtoContract]
public class LoadPattern {
	[ProtoMember(1, IsRequired = true)]
	public string Identifier { get; set; }
	
	[ProtoMember(2)]
	public Track[] Tracks { get; set; }
	
}

[ProtoContract]
public class PlayPattern {
	[ProtoMember(1, IsRequired = true)]
	public string Identifier { get; set; }
	
	[ProtoMember(2) , DefaultValue(80)]
	public int Priority { get; set; }
	
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
	
}

