using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Util.NPC
{
    public static class NPCS
    {
        private static readonly SerializableVector3 DefaultSpawnPosition = new Vector3(5.63f, 0.0f, 16.49f);

        public static NPC Q = new NPC
        {
            Name = "Q",
            Face = FaceType.Q,
            Hair = HairType.Q,
            Top = ClothingTopType.UniformTopRed,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NPC Tolstoy = new NPC
        {
            Name = "Tolstoy",
            Face = FaceType.Tolstoy,
            Hair = HairType.Tolstoy,
            Top = ClothingTopType.UniformTopBlue,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NPC Jannet = new NPC
        {
            Name = "Jannet",
            Face = FaceType.Jannet,
            Hair = HairType.Jannet,
            Top = ClothingTopType.UniformTopRed,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NPC McGraw = new NPC
        {
            Name = "McGraw",
            Face = FaceType.McGraw,
            Hair = HairType.McGraw,
            Top = ClothingTopType.UniformTopOrange,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static NPC Ellie = new NPC
        {
            Name = "Ellie",
            Face = FaceType.Ellie,
            Hair = HairType.Ellie,
            Top = ClothingTopType.UniformTopGreen,
            Bottom = ClothingBottomType.UniformBottom
        };

        public static Entity SpawnNpc(EntityStateSystem entitySystem, NPC npc, SerializableVector3? position = null)
        {
            position = position ?? DefaultSpawnPosition;

            return entitySystem.CreateEntity(new List<IState>
            {
                new ActionBlackboardState(null),
                new PrefabState(Prefabs.Person),
                new NameState(npc.Name, 2.0f),
                new PositionState(position.Value),
                new PathfindingState(null),
                new InventoryState(),
                new VisibleSlotState(),
                new PersonState(),
                new MoodState(Mood.Happy),
                new DialogueOutcomeState(),
                new ClothingState(npc.Top, npc.Bottom),
                new HairState(npc.Hair),
                new FaceState(npc.Face)
            });
        }
    }

    public struct NPC
    {
        public string Name;
        public FaceType Face;
        public HairType Hair;
        public ClothingTopType Top;
        public ClothingBottomType Bottom;

        public NPC(string name, FaceType face, HairType hair, ClothingTopType top, ClothingBottomType bottom) : this()
        {
            Top = top;
            Bottom = bottom;
            Hair = hair;
            Face = face;
            Name = name;
        }
    }
}
