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
using Assets.Scripts.GameActions.Composite;

namespace Assets.Scripts.Systems.Bar
{
    public class BarQueueSystem : IInitSystem, ITickSystem, IReactiveEntitySystem
    {
        public static BarQueueSystem Instance;

        private class EntityActionsPair
        {
            public Entity entity;
            public ActionSequence actions;

            public EntityActionsPair(Entity entity, ActionSequence actions)
            {
                this.entity = entity;
                this.actions = actions;
            }
        }

        private TimeState time;
        private PlayerState playerState;
        private Entity player;

        private Entity purchaseWaypoint;
        private Entity waitForPurchaseWaypoint;

        private readonly HashSet<Entity> allCharacters = new HashSet<Entity>();

        private readonly HashSet<Entity> inUseCharacters = new HashSet<Entity>();

        private readonly LinkedList<Entity> specialCharacters = new LinkedList<Entity>();
        private readonly Dictionary<Entity, ActionSequence> specialCharacterActions = new Dictionary<Entity, ActionSequence>();

        public BarQueueSystem()
        {
            Instance = this;
        }

        public void OnInit()
        {
            time = StaticStates.Get<TimeState>();
            playerState = StaticStates.Get<PlayerState>();
            player = playerState.Player;

            var dayPhase = StaticStates.Get<DayPhaseState>();
            dayPhase.DayPhaseChangedTo += DayPhaseChangedTo;
        }

        public void QueueEntityNext(Entity entity, ActionSequence actions)
        {
            specialCharacters.AddLast(entity);
            specialCharacterActions.Add(entity, actions);
        }

        private void DayPhaseChangedTo(DayPhase phase)
        {
            if (phase == DayPhase.Open)
            {
                inUseCharacters.Clear();
                specialCharacters.Clear();
                specialCharacterActions.Clear();

                if (time.GameTime.GetDay() == 1)
                {
                    var dayOneStart = DrugStory.DayOneStart();
                    foreach (var pair in dayOneStart)
                    {
                        specialCharacters.AddLast(pair.Key);
                        specialCharacterActions.Add(pair.Key, pair.Value);
                    }
                }
                else if (time.GameTime.GetDay() == 2)
                {
                    var dayOneStart = DrugStory.DayOneStart();
                    foreach (var pair in dayOneStart)
                    {
                        specialCharacters.AddLast(pair.Key);
                        specialCharacterActions.Add(pair.Key, pair.Value);
                    }
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
            if (StaticStates.Get<DayPhaseState>().CurrentDayPhase != DayPhase.Open || playerState.PlayerStatus != PlayerStatus.Bar)
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

            if (specialCharacters.Count > 0)
            {
                if (purchaseWaypointIsFree)
                {
                    var availableCharacter = specialCharacters.FirstOrDefault(entity => ActionManagerSystem.Instance.IsEntityIdle(entity));
                    if (availableCharacter != null)
                    {
                        ActionManagerSystem.Instance.QueueAction(availableCharacter, new TeleportAction(waitForPurchaseWaypoint.GameObject.transform.position));
                        AddPurchaseActionsForCharacter(availableCharacter, specialCharacterActions[availableCharacter]);

                        specialCharacters.Remove(availableCharacter);
                        specialCharacterActions.Remove(availableCharacter);
                    }
                }
            }
            else
            {
                if (purchaseWaypointIsFree)
                {
                    OnPurchaseWaypointFree();
                }

                if (waitForPurchaseWaypointIsFree)
                {
                    OnWaitForPurchaseWaypointFree();
                }
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

            var purchaseSequence = new ActionSequence("Purchase");
            purchaseSequence.Add(DrinkOrders.GetRandomOrder(waitingCharacter));
            purchaseSequence.Add(CommonActions.SitDown());
            purchaseSequence.Add(CommonActions.SitDownLoop());
            
            AddPurchaseActionsForCharacter(waitingCharacter, purchaseSequence);
        }

        private void AddPurchaseActionsForCharacter(Entity character, ActionSequence sequence)
        {
            character.GetState<ActionBlackboardState>().TargetEntity = purchaseWaypoint;
            purchaseWaypoint.GetState<UserState>().Reserve(character, "Bar Queue System");
            purchaseWaypoint.GetState<UserState>().Use(character, "Bar Queue System");
            ActionManagerSystem.Instance.QueueAction(character, new GoToWaypointAction());

            ActionManagerSystem.Instance.QueueAction(character, sequence);
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
            var idleCharacters = freeCharacters.Where(entity => ActionManagerSystem.Instance.IsEntityIdle(entity)).Where(entity => !specialCharacters.Contains(entity)).ToList();

            if (idleCharacters.Count == 0)
            {
                return null;
            }

            var randomChoice = UnityEngine.Random.Range(0, idleCharacters.Count - 1);
            return idleCharacters[randomChoice];
        }
    }
}
