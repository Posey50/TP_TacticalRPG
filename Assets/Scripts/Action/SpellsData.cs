using UnityEngine;

[CreateAssetMenu(fileName = "Spells", menuName = "ScriptableObjects/Spells", order = 1)]
public class SpellsData : ScriptableObject
{
    /// <summary>
    /// Name of the spell.
    /// </summary>
    [SerializeField] private string _name;

    /// <summary>
    /// Gets the name of the spell.
    /// </summary>
    public string Name { get { return _name; } private set { } }

    /// <summary>
    /// Cost of the spell.
    /// </summary>
    [SerializeField] private int _paCost;

    /// <summary>
    /// Gets the cost of the spell.
    /// </summary>
    public int PaCost { get { return _paCost; } private set { } }

    /// <summary>
    /// Damages inflincted or treated by the spell.
    /// </summary>
    [SerializeField] private int _damages;

    /// <summary>
    /// Gets damages inflincted or treated by the spell.
    /// </summary>
    public int Damages { get { return _damages; } private set { } }

    /// <summary>
    /// Range to attack of the spell.
    /// </summary>
    [SerializeField] private int _range;

    /// <summary>
    /// Get the range to attack of the spell.
    /// </summary>
    public int Range
    { get { return _range; } private set { } }

    /// <summary>
    /// A value indicating that this spell is for heal.
    /// </summary>
    [SerializeField] private bool _isForHeal;

    /// <summary>
    /// Gets a value indicating that this spell is for heal.
    /// </summary>
    public bool IsForHeal 
    { get { return _isForHeal; } private set { } }

    /// <summary>
    /// Sprite to show on the HUD of the spell.
    /// </summary>
    [SerializeField] private Sprite _image;

    /// <summary>
    /// Gets the sprite to show on the HUD of the spell.
    /// </summary>
    public Sprite Image
    { get { return _image; } private set { } }
}
