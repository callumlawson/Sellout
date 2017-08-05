using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.GameActions.Stories;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class DayThreeMorning
    {
        public static void Start(List<Entity> matchingEntities) {
            
            var loveStoryActions = LoveStory.DayTwoMorning();
            foreach (var actionPair in loveStoryActions)
            {
                var entity = actionPair.GetEntity();
                var action = actionPair.GetGameAction();
                ActionManagerSystem.Instance.QueueAction(entity, action);
            }
        }
    }
}
