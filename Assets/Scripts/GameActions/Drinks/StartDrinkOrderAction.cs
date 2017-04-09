using Assets.Scripts.GameActions.Framework;
using Assets.Framework.Entities;
using Assets.Scripts.Systems;

public class StartDrinkOrderAction : GameAction
{
    private DrinkOrder drink;

    public StartDrinkOrderAction(DrinkOrder drink)
    {
        this.drink = drink;
    }

    public override void OnStart(Entity entity)
    {
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
