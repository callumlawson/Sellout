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
            var cursorState = StaticStates.Get<CursorState>();
            var selectedEntity = cursorState.SelectedEntity;
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() || selectedEntity == null) 
            {
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedEntity.HasState<InteractiveState>() && !selectedEntity.GetState<InteractiveState>().CurrentlyInteractive)
                {
                    EventSystem.BroadcastEvent(new ClickEvent(selectedEntity, cursorState.MousedOverPosition, 0));
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                EventSystem.BroadcastEvent(new ClickEvent(selectedEntity, cursorState.MousedOverPosition, 1));
            }
        }
    }
}
