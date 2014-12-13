using System;
using System.Collections.Generic;


namespace Sensation {

public enum Connection {
    NotDefined,
    Straight,
    Smooth,
    Flat
}

public class TrackBuilder {
    private class KeyframeBuilder {
        private class Tangent {
            public float angleTangens = float.NaN;
            public float time = float.NaN;
            public float value = float.NaN;

            public float angle {
                get {
                    return (float)Math.Atan(angleTangens);
                }
                set {
                    angleTangens = (float)Math.Tan(value);
                }
            }

            public Tangent(float angleTangens) {
                this.angleTangens = angleTangens;
            }
        }

        public float time;
        public float value;
        public Connection leftConnection = Connection.NotDefined;
        public Connection rightConnection = Connection.NotDefined;

        private Tangent inTangent;
        private Tangent outTangent;

        public KeyframeBuilder previous;
        public KeyframeBuilder next;

        public KeyframeBuilder(float time, float value, Connection leftConnection, Connection rightConnection) {
            this.time = time;
            this.value = value;
            this.leftConnection = leftConnection;
            this.rightConnection = rightConnection;
        }

        public KeyframeBuilder(float time, float value, float inAngle, float outAngle) {
            this.time = time;
            this.value = value;
            this.inTangent = new Tangent(inAngle);
            this.outTangent = new Tangent(outAngle);
        }

        private float DeltaTime(float from, float to) {
            var delta = to - from;
            if (Math.Abs(delta) < 0.0001f) {
                delta = 0.0001f * Math.Sign(delta);
            }
            return delta;
        }

        public Track.Keyframe Build() {
            // calculate tangents for straight or flat connection (tan(angle))
            if (previous != null) {
                if (leftConnection != Connection.NotDefined) {
                    var angleTangens = leftConnection == Connection.Flat ? 0 : (value - previous.value) / DeltaTime(previous.time, time);
                    inTangent = new Tangent(angleTangens);
                }
            } else {
                inTangent = null;
                leftConnection = Connection.NotDefined;
            }
            if (next != null) {
                if (rightConnection != Connection.NotDefined) {
                    var angleTangens = rightConnection == Connection.Flat ? 0 : (next.value - value) / DeltaTime(time, next.time);
                    outTangent = new Tangent(angleTangens);
                }
            } else {
                outTangent = null;
                rightConnection = Connection.NotDefined;
            }

            // smooth
            if (inTangent != null && outTangent != null) {
                // the closer to a point, the higher the weight
                var outAngleWeight = DeltaTime(previous.time, time) / DeltaTime(previous.time, next.time);
                var inAngleWeight = 1 - outAngleWeight;
                var averageAngle = inTangent.angle * inAngleWeight + outTangent.angle * outAngleWeight;

                if (leftConnection == Connection.Smooth) {
                    inTangent.angle = averageAngle;
                }
                if (rightConnection == Connection.Smooth) {
                    outTangent.angle = averageAngle;
                }
            }

            // calculate tangents
            if (inTangent != null) {
                var tangentTime = DeltaTime(previous.time, time) / 3f;
                inTangent.time = time - tangentTime;
                inTangent.value = value - tangentTime * inTangent.angleTangens;
            }

            if (outTangent != null) {
                var tangentTime = DeltaTime(time, next.time) / 3f;
                outTangent.time = time + tangentTime;
                outTangent.value = value + tangentTime * outTangent.angleTangens;
            }

            // build result
            var keyframe = new Track.Keyframe();
            keyframe.ControlPoint = BuildPoint(time, value);
            if (inTangent != null) {
                keyframe.InTangentStart = BuildPoint(inTangent.time, inTangent.value);
            }
            if (outTangent != null) {
                keyframe.OutTangentEnd = BuildPoint(outTangent.time, outTangent.value);
            }

            return keyframe;
        }

        private Track.Keyframe.Point BuildPoint(float time, float value) {
            var point = new Track.Keyframe.Point();
            point.Time = time;
            point.Value = value;
            return point;
        }
    }

    private Vibration.Region region;
    private int actorIndex;

    private SortedDictionary<float, KeyframeBuilder> keyframeBuilders = new SortedDictionary<float, KeyframeBuilder>();

    public TrackBuilder(Vibration.Region region, int actorIndex) {
        this.region = region;
        this.actorIndex = actorIndex;
    }

    public TrackBuilder AddKeyframe(float time, float value) {
        return AddKeyframe(time, value, Connection.Smooth);
    }

    public TrackBuilder AddKeyframe(float time, float value, Connection connection) {
        return AddKeyframe(time, value, connection, connection);
    }

    public TrackBuilder AddKeyframe(float time, float value, Connection leftConnection, Connection rightConnection) {
        return AddKeyframeBuilder(new KeyframeBuilder(time, value, leftConnection, rightConnection));
    }

    public TrackBuilder AddKeyframe(float time, float value, float inAngle, float outAngle) {
        return AddKeyframeBuilder(new KeyframeBuilder(time, value, inAngle, outAngle));
    }

    private TrackBuilder AddKeyframeBuilder(KeyframeBuilder builder) {
        keyframeBuilders.Add(builder.time, builder);
        return this;
    }

    public Track Build() {
        KeyframeBuilder previousKeyframeBuilder = null;

        foreach (var builder in keyframeBuilders.Values) {
            builder.previous = previousKeyframeBuilder;
            if (previousKeyframeBuilder != null) {
                previousKeyframeBuilder.next = builder;
            }
            previousKeyframeBuilder = builder;
        }

        var keyframes = new List<Track.Keyframe>();
        foreach (var builder in keyframeBuilders.Values) {
            keyframes.Add(builder.Build());
        }
        
        var track = new Track();
        track.TargetRegion = region;
        track.ActorIndex = actorIndex;
        track.Keyframes = keyframes.ToArray();
        return track;
    }
}

public class PatternBuilder {

    private List<TrackBuilder> trackBuilders = new List<TrackBuilder>();
    private string identifier;

    public PatternBuilder(string identifier) {
        this.identifier = identifier;
    }

    public TrackBuilder AddTrack(Vibration.Region region, int actorIndex) {
        var builder = new TrackBuilder(region, actorIndex);
        trackBuilders.Add(builder);
        return builder;
    }

    public LoadPattern Build() {
        var pattern = new LoadPattern();
        pattern.Identifier = identifier;

        var tracks = new List<Track>();
        foreach (var builder in trackBuilders) {
            tracks.Add(builder.Build());
        }
        pattern.Tracks = tracks.ToArray();

        return pattern;
    }
}

}
