using Assets.Framework.Systems;
using Assets.Scripts.Systems;

public class PausingSystem : IEntityManager, IInitSystem
{
    private EntityStateSystem ess;

    public void OnInit()
    {
        EventSystem.PauseEvent += OnPauseEvent;
        EventSystem.ResumeEvent += OnResumeEvent;
    }

    public void SetEntitySystem(EntityStateSystem ess)
    {
        this.ess = ess;
    }

    private void OnPauseEvent()
    {
        ess.Pause();
    }

    private void OnResumeEvent()
    {
        ess.Resume();
    }    
}
