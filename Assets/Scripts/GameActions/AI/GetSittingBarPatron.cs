using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.States.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameActions.Cusscenes
{
    class GetSittingBarPatron : GameAction, ICancellableAction
    {
        private List<Entity> patrons;     

        public GetSittingBarPatron()
        {
            this.patrons = new List<Entity>(EntityStateSystem.Instance.GetEntitiesWithState<LifecycleState>());
        }

        public override void OnStart(Entity entity)
        {
            //Do Nothing
        }

        public override void OnFrame(Entity entity)
        {
            var sittingPatrons = patrons.FindAll(patron => patron.HasState<LifecycleState>() && patron.GetState<LifecycleState>().status == LifecycleState.LifecycleStatus.Sitting);
            if (sittingPatrons.Count == 0)
            {
                return;
            }

            int choice = Random.Range(0, sittingPatrons.Count);
            var chosenPatron = sittingPatrons[choice];
            entity.GetState<ActionBlackboardState>().TargetEntity = chosenPatron;
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void Pause()
        {
            //Do Nothing
        }

        public override void Unpause()
        {
            //Do Nothing;
        }

        public void Cancel()
        {
            //Do Nothing;
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
