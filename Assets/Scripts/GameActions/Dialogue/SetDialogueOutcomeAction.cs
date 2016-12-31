using System;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions.Dialogue
{
    class SetDialogueOutcomeAction : GameAction
    {
        public readonly DialogueOutcome outcome;

        public SetDialogueOutcomeAction(Entity entity, DialogueOutcome outcome)
        {
            this.outcome = outcome;
        }

        public override void OnStart(Entity entity)
        {
            entity.GetState<DialogueOutcomeState>().Outcome = outcome;
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void OnFrame(Entity entity)
        {
        }

        public override void Pause()
        {
        }

        public override void Unpause()
        {
        }
    }
}
