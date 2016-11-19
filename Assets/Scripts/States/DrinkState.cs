using Assets.Framework.States;
using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    class DrinkState : IState
    {
        [SerializeField] private Dictionary<Ingredient, int> contents;

        public DrinkState()
        {
            contents = new Dictionary<Ingredient, int>();
        }

        public DrinkState(DrinkState template)
        {
            contents = new Dictionary<Ingredient, int>();
            foreach (var content in template.GetContents())
            {
                contents.Add(content.Key, content.Value);
            }
        }

        public IEnumerable<KeyValuePair<Ingredient, int>> GetContents()
        {
            return contents;
        }

        public void Clear()
        {
            contents.Clear();
        }

        public void ChangeIngredientAmount(Ingredient ingredient, int delta)
        {
            var currentAmount = contents.ContainsKey(ingredient) ? contents[ingredient] : 0;
            var newAmount = Mathf.Clamp(currentAmount + delta, 0, int.MaxValue);

            if (newAmount > 0)
            {
                contents.Add(ingredient, currentAmount + delta);
            }
            else
            {
                contents.Remove(ingredient);
            }
        }

        public override string ToString()
        {
            var output = "Drink: \n";
            foreach (var content in contents)
            {
                output = output + "\t(" + content.Key + "->" + content.Value + ")\n";
            }
            return output;
        }
    }
}
