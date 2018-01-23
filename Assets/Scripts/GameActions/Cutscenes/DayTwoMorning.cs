using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.NPC;
using Assets.Scripts.GameActions.Stories;
using UnityEngine.Analytics;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class DayTwoMorning
    {
        public static void Start(List<Entity> matchingEntities) {

            Analytics.CustomEvent("Day Two Started");

            var mcGraw = EntityQueries.GetEntityWithName(matchingEntities, NPCS.McGraw.Name);
            var player = EntityQueries.GetEntityWithName(matchingEntities, NPCName.You);

            var loveStoryActions = LoveStory.DayTwoMorning();
            foreach (var actionPair in loveStoryActions)
            {
                var entity = actionPair.GetEntity();
                var action = actionPair.GetGameAction();
                ActionManagerSystem.Instance.QueueAction(entity, action);
            }

            //McGraw
            var mcGrawSequence = new ActionSequence("McGraw Day Two Morning");
            mcGrawSequence.Add(new CallbackAction(() =>
            {
                EventSystem.EndDrinkMakingEvent.Invoke();
                ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(player,
                    new TeleportAction(Locations.CenterOfBar()));
            })); //This is kind of dirty - but demo!
            mcGrawSequence.Add(new PauseAction(2.0f)); //WORKAROUND FOR SYNC ACTION BUG
            mcGrawSequence.Add(DrugStory.InspectorQuestions(mcGraw));
            ActionManagerSystem.Instance.QueueAction(mcGraw, mcGrawSequence);
        }
    }
}
