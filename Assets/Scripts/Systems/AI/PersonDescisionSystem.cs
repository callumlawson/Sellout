using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions;
using Assets.Scripts.States;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.Util;

namespace Assets.Scripts.Systems.AI
{
    //Currently disabled in favour of "DayDirectorSystem".
    class PersonDescisionSystem : ITickEntitySystem, IInitSystem
    {
        private const int CooldownBetweenStoriesInMins = 30;
        private TimeState time;
        private GameTime lastStoryTime;

        private readonly List<SingleStorySeqenceFiller> singleStoryActions = new List<SingleStorySeqenceFiller>()
        {
            new SingleStorySeqenceFiller("McGraw", StoryActions.GettingFrosty)
        };

        private readonly List<DoubleStorySequenceFiller> doubleStoryActions = new List<DoubleStorySequenceFiller>()
        {
            new DoubleStorySequenceFiller("Tolstoy", "Ellie", StoryActions.TolstoyRomantic)
        };

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(PersonState) };
        }

        public void OnInit()
        {
            time = StaticStates.Get<TimeState>();
            lastStoryTime = time.GameTime.GetCopy();
        }

        public void Tick(List<Entity> matchingEntities)
        {
            //TODO: Not used right now in favour of the Day scripts 
            //- but we might want to do this in the future
            //TryScheduleStory(matchingEntities);
            //CommonActions.DrinkOrWanderAroundIfIdle(matchingEntities);
        }

        private void TryScheduleStory(List<Entity> matchingEntities)
        {
            var timeSinceLastStory = time.GameTime - lastStoryTime;
            if (timeSinceLastStory > CooldownBetweenStoriesInMins)
            {
                var storyScheduled = TryScheduleSingleEntityStories(matchingEntities);
                if (!storyScheduled)
                {
                    storyScheduled = TryScheduleTwoEntityStories(matchingEntities);
                }
                if (storyScheduled)
                {
                    lastStoryTime = time.GameTime.GetCopy();
                }
            }
        }

        private bool TryScheduleSingleEntityStories(List<Entity> matchingEntities)
        {
            var storyScheduled = false;
            var toRemove = new List<SingleStorySeqenceFiller>();
            foreach (var story in singleStoryActions)
            {
                var main = matchingEntities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == story.Main);
                if (main != null && ActionManagerSystem.Instance.IsEntityIdle(main))
                {
                    ActionManagerSystem.Instance.QueueAction(main, story.StorySequence(main));
                    toRemove.Add(story);
                    storyScheduled = true;
                }
            }

            for (var i = 0; i < toRemove.Count; i++)
            {
                singleStoryActions.Remove(toRemove[i]);
            }

            return storyScheduled;
        }

        private bool TryScheduleTwoEntityStories(List<Entity> matchingEntities)
        {
            var storyScheduled = false;
            var toRemove = new List<DoubleStorySequenceFiller>();
            foreach (var story in doubleStoryActions)
            {
                var main = matchingEntities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == story.Main);
                var other = matchingEntities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == story.Other);
                if (main != null && other != null && ActionManagerSystem.Instance.IsEntityIdle(main) &&
                    ActionManagerSystem.Instance.IsEntityIdle(other))
                {
                    ActionSequence mainSequence;
                    ActionSequence otherSequence;
                    story.DoubleStorySequence(main, other, out mainSequence, out otherSequence);
                    ActionManagerSystem.Instance.QueueAction(main, mainSequence);
                    ActionManagerSystem.Instance.QueueAction(other, otherSequence);
                    toRemove.Add(story);
                    storyScheduled = true;
                }
            }

            for (var i = 0; i < toRemove.Count; i++)
            {
                doubleStoryActions.Remove(toRemove[i]);
            }

            return storyScheduled;
        }

        private delegate ActionSequence StorySequence(Entity entity);
        private delegate void DoubleStorySequence(Entity main, Entity other, out ActionSequence mainActionSequence, out ActionSequence otherActionSequence);

        private struct DoubleStorySequenceFiller
        {
            public readonly string Main;
            public readonly string Other;
            public readonly DoubleStorySequence DoubleStorySequence;

            public DoubleStorySequenceFiller(string main, string other, DoubleStorySequence doubleStorySequence)
            {
                Main = main;
                Other = other;
                DoubleStorySequence = doubleStorySequence;
            }
        }

        private struct SingleStorySeqenceFiller
        {
            public readonly string Main;
            public readonly StorySequence StorySequence;

            public SingleStorySeqenceFiller(string main, StorySequence sequence)
            {
                Main = main;
                StorySequence = sequence;
            }
        }
    }
}
