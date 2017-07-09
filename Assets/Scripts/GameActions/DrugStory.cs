using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Systems.AI;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.GameActions.Decorators;
using Assets.Scripts.GameActions.Inventory;
using Assets.Scripts.States.Bar;
using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.GameActions.Bar;
using Assets.Scripts.GameActions.Waypoints;
using Assets.Scripts.GameActions.AILifecycle;
using Assets.Framework.Systems;
using Assets.Scripts.Util.NPC;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.GameActions.Navigation;

namespace Assets.Scripts.GameActions
{
    static class DrugStory
    {
        public static List<EntityActionPair> DayOneStart()
        {
            var startSequences = new List<EntityActionPair>();

            var q = EntityStateSystem.Instance.GetEntityWithName(NPCS.Q.Name);
            startSequences.Add(new EntityActionPair(q, DrugPusherIntro(q)));

            return startSequences;
        }

        public static List<EntityActionPair> DayTwoState()
        {
            var startSequences = new List<EntityActionPair>();

            var q = EntityStateSystem.Instance.GetEntityWithName(NPCS.Q.Name);
            var mcgraw = EntityStateSystem.Instance.GetEntityWithName(NPCS.McGraw.Name);
            
            if (IsDrugPusherDrinking())
            {
                startSequences.Add(new EntityActionPair(mcgraw, InspectorAskToDrink(mcgraw)));
                startSequences.Add(new EntityActionPair(q, DrugPusherDrinkTest(q)));
            }
            else
            {
                startSequences.Add(new EntityActionPair(q, DrugPusherAskToDrink(q)));
                startSequences.Add(new EntityActionPair(mcgraw, InspectorDrinkText(mcgraw)));
            }

            return startSequences;
        }

        /**
         *  Util
         * */
        private static bool IsDrugPusherDrinking()
        {
            var playerDecisionState = StaticStates.Get<PlayerDecisionsState>();
            return playerDecisionState.ToldInspectorAboutDrugPusher || !playerDecisionState.AcceptedDrugPushersOffer;
        }

        private class SingleOutcomeConversation : Conversation
        {
            private string[] lines;
            private string response;
            private DialogueOutcome outcome;

            public SingleOutcomeConversation(string line, string response, DialogueOutcome outcome)
            {
                this.lines = new string[] { line };
                this.response = response;
                this.outcome = outcome;
            }

            public SingleOutcomeConversation(string[] lines, string response, DialogueOutcome outcome)
            {
                this.lines = lines;
                this.response = response;
                this.outcome = outcome;
            }

            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                foreach (var line in lines)
                {
                    DialogueSystem.Instance.WriteNPCLine(line);
                }
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>" + response + "</i>", EndConversation(outcome));
            }
        }

        /**
         *      Day 1
         * */
        #region Day 1
        public static ActionSequence DrugPusherIntro(Entity drugPusher)
        {
            var sequence = new ActionSequence("DrugPusherIntro");
            sequence.Add(new OnActionStatusDecorator(
                OfferDrugs(drugPusher),
                () => {
                    switch (drugPusher.GetState<ActionBlackboardState>().ReceivedItemResponse) {
                        case ActionBlackboardState.ReceiveItemDecisionResponse.GaveBack:
                            DoRejectActionSequence(drugPusher, new DrugPusherOfferRefusedConversation());
                            break;
                        case ActionBlackboardState.ReceiveItemDecisionResponse.ThrewOut:
                            DoRejectActionSequence(drugPusher, new DrugPusherOfferRefusedStronlyConversation());
                            break;
                        case ActionBlackboardState.ReceiveItemDecisionResponse.Kept:
                            DoAcceptActionSequence(drugPusher, new DrugPusherOfferAcceptedConversation());
                            break;
                        case ActionBlackboardState.ReceiveItemDecisionResponse.GaveOtherItem:
                            DoAcceptActionSequence(drugPusher, new DrugPusherOfferAcceptedWithGiftConversation());
                            break;
                        case ActionBlackboardState.ReceiveItemDecisionResponse.None:
                            Debug.LogError("Somehow the response to the drug offer was was none.");
                            break;
                    }
                },
                () => {
                    Debug.LogError("Drug intro failed!.");
                }
            ));

            return sequence;
        }

        private static void DoAcceptActionSequence(Entity drugPusher, Conversation conversation)
        {
            var acceptSequence = new ActionSequence("AcceptedDrugOffer");
            acceptSequence.Add(new ClearConversationAction());
            acceptSequence.Add(new ConversationAction(conversation));
            acceptSequence.Add(new UpdateMoodAction(Mood.Happy));
            acceptSequence.Add(new PauseAction(0.5f));
            acceptSequence.Add(new ReleaseWaypointAction());
            acceptSequence.Add(new GoToPositionAction(Locations.OutsideDoorLocation()));
            acceptSequence.Add(CommonActions.TalkToBarPatronsLoop());

            ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(drugPusher, acceptSequence);

            StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer = true;
        }

        private static void DoRejectActionSequence(Entity drugPusher, Conversation conversation)
        {
            var disagreeSequence = new ActionSequence("RefusedDrugOffer");
            disagreeSequence.Add(new ClearConversationAction());
            disagreeSequence.Add(new ConversationAction(conversation));
            disagreeSequence.Add(new UpdateMoodAction(Mood.Angry));
            disagreeSequence.Add(new PauseAction(0.5f));
            disagreeSequence.Add(new ReleaseWaypointAction());
            disagreeSequence.Add(new LeaveBarAction());

            ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(drugPusher, disagreeSequence);

            StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer = false;
        }

        private static ConditionalActionSequence OfferDrugs(Entity drugPusher)
        {
            var receiveSpot = StaticStates.Get<BarEntities>().ReceiveSpot;

            var drugsTemplate = new List<IState>
            {
                new PrefabState(Prefabs.Drugs),
                new DrinkState(new DrinkState()),
                new PositionState(receiveSpot.GameObject.transform.position),
                new InventoryState()
            };

            var sequence = new ConditionalActionSequence("DrugOfferSequence");
            sequence.Add(new ReportSuccessDecorator(new ConversationAction(new DrugPusherOfferConversation())));
            sequence.Add(new PlaceItemInReceiveSpot(drugsTemplate));
            sequence.Add(new WaitForReceivedItemDecision());
            sequence.Add(new ClearConversationAction());
            return sequence;
        }

        private class DrugPusherOfferConversation : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("I want to sell Space Weed in your bar.");
                DialogueSystem.Instance.WriteNPCLine("If you turn a blind eye I'll give you some product and a cut of the money");
                DialogueSystem.Instance.WriteNPCLine("Here's a sample on the house.");
            }
        }

        private class DrugPusherOfferAcceptedConversation : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("Glad we can do business. I'll be back tonight with your cut.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Nod.</i>", EndConversation(DialogueOutcome.Default));
            }
        }

        private class DrugPusherOfferAcceptedWithGiftConversation : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("I guess that's a yes. Glad we can do business. I'll be back tonight with your cut.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Nod.</i>", EndConversation(DialogueOutcome.Default));
            }
        }

        private class DrugPusherOfferRefusedConversation : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("Whatever, your loss.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>...</i>", EndConversation(DialogueOutcome.Default));
            }
        }

        private class DrugPusherOfferRefusedStronlyConversation : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("That was valuable product, moron. I'll take that as a no. Your loss.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>...</i>", EndConversation(DialogueOutcome.Default));
            }
        }
        #endregion

        /**
         *      Day 2
         * */

        #region Day 2 - Morning
        public static ActionSequence InspectorQuestions(Entity security)
        {
            var questionTime = new ActionSequence("InspectorQuestions");
            questionTime.Add(StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer
                ? CommonActions.TalkToPlayer(new InspectorSuspicious())
                : CommonActions.TalkToPlayer(new InspectorNice()));
            questionTime.Add(new DialogueBranchAction(new Dictionary<DialogueOutcome, Action>
            {
                {
                    DialogueOutcome.Agree, () =>
                    {
                        var sequence = new ActionSequence("Help inspector.");
                        sequence.Add(new ConversationAction(new SingleOutcomeConversation("Thanks for the help! Glad I could count on you to keep the ship safe.", "Nod", DialogueOutcome.Agree)));
                        sequence.Add(new UpdateMoodAction(Mood.Happy));

                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(security, sequence);
                        StaticStates.Get<PlayerDecisionsState>().ToldInspectorAboutDrugPusher = true;
                    }
                },
                {
                    DialogueOutcome.Disagree, () =>
                    {
                        var sequence = new ActionSequence("Didn't help inspector.");
                        sequence.Add(new ConversationAction(new SingleOutcomeConversation("Thanks for your time. Let me know if you see anything suspicious.", "Nod", DialogueOutcome.Default)));

                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(security, sequence);
                        StaticStates.Get<PlayerDecisionsState>().ToldInspectorAboutDrugPusher = false;
                    }
                }
            }));
            questionTime.Add(new LeaveBarAction());
            return questionTime;
        }

        private class InspectorNice : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("Had any shifty looking types approach you of late?");
                DialogueSystem.Instance.WritePlayerChoiceLine("Yeah. 'Q' came and asked if he could sell drugs in the bar! I said no.", EndConversation(DialogueOutcome.Agree));
                DialogueSystem.Instance.WritePlayerChoiceLine("Afraid not, business as usual.", EndConversation(DialogueOutcome.Disagree));
            }
        }

        private class InspectorSuspicious : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("I've head that 'Q' has been flagrantly pushing Space Weed here.");
                DialogueSystem.Instance.WriteNPCLine("What do you know about it?");
                DialogueSystem.Instance.WritePlayerChoiceLine("Yeah, 'Q' has been less than discreet. He's worst on the evening shift. Can you do something about it?", EndConversation(DialogueOutcome.Agree));
                DialogueSystem.Instance.WritePlayerChoiceLine("Not that I've seen, and I'm here all day.", EndConversation(DialogueOutcome.Disagree));
            }
        }
        #endregion

        #region Day 2 - Open

        // Intro
        public static ActionSequence InspectorAskToDrink(Entity inspector)
        {
            var incorrectDrinkConversation = new SingleOutcomeConversation("Well.. this isn't right. I hope you do better when you serve him drinks.", "Right.", DialogueOutcome.Default);
            var correctDrinkConversation = new SingleOutcomeConversation("Mm, great drink. Make them like this and he'll be sure to slip up.", "Nod", DialogueOutcome.Default);

            var sequence = new ActionSequence("InspectorAskToDrink");
            sequence.Add(new ConversationAction(new InspectorAskToGetDrugPusherDrunk()));
            sequence.Add(DrinkOrders.GetRandomAlcoholicDrinkOrder(inspector, correctDrinkConversation: correctDrinkConversation, incorrectDrinkConversation: incorrectDrinkConversation));
            sequence.Add(CommonActions.GoToSeat());
            sequence.Add(CommonActions.SitDownLoop());
            return sequence;
        }

        public static ActionSequence DrugPusherAskToDrink(Entity drugPusher)
        {
            var incorrectDrinkConversation = new SingleOutcomeConversation("Well.. this isn't right. I hope you do better when you serve him drinks.", "Right.", DialogueOutcome.Default);
            var correctDrinkConversation = new SingleOutcomeConversation("Mm, great drink. Make them like this and there's no way he can do his job.", "Nod", DialogueOutcome.Default);

            var sequence = new ActionSequence("DrugPusherAskToDrink");
            sequence.Add(new ConversationAction(new DrugPusherAskToGetinspectorDrunk()));
            sequence.Add(DrinkOrders.GetRandomAlcoholicDrinkOrder(drugPusher, correctDrinkConversation: correctDrinkConversation, incorrectDrinkConversation: incorrectDrinkConversation));
            sequence.Add(CommonActions.TalkToBarPatronsLoop());
            return sequence;
        }

        private class DrugPusherAskToGetinspectorDrunk : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("Hey, I think the Inspector McGraw is on to us.");
                DialogueSystem.Instance.WriteNPCLine("Keep him happy with drinks and he'll be to smashed to do anything.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Nod.</i>", EndConversation(DialogueOutcome.Default));
            }
        }

        private class InspectorAskToGetDrugPusherDrunk : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("Hey, Q is about to come in and order some drinks.");
                DialogueSystem.Instance.WriteNPCLine("I know he's dealing but I can't catch him, he's too fast.");
                DialogueSystem.Instance.WriteNPCLine("Keep him happy with drinks and I'm sure he'll slip up.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Nod.</i>", EndConversation(DialogueOutcome.Default));
            }
        }

        // Minigame
        public static ActionSequence InspectorDrinkText(Entity inspector)
        {
            var failureConversations = inspectorFailureLines;
            var successConversations = inspectorSuccessLines;
            var betweenDrinks = new List<GameAction> { CommonActions.SitDownAndDrink(), CommonActions.SitDownAndDrink(), CommonActions.SitDownAndDrink() };
            var afterSuccess = CommonActions.SitDownLoop();
            return DrinkTest(0, 3, inspector, failureConversations, successConversations, betweenDrinks, afterSuccess);
        }

        public static ActionSequence DrugPusherDrinkTest(Entity drugPusher)
        {
            var failureConversations = StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer ? drugPusherFailureLinesAccepted : drugPusherFailureLinesRejected;
            var successConversations = StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer ? drugPusherSuccessLinesAccepted : drugPusherSuccessLinesRejected;
            var betweenDrinks = new List<GameAction> { CommonActions.TalkToBarPatron(), CommonActions.TalkToBarPatron(), CommonActions.TalkToBarPatron() };
            var afterSuccess = CommonActions.TalkToBarPatronsLoop();
            return DrinkTest(0, 3, drugPusher, failureConversations, successConversations, betweenDrinks, afterSuccess);
        }

        public static ActionSequence DrinkTest(int currentSuccesses, int maxSuccesses, Entity drinker, List<Conversation> failureConversations, List<Conversation> successConversations, List<GameAction> betweenDrinks, GameAction afterSuccess)
        {
            var sequence = new ActionSequence("DrinkTest: " + drinker);

            var drinkOrder = DrinkOrders.GetRandomAlcoholicDrinkOrderWithoutFailure(drinker);
            sequence.Add(new OnActionStatusDecorator(
                drinkOrder,
                () =>
                {
                    var successSequence = new ActionSequence("DrinkTestSuccess1: " + drinker);
                    if (currentSuccesses + 1 == maxSuccesses)
                    {
                        successSequence.Add(new ClearConversationAction());
                        successSequence.Add(new EndDrinkOrderAction());
                        successSequence.Add(new ConversationAction(successConversations[currentSuccesses]));
                        successSequence.Add(new ReleaseWaypointAction());
                        successSequence.Add(afterSuccess);
                    }
                    else
                    {
                        var nextDrinkTest = DrinkTest(currentSuccesses + 1, maxSuccesses, drinker, failureConversations, successConversations, betweenDrinks, afterSuccess);

                        successSequence.Add(new ClearConversationAction());
                        successSequence.Add(new EndDrinkOrderAction());
                        successSequence.Add(new ConversationAction(successConversations[currentSuccesses]));
                        successSequence.Add(new ReleaseWaypointAction());
                        successSequence.Add(betweenDrinks[currentSuccesses]);
                        successSequence.Add(new DestoryEntityInInventoryAction());
                        successSequence.Add(new QueueForBarWithPriority(nextDrinkTest));
                    }
                    ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(drinker, successSequence);
                },
                () =>
                {
                    var failureSequence = new ActionSequence("DrinkTestFail: " + currentSuccesses + " " + drinker);
                    failureSequence.Add(new ClearConversationAction());
                    failureSequence.Add(new EndDrinkOrderAction());
                    failureSequence.Add(new ConversationAction(failureConversations[currentSuccesses]));
                    failureSequence.Add(new DestoryEntityInInventoryAction());
                    failureSequence.Add(new ReleaseWaypointAction());
                    failureSequence.Add(new LeaveBarAction());
                    ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(drinker, failureSequence);
                    StaticStates.Get<PlayerDecisionsState>().NumberOfDrinksServedInDrugStory = currentSuccesses;
                }
            ));

            return sequence;
        }

        // Inspector Sequence //
        private static List<Conversation> inspectorFailureLines = new List<Conversation> {
            new SingleOutcomeConversation("Hm... this doesn't taste right. I probably shouldn't be drinking anwyays.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Hm... this doesn't taste right. Too bad because the first one was great I probably should stop drinking anyways.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Hm... this doesn't taste right. *Hic* Well, I probably shouldn't drink anymore anways.", "Nod.", DialogueOutcome.Default),
        };
        private static List<Conversation> inspectorSuccessLines = new List<Conversation> {
            new SingleOutcomeConversation("Ahh, just the thing to take the day off.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Delicious! I'm really feeling these, must be tired from the long day.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Yum! *Hic* You're a great bartender. The best bartender. Glad you're on the ship. This is the last one, promise. *Hic*.", "Nod.", DialogueOutcome.Default),
        };

        // DrugPusher Sequence //
        private static List<Conversation> drugPusherFailureLinesAccepted = new List<Conversation> {
            new SingleOutcomeConversation("Hm... this doesn't taste right. Figures, you have business sense but not bartending skill.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Hm... this doesn't taste right. Guess the first was a fluke.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Hm... this doesn't taste right. *Hic* Well, I should stop anyways.", "Nod.", DialogueOutcome.Default),
        };
        private static List<Conversation> drugPusherFailureLinesRejected = new List<Conversation> {
            new SingleOutcomeConversation("Hm... this doesn't taste right. Figures, you have no business sense or bartending skill.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Hm... this doesn't taste right. Guess the first was a fluke. Figures.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Hm... this doesn't taste right. *Hic* Figures, you have no business sense or bartending skill. *Hic*", "Nod.", DialogueOutcome.Default),
        };

        private static List<Conversation> drugPusherSuccessLinesAccepted = new List<Conversation> {
            new SingleOutcomeConversation("Ahh, I've been needing this.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Good stuff, really takes the edge off.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Yum! *Hic* You're a great bartender. The best bartender. Glad we're business partners. *Hic*.", "Nod.", DialogueOutcome.Default),
        };
        private static List<Conversation> drugPusherSuccessLinesRejected = new List<Conversation> {
            new SingleOutcomeConversation("Ahh, I've been needing this. No hard feelings from before", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("Good stuff, really takes the edge off.", "Nod.", DialogueOutcome.Default),
            new SingleOutcomeConversation("*Hic* You may have terrible business sense but you make a good drink. *Hic* I'd better stop before I lose it.", "Nod.", DialogueOutcome.Default),
        };


        #endregion

        #region Day 2 - Night
        public static void DrugPusherInspectorShowdown(Entity inspector, Entity drugPusher, Transform sitDownPoint)
        {
            var showdownPusher = new ActionSequence("ShowdownPusher");
            var showdownInspector = new ActionSequence("ShowdownInspector");

            showdownPusher.Add(new TeleportAction(sitDownPoint));
            showdownPusher.Add(new ReportSuccessDecorator(CommonActions.SitDownLoop()));

            //Confront each other! (Consider make this be in front of bar).
            CommonActions.AddSyncEntityAction(inspector, drugPusher, showdownInspector, showdownPusher);

            showdownInspector.Add(new SetTargetEntityAction(drugPusher));
            showdownInspector.Add(new GoToMovingEntityAction());

            CommonActions.AddSyncEntityAction(inspector, drugPusher, showdownInspector, showdownPusher);

            //Placeholder "agument". Perhaps the player could see what was being said?
            //Floating speach bubbles?
            showdownInspector.Add(new PauseAction(2));
            showdownInspector.Add(new UpdateMoodAction(Mood.Angry));

            showdownPusher.Add(new PauseAction(3));
            showdownPusher.Add(new UpdateMoodAction(Mood.Angry));

            showdownInspector.Add(new PauseAction(5));
            showdownInspector.Add(new UpdateMoodAction(Mood.Angry));

            showdownPusher.Add(new PauseAction(6));
            showdownPusher.Add(new UpdateMoodAction(Mood.Angry));

            CommonActions.AddSyncEntityAction(inspector, drugPusher, showdownInspector, showdownPusher);

            showdownPusher.Add(CommonActions.StandUp());
            showdownPusher.Add(new SetTargetEntityAction(inspector));
            showdownPusher.Add(new GoToMovingEntityAction(1.0f));

            CommonActions.AddSyncEntityAction(inspector, drugPusher, showdownInspector, showdownPusher);

            showdownInspector.Add(new PlayParticleEffectAction(Particles.Dustup, inspector, drugPusher));
            showdownPusher.Add(new PauseAction(5));

            CommonActions.AddSyncEntityAction(inspector, drugPusher, showdownInspector, showdownPusher);            

            ActionManagerSystem.Instance.QueueAction(drugPusher, showdownPusher);
            ActionManagerSystem.Instance.QueueAction(inspector, showdownInspector);

            Resolution(inspector, drugPusher);
        }

        private static void Resolution(Entity inspector, Entity drugPusher)
        {
            var decisionState = StaticStates.Get<PlayerDecisionsState>();
            var successfulDrinks = decisionState.NumberOfDrinksServedInDrugStory;

            var drugPusherActions = new ActionSequence("DrugPusher Resolution");
            var inspectorActions = new ActionSequence("Inspector Resolution");

            if (IsDrugPusherDrinking())
            {
                if (successfulDrinks == 0 || successfulDrinks == 1)
                {
                    drugPusherActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Run));
                    drugPusherActions.Add(new LeaveBarAction());
                    drugPusherActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Walk));

                    inspectorActions.Add(new ConversationAction(
                        new SingleOutcomeConversation("Damn it, he got away. I guess those drinks didn't do much did they. Maybe work on your bartending skills next time.",
                        "...",
                        DialogueOutcome.Bad)));


                    inspectorActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Slow));
                    inspectorActions.Add(new LeaveBarAction());
                    inspectorActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Walk));
                }
                else if (successfulDrinks == 2)
                {
                    if (decisionState.AcceptedDrugPushersOffer)
                    {
                        inspectorActions.Add(new ConversationAction(
                            new SingleOutcomeConversation(
                                new string[] {
                                    "Thanks to your help I was able to stop this criminal. He had enough Space Weed on him to put him away for a good while.",
                                    "He did say that you were in on it... he's pretty drunk though so he's probably just making it up."},
                                "...",
                                DialogueOutcome.Bad)
                            )
                        );
                    }
                    else
                    {
                        inspectorActions.Add(new ConversationAction(
                            new SingleOutcomeConversation(
                                new string[] {
                                    "Thanks to your help I was able to stop this criminal. He had enough Space Weed on him to put him away for a good while.",
                                    "You're a great asset to this crew."},
                                "...",
                                DialogueOutcome.Bad)
                            )
                        );
                    }

                    CommonActions.AddSyncEntityAction(inspector, drugPusher, inspectorActions, drugPusherActions);
                    drugPusherActions.Add(new LeaveBarAction());
                    inspectorActions.Add(new LeaveBarAction());
                }
                else if (successfulDrinks == 3)
                {
                    if (decisionState.AcceptedDrugPushersOffer)
                    {
                        inspectorActions.Add(new ConversationAction(
                            new SingleOutcomeConversation(
                                new string[] {
                                    "Thanks to your help I was able to stop this criminal. He had enough Space Weed on him to put him away for a good while.",
                                    "He tried to tell me something but he's so drunk he can barely speak! Good job."},
                                "...",
                                DialogueOutcome.Bad)
                            )
                        );
                    }
                    else
                    {
                        inspectorActions.Add(new ConversationAction(
                            new SingleOutcomeConversation(
                                new string[] {
                                    "Thanks to your help I was able to stop this criminal. He had enough Space Weed on him to put him away for a good while.",
                                    "You're a great asset to this crew."},
                                "...",
                                DialogueOutcome.Bad)
                            )
                        );
                    }

                    CommonActions.AddSyncEntityAction(inspector, drugPusher, inspectorActions, drugPusherActions);
                    drugPusherActions.Add(new LeaveBarAction());
                    inspectorActions.Add(new LeaveBarAction());
                }
            }
            else
            {
                if (successfulDrinks == 0 || successfulDrinks == 1)
                {
                    inspectorActions.Add(new ConversationAction(
                            new SingleOutcomeConversation(
                                new string[] {
                                    "No thanks to you I was able to catch this criminal. He had enough Space Weed on him to put him away for a good while.",
                                    "He tells me that you were in on it, I'm going to have my eye on you for a long time."},
                                "...",
                                DialogueOutcome.Bad)
                            )
                        );
                    CommonActions.AddSyncEntityAction(inspector, drugPusher, inspectorActions, drugPusherActions);
                    drugPusherActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Run));
                    drugPusherActions.Add(new LeaveBarAction());
                    drugPusherActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Walk));

                    inspectorActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Slow));
                    inspectorActions.Add(new LeaveBarAction());
                    inspectorActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Walk));
                }
                else if (successfulDrinks == 2)
                {
                    drugPusherActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Run));
                    drugPusherActions.Add(new LeaveBarAction());
                    drugPusherActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Walk));

                    inspectorActions.Add(new ConversationAction(
                        new SingleOutcomeConversation("*Hic* Damn, he got away. I shouldn't have drank so much. I'm sorry you had to see that.",
                        "...",
                        DialogueOutcome.Bad)));

                    inspectorActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Slow));
                    inspectorActions.Add(new LeaveBarAction());
                    inspectorActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Walk));
                }
                else if (successfulDrinks == 3)
                {
                    drugPusherActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Run));
                    drugPusherActions.Add(new LeaveBarAction());
                    drugPusherActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Walk));

                    inspectorActions.Add(new ConversationAction(
                        new SingleOutcomeConversation("*Hic* Damn... I shouldn't have drank so much. Sorry you had to see that, you're a good kid. *Hic*",
                        "...",
                        DialogueOutcome.Bad)));

                    inspectorActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Slow));
                    inspectorActions.Add(new LeaveBarAction());
                    inspectorActions.Add(new SetMovementSpeedAction(SetMovementSpeedAction.MovementType.Walk));
                }
            }

            ActionManagerSystem.Instance.QueueAction(drugPusher, drugPusherActions);
            ActionManagerSystem.Instance.QueueAction(inspector, inspectorActions);
        }

#endregion
    }
}
