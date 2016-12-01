using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util.Events;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class EntityInteractionSystem : IFrameSystem
    {
        public void OnFrame()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var selectedGameObject = StaticStates.Get<SelectedState>().SelectedEntity;
                if (selectedGameObject != null)
                {
                    EventSystem.BroadcastEvent(new ClickEvent(selectedGameObject));
                }
            }
        }
    }
}
