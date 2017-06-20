using Assets.Framework.States;
using Assets.Scripts.States;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visualizers.Window
{
    public class StarfieldRootVisualizer : MonoBehaviour
    {
        GameObject player;

        [UsedImplicitly]
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
    
        [UsedImplicitly]
        void Update()
        {
            if (!GameRunner.Instance.GameStarted)
            {
                return;
            }

            if (player == null)
            {
                var playerState = StaticStates.Get<PlayerState>();
                if (playerState != null)
                {
                    player = playerState.Player.GameObject;
                }

                if (player == null) return;
            }

            transform.position = player.transform.position;
        }
    }
}
