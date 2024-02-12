using System;
using UnityEngine;

[Serializable]
public class Spell
{
    /// <summary>
    /// Datas of the spell.
    /// </summary>
    [SerializeField]
    private SpellDatas _spellDatas;

    /// <summary>
    /// Gets datas of the spell.
    /// </summary>
    public SpellDatas SpellDatas { get { return _spellDatas; } private set { } }
}