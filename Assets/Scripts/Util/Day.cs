using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.NPC;
using Debug = UnityEngine.Debug;

//We assume that at the start of each day there is no one in the bar. 
//Def want to replace with with something much more data driven.

namespace Assets.Scripts.Util
{
    public abstract class Day
    {
        public abstract void UpdateDay(DateTime timeState, List<Entity> people);
    }
}

class FirstDay : Day
{
    public override void UpdateDay(DateTime timeState, List<Entity> people)
    {
        //TODO: Cache this.
        var q = EntityQueries.GetNPCWithName(people, NPCS.Q.Name);
        var tolstoy = EntityQueries.GetNPCWithName(people, NPCS.Tolstoy.Name);
        var ellie = EntityQueries.GetNPCWithName(people, NPCS.Ellie.Name);
        var mcGraw = EntityQueries.GetNPCWithName(people, NPCS.McGraw.Name);
        var someGuys = EntityQueries.GetNPCSWithName(people, "Crewperson");
        var hallwayWalkers = EntityQueries.GetNPCSWithName(people, "Expendable");

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

        if (ActionManagerSystem.Instance.IsEntityIdle(q) && timeState.Hour > 17)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(ellie, CommonActions.Wander());
        }

        foreach (var person in someGuys)
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

        if (timeState.Hour > 20)
        {
            foreach (var person in people)
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
}

class SecondDay : Day
{
    public override void UpdateDay(DateTime timeState, List<Entity> people)
    {
        var tolstoy = people.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == "Tolstoy");

        if (timeState.Hour == 9 && timeState.Minute == 7 && !GameSettings.DisableTalkingToPlayer)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(tolstoy,
                CommonActions.TalkToPlayer(new Dialogues.TellTheTimeConverstation(timeState.ToString())));
        }

        CommonActions.DrinkOrWanderAroundIfIdle(people);
    }
}
