using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class EntityInteractionSystem : IFrameSystem
    {
        public void OnFrame()
        {
            if (Input.GetMouseButton(0))
            {
                var selectedGameObject = StaticStates.Get<SelectedState>().SelectedEntity;
                if (selectedGameObject != null)
                {
                    var interactionState = selectedGameObject.GetState<InteractionState>();
                    if (interactionState != null)
                    {
                        var message = interactionState.message;
                        EventSystem.BroadcastMessage(message);
                    }
                }
            }
        }
    }
}
