using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.NPC;

namespace Assets.Scripts.Systems
{
    class GameSetupSystem : IEndInitEntitySystem, ITickEntitySystem
    {
        //TODO: Remove evil local state.
        private bool setup;

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(PersonState) };
        }

        public void OnEndInit(List<Entity> matchingEntities)
        {
            if (GameSettings.DisableStory)
            {
                return;
            }

            SpawnPoints.ResetPeopleToSpawnPoints(matchingEntities);
        }

        public void Tick(List<Entity> matchingEntities)
        {
            if (!setup)
            {
                var mcGraw = EntityQueries.GetNPC(matchingEntities, NPCS.McGraw.Name);
                var player = EntityQueries.GetNPC(matchingEntities, "You");
              
                ActionManagerSystem.Instance.QueueAction(mcGraw, TutorialAction.Tutorial(mcGraw, player));
                setup = true;
            }
        }
    }
}
