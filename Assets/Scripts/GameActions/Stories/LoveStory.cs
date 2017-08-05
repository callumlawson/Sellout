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
using System;
using System.Linq;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions.Stories
{
    static class LoveStory
    {
        private static string elliesFavoriteDrink = DrinkRecipes.GetRandomAlcoholicDrinkRecipe().DrinkName;

        public static List<EntityActionPair> DayTwoState()
        {
            var startSequences = new List<EntityActionPair>();

            if (StaticStates.Get<PlayerDecisionsState>().TolstoyAskedToMakeDrink)
            {
                var ellie = EntityStateSystem.Instance.GetEntityWithName(NPCS.Ellie.Name);
                startSequences.Add(new EntityActionPair(ellie, EllieAskForDrink(ellie)));
            }

            return startSequences;
        }

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
                    DialogueSystem.Instance.WriteNPCLine("Handing me that drink this morning was ");
                    DialogueSystem.Instance.WriteNPCLine("It gave me enough liquid courage to talk to Ellie!");
                    DialogueSystem.Instance.WriteNPCLine("I found out that her favorite drink is a " + elliesFavoriteDrink + ".");
                    DialogueSystem.Instance.WriteNPCLine("Do you think tomorrow you could give her one and say it's from me?");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Sure thing.", TolstoyAsked(DialogueOutcome.Nice));
                    DialogueSystem.Instance.WritePlayerChoiceLine("Maybe. If I remember. I'll be busy working.", TolstoyAsked(DialogueOutcome.Bad));
                }
                else
                {
                    DialogueSystem.Instance.WriteNPCLine("I know it's late. I'm finishing up, don't worry.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("I'm not in a rush - take your time.", () => IfNiceToTolstoy());
                    DialogueSystem.Instance.WritePlayerChoiceLine("You have 10 minutes.", EndConversation(DialogueOutcome.Mean));
                }
            }

            private void IfNiceToTolstoy()
            {
                DialogueSystem.Instance.StartDialogue("Tolstoy");
                DialogueSystem.Instance.WriteNPCLine("If you have some time to talk... do you think you could give me some advice?");
                DialogueSystem.Instance.WriteNPCLine("I really want to talk to Ellie but I don't know what to say.");
                DialogueSystem.Instance.WriteNPCLine("You talk to a lot of people every day, I figured you'd be good at this.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Tell her how intelligent and beautiful she is.", TolstoyAdvice(DialogueOutcome.Nice, PlayerDecisionsState.TolstoyAdviceChoices.Compliment));
                DialogueSystem.Instance.WritePlayerChoiceLine("Ask her to have a drink with you tomorrow.", TolstoyAdvice(DialogueOutcome.Nice, PlayerDecisionsState.TolstoyAdviceChoices.AskOnDate));
                DialogueSystem.Instance.WritePlayerChoiceLine("Find out something she likes and get it for her, like a drink.", TolstoyAdvice(DialogueOutcome.Nice, PlayerDecisionsState.TolstoyAdviceChoices.FindFavoriteDrink));
                DialogueSystem.Instance.WritePlayerChoiceLine("Your're asking the wrong person.", EndConversation(DialogueOutcome.Mean));
            }

            private Action TolstoyAsked(DialogueOutcome outcome)
            {
                return () =>
                {
                    StaticStates.Get<PlayerDecisionsState>().TolstoyAskedToMakeDrink = true;
                    EndConversation(outcome).Invoke();
                };
            }

            private Action TolstoyAdvice(DialogueOutcome outcome, PlayerDecisionsState.TolstoyAdviceChoices choice)
            {
                return () =>
                {
                    StaticStates.Get<PlayerDecisionsState>().TolstoyAdviceChoice = choice;

                    if (choice == PlayerDecisionsState.TolstoyAdviceChoices.FindFavoriteDrink)
                    {
                        StaticStates.Get<PlayerDecisionsState>().TolstoyAskedToMakeDrink = true;
                    }

                    EndConversation(outcome).Invoke();
                };
            }
        }

        private class EllieNightOne : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue("Ellie");
                DialogueSystem.Instance.WriteNPCLine("I love when bartenders introduce me to new drinks using my favorite ingredients.");
                DialogueSystem.Instance.WriteNPCLine("It's way less boring than ordering the same thing every time!");
                DialogueSystem.Instance.WritePlayerChoiceLine("Good advice, I'll keep that in mind.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("More work for me. Why don't you find new drinks yourself?", EndConversation(DialogueOutcome.Mean));
            }
        }
        #endregion

        #region Day 2 - Morning
        public static List<EntityActionPair> DayTwoMorning()
        {
            var actions = new List<EntityActionPair>();

            var playerChoices = StaticStates.Get<PlayerDecisionsState>();

            if (!playerChoices.GaveTolstoyDrink && playerChoices.TolstoyAdviceChoice == PlayerDecisionsState.TolstoyAdviceChoices.FindFavoriteDrink)
            {
                var tolstoy = EntityStateSystem.Instance.GetEntityWithName(NPCS.Tolstoy.Name);

                //Tolstoy
                var tolstoySequence = new ActionSequence("Tolstoy Day Two Morning");
                tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint2()));
                tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyMorningTwo(), tolstoy));
                tolstoySequence.Add(CommonActions.SitDownLoop());
                actions.Add(new EntityActionPair(tolstoy, tolstoySequence));
            }

            return actions;
        }

        private class TolstoyMorningTwo : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(NPCName.Tolstoy.ToString());
                DialogueSystem.Instance.WriteNPCLine("Hey, I found out Ellie's favorite drink.");
                DialogueSystem.Instance.WriteNPCLine("It's " + elliesFavoriteDrink + "?");
                DialogueSystem.Instance.WriteNPCLine("Do you think you can make one for her tonight and say it's from me?");
                DialogueSystem.Instance.WritePlayerChoiceLine("Sure thing.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Maybe, it'll be a busy night for me.", EndConversation(DialogueOutcome.Default));
            }
        }
        #endregion

        #region Day 2 - Open
        public static ActionSequence EllieAskForDrink(Entity ellie)
        {
            var incorrectDrinkConversation = new NoResponseConversation("This doesn't seem quite right. I'm sure you'll do better next time.", DialogueOutcome.Default);
            var correctDrinkConversation = new NoResponseConversation("Mm, delicious!.", DialogueOutcome.Default);

            var favoriteDrinkActions = new ActionSequence("Favorite drink received.");
            favoriteDrinkActions.Add(new ConversationAction(new FavoriteDrinkConversation()));
            favoriteDrinkActions.Add(new UpdateMoodAction(Mood.Happy));

            var otherDrinkActions = new Dictionary<String, GameAction>()
            {
                {elliesFavoriteDrink, favoriteDrinkActions}
            };

            var sequence = new ActionSequence("Ellie ask for drink.");

            var randomRecipeNotFavorite = DrinkRecipes.Recipes.Where(recipe => recipe.DrinkName != elliesFavoriteDrink).PickRandom();            
            var drinkOrder = new DrinkOrders.ExactDrinkorder(randomRecipeNotFavorite, ellie.GetState<NameState>().Name);

            sequence.Add(DrinkOrders.GetRandomAlcoholicDrinkOrder(ellie, correctDrinkConversation: correctDrinkConversation, incorrectDrinkConversation: incorrectDrinkConversation, otherDrinkActions: otherDrinkActions));
            sequence.Add(CommonActions.GoToSeat());
            sequence.Add(CommonActions.SitDownLoop());
            return sequence;
        }

        private class FavoriteDrinkConversation : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(NPCName.Ellie.ToString());
                DialogueSystem.Instance.WriteNPCLine("Wow, this is my favorite drink! How did you know?");
                DialogueSystem.Instance.WritePlayerChoiceLine("Tolstoy asked me to make one for you.", ToldAboutTolstoy());
                DialogueSystem.Instance.WritePlayerChoiceLine("Lucky guess!", DidntTellAboutTolstoy());
            }

            private Action DidntTellAboutTolstoy()
            {
                return () =>
                {
                    var ellie = EntityStateSystem.Instance.GetEntityWithName(NPCS.Ellie.Name);
                    ellie.GetState<RelationshipState>().PlayerOpinion += 2;

                    StaticStates.Get<PlayerDecisionsState>().GaveEllieTolstoysDrink = false;
                    EndConversation(DialogueOutcome.Nice).Invoke();                    
                };
            }

            private Action ToldAboutTolstoy()
            {
                return () =>
                {
                    var ellie = EntityStateSystem.Instance.GetEntityWithName(NPCS.Ellie.Name);
                    ellie.GetState<RelationshipState>().PlayerOpinion++;

                    StaticStates.Get<PlayerDecisionsState>().GaveEllieTolstoysDrink = true;
                    EndConversation(DialogueOutcome.Nice).Invoke();
                };
            }
        }
        #endregion

        #region Day 2 - Night
        public static List<EntityActionPair> DayTwoNight()
        {
            var actions = new List<EntityActionPair>();

            var tolstoy = EntityStateSystem.Instance.GetEntityWithName(NPCS.Tolstoy.Name);
            var ellie = EntityStateSystem.Instance.GetEntityWithName(NPCS.Ellie.Name);

            var tolstoySequence = new ActionSequence("Tolstoy night two");
            var ellieSequence = new ActionSequence("Ellie night two");

            if (StaticStates.Get<PlayerDecisionsState>().GaveEllieTolstoysDrink)
            {
                tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint2()));
                tolstoySequence.Add(new SetReactiveConversationAction(new TosltoyNightTwoSuccess()));

                ellieSequence.Add(new TeleportAction(Locations.SitDownPoint3()));
                ellieSequence.Add(new SetReactiveConversationAction(new EllieNightTwoSuccess()));
            }
            else
            {
                tolstoySequence.Add(new TeleportAction(Locations.SitDownPoint2()));
                tolstoySequence.Add(new SetReactiveConversationAction(new TolstoyNightTwoFailure()));

                ellieSequence.Add(new TeleportAction(Locations.SitDownPoint3()));
                ellieSequence.Add(new SetReactiveConversationAction(new EllieNightTwoFailure(ellie.GetState<RelationshipState>()), ellie));
            }

            tolstoySequence.Add(new TriggerAnimationAction(Util.AnimationEvent.SittingStartTrigger));
            actions.Add(new EntityActionPair(tolstoy, tolstoySequence));

            ellieSequence.Add(new TriggerAnimationAction(Util.AnimationEvent.SittingStartTrigger));
            ActionManagerSystem.Instance.QueueAction(ellie, ellieSequence);

            return actions;
        }

        private class TosltoyNightTwoSuccess : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.WriteNPCLine("Thank you for your help!");
                DialogueSystem.Instance.WriteNPCLine("I've liked " + NPCName.Ellie + " for a long time.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Always glad to help!", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Just doing my job.", EndConversation(DialogueOutcome.Default));
            }
        }

        private class TolstoyNightTwoFailure : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.WriteNPCLine("On well, I guess it didn't work.");
                DialogueSystem.Instance.WriteNPCLine("I guess there's always next time.");
                DialogueSystem.Instance.WritePlayerChoiceLine("Just keep trying.", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Might as well give up.", EndConversation(DialogueOutcome.Mean));
            }
        }

        private class EllieNightTwoSuccess : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.WriteNPCLine("Thank you for the drink earlier, it's my favorite.");
                DialogueSystem.Instance.WriteNPCLine("I'm having a great time with " + NPCName.Tolstoy + ".");
                DialogueSystem.Instance.WritePlayerChoiceLine("Glad you two are getting along!", EndConversation(DialogueOutcome.Nice));
                DialogueSystem.Instance.WritePlayerChoiceLine("Just doing my job.", EndConversation(DialogueOutcome.Default));
            }
        }

        private class EllieNightTwoFailure : Conversation
        {
            private readonly RelationshipState relationship;

            public EllieNightTwoFailure(RelationshipState relationship)
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
    }
}
