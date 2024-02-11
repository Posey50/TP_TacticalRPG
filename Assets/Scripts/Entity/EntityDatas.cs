using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Entity")]
public class EntityDatas : ScriptableObject
{
    /// <summary>
    /// Name of the entity.
    /// </summary>
    [SerializeField]
    private string _name;

    /// <summary>
    /// Gets the name of the entity.
    /// </summary>
    public string Name { get { return _name; } private set { } }

    /// <summary>
    /// Class of the entity.
    /// </summary>
    [SerializeField]
    private string _class;

    /// <summary>
    /// Gets the class of the entity.
    /// </summary>
    public string Class { get { return _class; } private set { } }

    /// <summary>
    /// Movement points which determines the number of movement that an entity cans do in one turn.
    /// </summary>
    [SerializeField]
    private int _mP;

    /// <summary>
    /// Gets movement points which determines the number of movement that an entity cans do in one turn.
    /// </summary>
    public int MP { get { return _mP; } private set { } }

    /// <summary>
    /// Action points which determines the total ammount of action points that an entity can use in one turn.
    /// </summary>
    [SerializeField]
    private int _aP;

    /// <summary>
    /// Gets action points which determines the total ammount of action points that an entity can use in one turn.
    /// </summary>
    public int AP { get { return _aP; } private set { } }

    /// <summary>
    /// Maximum of health points of the entity.
    /// </summary>
    [SerializeField]
    private int _maxHP;

    /// <summary>
    /// Gets the maximum of health points of the entity.
    /// </summary>
    public int MaxHP { get { return _maxHP; } private set { } }

    /// <summary>
    /// Speed of the entity which determines the order of action in a turn.
    /// </summary>
    [SerializeField]
    private int _speed;

    /// <summary>
    /// Gets the speed of the entity which determines the order of action in a turn.
    /// </summary>
    public int Speed { get { return _speed; } private set { } }

    /// <summary>
    /// List of spells that the entity can use.
    /// </summary>
    [SerializeField]
    private List<Spell> _spells;

    /// <summary>
    /// Gets the list of spells that the entity can use.
    /// </summary>
    public List<Spell> Spells { get { return _spells; } private set { } }

    /// <summary>
    /// Speed at which the entity moves.
    /// </summary>
    [SerializeField]
    private float _moveSpeed;

    /// <summary>
    /// Gets the speed at which the entity moves.
    /// </summary>
    public float MoveSpeed { get { return _moveSpeed; } private set { } }
}