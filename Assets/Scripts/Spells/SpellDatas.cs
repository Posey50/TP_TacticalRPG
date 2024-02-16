using UnityEngine;

[CreateAssetMenu(fileName = "Spells", menuName = "ScriptableObjects/Spells", order = 1)]
public class SpellDatas : ScriptableObject
{
    /// <summary>
    /// Name of the spell.
    /// </summary>
    [SerializeField]
    private string _name;

    /// <summary>
    /// Gets the name of the spell.
    /// </summary>
    public string Name { get { return _name; } private set { } }

    /// <summary>
    /// Cost of the spell.
    /// </summary>
    [SerializeField]
    private int _apCost;

    /// <summary>
    /// Gets the cost of the spell.
    /// </summary>
    public int APCost { get { return _apCost; } private set { } }

    /// <summary>
    /// Damages inflincted or treated by the spell.
    /// </summary>
    [SerializeField]
    private int _damages;

    /// <summary>
    /// Gets damages inflincted or treated by the spell.
    /// </summary>
    public int Damages { get { return _damages; } private set { } }

    /// <summary>
    /// Minimum range of the spell.
    /// </summary>
    [SerializeField]
    private int _minRange;

    /// <summary>
    /// Get the minimum range of the spell.
    /// </summary>
    public int MinRange { get { return _minRange; } private set { } }

    /// <summary>
    /// Maximum range of the spell.
    /// </summary>
    [SerializeField]
    private int _maxRange;

    /// <summary>
    /// Get the maximum range of the spell.
    /// </summary>
    public int MaxRange { get { return _maxRange; } private set { } }

    /// <summary>
    /// The type of the spell.
    /// </summary>
    [SerializeField]
    private Type _type;

    /// <summary>
    /// Gets the type of the attack.
    /// </summary>
    public Type Type { get { return _type; } private set { } }

    /// <summary>
    /// Sprite to show on the HUD of the spell.
    /// </summary>
    [SerializeField]
    private Sprite _sprite;

    /// <summary>
    /// Gets the sprite to show on the HUD of the spell.
    /// </summary>
    public Sprite Sprite { get { return _sprite; } private set { } }
}

public enum Type
{
    heal,
    distance,
    melee
}
