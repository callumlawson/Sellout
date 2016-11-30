using NodeEditorFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.NPCs.Scheduling
{
    class StoryUtil
    {
        private static NodeCanvas GetNodeCanvas(string path)
        {
            Debug.Log("Loading canvas at " + path);
            NodeCanvas canvas = (NodeCanvas)Resources.Load(path, typeof(NodeCanvas));

            if (canvas == null)
            {
                Debug.LogErrorFormat("Unable to find schedule canvas at path: {0}", path);
            }

            return canvas;
        }

        /*
        public static SceneBehaviour GetCurrentBehaviour(DayTime time, NodeCanvas canvas)
        {
            var days = getInputNodes(canvas);
            var day = days[0];

            if (day.Outputs.Count == 0)
            {
                return null;
            }

            var currentBehaviour = (SceneBehaviourNode)day.Outputs[0].GetNodeAcrossConnection();
            SceneName previousScene = currentBehaviour.GetScene();
            while (currentBehaviour != null)
            {
                if (time.IsInRange(currentBehaviour.GetStartTime(), currentBehaviour.GetEndTime()))
                {
                    return currentBehaviour.GetBehaviour(previousScene);
                }
                if (currentBehaviour.Outputs.Count > 0)
                {
                    previousScene = currentBehaviour.GetScene();
                    currentBehaviour = (SceneBehaviourNode)currentBehaviour.Outputs[0].GetNodeAcrossConnection();
                }
                else {
                    return null;
                }
            }

            return null;
        }

        private static List<Node> getInputNodes(NodeCanvas canvas)
        {
            return canvas.nodes.Where((Node node) => (node.Inputs.Count == 0 && node.Outputs.Count != 0) || node.Inputs.TrueForAll((NodeInput input) => input.connection == null)).ToList();
        }
        */
    }
}
