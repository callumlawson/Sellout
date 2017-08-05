using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.Util;
using Assets.Framework.Systems;
using Assets.Scripts.Util.NPC;
using Assets.Framework.Entities;

namespace Assets.Scripts.GameActions.Stories
{
    static class LoveStory
    {
        #region Day 1 - Morning
        public static List<EntityActionPair> DayOneMorning()
        {
            var actions = new List<EntityActionPair>();

            var tolstoy = EntityStateSystem.Instance.GetEntityWithName(NPCS.Tolstoy.Name);
            var ellie = EntityStateSystem.Instance.GetEntityWithName(NPCS.Ellie.Name);

            //Ellie
            var ellieSequence = new ActionSequence("Ellie morning");
            ellieSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
            ellieSequence.Add(new SetReactiveConversationAction(new EllieMorningOne(), ellie));
            ellieSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy morning");
            tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint2()));
            tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyMorningOne(), tolstoy));
            tolstoySequence.Add(new TriggerAnimationAction(Util.AnimationEvent.SittingStartTrigger));
            tolstoySequence.Add(CommonActions.WaitForDrink(tolstoy, "None", new DrinkOrders.AlwaysSucceedsDrinkOrder(), 99999));
            tolstoySequence.Add(new UpdateMoodAction(Mood.Happy));
            tolstoySequence.Add(new ConversationAction(new TolstoyMorningGivenDrink()));
            tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyMorningAfterDrink()));
            tolstoySequence.Add(new CallbackAction(() => StaticStates.Get<PlayerDecisionsState>().GaveTolstoyDrink = true));
            tolstoySequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(tolstoy, tolstoySequence);
            
            return actions;
        }

        private class TolstoyMorningOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(NPCName.Tolstoy.ToString());
                DialogueSystem.Instance.WriteNPCLine("Hey, how you doing? Fine? Good.");
                DialogueSystem.Instance.WriteNPCLine("Don't you think Ellie's great?");
                DialogueSystem.Instance.WriteNPCLine("You should talk to her.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Err, sure. I want to meet everyone.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Perhaps some other time. Got to get the bar ready!", EndConversation(DialogueOutcome.Default));
            }
        }

        private class TolstoyMorningGivenDrink : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(NPCName.Tolstoy.ToString());
                DialogueSystem.Instance.WriteNPCLine("Wow, thanks. I really needed this.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Everyone has one of those days occasionally.", EndConversation(DialogueOutcome.Default));
            }
        }

        private class TolstoyMorningAfterDrink : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(NPCName.Tolstoy.ToString());
                DialogueSystem.Instance.WriteNPCLine("Really, thank you!");
                DialogueSystem.Instance.WritePlayerChoiceLine("No worries", EndConversation(DialogueOutcome.Default));
            }
        }

        private class EllieMorningOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                DialogueSystem.Instance.WriteNPCLine("Hi. Good luck with your new post. I'm sure you'll do fine.");
                DialogueSystem.Instance.WriteNPCLine("Hmm, Tolstoy really looks like he needs a drink.");
                DialogueSystem.Instance.WriteNPCLine("He's been all wierd and agitated recently.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Luckily serving drinks is my forte!", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Sure.", EndConversation(DialogueOutcome.Mean));
            }
        }
        #endregion

        #region Day 1 - Night
        public static List<EntityActionPair> DayOneNight(Entity[] chosenSeats)
        {
            var actions = new List<EntityActionPair>();

            var tolstoy = EntityStateSystem.Instance.GetEntityWithName(NPCS.Tolstoy.Name);
            var ellie = EntityStateSystem.Instance.GetEntityWithName(NPCS.Ellie.Name);

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy night");
            tolstoySequence.Add(new TeleportAction(chosenSeats[1].GameObject.transform));
            tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyNightOne(), tolstoy));
            tolstoySequence.Add(CommonActions.SitDownLoop());
            actions.Add(new EntityActionPair(tolstoy, tolstoySequence));

            //Ellie
            var ellieSequence = new ActionSequence("Ellie night");
            ellieSequence.Add(new TeleportAction(chosenSeats[2].GameObject.transform));
            ellieSequence.Add(new SetReactiveConversationAction(new EllieNightOne(), ellie));
            ellieSequence.Add(CommonActions.SitDownLoop());
            actions.Add(new EntityActionPair(ellie, ellieSequence));

            return actions;
        }

        private class TolstoyNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                if (StaticStates.Get<PlayerDecisionsState>().GaveTolstoyDrink)
                {
                    DialogueSystem.Instance.WriteNPCLine("Handing me that drink this morning was a little thing.");
                    DialogueSystem.Instance.WriteNPCLine("But you don't even know me, don't owe me anything.");
                    DialogueSystem.Instance.WriteNPCLine("Dave wouldn't have done that.");
                    DialogueSystem.Instance.WriteNPCLine("Thank you.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("It was nothing, glad to help.", EndConversation(DialogueOutcome.Nice));
                    DialogueSystem.Instance.WritePlayerChoiceLine("A happy customer comes back.", EndConversation(DialogueOutcome.Default));
                }
                else
                {
                    DialogueSystem.Instance.WriteNPCLine("I know it's late. I'm finishing up, don't worry.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("I'm not in a rush - take your time.", EndConversation(DialogueOutcome.Nice));
                    DialogueSystem.Instance.WritePlayerChoiceLine("You have 10 minutes.", EndConversation(DialogueOutcome.Mean));
                }
            }
        }

        private class EllieNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                DialogueSystem.Instance.WriteNPCLine("When I ask for a drink containing my favorite ingredient I don't always want the same thing.");
                DialogueSystem.Instance.WriteNPCLine("That would be boring.");
                DialogueSystem.Instance.WritePlayerChoiceLine("I'll keep that in mind.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Why don't you just order exactly what you want!", EndConversation(DialogueOutcome.Mean));
            }
        }
        #endregion

        #region Day 2 - Morning
        public static List<EntityActionPair> DayTwoMorning()
        {
            var actions = new List<EntityActionPair>();

            var tolstoy = EntityStateSystem.Instance.GetEntityWithName(NPCS.Tolstoy.Name);
            var ellie = EntityStateSystem.Instance.GetEntityWithName(NPCS.Ellie.Name);

            //Ellie
            var ellieSequence = new ActionSequence("Ellie Day Two Morning");
            ellieSequence.Add(new PauseAction(0.5f)); //WORKAROUND FOR SYNC ACTION BUG
            ellieSequence.Add(new TeleportAction(Locations.SitDownPoint1()));
            ellieSequence.Add(new SetReactiveConversationAction(new EllieMorningOne(), ellie));
            ellieSequence.Add(CommonActions.SitDownLoop());
            actions.Add(new EntityActionPair(ellie, ellieSequence));

            //Tolstoy
            var tolstoySequence = new ActionSequence("Tolstoy Day Two Morning");
            tolstoySequence.Add(new PauseAction(0.5f)); //WORKAROUND FOR SYNC ACTION BUG
            tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint2()));
            tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyMorningOne(), tolstoy));
            tolstoySequence.Add(CommonActions.SitDownLoop());
            actions.Add(new EntityActionPair(tolstoy, tolstoySequence));            

            return actions;
        }
        #endregion

        #region Day 2 - Night
        public static List<EntityActionPair> DayTwoNight()
        {
            var actions = new List<EntityActionPair>();

            var tolstoy = EntityStateSystem.Instance.GetEntityWithName(NPCS.Tolstoy.Name);
            var ellie = EntityStateSystem.Instance.GetEntityWithName(NPCS.Ellie.Name);

            //Ellie
            var ellieSequence = new ActionSequence("Ellie night two");
            ellieSequence.Add(new TeleportAction(Locations.SitDownPoint3()));
            ellieSequence.Add(new SetReactiveConversationAction(new EllieNightTwo(ellie.GetState<RelationshipState>()), ellie));
            ellieSequence.Add(CommonActions.SitDownLoop());
            ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);

            return actions;
        }

        private class EllieNightTwo : Conversation
        {
            private readonly RelationshipState relationship;

            public EllieNightTwo(RelationshipState relationship)
            {
                this.relationship = relationship;
            }

            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                if (relationship.PlayerOpinion > 0)
                {
                    DialogueSystem.Instance.WriteNPCLine("I'm really glad you work here. You always have a nice thing to say.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Thanks!", EndConversation(DialogueOutcome.Nice));
                }
                else
                {
                    DialogueSystem.Instance.WriteNPCLine("You know, a few kind words can go along way. People come here looking for support sometimes.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Fair point, I'll work on that.", EndConversation(DialogueOutcome.Nice));
                    DialogueSystem.Instance.WritePlayerChoiceLine("I'm not the ship's physiatrist. They should find somewhere else!", EndConversation(DialogueOutcome.Mean));
                }
            }
        }
        #endregion

        #region Day 3 - Morning
        public static List<EntityActionPair> DayThreeMorning()
        {
            var actions = new List<EntityActionPair>();

            var tolstoy = EntityStateSystem.Instance.GetEntityWithName(NPCS.Tolstoy.Name);
            var ellie = EntityStateSystem.Instance.GetEntityWithName(NPCS.Ellie.Name);



            return actions;
        }
        #endregion
    }
}
