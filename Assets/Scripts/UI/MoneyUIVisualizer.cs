using Assets.Framework.States;
using Assets.Scripts.States;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    class MoneyUIVisualizer : MonoBehaviour
    {
        [UsedImplicitly] public Text MoneyAmount;

        private MoneyState moneyState;
        
        [UsedImplicitly]
        public void Update()
        {
            if (moneyState == null)
            {
                moneyState = StaticStates.Get<MoneyState>();
            }

            if (moneyState == null)
            {
                return;
            }

            MoneyAmount.text = string.Format("${0}", moneyState.CurrentMoney);
        }
    }
}
