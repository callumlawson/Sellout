using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Util.Cameras
{
    class CameraFollow : MonoBehaviour, ICameraBehaviour
    {
        private const float SMOOTH_TIME = 0.1f;

        [UsedImplicitly] public bool LockX;
        [UsedImplicitly] public float offSetX;
        [UsedImplicitly] public float offSetY;
        [UsedImplicitly] public float forwardShift;
        [UsedImplicitly] public bool LockY;
        [UsedImplicitly] public bool LockZ;
        [UsedImplicitly] public bool UseSmoothing;

        private Transform target;

        private Transform thisTransform;

        private Vector3 velocity;

        private bool Finished;
        private bool IsActive;

        private Vector3 followCameraRotation;

        private Collider cameraBounds;
        
        [UsedImplicitly]
        void Start()
        {
            cameraBounds = GameObject.FindGameObjectWithTag("CameraBounds").GetComponent<Collider>();
        }

        [UsedImplicitly]
        void OnValidate()
        {
            if (target != null)
            {
                SetTarget(target);
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;

            var newPos = Vector3.zero;

            newPos.x = target.position.x + offSetX;
            newPos.y = target.position.y + offSetY;
            newPos.z = target.position.z;

            transform.position = newPos;

            transform.LookAt(target.position);

            followCameraRotation = transform.rotation.eulerAngles;
        }

        [UsedImplicitly]
        private void Awake()
        {
            thisTransform = transform;
            velocity = new Vector3(0.5f, 0.5f, 0.5f);
        }

        // ReSharper disable UnusedMember.Local
        private void LateUpdate()
        // ReSharper restore UnusedMember.Local
        {
            if (target == null || !IsActive)
            {
                return;
            }

            var newPos = GetNextCameraPosition(UseSmoothing);

            transform.position = Vector3.Slerp(transform.position, newPos, Time.time);
        }

        public Vector3 GetNextCameraPosition(bool useSmoothing)
        {
            var newPos = Vector3.zero;

            if (useSmoothing)
            {
                newPos.x = Mathf.SmoothDamp(thisTransform.position.x, target.position.x + offSetX, ref velocity.x, SMOOTH_TIME) - forwardShift;
                newPos.y = Mathf.SmoothDamp(thisTransform.position.y, target.position.y + offSetY, ref velocity.y, SMOOTH_TIME);
                newPos.z = Mathf.SmoothDamp(thisTransform.position.z, target.position.z, ref velocity.z, SMOOTH_TIME);
            }
            else
            {
                newPos.x = target.position.x + offSetX - forwardShift;
                newPos.y = target.position.y + offSetY;
                newPos.z = target.position.z;
            }

            if (LockX)
            {
                newPos.x = thisTransform.position.x;
            }

            if (LockY)
            {
                newPos.y = thisTransform.position.y;
            }

            if (LockZ)
            {
                newPos.z = thisTransform.position.z;
            }

            var bounds = cameraBounds.bounds;
            var extents = cameraBounds.bounds.extents;

            if (newPos.x < bounds.center.x - extents.x * 0.5f)
            {
                newPos.x = bounds.center.x - extents.x * 0.5f;
            }
            else if(newPos.x > bounds.center.x + extents.x * 0.5f)
            {
                newPos.x = bounds.center.x + extents.x * 0.5f;
            }

            if (newPos.z < bounds.center.z - extents.z * 0.5f)
            {
                newPos.z = bounds.center.z - extents.z * 0.5f;
            }
            if (newPos.z > bounds.center.z + extents.z * 0.5f)
            {
                newPos.z = bounds.center.z + extents.z * 0.5f;
            }

            return newPos;
        }

        public Vector3 GetFollowRotation()
        {
            return followCameraRotation;
        }

        public void StartCameraBehaviour()
        {
            Finished = false;
            IsActive = true;
        }

        public void StopCameraBehaviour()
        {
            Finished = true;
            IsActive = false;
        }

        public bool IsFinished()
        {
            return Finished;
        }
    }
}