using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;

[CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
public class Progression : ScriptableObject
{
    [SerializeField] ProgressionCharacterClass[] characterClasses = null;
    private Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

    public float GetStat(Stat stat, CharacterClass characterClass, int level)
    {
        BuildLookup();

        float[] levels = lookupTable[characterClass][stat];

        if(levels.Length < level)
        {
            return 0;
        }

        return levels[level - 1];
    }

    public int GetLevels(Stat stat, CharacterClass characterClass)
    {
        BuildLookup();
        float[] levels = lookupTable[characterClass][stat];
        return levels.Length;
    }

    private void BuildLookup()
    {
        if(lookupTable != null)
        {
            return;
        }

        lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

        foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
        {
            Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();

            foreach (ProgressionStat stat in progressionCharacterClass.stats)
            {
                statLookupTable.Add(stat.Stat, stat.Levels);
            }

            lookupTable.Add(progressionCharacterClass.characterClassName, statLookupTable);
        }
    }

    [System.Serializable]
    class ProgressionCharacterClass
    {
        public CharacterClass characterClassName;
        public ProgressionStat[] stats;
    }

    [System.Serializable]
    class ProgressionStat
    {
        public Stat Stat;
        public float[] Levels;
    }
}
