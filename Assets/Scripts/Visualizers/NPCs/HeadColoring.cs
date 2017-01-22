using Assets.Framework.Entities;
using UnityEngine;
using Assets.Scripts.States;
using JetBrains.Annotations;

namespace Assets.Scripts.Visualizers.NPCs
{
    class HeadColoring : MonoBehaviour, IEntityVisualizer
    {
        [UsedImplicitly] public MeshRenderer headMesh;

        private Material material;

        public void OnStartRendering(Entity entity)
        {
            material = headMesh.material;
            material.color = entity.GetState<ColorState>().Color; 
        }

        public void OnFrame()
        {
            
        }

        public void OnStopRendering(Entity entity)
        {
            Destroy(material);
        }
    }
}
