using Assets.Scripts.GameActions.Framework;
using Assets.Framework.Entities;
using Assets.Scripts.Systems;

public class EndDrinkOrderAction : GameAction
{
    public override void OnStart(Entity entity)
    {
        EventSystem.EndDrinkOrderEvent.Invoke();
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
