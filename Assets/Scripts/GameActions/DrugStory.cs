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

namespace Assets.Scripts.GameActions
{
    static class DrugStory
    {
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
                    acceptSequence.Add(CommonActions.TalkToBarPatrons());
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
            var sync = new SyncedAction(inspector, drugPusher);
            showdownPusher.Add(sync);
            showdownInspector.Add(sync);

            showdownInspector.Add(new SetTargetEntityAction(drugPusher));
            showdownInspector.Add(new GoToMovingEntityAction());

            var sync2 = new SyncedAction(inspector, drugPusher);
            showdownPusher.Add(sync2);
            showdownInspector.Add(sync2);

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

            var sync3 = new SyncedAction(inspector, drugPusher);
            showdownPusher.Add(sync3);
            showdownInspector.Add(sync3);

            showdownInspector.Add(new LeaveBarAction());

            showdownPusher.Add(CommonActions.StandUp());
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
