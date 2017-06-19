using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.DayPhases;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Systems.Bar
{
    public class BarQueueSystem : IInitSystem, ITickSystem, IReactiveEntitySystem
    {
        private TimeState time;
        private Entity player;

        private Entity purchaseWaypoint;
        private Entity waitForPurchaseWaypoint;

        private readonly HashSet<Entity> allCharacters = new HashSet<Entity>();

        private readonly HashSet<Entity> inUseCharacters = new HashSet<Entity>();
        private readonly HashSet<Entity> specialCharacters = new HashSet<Entity>();

        public void OnInit()
        {
            time = StaticStates.Get<TimeState>();
            player = StaticStates.Get<PlayerState>().Player;

            var dayPhase = StaticStates.Get<DayPhaseState>();
            dayPhase.DayPhaseChangedTo += DayPhaseChangedTo;
        }

        private void DayPhaseChangedTo(DayPhase phase)
        {
            if (phase == DayPhase.Open)
            {
                inUseCharacters.Clear();
                specialCharacters.Clear();

                if (time.GameTime.GetDay() == 1)
                {
                    specialCharacters.Add(allCharacters.First(entity => entity.GetState<NameState>().Name == "Q"));
                }
                else if (time.GameTime.GetDay() == 2)
                {
                    inUseCharacters.Add(allCharacters.First(entity => entity.GetState<NameState>().Name == "Q"));
                }
            }        
        }

        public void OnEntityAdded(Entity entity)
        {
            if (entity.GetState<PrefabState>().PrefabName != Prefabs.Player && entity.GetState<NameState>().Name != "Expendable")
            {
                allCharacters.Add(entity);
            }
        }

        public void OnEntityRemoved(Entity entity)
        {
            allCharacters.Remove(entity);
            inUseCharacters.Remove(entity);
        }

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(IsPersonState), typeof(PrefabState) };
        }

        public void Tick()
        {
            if (StaticStates.Get<DayPhaseState>().CurrentDayPhase != DayPhase.Open)
            {
                return;
            }
        
            if (time.GameTime.GetHour() >= Constants.ClosingHour)
            {
                return;
            }

            if (purchaseWaypoint == null)
            {
                purchaseWaypoint = WaypointSystem.Instance.GetWaypointThatSatisfiesGoal(Goal.PayFor);
            }

            if (waitForPurchaseWaypoint == null)
            {
                waitForPurchaseWaypoint = WaypointSystem.Instance.GetWaypointThatSatisfiesGoal(Goal.WaitForPurchaseWaypoint);
            }

            if (purchaseWaypoint == null || waitForPurchaseWaypoint == null)
            {
                return;
            }

            var purchaseWaypointIsFree = purchaseWaypoint.GetState<UserState>().IsFree();
            var waitForPurchaseWaypointIsFree = waitForPurchaseWaypoint.GetState<UserState>().IsFree();

            if (purchaseWaypointIsFree)
            {
                OnPurchaseWaypointFree();
            }

            if (waitForPurchaseWaypointIsFree)
            {
                OnWaitForPurchaseWaypointFree();
            }

            if (GetNextCharacter() == null && purchaseWaypoint.GetState<UserState>().IsFree() && waitForPurchaseWaypoint.GetState<UserState>().IsFree())
            {
                ActionManagerSystem.Instance.QueueAction(player, new CloseBarIfOpenAction());
            }
        }

        private void OnPurchaseWaypointFree()
        {
            var waitingCharacter = waitForPurchaseWaypoint.GetState<UserState>().User;

            if (waitingCharacter == null)
            {
                return;
            }

            if (!ActionManagerSystem.Instance.IsEntityIdle(waitingCharacter))
            {
                return;
            }

            waitForPurchaseWaypoint.GetState<UserState>().ClearReserver();
            waitForPurchaseWaypoint.GetState<UserState>().ClearUser();
            
            AddPurchaseActionsForCharacter(waitingCharacter);
        }

        private void AddPurchaseActionsForCharacter(Entity character)
        {
            character.GetState<ActionBlackboardState>().TargetEntity = purchaseWaypoint;
            purchaseWaypoint.GetState<UserState>().Reserve(character, "Bar Queue System");
            purchaseWaypoint.GetState<UserState>().Use(character, "Bar Queue System");
            ActionManagerSystem.Instance.QueueAction(character, new GoToWaypointAction());

            if (time.GameTime.GetDay() == 1 && character.GetState<NameState>().Name == "Q")
            {
                ActionManagerSystem.Instance.QueueAction(character, DrugStory.DrugPusherIntro(character));
            }
            else
            {
                ActionManagerSystem.Instance.QueueAction(character, DrinkOrders.GetRandomOrder(character));
                ActionManagerSystem.Instance.QueueAction(character, CommonActions.SitDown());
                ActionManagerSystem.Instance.QueueAction(character, CommonActions.SitDownLoop());
            }
        }

        private void OnWaitForPurchaseWaypointFree()
        {
            var nextCharacter = GetNextCharacter();

            if (nextCharacter == null)
            {
                return;
            }

            inUseCharacters.Add(nextCharacter);

            nextCharacter.GetState<ActionBlackboardState>().TargetEntity = waitForPurchaseWaypoint;
            waitForPurchaseWaypoint.GetState<UserState>().Reserve(nextCharacter, "Bar Queue System");
            waitForPurchaseWaypoint.GetState<UserState>().Use(nextCharacter, "Bar Queue System");

            ActionManagerSystem.Instance.QueueAction(nextCharacter, new GoToWaypointAction());
        }

        private Entity GetNextCharacter()
        {
            var freeCharacters = new HashSet<Entity>(allCharacters);
            freeCharacters.ExceptWith(inUseCharacters);
            var idleCharacters = freeCharacters.Where(entity => ActionManagerSystem.Instance.IsEntityIdle(entity)).ToList();

            if (idleCharacters.Count == 0)
            {
                return null;
            }

            if (specialCharacters.Count > 0)
            {
                var choice = specialCharacters.First();

                if (!idleCharacters.Contains(choice))
                {
                    Debug.LogError("Want to use special character " + choice + " but that character is busy!");
                }

                specialCharacters.Remove(choice);
                return choice;
            }

            var randomChoice = UnityEngine.Random.Range(0, idleCharacters.Count - 1);
            return idleCharacters[randomChoice];
        }
    }
}
