using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    public enum Mood
    {
        Angry,
        Happy
    }

    [Serializable]
    class MoodState : IState
    {
        public Mood Mood { get; private set; }

        public MoodState(Mood mood)
        {
            Mood = mood;
        }

        public void UpdateMood(Mood mood)
        {
            Mood = mood;
            MoodEvent.Invoke(Mood);
        }

        public Action<Mood> MoodEvent = delegate {  };

        public override string ToString()
        {
            return Mood.ToString();
        }
    }
}
