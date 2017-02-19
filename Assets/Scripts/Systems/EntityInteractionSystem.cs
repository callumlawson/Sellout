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
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                var cursorState = StaticStates.Get<CursorState>();
                EventSystem.BroadcastEvent(new ClickEvent(cursorState.SelectedEntity, cursorState.MousedOverPosition, 0));
            }

            if (Input.GetMouseButtonDown(1) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                var cursorState = StaticStates.Get<CursorState>();
                EventSystem.BroadcastEvent(new ClickEvent(cursorState.SelectedEntity, cursorState.MousedOverPosition, 1));
            }
        }
    }
}
