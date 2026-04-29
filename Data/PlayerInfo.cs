using Godot;
using Godot.Collections;

namespace CardDemo.Data
{
    public partial class PlayerInfo : Resource
    {
        //存储时的时间戳
        [Export]
        public long nowTime;
        [Export]
        public int money;
        [Export]
        public string PlayerName { get; set; }
        [Export]
        public string Star { get; set; }
        [Export]
        public string Planet { get; set; }
        [Export]
        public string Location { get; set; }
        [Export]
        public int HandMax { get; set; }
        [Export]
        public float HPMax { get; set; }
        [Export]
        public float HPRate { get; set; }
        [Export]
        public float HandCurrent { get; set; }
        [Export]
        public Array<string> Recipes { get; set; }
        [Export]
        public string FolderPath { get; set; }

        [Export]
        public Dictionary<string, DeckSaveCards> Decks { get; set; }

        [Export]
        public int CurrentMapSegmentId { get; set; } = 1;
        [Export]
        public int CurrentMapNodeId { get; set; } = -1;
        [Export]
        public Dictionary<int, Array<int>> VisitedNodesBySegment { get; set; } = new Dictionary<int, Array<int>>();

        public override string ToString()
        {
            return string.Format("PlayerName: {0}, Star: {1}, Planet: {2}, Location: {3}, HandMax: {4}, HPMax: {5}, HPRate: {6}, HandCurrent: {7}, Recipes: {8}, FolderPath: {9}, Decks: {10}",
             PlayerName, Star, Planet, Location, HandMax, HPMax, HPRate, HandCurrent, Recipes, FolderPath, Decks);
        }
    }
}
