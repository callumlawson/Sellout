using JetBrains.Annotations;
using UnityEngine;

namespace Assets
{
    public class RenderDepth : MonoBehaviour {

        [UsedImplicitly]
        void OnEnable()
        {
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
        }
    }
}
