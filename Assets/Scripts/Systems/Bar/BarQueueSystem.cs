using Assets.Framework.Systems;
using Assets.Framework.Entities;
using Assets.Scripts.States;
using UnityEngine;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.GameActions;
using System.Collections.Generic;
using System;
using Assets.Framework.States;
using System.Linq;
using Assets.Scripts.GameActions.DayPhases;

public class BarQueueSystem : IInitSystem, ITickSystem, IReactiveEntitySystem
{
    private Entity purchaseWaypoint;
    private Entity waitForPurchaseWaypoint;

    private GameObject spawnPoint;

    private readonly HashSet<Entity> allCharacters = new HashSet<Entity>();
    private readonly HashSet<Entity> inUseCharacters = new HashSet<Entity>();

    public void OnInit()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
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

        bool purchaseWaypointIsFree = purchaseWaypoint.GetState<UserState>().IsFree();
        bool waitForPurchaseWaypointIsFree = waitForPurchaseWaypoint.GetState<UserState>().IsFree();

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
            var player = StaticStates.Get<PlayerState>().Player;
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
        ActionManagerSystem.Instance.QueueAction(waitingCharacter, CommonActions.OrderDrinkFromPayPoint(waitingCharacter, DrinkRecipes.GetRandomDrinkRecipe()));
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