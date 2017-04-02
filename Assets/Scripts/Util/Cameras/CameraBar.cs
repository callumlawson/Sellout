using UnityEngine;
using DG.Tweening;
using Assets.Framework.States;
using Assets.Scripts.States;
using System.Linq;

namespace Assets.Scripts.Util.Cameras
{
    class CameraBar : MonoBehaviour, ICameraBehaviour
    {
        private Transform TargetCameraPosition;
        private bool Finished;        

        private bool MoveFinished;
        private bool RotateFinished;
        
        private Material[] playerMaterials;
        private Collider[] playerColliders;

        void Start()
        {
            var player = StaticStates.Get<PlayerState>().Player.GameObject;
            playerMaterials = player.GetComponentsInChildren<Renderer>().Select(renderer => renderer.material).ToArray();
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
            
            foreach (var material in playerMaterials)
            {
                SetMaterialaToTransparent(material);
                material.DOFade(0.0f, 1.0f);
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

            foreach (var material in playerMaterials)
            {
                material.DOFade(1.0f, 1.0f);
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

            foreach (var material in playerMaterials)
            {
                SetMaterialToOpaque(material);
            }
        }

        private void SetMaterialaToTransparent(Material material)
        {
            material.SetFloat("_Mode", 3);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }

        private void SetMaterialToOpaque(Material material)
        {
            material.SetFloat("_Mode", 0);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
        }
    }
}
