using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetStatModifiers(Stat stat);
        IEnumerable<float> GetStatMultipliers(Stat stat);
    }

}

