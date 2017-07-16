using System;
using Assets.Framework.States;
using Assets.Scripts.Util.NPC;

namespace Assets.Scripts.States.NPC
{
    [Serializable]
    class SpeciesState : IState
    {
        public SpeciesType species;

        public SpeciesState(SpeciesType species)
        {
            this.species = species;
        }

        public override string ToString()
        {
            return "Spceies: " + species.ToString();
        }
    }
}
