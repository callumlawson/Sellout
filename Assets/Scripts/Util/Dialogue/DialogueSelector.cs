using Assets.Framework.Entities;
using Assets.Scripts.States.NPC;
using Assets.Scripts.Util.NPC;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Util.Dialogue
{
    public static class DialogueSelector
    {
        private struct DialogueOption
        {
            public string line;
            public DialogueOutcome outcome;
            public SpeciesType species;
            public JobType job;
            public Required required;

            public DialogueOption(string line, DialogueOutcome outcome, SpeciesType species, JobType job, Required required)
            {
                this.line = line;
                this.outcome = outcome;
                this.species = species;
                this.job = job;
                this.required = required;
            }
        }

        private struct IncorrectDrinkDialogueOption
        {
            public string line;
            public SpeciesType species;
            public IncorrectDrinkReason reason;
            public bool destroyDrink;

            public IncorrectDrinkDialogueOption(string line, SpeciesType species, IncorrectDrinkReason reason, bool destroyDrink)
            {
                this.line = line;
                this.species = species;
                this.reason = reason;
                this.destroyDrink = destroyDrink;
            }
        }

        private struct CorrectDrinkDialogueOption
        {
            public string line;
            public SpeciesType species;

            public CorrectDrinkDialogueOption(string line, SpeciesType species)
            {
                this.line = line;
                this.species = species;
            }
        }

        private static List<DialogueOption> drinkOrderOptions = LoadOptions("Conversations_Order_Exact");
        private static List<IncorrectDrinkDialogueOption> incorectDrinkOptions = LoadIncorrectDrinkDialogueOptions("Conversations_Incorrect_Drink");
        private static List<CorrectDrinkDialogueOption> correctDrinkOptions = LoadCorrcetDrinkDialogueOptions("Conversations_Correct_Drink");

        public static Conversation GetExactDrinkOrderConversation(string drinkName, Entity entity, Required required = Required.None)
        {
            var choice = drinkOrderOptions.Where(option => OptionIsValid(option, GetEntitySpecies(entity), GetEntityJob(entity), required)).PickRandom();

            var line = choice.line.Replace("{0}", drinkName);

            var conversation = new WaitConversation(line);
            return conversation;
        }

        public static Conversation GetIncorrectDrinkOrderConversation(string ordered, Entity entity, IncorrectDrinkReason reason, out bool destroyDrink)
        {
            var choice = incorectDrinkOptions.Where(option => OptionIsValid(option, GetEntitySpecies(entity), reason)).PickRandom();

            var line = choice.line;
            if (reason == IncorrectDrinkReason.DoesNotContainIngredient || reason == IncorrectDrinkReason.ContainsIngredient)
            {
                line = line.Replace("{0}", ordered);
            }

            var conversation = new NoResponseConversation(line, DialogueOutcome.Bad);
            destroyDrink = choice.destroyDrink;
            return conversation;
        }

        public static Conversation GetCorrectDrinkOrderConversation(Entity entity)
        {
            var choice = correctDrinkOptions.Where(option => OptionIsValid(option, GetEntitySpecies(entity))).PickRandom();

            var conversation = new NoResponseConversation(choice.line, DialogueOutcome.Nice);
            return conversation;
        }

        private static bool OptionIsValid(DialogueOption option, SpeciesType species, JobType job, Required required)
        {
            var speciesIsValid = option.species == SpeciesType.None || option.species == species;
            var jobIsValid = option.job == JobType.None || option.job == job;
            var requiredIsValid = option.required == Required.None || option.required == required;
            return speciesIsValid && jobIsValid && requiredIsValid;
        }

        private static bool OptionIsValid(IncorrectDrinkDialogueOption option, SpeciesType species, IncorrectDrinkReason reason)
        {
            var speciesIsValid = option.species == SpeciesType.None || option.species == species;
            var reasonIsValid = option.reason == IncorrectDrinkReason.None || option.reason == reason;
            return speciesIsValid && reasonIsValid;
        }

        private static bool OptionIsValid(CorrectDrinkDialogueOption option, SpeciesType species)
        {
            return option.species == SpeciesType.None || option.species == species;
        }

        private static SpeciesType GetEntitySpecies(Entity entity)
        {
            return entity.HasState<SpeciesState>() ? entity.GetState<SpeciesState>().species : SpeciesType.None;
        }

        private static JobType GetEntityJob(Entity entity)
        {
            return entity.HasState<JobState>() ? entity.GetState<JobState>().job : JobType.None;
        }

        private static List<DialogueOption> LoadOptions(string file)
        {
            var path = Path.Combine("Dialogue", file);
            var data = CSVReader.Read(path);

            var options = new List<DialogueOption>();

            for (var i = 0; i < data.Count; i++)
            {
                var datum = data[i];

                var dialogueLine = data[i]["Line"] as string;
                var dialogueOutcomeString = data[i]["Dialogue Outcome"] as string;
                var speciesTypeString = data[i]["Species"] as string;
                var jobTypeString = data[i]["Job"] as string;
                var alcoholicRequiredString = data[i]["Alcoholic"] as string;

                DialogueOutcome dialogueOutcome = dialogueOutcomeString.Length == 0 ? dialogueOutcome = DialogueOutcome.Default : (DialogueOutcome)System.Enum.Parse(typeof(DialogueOutcome), dialogueOutcomeString);
                SpeciesType species = speciesTypeString.Length == 0 ? species = SpeciesType.None : (SpeciesType)System.Enum.Parse(typeof(SpeciesType), data[i]["Species"] as string);
                JobType job = jobTypeString.Length == 0 ? job = JobType.None : (JobType)System.Enum.Parse(typeof(JobType), data[i]["Job"] as string);
                Required alcoholRequired = alcoholicRequiredString.Length == 0 ? Required.None : (Required)System.Enum.Parse(typeof(Required), data[i]["Alcoholic"] as string);

                options.Add(new DialogueOption(dialogueLine, dialogueOutcome, species, job, alcoholRequired));
            }

            return options;
        }

        private static List<IncorrectDrinkDialogueOption> LoadIncorrectDrinkDialogueOptions(string file)
        {
            var path = Path.Combine("Dialogue", file);
            var data = CSVReader.Read(path);

            var options = new List<IncorrectDrinkDialogueOption>();

            for (var i = 0; i < data.Count; i++)
            {
                var datum = data[i];

                var dialogueLine = data[i]["Line"] as string;
                var speciesTypeString = data[i]["Species"] as string;
                var reasonString = data[i]["Reason"] as string;
                var destroyDrinkString = data[i]["Destroy Drink"] as string;

                SpeciesType species = speciesTypeString.Length == 0 ? species = SpeciesType.None : (SpeciesType)System.Enum.Parse(typeof(SpeciesType), data[i]["Species"] as string);
                IncorrectDrinkReason reason = reasonString.Length == 0 ? reason = IncorrectDrinkReason.None : (IncorrectDrinkReason)System.Enum.Parse(typeof(IncorrectDrinkReason), data[i]["Reason"] as string);
                bool destroyDrink = destroyDrinkString.Length == 0 ? false : destroyDrinkString == "Yes";

                options.Add(new IncorrectDrinkDialogueOption(dialogueLine, species, reason, destroyDrink));
            }

            return options;
        }

        private static List<CorrectDrinkDialogueOption> LoadCorrcetDrinkDialogueOptions(string file)
        {
            var path = Path.Combine("Dialogue", file);
            var data = CSVReader.Read(path);

            var options = new List<CorrectDrinkDialogueOption>();

            for (var i = 0; i < data.Count; i++)
            {
                var datum = data[i];

                var dialogueLine = data[i]["Line"] as string;
                var speciesTypeString = data[i]["Species"] as string;

                SpeciesType species = speciesTypeString.Length == 0 ? species = SpeciesType.None : (SpeciesType)System.Enum.Parse(typeof(SpeciesType), data[i]["Species"] as string);

                options.Add(new CorrectDrinkDialogueOption(dialogueLine, species));
            }

            return options;
        }
    }
}
