using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.NPC;

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
        var someGuys = EntityQueries.GetNPCSWithName(people, NPCS.Annon.Name);

        if (timeState.Hour == 10 && timeState.Minute == 50)
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

        if (timeState.Hour == 13 && timeState.Minute == 20)
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
    }
}

class SecondDay : Day
{
    public override void UpdateDay(DateTime timeState, List<Entity> people)
    {
        var tolstoy =
            people.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == "Tolstoy");

        if (timeState.Hour == 9 && timeState.Minute == 7)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(tolstoy,
                CommonActions.TalkToPlayer(new Dialogues.TellTheTimeConverstation(timeState.ToString())));
        }

        CommonActions.DrinkOrWanderAroundIfIdle(people);
    }
}

class ZeroDay : Day
{
    public override void UpdateDay(DateTime timeState, List<Entity> people)
    {
    }
}