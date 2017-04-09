using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;

public class CurrentDrinkOrderUI : MonoBehaviour
{
    GameObject panel;
    Text CurrentDrinkText;

    private readonly string DefaultDrinkOrderName = "????";
    
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

    private void OnStartDrinkOrder(DrinkOrder drink)
    {
        var name = drink.Recipe != null ? drink.Recipe.DrinkName : DefaultDrinkOrderName;
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
