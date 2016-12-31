using Assets.Framework.States;
using Assets.Scripts.Util.Dialogue;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    class DialogueOutcomeState : IState
    {
        public DialogueOutcome Outcome;
    }
}
