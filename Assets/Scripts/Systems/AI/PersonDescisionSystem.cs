using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using Random = UnityEngine.Random;
using Assets.Scripts.Util.GameActions;
using Assets.Scripts.GameActions.Composite;

namespace Assets.Scripts.Systems.AI
{
    class PersonDescisionSystem : ITickEntitySystem
    {
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

        public void Tick(List<Entity> matchingEntities)
        {
            ScheduleTwoEntityStories(matchingEntities);
            ScheduleSingleEntityStories(matchingEntities);

            foreach (var entity in matchingEntities)
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(entity))
                {
                    if (Random.value > 0.8f)
                    {
                        var drinkRecipe = DrinkRecipes.GetRandomDrinkRecipe();
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, CommonActions.OrderDrinkAndSitDown(entity, drinkRecipe));
                    }
                    else
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, CommonActions.Wander());
                    }
                }
            }
        }

        private void ScheduleSingleEntityStories(List<Entity> matchingEntities)
        {
            var toRemove = new List<SingleStorySeqenceFiller>();
            foreach (var story in singleStoryActions)
            {
                var main = matchingEntities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == story.Main);
                if (main != null && ActionManagerSystem.Instance.IsEntityIdle(main))
                {
                    ActionManagerSystem.Instance.QueueActionForEntity(main, story.StorySequence(main));
                    toRemove.Add(story);
                }
            }

            for (var i = 0; i < toRemove.Count; i++)
            {
                singleStoryActions.Remove(toRemove[i]);
            }
        }

        private void ScheduleTwoEntityStories(List<Entity> matchingEntities)
        {
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
                    ActionManagerSystem.Instance.QueueActionForEntity(main, mainSequence);
                    ActionManagerSystem.Instance.QueueActionForEntity(other, otherSequence);
                    toRemove.Add(story);
                }
            }

            for (var i = 0; i < toRemove.Count; i++)
            {
                doubleStoryActions.Remove(toRemove[i]);
            }
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
