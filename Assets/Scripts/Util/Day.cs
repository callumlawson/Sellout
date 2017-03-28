using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.States;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util.GameActions;

namespace Assets.Scripts.Util
{
    public abstract class Day
    {
        public abstract void UpdateDay(DateTime timeState, List<Entity> entities);
    }
}

class FirstDay : Day
{
    public override void UpdateDay(DateTime timeState, List<Entity> entities)
    {
        var q = entities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == "Q");
        var tolstoy = entities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == "Tolstoy");
        var ellie = entities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == "Ellie");

        if (timeState.Hour == 9 && timeState.Minute == 10)
        {
            ActionSequence mainSequence;
            ActionSequence otherSequence;
            StoryActions.TolstoyRomantic(tolstoy, ellie, out mainSequence, out otherSequence);
            ActionManagerSystem.Instance.QueueActionForEntity(tolstoy, mainSequence);
            ActionManagerSystem.Instance.QueueActionForEntity(ellie, otherSequence);
        }

        if (timeState.Hour == 9 && timeState.Minute == 23)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(q, CommonActions.TalkToPlayer(new Dialogues.TellTheTimeConverstation(timeState.ToString())));
        }

        if (timeState.Hour == 10 && timeState.Minute == 13)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(q, CommonActions.TalkToPlayer(new Dialogues.TellTheTimeConverstation(timeState.ToString())));
        }

        CommonActions.DrinkOrWanderAroundIfIdle(entities);
    }
}

class SecondDay : Day
{
    public override void UpdateDay(DateTime timeState, List<Entity> entities)
    {
        var tolstoy = entities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == "Tolstoy");

        if (timeState.Hour == 9 && timeState.Minute == 7)
        {
            ActionManagerSystem.Instance.QueueActionForEntity(tolstoy, CommonActions.TalkToPlayer(new Dialogues.TellTheTimeConverstation(timeState.ToString())));
        }

        CommonActions.DrinkOrWanderAroundIfIdle(entities);
    }
}