using UnityEngine;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.States;

public class StarfieldRootVisualizer : MonoBehaviour
{
    GameObject player;

    void Start()
    {
        var particles = GameObject.FindGameObjectsWithTag("Starfield");
        foreach (var particle in particles)
        {
            var system = particle.GetComponent<ParticleSystem>();
            var pm = system.main;
            pm.simulationSpace = ParticleSystemSimulationSpace.Custom;
            pm.customSimulationSpace = transform;
            system.Stop();
            system.Clear();
            system.Play();
        }
    }
    
    void Update()
    {
        if (player == null)
        {
            var playerState = StaticStates.Get<PlayerState>();
            if (playerState != null)
            {
                player = playerState.Player.GameObject;
            }

            if (player == null) return;
        }

        this.transform.position = player.transform.position;
    }
}
