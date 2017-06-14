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
            currentDrinkText.text = order.ToString();
            panel.SetActive(true);
        }

        private void OnEndDrinkOrderEvent()
        {
            currentDrinkText.text = "";
            panel.SetActive(false);
        }
    }
}
