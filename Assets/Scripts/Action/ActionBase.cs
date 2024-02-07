using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Actions", menuName = "ScriptableObjects/Actions", order = 1)]
public class ActionBase : ScriptableObject
{
    private string _name;
    public string Name;

    private int _paCost;
    public int PaCost;

    private int _damages;
    public int Damages;

    private int _range;
    public int Range;

    private bool _isForHeal;
    public bool IsForHeal;
}
