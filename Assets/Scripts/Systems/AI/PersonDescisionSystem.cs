using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using Random = UnityEngine.Random;
using Assets.Scripts.Util.GameActions;
using Assets.Scripts.GameActions.Composite;
using UnityEngine;

namespace Assets.Scripts.Systems.AI
{
    class PersonDescisionSystem : ITickEntitySystem
    {
        private delegate ActionSequence StorySequence(Entity entity);
        private delegate void DoubleStorySequence(Entity main, Entity other, out ActionSequence mainActionSequence, out ActionSequence otherActionSequence);

        private struct DoubleStorySequenceFiller
        {
            public readonly string main;
            public readonly string other;
            public readonly DoubleStorySequence sequence;

            public DoubleStorySequenceFiller(string main, string other, DoubleStorySequence sequence)
            {
                this.main = main;
                this.other = other;
                this.sequence = sequence;
            }
        }

        private Dictionary<string, StorySequence> storyActions = new Dictionary<string, StorySequence>()
        {

        };

        private List<DoubleStorySequenceFiller> doubleStoryActions = new List<DoubleStorySequenceFiller>()
        {
            new DoubleStorySequenceFiller("Tolstoy", "Ellie", StoryActions.TolstoyRomantic)
        };

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(PersonState) };
        }

        public void Tick(List<Entity> matchingEntities)
        {
            var toRemove = new List<DoubleStorySequenceFiller>();
            foreach (var story in doubleStoryActions)
            {
                var main = matchingEntities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == story.main);
                var other = matchingEntities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == story.main);
                if (main != null && other != null && ActionManagerSystem.Instance.IsEntityIdle(main) && ActionManagerSystem.Instance.IsEntityIdle(other))
                {
                    Debug.Log("Found Tolstoy and Ellie!");
                    ActionSequence mainSequence;
                    ActionSequence otherSequence;
                    story.sequence(main, other, out mainSequence, out otherSequence);
                    ActionManagerSystem.Instance.QueueActionForEntity(main, mainSequence);
                    ActionManagerSystem.Instance.QueueActionForEntity(other, otherSequence);
                    toRemove.Add(story);
                }
            }

            for (var i = 0; i < toRemove.Count; i++)
            {
                doubleStoryActions.Remove(toRemove[i]);
            }

            foreach (var entity in matchingEntities)
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(entity))
                {
                    var entityName = entity.GetState<NameState>();
                    if (entityName != null && storyActions.ContainsKey(entityName.Name))
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, storyActions[entityName.Name](entity));
                        storyActions.Remove(entityName.Name);
                    }
                    else if (Random.value > 0.8f)
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
    }
}
