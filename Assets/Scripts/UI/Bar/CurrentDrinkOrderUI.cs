using Assets.Scripts.GameActions;
using Assets.Scripts.Systems;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Bar
{
    public class CurrentDrinkOrderUI : MonoBehaviour
    {
        private GameObject panel;
        private Text currentDrinkText;
    
        [UsedImplicitly]
        public void Awake()
        {
            panel = transform.GetChild(0).gameObject;
            currentDrinkText = GetComponentInChildren<Text>();
            panel.SetActive(false);
        }

        [UsedImplicitly]
        public void Start()
        {
            EventSystem.StartDrinkOrderEvent += OnStartDrinkOrder;
            EventSystem.EndDrinkOrderEvent += OnEndDrinkOrderEvent;
        }

        private void OnStartDrinkOrder(DrinkOrders.DrinkOrder order)
        {
            switch (order.OrderType)
            {
                case DrinkOrders.DrinkOrderType.Exact:
                    var exactOrder = (DrinkOrders.ExactDrinkorder) order;
                    currentDrinkText.text = exactOrder.Recipe.DrinkName;
                    break;
                case DrinkOrders.DrinkOrderType.NonAlcoholic:
                    currentDrinkText.text = "Non Alcoholic";
                    break;
            }

            panel.SetActive(true);
        }

        private void OnEndDrinkOrderEvent()
        {
            currentDrinkText.text = "";
            panel.SetActive(false);
        }
    }
}
