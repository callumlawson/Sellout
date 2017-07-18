namespace Assets.Scripts.Util.Dialogue
{
    public enum IncorrectDrinkReason
    {
        None,
        WrongRecipe,
        RecipeDoesNotExist,
        Alcoholic,
        NonAlcoholic,
        EmptyGlass,
        DoesNotContainIngredient,
        ContainsIngredient,
        Timeout
    }
}
