using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Entity", order = 1)]
public class EntityData : ScriptableObject
{
    [SerializeField]
    private string _name;
    public string Name { get { return _name; } private set { } }

    [SerializeField]
    private string _class;
    public string Class { get { return _class; } private set { } }

    [SerializeField]
    private int _mPs;
    public int MPs { get { return _mPs; } private set { } }

    [SerializeField]
    private int _aPs;
    public int APs { get { return _aPs; } private set { } }

    [SerializeField]
    private int _maxHP;
    public int MaxHP { get { return _maxHP; } private set { } }

    [SerializeField]
    private int _speed;
    public int Speed { get { return _speed; } private set { } }

    [SerializeField]
    private int _moveSpeed;
    public int MoveSpeed { get { return _moveSpeed; } private set { } }

    [SerializeField]
    private List<Spells> _spells;
    public List<Spells> Spells { get { return _spells; } private set { } }
}
