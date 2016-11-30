using NodeEditorFramework;
using System;
using UnityEngine;

namespace Assets.Scripts.Editor.StoryEditor
{
    [Node(false, "Story/Generic")]
    class StoryNode : Node
    {   
        private const string ID = "dayNode";
        public override string GetID { get { return ID; } }

        public override bool AllowRecursion { get { return false; } }

#pragma warning disable 649
        [SerializeField] private DayOfWeek day;
#pragma warning restore 649

        public override Node Create(Vector2 pos)
        {
            StoryNode node = CreateInstance<StoryNode>();

            node.rect = new Rect(pos.x, pos.y, 100, 100);
            node.name = "StoryNode";

            node.CreateOutput("Output Bottom", "Bool", NodeSide.Bottom, 50);

            return node;
        }

        protected override void NodeGUI()
        {
            GUILayout.Label(day.ToString());
        }

        public override bool Calculate()
        {
            Outputs[0].SetValue(true);
            return true;
        }
    }
    
}
