using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.Systems
{
    class GameSetupSystem : IEndInitEntitySystem
    {
        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(PersonState) };
        }

        public void OnEndInit(List<Entity> allPeople)
        {
            if (GameSettings.DisableTutorial)
            {
                return;
            }
            
            StaticStates.Get<DayPhaseState>().SetDayPhase(DayPhase.Morning);
        }
    }
}
