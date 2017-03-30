namespace Assets.Scripts.Util.Cameras
{
    interface ICameraBehaviour
    {
        void StartCameraBehaviour();
        void StopCameraBehaviour();

        bool IsFinished();
    }
}
