namespace Assets.Framework.Systems
{
    public interface IEntityManager : ISystem
    {
        void SetEntitySystem(EntityStateSystem ess);
    }
}
