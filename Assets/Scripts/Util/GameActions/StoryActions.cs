using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.Util.GameActions
{
    class StoryActions
    {
        public static ActionSequence TolstoyOne(Entity entity)
        {
            var story = new ActionSequence("TolstoyOne");

            var ordering = CommonActions.OrderDrink(entity, DrinkRecipes.GetRandomDrinkRecipe());
            var talkToPlayer = CommonActions.TalkToPlayer(new TolstoyOneDialogue());

            var startTime = StaticStates.Get<TimeState>().startTime;

            story.Add(ordering);
            story.Add(new WaitUntilAction(startTime.AddHours(2)));
            story.Add(talkToPlayer);

            return story;
        }

        private class TolstoyOneDialogue : Conversation
        {
            protected override void StartConversation()
            {
                DialogueSystem.Instance.StartDialogue();
                DialogueSystem.Instance.WriteNPCLine("Hello there, what you up to?");
                DialogueSystem.Instance.WritePlayerChoiceLine("You're a bit friendly.", BitFriendly);
                DialogueSystem.Instance.WritePlayerChoiceLine("I'm running the bar now.", RunningBar);
                DialogueSystem.Instance.WritePlayerChoiceLine("Sorry, gotta wipe this up. Can't talk now.", EndConversation);
            }

            private void BitFriendly()
            {
                DialogueSystem.Instance.WriteNPCLine("It's a small boat. Friendly will get you far.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Fair point - thanks.", EndConversation);
            }

            private void RunningBar()
            {
                DialogueSystem.Instance.WriteNPCLine("Ah, shame about poor Fred... still, glad you'll have the taps flowing again.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll do my best.", EndConversation);
            }
        }
    }
}
