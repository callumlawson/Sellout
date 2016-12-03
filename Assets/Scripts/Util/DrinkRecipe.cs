using Assets.Scripts.States;

namespace Assets.Scripts.Util
{
    class DrinkRecipe
    {
        public string name { get; private set; }
        public DrinkState contents { get; private set; }

        public DrinkRecipe(string name, DrinkState contents)
        {
            this.name = name;
            this.contents = contents;
        }
    }
}
