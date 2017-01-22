using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Visualizers
{
    class MovementFromNavAgent : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Animator animator;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            var currentSpeed = agent.velocity.sqrMagnitude;
            animator.SetFloat("Velocity", currentSpeed);
            animator.SetBool("Moving", currentSpeed > 0.1f);
        }           
    }
}
