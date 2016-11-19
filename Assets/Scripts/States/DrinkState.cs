using Assets.Framework.States;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    class DrinkState : IState
    {
        public enum Ingredient
        {
            Alcohol
        };

        [SerializeField] private Dictionary<Ingredient, int> contents;

        public DrinkState(Dictionary<Ingredient, int> contents)
        {
            this.contents = contents;
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
