using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions
{
    class UpdateMoodAction : GameAction
    {
        private readonly Mood mood;

        public UpdateMoodAction(Mood mood)
        {
            this.mood = mood;
        }

        public override void OnStart(Entity entity)
        {
            var moodState = entity.GetState<MoodState>();
            moodState.UpdateMood(mood);
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void OnFrame(Entity entity)
        {
            //Do Nothing
        }

        public override void Pause()
        {
            //Do Nothing
        }

        public override void Unpause()
        {
            //Do Nothing
        }
    }
}
