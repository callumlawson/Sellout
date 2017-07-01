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

            var playerDecisionState = StaticStates.Get<PlayerDecisionsState>();
            if (playerDecisionState.ToldInspectorAboutDrugPusher || !playerDecisionState.AcceptedDrugPushersOffer)
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
         *      Day 2
         * */
        public static ActionSequence InspectorAskToDrink(Entity inspector)
        {
            var sequence = new ActionSequence("InspectorAskToDrink");
            sequence.Add(new ConversationAction(new InspectorAskToGetDrugPusherDrunk()));
            sequence.Add(DrinkOrders.GetRandomAlcoholicDrinkOrder(inspector));
            sequence.Add(CommonActions.SitDown());
            sequence.Add(CommonActions.SitDownLoop());
            return sequence;
        }

        public static ActionSequence DrugPusherAskToDrink(Entity drugPusher)
        {
            var sequence = new ActionSequence("DrugPusherAskToDrink");
            sequence.Add(new ConversationAction(new DrugPusherAskToGetinspectorDrunk()));
            sequence.Add(DrinkOrders.GetRandomAlcoholicDrinkOrder(drugPusher));
            sequence.Add(CommonActions.TalkToBarPatronsLoop());
            return sequence;
        }

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

        private class SingleLineConversation : Conversation
        {
            private string line;
            private string response;
            private DialogueOutcome outcome;

            public SingleLineConversation(string line, string response, DialogueOutcome outcome)
            {
                this.line = line;
                this.response = response;
                this.outcome = outcome;
            }

            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine(line);
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>" + response + "</i>", EndConversation(outcome));
            }
        }

        // Inspector Sequence //
        private static List<Conversation> inspectorFailureLines = new List<Conversation> {
            new SingleLineConversation("Hm... this doesn't taste right. I probably shouldn't be drinking anwyays.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Hm... this doesn't taste right. Too bad because the first one was great I probably should stop drinking anyways.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Hm... this doesn't taste right. *Hic* Well, I probably shouldn't drink anymore anways.", "Nod.", DialogueOutcome.Default),
        };
        private static List<Conversation> inspectorSuccessLines = new List<Conversation> {
            new SingleLineConversation("Ahh, just the thing to take the day off.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Delicious! I'm really feeling these, must be tired from the long day.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Yum! *Hic* You're a great bartender. The best bartender. Glad you're on the ship. This is the last one, promise. *Hic*.", "Nod.", DialogueOutcome.Default),
        };

        // DrugPusher Sequence //
        private static List<Conversation> drugPusherFailureLinesAccepted = new List<Conversation> {
            new SingleLineConversation("Hm... this doesn't taste right. Figures, you have business sense but not bartending skill.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Hm... this doesn't taste right. Guess the first was a fluke.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Hm... this doesn't taste right. *Hic* Well, I should stop anyways.", "Nod.", DialogueOutcome.Default),
        };
        private static List<Conversation> drugPusherFailureLinesRejected = new List<Conversation> {
            new SingleLineConversation("Hm... this doesn't taste right. Figures, you have no business sense or bartending skill.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Hm... this doesn't taste right. Guess the first was a fluke. Figures.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Hm... this doesn't taste right. *Hic* Figures, you have no business sense or bartending skill. *Hic*", "Nod.", DialogueOutcome.Default),
        };

        private static List<Conversation> drugPusherSuccessLinesAccepted = new List<Conversation> {
            new SingleLineConversation("Ahh, I've been needing this.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Good stuff, really takes the edge off.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Yum! *Hic* You're a great bartender. The best bartender. Glad we're business partners. *Hic*.", "Nod.", DialogueOutcome.Default),
        };
        private static List<Conversation> drugPusherSuccessLinesRejected = new List<Conversation> {
            new SingleLineConversation("Ahh, I've been needing this. No hard feelings from before", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("Good stuff, really takes the edge off.", "Nod.", DialogueOutcome.Default),
            new SingleLineConversation("*Hic* You may have terrible business sense but you make a good drink. *Hic* I'd better stop before I lose it.", "Nod.", DialogueOutcome.Default),
        };


        /**
         *      Day 1
         * */
        public static ActionSequence DrugPusherIntro(Entity drugPusher)
        {
            var sequence = new ActionSequence("DrugPusherIntro");
            sequence.Add(new OnActionStatusDecorator(
                OfferDrugs(drugPusher),
                () => {
                    var acceptSequence = new ActionSequence("AcceptedDrugOffer");
                    acceptSequence.Add(new ClearConversationAction());
                    acceptSequence.Add(new ConversationAction(new DrugPusherOfferAcceptedConversation()));
                    acceptSequence.Add(new UpdateMoodAction(Mood.Happy));
                    acceptSequence.Add(new PauseAction(0.5f));
                    acceptSequence.Add(new ReleaseWaypointAction());
                    acceptSequence.Add(new GoToPositionAction(Locations.OutsideDoorLocation()));
                    acceptSequence.Add(CommonActions.TalkToBarPatronsLoop());
                    sequence.Add(acceptSequence);

                    ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(drugPusher, acceptSequence);

                    StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer = true;
                },
                () => {
                    var disagreeSequence = new ActionSequence("RefusedDrugOffer");
                    disagreeSequence.Add(new ClearConversationAction());
                    disagreeSequence.Add(new ConversationAction(new DrugPusherOfferRefusedConversation()));
                    disagreeSequence.Add(new UpdateMoodAction(Mood.Angry));
                    disagreeSequence.Add(new PauseAction(0.5f));
                    disagreeSequence.Add(new ReleaseWaypointAction());
                    disagreeSequence.Add(new LeaveBarAction());

                    ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(drugPusher, disagreeSequence);

                    StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer = false;
                }
            ));

            return sequence;
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

        private class DrugPusherOfferRefusedConversation : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("Whatever, your loss.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>...</i>", EndConversation(DialogueOutcome.Default));
            }
        }

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
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(security,
                            new UpdateMoodAction(Mood.Happy));
                        StaticStates.Get<PlayerDecisionsState>().ToldInspectorAboutDrugPusher = true;
                    }
                },
                {
                    DialogueOutcome.Disagree, () =>
                    {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(security,
                            new UpdateMoodAction(Mood.Angry));
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

        public static void DrugPusherInspectorShowdown(Entity inspector, Entity drugPusher, Transform sitDownPoint)
        {
            var showdownPusher = new ActionSequence("ShowdownPusher");
            var showdownInspector = new ActionSequence("ShowdownInspector");
            var tookDrugMoney = StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer;
            var helpedInspector = StaticStates.Get<PlayerDecisionsState>().ToldInspectorAboutDrugPusher;

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

            //Do do consequence? Handcuffs? Fight?
            showdownInspector.Add(CommonActions.TalkToPlayer(new InspectorResolution(tookDrugMoney, helpedInspector)));
            showdownInspector.Add(new DialogueBranchAction(new Dictionary<DialogueOutcome, Action>
            {
                {
                    DialogueOutcome.Nice, () =>
                    {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(inspector, new UpdateMoodAction(Mood.Happy));
                    }
                },
                {
                    DialogueOutcome.Default, () =>
                    {
                    }
                },
                {
                    DialogueOutcome.Mean, () =>
                    {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(inspector, new UpdateMoodAction(Mood.Angry));
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(inspector, new ModifyMoneyAction(-100));
                    }
                }
            }));

            CommonActions.AddSyncEntityAction(inspector, drugPusher, showdownInspector, showdownPusher);

            showdownInspector.Add(new LeaveBarAction());

            showdownPusher.Add(new LeaveBarAction());

            ActionManagerSystem.Instance.QueueAction(drugPusher, showdownPusher);
            ActionManagerSystem.Instance.QueueAction(inspector, showdownInspector);
        }

        private class InspectorResolution : Conversation
        {
            private readonly bool tookDrugMoney;
            private readonly bool helpedInspector;

            public InspectorResolution(bool tookDrugMoney, bool helpedInspector)
            {
                this.tookDrugMoney = tookDrugMoney;
                this.helpedInspector = helpedInspector;
            }

            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("I've got him red handed - drugs and cash.");

                if (helpedInspector && !tookDrugMoney)
                {
                    DialogueSystem.Instance.WriteNPCLine("Thanks to your info I was able to stop this criminal. He had enough Space Weed on him to put him away for a good while.");
                    DialogueSystem.Instance.WriteNPCLine("Glad I could count on you to help keep this ship safe.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Nod.", EndConversation(DialogueOutcome.Nice));
                }
                else if (helpedInspector && tookDrugMoney)
                {
                    DialogueSystem.Instance.WriteNPCLine("Thanks to your info I was able to stop this criminal. He had enough Space Weed on him to put him away for a good while.");
                    DialogueSystem.Instance.WriteNPCLine("That said he was pretty brazen selling in the bar. You sure you weren't turning a blind eye?");
                    DialogueSystem.Instance.WritePlayerChoiceLine("...", EndConversation(DialogueOutcome.Default));
                }
                else if (!helpedInspector && tookDrugMoney)
                {
                    DialogueSystem.Instance.WriteNPCLine("This guy is going to space jail and you are lucky to not be going there with him.");
                    DialogueSystem.Instance.WriteNPCLine("He admitted to giving you kickbacks. I'll be having that - and then some.");
                    DialogueSystem.Instance.WriteNPCLine("Consider yourself on very thin ice.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("...", EndConversation(DialogueOutcome.Mean));
                }
                else //!helpedInpsector and !tookDrugMoney
                {
                    DialogueSystem.Instance.WriteNPCLine("I was lucky to catch him on his way in. I'm also pretty suprised you didn't see anything.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("...", EndConversation(DialogueOutcome.Default));
                }
            }
        }
    }
}
