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
        private delegate ActionSequence StorySequence(Entity entity);

        private Dictionary<string, StorySequence> storyActions = new Dictionary<string, StorySequence>()
        {
            {"Tolstoy",  StoryActions.TolstoyOne}
        };

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(PersonState) };
        }

        public void Tick(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                if (ActionManagerSystem.Instance.IsEntityIdle(entity))
                {
                    var entityName = entity.GetState<NameState>();
                    if (entityName != null && storyActions.ContainsKey(entityName.Name))
                    {
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, storyActions[entityName.Name](entity));
                    }
                    else if (Random.value > 0.8f)
                    {
                        var drinkRecipe = DrinkRecipes.GetRandomDrinkRecipe();
                        ActionManagerSystem.Instance.QueueActionForEntity(entity, CommonActions.OrderDrink(entity, drinkRecipe));
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
