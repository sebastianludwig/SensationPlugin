# Sensation Plugin

A Unity Plugin to interact with the SensationDriver.

## Installation

There's no `.unityPackage` yet - sorry, but you've to copy the files manully into your `Assets` directory.

## Usage

### The Hub

- Create an empty GameObject
- Call it "Hub".
- Add a 'Hub': Component > Scripts > Sensation > Hub
- Adjust the Sensation Driver Network Name, if necessary. If you're running the server locally, you probably want to set this to `localhost`.

If you have a server running, you should see a `connection from 127.0.0.1` log entry on the server when your run the scene.

### Probes

A Probe casts a ray and controls an actors intensity depending on the distance where the ray hits another object.

To try it out:

- Create a cube (Game Object > 3D Object > Cube)
- Add a 'Probe': Component > Scripts > Sensation > Probe
<!-- TODO: Add annotated screenshot -->
- Adjust the probe's origin, reach and direction by manipulating the turquoise arrow.
  Initially the origin is in the objects center so the two move handles overlap.
  Select the Hand tool (<kbd>q</kbd>) to modify the origin.
  If you want to hide the probe manipulation handles, collapse the script in the inspector.
- Add a second cube
- Disable "Maximize on Play" on the 'Game' tab, start the scene and open switch back to the 'Scene' tab.
- Move the game object with the probe around so that its probe ray hits the other game object.
  Your server should log some received commands.


Properties:

- 'Region' and 'Actor Index' specify which actor will be controlled by the probe.
  [What's where?](/documentation/actor_numbering.jpg?raw=true)
- 'Origin' specifies the rays start point in object coordinates (`(0, 0, 0)` being the center of the object)
- 'Direction' is the rays direction, also in object coordinates
- 'Reach' defines the length of the ray
- The 'Layer Mask' specifies what objects on which Layers the ray will hit
- The 'Intensity Curve' specifies how a hit is translated into actor intensity.
  The curve always goes from 0 to 1 on the x-axis; 0 represents a hit at the origin of the ray and 1 represents a hit right at the tip of the ray.
  For example an intensity curve from (0, 1) down to (1, 0) means the actor intensity increases linearly the closer a hit moves from the tip towards the origin of the ray.
- The 'Out Of Reach Value' is used to define the actor intensity if the ray does not hit anything.
  The actor can be turned of or the intensity curve value at either 0 or 1 can be used.
- The 'Transmit Interval' defines the time interval in seconds at which the probe transmits it's intensity values to the server.
- To ensure the transmitted value represents the whole interval and not only the current value, all values are averaged with an exponential decay (most recent values have more impact on the result than older values).
  Use 1 to effectively disable the average calculation and use 0.1 to get a sloooow moving average.
- The 'Update Mode' defines when intensities are transmitted to the server: Either at every transmit interval or only if the value changed significantly.
- Use 'Sensitivity' to specify what "significantly" means for you (eg. 1% change in intensity or 50%).


### Patterns

#### Pattern Designer

The Pattern Designer is a tool to design intensity patterns that can be triggered/played on the server once they are transmitted (loaded).
It's usage is not bound to Unity. The saved patterns are serialized protobuf messages that can directly be used by other clients.

Create a pattern:

- Create a new scene (not necessary but suggested)
- Create an empty Game Object
- Add 'Patterns': Component > Scripts > Sensation > Patterns
- Type a name in the 'New Pattern' field, click 'Create' and chose a location where to save the pattern
- Open the 'Animation' tab
- Select 'Curves' at the bottom
- Ensure that your pattern name is selected at the top (under the playback control buttons)
- Click 'Add Property' and expand 'Patterns (Script)' in the popover
- Select an actor that should participate in the pattern
- Design the desired intensity curve. You don't need to worry about the scale, all values will be scaled to lay between 0 and 1
- Repeat the last three steps for all actors that should be part of the pattern
- Click 'Save' in the 'Patterns' script inspector. 
  Ensure the pattern you want to save has a checkmark in front of it.
- Choose a directory somewhere in your `Assets` folder (or directly your `Assets` folder)
- The result will be a `.bytes` file for every pattern

#### Use a pattern

- Add a `public TextAsset pattern;` property to one of your scripts
- Let that property reference one of your saved patterns (one of the `.bytes` files)
- In your code, get hold of a reference to the `Hub` script
- Call `hub.LoadPattern(pattern)` to transfer the pattern to the server
- Call `hub.PlayPattern("<pattern_name>");` to start playing the pattern.
  `<pattern_name>` is the name of the animation, that is whatever you put in the 'New Pattern' text field when creating the pattern.
  