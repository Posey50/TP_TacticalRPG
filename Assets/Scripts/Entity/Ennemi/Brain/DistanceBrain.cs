using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DistanceBrain : Brain
{
    public override void InitBrain()
    {
        base.InitBrain();

        for (int i = 0; i < _spells.Count; i++)
        {
            if (_spells[i].SpellDatas.Type != Type.distance)
            {
                Debug.LogError(_spells[i].SpellDatas.Name + " is not a valid spell for " + _enemyMain.Name);
            }
        }
    }
}
