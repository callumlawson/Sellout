using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.NPC;
using Assets.Framework.States;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions.Cutscenes
{
    static class DayTwoOpen
    {
        public static void Start(List<Entity> matchingEntities) {

            if (StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer)
            {
                var q = EntityQueries.GetEntityWithName(matchingEntities, NPCS.Q.Name);
                var qSequence = new ActionSequence("Q open");
                qSequence.Add(CommonActions.TalkToBarPatrons());
                ActionManagerSystem.Instance.QueueAction(q, qSequence);
            }
        }
    }
}
