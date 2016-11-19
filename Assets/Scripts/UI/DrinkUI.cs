using UnityEngine;

namespace Assets.Scripts.Systems
{
    class DrinkUI : MonoBehaviour
    {
        public delegate void OnMixEvent();
        public delegate void OnCloseEvent();

        public OnMixEvent onMixEvent;
        public OnCloseEvent onCloseEvent;

        public void Mix()
        {
            onMixEvent();
        }
        
        public void Close()
        {
            onCloseEvent();
        }
    }
}
