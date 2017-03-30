using UnityEngine;
using DG.Tweening;
using Assets.Framework.States;
using Assets.Scripts.States;

namespace Assets.Scripts.Util.Cameras
{
    class CameraBar : MonoBehaviour, ICameraBehaviour
    {
        private Transform TargetCameraPosition;
        private bool Finished;        

        private bool MoveFinished;
        private bool RotateFinished;
        
        private Renderer[] playerRenderers;
        private Collider[] playerColliders;

        void Start()
        {
            var player = StaticStates.Get<PlayerState>().Player.GameObject;
            playerRenderers = player.GetComponentsInChildren<Renderer>();
            playerColliders = player.GetComponentsInChildren<Collider>();
        }

        public bool IsFinished()
        {
            return Finished;
        }

        public void StartCameraBehaviour()
        {
            if (TargetCameraPosition == null)
            {
                TargetCameraPosition = GameObject.FindGameObjectWithTag("BarCameraTransform").transform;
            }

            Finished = false;
            MoveFinished = false;
            RotateFinished = false;

            transform.DOMove(TargetCameraPosition.position, 1.0f);
            transform.DORotate(TargetCameraPosition.rotation.eulerAngles, 1.0f);
            
            foreach (var renderer in playerRenderers)
            {
                SetMaterialaToTransparent(renderer);
                renderer.sharedMaterial.DOFade(0.0f, 1.0f);
            }

            foreach (var collider in playerColliders)
            {
                collider.enabled = false;
            }
        }

        public void StopCameraBehaviour()
        {
            var cameraFollow = GetComponent<CameraFollow>();
            var cameraFollowPosition = cameraFollow.GetNextCameraPosition(false);
            var cameraFollowRotation = cameraFollow.GetFollowRotation();
            
            transform.DOMove(cameraFollowPosition, 1.0f).OnComplete(OnMoveComplete);
            transform.DORotate(cameraFollowRotation, 1.0f).OnComplete(OnRotateComplete);

            foreach (var renderer in playerRenderers)
            {
                renderer.sharedMaterial.DOFade(1.0f, 1.0f);
            }

            foreach (var collider in playerColliders)
            {
                collider.enabled = true;
            }
        }

        private void OnRotateComplete()
        {
            RotateFinished = true;
            Finished = RotateFinished && MoveFinished;
        }

        private void OnMoveComplete()
        {
            MoveFinished = true;
            Finished = RotateFinished && MoveFinished;

            foreach (var renderer in playerRenderers)
            {
                SetMaterialToOpaque(renderer);
            }
        }

        private void SetMaterialaToTransparent(Renderer renderer)
        {
            renderer.sharedMaterial.SetFloat("_Mode", 3);
            renderer.sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            renderer.sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            renderer.sharedMaterial.SetInt("_ZWrite", 0);
            renderer.sharedMaterial.DisableKeyword("_ALPHATEST_ON");
            renderer.sharedMaterial.DisableKeyword("_ALPHABLEND_ON");
            renderer.sharedMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            renderer.sharedMaterial.renderQueue = 3000;
        }

        private void SetMaterialToOpaque(Renderer renderer)
        {
            renderer.sharedMaterial.SetFloat("_Mode", 0);
            renderer.sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            renderer.sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            renderer.sharedMaterial.SetInt("_ZWrite", 1);
            renderer.sharedMaterial.DisableKeyword("_ALPHATEST_ON");
            renderer.sharedMaterial.DisableKeyword("_ALPHABLEND_ON");
            renderer.sharedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            renderer.sharedMaterial.renderQueue = -1;
        }
    }
}
