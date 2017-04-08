using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.NPC;

//We assume that at the start of each day there is no one in the bar. 
//Def want to replace with with something much more data driven.
namespace Assets.Scripts.Util
{
    public abstract class Day
    {
        public abstract void UpdateDay(DateTime timeState, List<Entity> allPeople);
        public abstract void EndDay(List<Entity> allPeople);
    }
}

internal class FirstDay : Day
{
    public override void UpdateDay(DateTime timeState, List<Entity> allPeople)
    {
        //TODO: Cache this.
        var q = EntityQueries.GetNPCWithName(allPeople, NPCS.Q.Name);
        var tolstoy = EntityQueries.GetNPCWithName(allPeople, NPCS.Tolstoy.Name);
        var ellie = EntityQueries.GetNPCWithName(allPeople, NPCS.Ellie.Name);
        var mcGraw = EntityQueries.GetNPCWithName(allPeople, NPCS.McGraw.Name);
        var randomPeople = EntityQueries.GetNPCSWithName(allPeople, "Crewperson");
        var hallwayWalkers = EntityQueries.GetNPCSWithName(allPeople, "Expendable");

        if (timeState.Hour == 11 && timeState.Minute == 10 && !GameSettings.DisableTalkingToPlayer)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(mcGraw, TutorialAction.Tutorial(mcGraw));
        }

        if (ActionManagerSystem.Instance.IsEntityIdle(tolstoy) && timeState.Hour > 12)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(tolstoy, CommonActions.Wander());
        }

        if (ActionManagerSystem.Instance.IsEntityIdle(ellie) && timeState.Hour > 13)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(ellie, CommonActions.Wander());
        }

        if (timeState.Hour == 12 && timeState.Minute == 1)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(mcGraw, CommonActions.LeaveBar());
        }

        if (timeState.Hour == 13 && timeState.Minute == 20 && !GameSettings.DisableTalkingToPlayer)
        {
            ActionSequence mainSequence;
            ActionSequence otherSequence;
            StoryActions.TolstoyRomantic(tolstoy, ellie, out mainSequence, out otherSequence);
            ActionManagerSystem.Instance.QueueActionForEntity(tolstoy, mainSequence);
            ActionManagerSystem.Instance.QueueActionForEntity(ellie, otherSequence);
        }

        if (ActionManagerSystem.Instance.IsEntityIdle(ellie) && timeState.Hour > 15)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(ellie, CommonActions.LeaveBar());
        }

        foreach (var person in randomPeople)
        {
            if ((timeState.Hour > 12 && timeState.Hour < 14) || (timeState.Hour > 17 && timeState.Hour < 20))
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(person))
                {
                    ActionManagerSystem.Instance.QueueActionForEntity(person, CommonActions.Wander());
                }
            }
            else
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(person))
                {
                    ActionManagerSystem.Instance.QueueActionForEntity(person, CommonActions.LeaveBar());
                }
            }
        }

        if (timeState.Hour == 13 && timeState.Minute == 30)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(q, DrugStory.DrugPusherIntro(q));
        }

        if (timeState.Hour == 18 && timeState.Minute == 30)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(q, DrugStory.DrugPusherPaysYou(q));
        }

        if (timeState.Hour > 20)
        {
            foreach (var person in randomPeople)
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(person))
                {
                    ActionManagerSystem.Instance.QueueActionForEntity(person, CommonActions.LeaveBar());
                }
            }
        }

        foreach (var walker in hallwayWalkers)
        {
            if (ActionManagerSystem.Instance.IsEntityIdle(walker))
            {
                ActionManagerSystem.Instance.QueueActionForEntity(walker, CommonActions.WalkToWaypoint());
            }
        }
    }

    public override void EndDay(List<Entity> allPeople)
    {
        SpawnPoints.ResetPeopleToSpawnPoints(allPeople);
    }
}

internal class SecondDay : Day
{
    public override void UpdateDay(DateTime timeState, List<Entity> allPeople)
    {
        var q = EntityQueries.GetNPCWithName(allPeople, NPCS.Q.Name);
        var mcGraw = EntityQueries.GetNPCWithName(allPeople, NPCS.McGraw.Name);
        var hallwayWalkers = EntityQueries.GetNPCSWithName(allPeople, "Expendable");
        var randomPeople = EntityQueries.GetNPCSWithName(allPeople, "Crewperson");

        if (timeState.Hour == 11 && timeState.Minute == 12 && !GameSettings.DisableTalkingToPlayer)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(mcGraw, DrugStory.InspectorQuestions(mcGraw));
        }

        if (timeState.Hour == 17 && timeState.Minute == 3 && !GameSettings.DisableTalkingToPlayer)
        {
            DrugStory.DrugPusherInspectorShowdown(mcGraw, q);
        }

        foreach (var person in randomPeople)
        {
            if ((timeState.Hour > 12 && timeState.Hour < 14) || (timeState.Hour > 17 && timeState.Hour < 20))
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(person))
                {
                    ActionManagerSystem.Instance.QueueActionForEntity(person, CommonActions.Wander());
                }
            }
            else
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(person))
                {
                    ActionManagerSystem.Instance.QueueActionForEntity(person, CommonActions.LeaveBar());
                }
            }
        }

        foreach (var walker in hallwayWalkers)
        {
            if (ActionManagerSystem.Instance.IsEntityIdle(walker))
            {
                ActionManagerSystem.Instance.QueueActionForEntity(walker, CommonActions.WalkToWaypoint());
            }
        }
    }

    public override void EndDay(List<Entity> allPeople)
    {
        SpawnPoints.ResetPeopleToSpawnPoints(allPeople);
    }
}

