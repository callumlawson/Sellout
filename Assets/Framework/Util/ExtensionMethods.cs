using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Framework.Util
{
    public static class ExtensionMethods
    {
        public static T DeepClone<T>(this T a)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public static int GetEntityId(this GameObject go)
        {
            var idComponent = go.GetComponentInParent<EntityIdComponent>();
            return idComponent != null ? idComponent.EntityId : EntityIdComponent.InvalidEntityId;
        }

        public static int GetEntityIdRecursive(this GameObject go)
        {
            var idComponent = go.GetComponentInParent<EntityIdComponent>();
            if (idComponent == null && go.transform.parent != null)
            {
                GetEntityIdRecursive(go.transform.parent.gameObject);
            }
            return idComponent != null ? idComponent.EntityId : EntityIdComponent.InvalidEntityId;
        }

        public static GameObject GetEntityObject(this GameObject go)
        {
            var idComponent = go.GetComponentInParent<EntityIdComponent>();
            return idComponent != null ? idComponent.gameObject : null;
        }
    }
}
