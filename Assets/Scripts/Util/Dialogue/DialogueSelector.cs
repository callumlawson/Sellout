using Assets.Framework.Entities;
using Assets.Scripts.States.NPC;
using Assets.Scripts.Util.NPC;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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

        private static List<DialogueOption> drinkOrderOptions = LoadOptions("Conversations_Order_Exact");

        public static Conversation GetExactDrinkOrderConversation(string drinkName, Entity entity, Required required = Required.None)
        {
            var choice = drinkOrderOptions.Where(option => OptionIsValid(option, GetEntitySpecies(entity), GetEntityJob(entity), required)).PickRandom();

            var line = choice.line.Replace("{0}", drinkName);

            var conversation = new WaitConversation(line);
            return conversation;
        }

        private static bool OptionIsValid(DialogueOption option, SpeciesType species, JobType job, Required required)
        {
            var speciesIsValid = option.species == SpeciesType.None || option.species == species;
            var jobIsValid = option.job == JobType.None || option.job == job;
            var requiredIsValid = option.required == Required.None || option.required == required;
            return speciesIsValid && jobIsValid && requiredIsValid;
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

                DialogueOutcome dialogueOutcome = dialogueOutcome = dialogueOutcomeString.Length == 0 ? dialogueOutcome = DialogueOutcome.Default : (DialogueOutcome)System.Enum.Parse(typeof(DialogueOutcome), dialogueOutcomeString);
                SpeciesType species = speciesTypeString.Length == 0 ? species = SpeciesType.None : (SpeciesType)System.Enum.Parse(typeof(SpeciesType), data[i]["Species"] as string);
                JobType job = jobTypeString.Length == 0 ? job = JobType.None : (JobType)System.Enum.Parse(typeof(JobType), data[i]["Job"] as string);
                Required alcoholRequired = alcoholicRequiredString.Length == 0 ? Required.None : (Required)System.Enum.Parse(typeof(Required), data[i]["Alcoholic"] as string);

                options.Add(new DialogueOption(dialogueLine, dialogueOutcome, species, job, alcoholRequired));
            }

            return options;
        }
    }
}
