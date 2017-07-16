using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;
using Assets.Scripts.States.AI;
using Assets.Scripts.States.NPC;

namespace Assets.Scripts.Util.NPC
{
    public enum NPCName
    {
        Q,
        Tolstoy,
        Jannet,
        McGraw,
        Ellie,
        You,

        Jonathan,
        Gregory,
        Walter,
        Mary,
        Laura,
        Casey,

        Diplomat,
        Trader,
        Traveller,

        Expendable
    }

    public static class NPCS
    {
        private static readonly List<NPCName> AnonymousNames = new List<NPCName>
        {
            NPCName.Diplomat, NPCName.Trader, NPCName.Traveller
        };

        public static NpcTemplate Q = new NpcTemplate
        (
            name: NPCName.Q,
            species: SpeciesType.Human,
            job: JobType.Crew,                                                          // TODO: make Q a drug dealer
            face: FaceType.Face_Q,
            hair: HairType.Hair_Q,
            top: ClothingTopType.UniformTopRed,
            bottom: ClothingBottomType.UniformBottom
        );

        public static NpcTemplate Tolstoy = new NpcTemplate
        (
            name: NPCName.Tolstoy,
            species: SpeciesType.Human,
            job: JobType.Crew,
            face: FaceType.Face_Tolstoy,
            hair: HairType.Hair_Tolstoy,
            top: ClothingTopType.UniformTopBlue,
            bottom: ClothingBottomType.UniformBottom
        );

        public static NpcTemplate Jannet = new NpcTemplate
        (
            name: NPCName.Jannet,
            species: SpeciesType.Human,
            job: JobType.Crew,
            face: FaceType.Face_Jannet,
            hair: HairType.Hair_Jannet,
            top: ClothingTopType.UniformTopRed,
            bottom: ClothingBottomType.UniformBottom
        );

        public static NpcTemplate McGraw = new NpcTemplate
        (
            name: NPCName.McGraw,
            species: SpeciesType.Human,
            job: JobType.Crew,
            face: FaceType.Face_McGraw,
            hair: HairType.Hair_McGraw,
            top: ClothingTopType.UniformTopOrange,
            bottom: ClothingBottomType.UniformBottom
        );

        public static NpcTemplate Ellie = new NpcTemplate
        (
            name: NPCName.Ellie,
            species: SpeciesType.Human,
            job: JobType.Crew,
            face: FaceType.Face_Ellie,
            hair: HairType.Hair_Ellie,
            top: ClothingTopType.UniformTopGreen,
            bottom: ClothingBottomType.UniformBottom
        );        

        public static NpcTemplate GenerateAnon(SpeciesType species)
        {
            var job = JobTypeUtil.GetRandomNonCrewJobType();
            var name = (NPCName)System.Enum.Parse(typeof(NPCName), job.ToString());
            return new NpcTemplate
            (
                name: name,
                species: species,
                job: job,
                face: FaceUtil.GetRandomFace(species),
                hair: HairUtil.GetRandomHiar(species),
                top: ClothingUtil.GetRandomTop(species),
                bottom: ClothingUtil.GetRandomBottom(species)
            );
        }

        public static NpcTemplate GenerateHallwayWalker()
        {
            return new NpcTemplate
            (
                name: NPCName.Expendable,
                species: SpeciesType.Human,
                job: JobType.None,
                face: EnumExtensions.RandomEnumValue<FaceType>(),
                hair: EnumExtensions.RandomEnumValue<HairType>(),
                top: EnumExtensions.RandomEnumValue<ClothingTopType>(),
                bottom: EnumExtensions.RandomEnumValue<ClothingBottomType>()
            );
        }

        public static Entity SpawnNpc(EntityStateSystem entitySystem, NpcTemplate npcTemplate, Vector3 position)
        {
            return entitySystem.CreateEntity(new List<IState>
            {
                new ActionBlackboardState(null),
                new PrefabState(Prefabs.Person),
                new NameState(npcTemplate.Name.ToString(), 2.0f),
                new PositionState(position),
                new PathfindingState(position, null),
                new InventoryState(),
                new VisibleSlotState(),
                new IsPersonState(),
                new MoodState(Mood.Happy),
                new ConversationState(null),
                new RelationshipState(),
                new DialogueOutcomeState(),
                new PersonAnimationState(),
                new ClothingState(npcTemplate.Top, npcTemplate.Bottom),
                new HairState(npcTemplate.Hair),
                new FaceState(npcTemplate.Face),
                new LifecycleState(),
                new InteractiveState(),
                new SpeciesState(npcTemplate.Species),
                new JobState(npcTemplate.Job)
            });
        }

        private static NPCName GetRandomAnonymousName()
        {
            var randomChoice = Random.Range(0, AnonymousNames.Count);
            return AnonymousNames[randomChoice];
        }
    }

    public struct NpcTemplate
    {
        public NPCName Name;
        public SpeciesType Species;
        public JobType Job;
        public FaceType Face;
        public HairType Hair;
        public ClothingTopType Top;
        public ClothingBottomType Bottom;

        public NpcTemplate(NPCName name, SpeciesType species, JobType job, FaceType face, HairType hair, ClothingTopType top, ClothingBottomType bottom) : this()
        {
            Name = name;
            Species = species;
            Job = job;

            Top = top;
            Bottom = bottom;
            Hair = hair;
            Face = face;
        }
    }
}