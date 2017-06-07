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

namespace Assets.Scripts.GameActions
{
    static class DrugStory
    {
        public static ActionSequence DrugPusherIntro(Entity drugPusher)
        {
            var sequence = new ActionSequence("DrugPusherIntro");
            sequence.Add(CommonActions.TalkToPlayer(new DrugPusherOffer()));
            sequence.Add(new DialogueBranchAction(new Dictionary<DialogueOutcome, Action>
            {
                {
                    DialogueOutcome.Agree, () =>
                    {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(drugPusher,
                            new UpdateMoodAction(Mood.Happy));
                        StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer = true;
                    }
                },
                {
                    DialogueOutcome.Disagree, () =>
                    {
                        ActionManagerSystem.Instance.AddActionToFrontOfQueueForEntity(drugPusher,
                            new UpdateMoodAction(Mood.Angry));
                        StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer = false;
                    }
                }
            }));
            sequence.Add(CommonActions.LeaveBar());
            return sequence;
        }

        private class DrugPusherOffer : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("I want to sell Space Weed in your bar.");
                DialogueSystem.Instance.WriteNPCLine("If you turn a blind eye I'll give you some product and a cut of the money");
                DialogueSystem.Instance.WriteNPCLine("You in?");
                DialogueSystem.Instance.WritePlayerChoiceLine("No. Not my thing. Don't come back.", EndConversation(DialogueOutcome.Disagree));
                DialogueSystem.Instance.WritePlayerChoiceLine("Sure, if the money is right.", EndConversation(DialogueOutcome.Agree));
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
            questionTime.Add(CommonActions.LeaveBar());
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

        public static void DrugPusherInspectorShowdown(Entity inspector, Entity drugPusher)
        {
            var showdownPusher = new ActionSequence("ShowdownPusher");
            var showdownInspector = new ActionSequence("ShowdownInspector");
            var tookDrugMoney = StaticStates.Get<PlayerDecisionsState>().AcceptedDrugPushersOffer;
            var helpedInspector = StaticStates.Get<PlayerDecisionsState>().ToldInspectorAboutDrugPusher;

            showdownPusher.Add(CommonActions.Wander());
            showdownInspector.Add(CommonActions.Wander());

            //Confront each other! (Consider make this be in front of bar).
            var sync = new SyncedAction(inspector, drugPusher);
            showdownPusher.Add(sync);
            showdownInspector.Add(sync);

            showdownPusher.Add(new SetTargetEntityAction(inspector));
            showdownPusher.Add(new GoToMovingEntityAction());

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

            showdownInspector.Add(CommonActions.LeaveBar());
            showdownPusher.Add(CommonActions.LeaveBar());

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
                    DialogueSystem.Instance.WriteNPCLine("Thanks to your info I nabbed him just as he came in. Enough Space Weed on him to be a criminal offense.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Alright.", EndConversation(DialogueOutcome.Nice));
                }
                else if (helpedInspector && tookDrugMoney)
                {
                    DialogueSystem.Instance.WriteNPCLine("Thanks to your info I got him just as he came in. Enough Space Weed on him to be a criminal offense.");
                    DialogueSystem.Instance.WriteNPCLine("That said I'm pretty sure you were turning a blind eye. I'll be watching you carefully.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Alright.", EndConversation(DialogueOutcome.Default));
                }
                else if (!helpedInspector && tookDrugMoney)
                {
                    DialogueSystem.Instance.WriteNPCLine("This guy is going to space jail and you are lucky to not be going there with him.");
                    DialogueSystem.Instance.WriteNPCLine("He admitted to giving you kickbacks. I'll be having that - and then some.");
                    DialogueSystem.Instance.WriteNPCLine("Consider yourself on very thin ice.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Alright.", EndConversation(DialogueOutcome.Mean));
                }
                else //!helpedInpsector and !tookDrugMoney
                {
                    DialogueSystem.Instance.WriteNPCLine("I was lucky to catch him on his way in. I'm also pretty suprised you didn't see anything.");
                    DialogueSystem.Instance.WriteNPCLine("I'll be watching you carefully.");
                    DialogueSystem.Instance.WritePlayerChoiceLine("Alright.", EndConversation(DialogueOutcome.Default));
                }
            }
        }
    }
}