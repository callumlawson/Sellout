using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Events;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class CharacterResponseSystem : IInitSystem
    {
        public void OnInit()
        {
            EventSystem.onClickInteraction += OnClickInteraction;
        }

        private static void OnClickInteraction(ClickEvent clickevent)
        {
            var targetEntity = clickevent.target;
            if (targetEntity.HasState<PrefabState>())
            {
                var prefab = targetEntity.GetState<PrefabState>();
                if (prefab.PrefabName == Prefabs.Person)
                {
                    if (Random.value > 0.4)
                    {
                        DemoDialogueOne.Start();
                    }
                    else
                    {
                        DemoDialogueTwo.Start();
                    }
                }
            }
        }

        private static class DemoDialogueOne
        {
            public static void Start()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("Hello there, what you up to?");
                DialogueSystem.Instance.WritePlayerChoiceLine("You're a bit friendly.", BitFriendly);
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm running the bar now.", RunningBar);
                DialogueSystem.Instance.WritePlayerChoiceLine("Sorry, gotta wipe this up. Can't talk now.", End);
            }

            private static void BitFriendly()
            {
                DialogueSystem.Instance.WriteNPCLine("It's a small boat. Friendly will get you far.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Fair point - thanks.", End);
            }

            private static void RunningBar()
            {
                DialogueSystem.Instance.WriteNPCLine("Ah, shame about poor Fred... still, glad you'll have the taps flowing again.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll do my best.", End);
            }

            private static void End()
            {
                DialogueSystem.Instance.StopDialogue();
            }
        }

        private static class DemoDialogueTwo
        {
            public static void Start()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("What you looking at?");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm looking at you.", Whoops);
            }

            private static void Whoops()
            {
                DialogueSystem.Instance.WriteNPCLine("What you looking at <b>me</b> for?" );
                DialogueSystem.Instance.WritePlayerChoiceLine("I, err, don't know.", DiggingHole);
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>walk away</i>", End);
            }

            private static void DiggingHole()
            {
                DialogueSystem.Instance.WriteNPCLine("Well sodd off then.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>walk quickly away</i>", End);
            }

            private static void End()
            {
                DialogueSystem.Instance.StopDialogue();
            }
        }
    }
}
