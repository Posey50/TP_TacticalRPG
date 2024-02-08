using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Entity", order = 1)]
public class EntityData : ScriptableObject
{
    private string _name;
    public string Name;

    private string _class;
    public string Class;

    private int _mPs;
    public int MPs;

    private int _aPs;
    public int APs;

    private int _maxHP;
    public int MaxHP;

    private int _speed;
    public int Speed;

    //TODO : Add Action
    private List<Action> _actions;
    public List<Action> Actions;
}
