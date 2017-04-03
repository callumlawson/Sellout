using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;

public class CurrentDrinkOrderUI : MonoBehaviour
{
    GameObject panel;
    Text CurrentDrinkText;
    
    void Awake()
    {
        panel = transform.GetChild(0).gameObject;
        CurrentDrinkText = GetComponentInChildren<Text>();
        panel.SetActive(false);
    }

    private void Start()
    {
        EventSystem.StartDrinkOrderEvent += OnStartDrinkOrder;
        EventSystem.EndDrinkOrderEvent += OnEndDrinkOrderEvent;
    }

    private void OnStartDrinkOrder(DrinkRecipe drink)
    {
        var name = drink.DrinkName;
        CurrentDrinkText.text = name;
        panel.SetActive(true);
    }

    private void OnEndDrinkOrderEvent()
    {
        CurrentDrinkText.text = "";
        panel.SetActive(false);
    }

    void Update()
    {

    }
}
