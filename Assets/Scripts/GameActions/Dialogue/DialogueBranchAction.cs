using System;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using System.Collections.Generic;
using Assets.Scripts.Util.Dialogue;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.GameActions.Dialogue
{
    class DialogueBranchAction : GameAction
    {
        private readonly Dictionary<DialogueOutcome, Action> onFinishActions;
        private readonly Action onFinishDefaultAction;

        public DialogueBranchAction(Dictionary<DialogueOutcome, Action> onFinishActions)
        {
            this.onFinishActions = onFinishActions;
            onFinishDefaultAction = NoDefaultAction;
        }

        public DialogueBranchAction(Dictionary<DialogueOutcome, Action> onFinishActions, Action onFinishDefaultAction)
        {
            this.onFinishActions = onFinishActions;
            this.onFinishDefaultAction = onFinishDefaultAction;
        }

        private void NoDefaultAction()
        {
            Debug.LogError("No default set for conversation branch, but we hit it anways!");
        }

        public override void OnStart(Entity entity)
        {
            var outcome = entity.GetState<DialogueOutcomeState>().Outcome;

            if (onFinishActions.ContainsKey(outcome))
            {
                onFinishActions[outcome].Invoke();
            }
            else
            {
                onFinishDefaultAction.Invoke();
            }

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
