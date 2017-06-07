using JetBrains.Annotations;
using UnityEngine;

public class LightControllerVisualizer : MonoBehaviour {

    public static LightControllerVisualizer Instance;

    [UsedImplicitly] public GameObject DayLights;
    [UsedImplicitly] public GameObject NightLights;

    [UsedImplicitly]
    void Start()
    {
        Instance = this;
    }

    public void SetLighting(Assets.Scripts.States.DayPhase dayPhase)
    {
        switch (dayPhase)
        {
            case Assets.Scripts.States.DayPhase.Morning:
                SetLighting(true);
                break;
            case Assets.Scripts.States.DayPhase.Open:
                SetLighting(true);
                break;
            case Assets.Scripts.States.DayPhase.Night:
                SetLighting(false);
                break;
        }
    }

    private void SetLighting(bool daytime)
    {
        DayLights.SetActive(daytime);
        NightLights.SetActive(!daytime);
    }
}
