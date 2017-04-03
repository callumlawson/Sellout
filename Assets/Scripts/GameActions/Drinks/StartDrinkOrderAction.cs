using Assets.Scripts.GameActions.Framework;
using Assets.Framework.Entities;
using Assets.Scripts.Util;
using Assets.Scripts.Systems;
using UnityEngine;

public class StartDrinkOrderAction : GameAction
{
    private DrinkRecipe drink;

    public StartDrinkOrderAction(DrinkRecipe drink)
    {
        this.drink = drink;
    }

    public override void OnStart(Entity entity)
    {
        Debug.Log("Starting drink order for " + drink);
        EventSystem.StartDrinkOrderEvent.Invoke(drink);
        ActionStatus = ActionStatus.Succeeded;
    }

    public override void OnFrame(Entity entity)
    {
        // do nothing
    }    

    public override void Pause()
    {
        // do nothing
    }

    public override void Unpause()
    {
        // do nothing
    }
}
