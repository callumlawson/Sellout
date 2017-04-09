﻿using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util;
using Assets.Scripts.Util.NPC;

internal class FirstDay : Day
{
    public FirstDay(List<Entity> allPeople)
    {
        var q = EntityQueries.GetNPC(allPeople, NPCS.Q.Name);
        var tolstoy = EntityQueries.GetNPC(allPeople, NPCS.Tolstoy.Name);
        var ellie = EntityQueries.GetNPC(allPeople, NPCS.Ellie.Name);
        var mcGraw = EntityQueries.GetNPC(allPeople, NPCS.McGraw.Name);

        ScheduleEvent(11, 10, () => { ActionManagerSystem.Instance.QueueAction(mcGraw, TutorialAction.Tutorial(mcGraw)); });

        ScheduleEvent(12, 0, () => { ActionManagerSystem.Instance.QueueAction(tolstoy, CommonActions.Wander()); });

        ScheduleEvent(12, 1, () => { ActionManagerSystem.Instance.QueueAction(mcGraw, CommonActions.LeaveBar()); });

        ScheduleEvent(13, 0, () => { ActionManagerSystem.Instance.QueueAction(ellie, CommonActions.Wander()); });

        ScheduleEvent(13, 15, () => { ActionManagerSystem.Instance.QueueAction(q, DrugStory.DrugPusherIntro(q)); });

        ScheduleEvent(13, 20, () =>
        {
            ActionSequence mainSequence;
            ActionSequence otherSequence;
            StoryActions.TolstoyRomantic(tolstoy, ellie, out mainSequence, out otherSequence);
            ActionManagerSystem.Instance.QueueAction(tolstoy, mainSequence);
            ActionManagerSystem.Instance.QueueAction(ellie, otherSequence);
        });

        ScheduleEvent(15, 0, () => { ActionManagerSystem.Instance.QueueAction(ellie, CommonActions.LeaveBar()); });

        ScheduleEvent(18, 30, () => { ActionManagerSystem.Instance.QueueAction(q, DrugStory.DrugPusherPaysYou(q)); });

        SchedualEventDuringInterval(12, 0, 14, 0, () =>
        {
            foreach (var person in EntityQueries.GetNPCSWithName(allPeople, "Crewperson"))
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(person))
                {
                    ActionManagerSystem.Instance.QueueAction(person, CommonActions.Wander());
                }
            }
        });

        SchedualEventDuringInterval(17, 0, 20, 0, () =>
        {
            foreach (var person in EntityQueries.GetNPCSWithName(allPeople, "Crewperson"))
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(person))
                {
                    ActionManagerSystem.Instance.QueueAction(person, CommonActions.LeaveBar());
                }
            }
        });

        SchedualEventDuringInterval(11, 0, 21, 0, () =>
        {
            foreach (var walker in EntityQueries.GetNPCSWithName(allPeople, "Expendable"))
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(walker))
                {
                    ActionManagerSystem.Instance.QueueAction(walker, CommonActions.WalkToWaypoint());
                }
            }
        });
    }

    public override void OnEndOfDay(List<Entity> allPeople)
    {
        SpawnPoints.ResetPeopleToSpawnPoints(allPeople);
    }
}

internal class SecondDay : Day
{
    public SecondDay(List<Entity> allPeople)
    {
        ScheduleEvent(11, 13, () =>
        {
            ActionManagerSystem.Instance.QueueAction(
                EntityQueries.GetNPC(allPeople, NPCS.McGraw.Name),
                DrugStory.InspectorQuestions(EntityQueries.GetNPC(allPeople, NPCS.McGraw.Name))
            );
        });

        ScheduleEvent(17, 3, () =>
        {
            DrugStory.DrugPusherInspectorShowdown(
                EntityQueries.GetNPC(allPeople, NPCS.McGraw.Name),
                EntityQueries.GetNPC(allPeople, NPCS.Q.Name)
            );
        });

        SchedualEventDuringInterval(12, 0, 14, 0, () =>
        {
            foreach (var person in EntityQueries.GetNPCSWithName(allPeople, "Crewperson"))
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(person))
                {
                    ActionManagerSystem.Instance.QueueAction(person, CommonActions.Wander());
                }
            }
        });

        SchedualEventDuringInterval(17, 0, 20, 0, () =>
        {
            foreach (var person in EntityQueries.GetNPCSWithName(allPeople, "Crewperson"))
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(person))
                {
                    ActionManagerSystem.Instance.QueueAction(person, CommonActions.LeaveBar());
                }
            }
        });

        SchedualEventDuringInterval(11, 0, 21, 0, () =>
        {
            foreach (var walker in EntityQueries.GetNPCSWithName(allPeople, "Expendable"))
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(walker))
                {
                    ActionManagerSystem.Instance.QueueAction(walker, CommonActions.WalkToWaypoint());
                }
            }
        });
    }

    public override void OnEndOfDay(List<Entity> allPeople)
    {
        SpawnPoints.ResetPeopleToSpawnPoints(allPeople);
    }
}


//We assume that at the start of each day there is no one in the bar. 
//Def want to replace with with something much more data driven.

namespace Assets.Scripts.Util
{
    public struct DayTime
    {
        public int Hour;
        public int Min;

        public DayTime(int hour, int min)
        {
            Hour = hour;
            Min = min;
        }
    }

    public struct DayTimeSpan
    {
        public DayTime Start;
        public DayTime End;

        public DayTimeSpan(DayTime start, DayTime end)
        {
            Start = start;
            End = end;
        }
    }

    public abstract class Day
    {
        private readonly Dictionary<DayTimeSpan, Action> dayEvents = new Dictionary<DayTimeSpan, Action>();

        public void UpdateDay(DateTime currentTime, List<Entity> allPeople)
        {
            var possibleActions = new List<Action>();
            foreach (var timeSpan in dayEvents.Keys)
            {
                if ((currentTime.Hour >= timeSpan.Start.Hour && currentTime.Hour <= timeSpan.End.Hour) &&
                    (currentTime.Minute >= timeSpan.Start.Min && currentTime.Minute <= timeSpan.End.Min))
                {
                    possibleActions.Add(dayEvents[timeSpan]);
                }
            }
            possibleActions.ForEach(action => action());
        }

        public void ScheduleEvent(int hour, int min, Action gameEvent)
        {
            var dayTime = new DayTime(hour, min);
            dayEvents.Add(new DayTimeSpan(dayTime, dayTime), gameEvent);
        }

        public void SchedualEventDuringInterval(int hourStart, int minStart, int hourEnd, int minEnd, Action gameEvent)
        {
            var startTime = new DayTime(hourStart, minStart);
            var endTime = new DayTime(hourEnd, minEnd);
            dayEvents.Add(new DayTimeSpan(startTime, endTime), gameEvent);
        }

        public abstract void OnEndOfDay(List<Entity> allPeople);
    }
}

