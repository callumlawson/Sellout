using System;
using System.Collections.Generic;

namespace Assets.Framework.Systems
{
    public interface IFilteredSystem : ISystem
    {
        List<Type> RequiredStates();
    }
}
