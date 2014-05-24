using ProtoBuf;

[ProtoContract]
public class Sensation {
	public enum Region { LeftHand = 0, LeftForearm = 1, LeftUpperArm = 2 }

	[ProtoMember(1)]
    public Region TargetRegion { get; set; }
	
	[ProtoMember(2)]
    public int ActorIndex {get;set;}
	
    [ProtoMember(3)]
    public float Intensity {get;set;}
}
