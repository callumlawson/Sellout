using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;
using Assets.Scripts.States.AI;

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
        private static List<NPCName> AnonymousNames = new List<NPCName>()
        {
            NPCName.Diplomat, NPCName.Trader, NPCName.Traveller
        };

        public static NpcTemplate Q = new NpcTemplate
        {
            Name = NPCName.Q,
            Face = FaceType.Q,
            Hair = HairType.Q,
            Top = ClothingTopType.UniformTopRed,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate Tolstoy = new NpcTemplate
        {
            Name = NPCName.Tolstoy,
            Face = FaceType.Tolstoy,
            Hair = HairType.Tolstoy,
            Top = ClothingTopType.UniformTopBlue,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate Jannet = new NpcTemplate
        {
            Name = NPCName.Jannet,
            Face = FaceType.Jannet,
            Hair = HairType.Jannet,
            Top = ClothingTopType.UniformTopRed,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate McGraw = new NpcTemplate
        {
            Name = NPCName.McGraw,
            Face = FaceType.McGraw,
            Hair = HairType.McGraw,
            Top = ClothingTopType.UniformTopOrange,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate Ellie = new NpcTemplate
        {
            Name = NPCName.Ellie,
            Face = FaceType.Ellie,
            Hair = HairType.Ellie,
            Top = ClothingTopType.UniformTopGreen,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate Jonathan = new NpcTemplate
        {
            Name = NPCName.Jonathan,
            Face = FaceType.None,
            Hair = HairType.None,
            Top = ClothingTopType.UniformTopGreen,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate Gregory = new NpcTemplate
        {
            Name = NPCName.Gregory,
            Face = FaceType.None,
            Hair = HairType.None,
            Top = ClothingTopType.UniformTopGreen,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate Walter = new NpcTemplate
        {
            Name = NPCName.Walter,
            Face = FaceType.None,
            Hair = HairType.None,
            Top = ClothingTopType.UniformTopGreen,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate Mary = new NpcTemplate
        {
            Name = NPCName.Mary,
            Face = FaceType.None,
            Hair = HairType.None,
            Top = ClothingTopType.UniformTopGreen,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate Laura = new NpcTemplate
        {
            Name = NPCName.Laura,
            Face = FaceType.None,
            Hair = HairType.None,
            Top = ClothingTopType.UniformTopGreen,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate Casey = new NpcTemplate
        {
            Name = NPCName.Casey,
            Face = FaceType.None,
            Hair = HairType.None,
            Top = ClothingTopType.UniformTopGreen,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NpcTemplate GenerateAnnonHuman()
        {
            return new NpcTemplate
            {
                Name = GetRandomAnonymousName(),
                Face = EnumExtensions.RandomEnumValue<FaceType>(),
                Hair = HairUtil.GetRandomHumanHair(),
                Top = EnumExtensions.RandomEnumValue<ClothingTopType>(),
                Bottom = EnumExtensions.RandomEnumValue<ClothingBottomType>()
            };
        }

        public static NpcTemplate GenerateBirdPerson()
        {
            return new NpcTemplate
            {
                Name = GetRandomAnonymousName(),
                Face = FaceType.BirdPerson,
                Hair = HairType.BirdPerson,
                Top = ClothingTopType.BirdPersonTop,
                Bottom = ClothingBottomType.BirdPersonBottom
            };
        }

        public static NpcTemplate GenerateShadowPerson()
        {
            return new NpcTemplate
            {
                Name = GetRandomAnonymousName(),
                Face = FaceType.ShadowPerson,
                Hair = HairType.ShadowPerson,
                Top = ClothingTopType.ShadowPersonTop,
                Bottom = ClothingBottomType.ShadowPersonBottom
            };
        }

        public static NpcTemplate GenerateHallwayWalker()
        {
            return new NpcTemplate
            {
                Name = GetRandomAnonymousName(),
                Face = EnumExtensions.RandomEnumValue<FaceType>(),
                Hair = EnumExtensions.RandomEnumValue<HairType>(),
                Top = EnumExtensions.RandomEnumValue<ClothingTopType>(),
                Bottom = EnumExtensions.RandomEnumValue<ClothingBottomType>()
            };
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
                new InteractiveState()
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
        public FaceType Face;
        public HairType Hair;
        public ClothingTopType Top;
        public ClothingBottomType Bottom;

        public NpcTemplate(NPCName name, FaceType face, HairType hair, ClothingTopType top, ClothingBottomType bottom) : this()
        {
            Top = top;
            Bottom = bottom;
            Hair = hair;
            Face = face;
            Name = name;
        }
    }
}