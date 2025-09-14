using Godot;
using Godot.Collections;
public partial class DeckSaveCards : Resource

{
    [Export]
    public Array<PackedScene> CardsSaved { get; set; }
}
