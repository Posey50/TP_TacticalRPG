using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brain : MonoBehaviour
{
    /// <summary>
    /// Main component of the enemy.
    /// </summary>
    protected EnemyMain _enemyMain;

    /// <summary>
    /// List of spells of the enemy.
    /// </summary>
    protected List<Spell> _spells;

    /// <summary>
    /// Called to initialise the brain and checks some informations.
    /// </summary>
    public virtual void InitBrain()
    {
        _enemyMain = GetComponent<EnemyMain>();
        _spells = _enemyMain.Spells;
    }

    /// <summary>
    /// Called to start the pattern.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator EnemyPattern()
    {
        yield return null;
    }
}
