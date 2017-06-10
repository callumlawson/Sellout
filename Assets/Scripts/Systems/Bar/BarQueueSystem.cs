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

        public void OnInit()
        {
            time = StaticStates.Get<TimeState>();
            player = StaticStates.Get<PlayerState>().Player;
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
            return new List<Type> { typeof(PersonState), typeof(PrefabState) };
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
                Debug.LogError("Could not find all the waypoints for the bar queue! Purchase waypoint: " + purchaseWaypoint + ". Wait for purchase waypoint: " + waitForPurchaseWaypoint + ".");
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

            waitingCharacter.GetState<ActionBlackboardState>().TargetEntity = purchaseWaypoint;
            purchaseWaypoint.GetState<UserState>().Reserve(waitingCharacter, "Bar Queue System");
            purchaseWaypoint.GetState<UserState>().Use(waitingCharacter, "Bar Queue System");

            ActionManagerSystem.Instance.QueueAction(waitingCharacter, new GoToWaypointAction());
            ActionManagerSystem.Instance.QueueAction(waitingCharacter, DrinkOrders.GetRandomOrder(waitingCharacter));
            ActionManagerSystem.Instance.QueueAction(waitingCharacter, CommonActions.SitDown());
            ActionManagerSystem.Instance.QueueAction(waitingCharacter, CommonActions.SitDownLoop());
        }

        private void OnWaitForPurchaseWaypointFree()
        {
            var nextCharacter = GetNextCharacter();

            if (nextCharacter == null)
            {
                return;
            }

            //ActionManagerSystem.Instance.QueueAction(nextCharacter, new TeleportAction(SpawnPoint.transform));

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

            var randomChoice = UnityEngine.Random.Range(0, idleCharacters.Count - 1);
            return idleCharacters[randomChoice];
        }
    }
}