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

public class BarQueueSystem : ITickSystem, IReactiveEntitySystem
{
    private Entity purchaseWaypoint;
    private Entity waitForPurchaseWaypoint;

    private HashSet<Entity> AllCharacters = new HashSet<Entity>();
    private HashSet<Entity> InUseCharacters = new HashSet<Entity>();

    public void OnEntityAdded(Entity entity)
    {
        if (entity.GetState<PrefabState>().PrefabName != Prefabs.Player)
        {
            AllCharacters.Add(entity);
        }
    }

    public void OnEntityRemoved(Entity entity)
    {
        AllCharacters.Remove(entity);
        InUseCharacters.Remove(entity);
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

        if (purchaseWaypoint.GetState<UserState>().IsFree())
        {
            OnPurchaseWaypointFree();
        }

        if (waitForPurchaseWaypoint.GetState<UserState>().IsFree())
        {
            OnWaitForPurchaseWaypointFree();
        }
    }

    private void OnPurchaseWaypointFree()
    {
        var waitingCharacter = waitForPurchaseWaypoint.GetState<UserState>().User;

        if (waitingCharacter == null)
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
    }

    private void OnWaitForPurchaseWaypointFree()
    {
        var nextCharacter = GetNextCharacter();

        InUseCharacters.Add(nextCharacter);

        nextCharacter.GetState<ActionBlackboardState>().TargetEntity = waitForPurchaseWaypoint;
        waitForPurchaseWaypoint.GetState<UserState>().Reserve(nextCharacter, "Bar Queue System");
        waitForPurchaseWaypoint.GetState<UserState>().Use(nextCharacter, "Bar Queue System");
        
        ActionManagerSystem.Instance.QueueAction(nextCharacter, new GoToWaypointAction());
    }

    private Entity GetNextCharacter()
    {
        var freeCharacters = new HashSet<Entity>(AllCharacters);
        freeCharacters.ExceptWith(InUseCharacters);

        if (freeCharacters.Count == 0)
        {
            return null;
        }

        var randomChoice = UnityEngine.Random.Range(0, freeCharacters.Count - 1);
        return new List<Entity>(freeCharacters)[randomChoice];
    }
}
