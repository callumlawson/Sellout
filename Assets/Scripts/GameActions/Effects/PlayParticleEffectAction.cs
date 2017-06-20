using Assets.Scripts.GameActions.Framework;
using Assets.Framework.Entities;
using Assets.Framework.Util;
using UnityEngine;

public class PlayParticleEffectAction : GameAction
{
    private string ParticleName;
    private Entity EntityA;
    private Entity EntityB;

    public PlayParticleEffectAction(string particleName, Entity a, Entity b)
    {
        ParticleName = particleName;
        EntityA = a;
        EntityB = b;
    }

    public override void OnStart(Entity entity)
    {
        var effectPrefab = AssetLoader.LoadAsset(ParticleName);
        var effect = GameObject.Instantiate(effectPrefab);

        var position = (EntityA.GameObject.transform.position + EntityB.GameObject.transform.position) / 2.0f;

        effect.transform.position = position;
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
