using Assets.Framework.Entities;
using UnityEngine;
using Assets.Scripts.States;

namespace Assets.Scripts.Visualizers.Clothing
{
    class HeadColoring : MonoBehaviour, IEntityVisualizer
    {
        public MeshRenderer headMesh;

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
