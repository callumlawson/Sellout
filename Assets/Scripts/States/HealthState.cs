using System;
using Assets.Framework.States;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    class HealthState : IState
    {
        public float CurrentHealth;

        public HealthState(float health)
        {
            CurrentHealth = health;
        }

        public void DoDamage(float amount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, 100);
        }

        public override string ToString()
        {
            return string.Format("Health: {0}", CurrentHealth);
        }
    }
}
