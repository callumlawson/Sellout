using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions.Cutscenes;
using Assets.Scripts.States;
using Assets.Scripts.Util;

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

            Locations.ResetPeopleToSpawnPoints(matchingEntities);
        }

        public void Tick(List<Entity> matchingEntities)
        {
            if (!setup)
            {
                TutorialCutscene.Start(matchingEntities);
                setup = true;
            }
        }
    }
}
