using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Spells", menuName = "ScriptableObjects/Spells", order = 1)]
public class SpellsData : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name
    {
        get { return _name; }
        private set { _name = value; }
    }

    [SerializeField] private int _paCost;
    public int PaCost
    {
        get { return _paCost; }
        private set { _paCost = value; }
    }

    [SerializeField] private int _damages;
    public int Damages
    {
        get { return _damages; }
        private set { _damages = value; }
    }

    [SerializeField] private int _range;
    public int Range
    {
        get { return _range; }
        private set { _range = value; }
    }

    [SerializeField] private bool _isForHeal;
    public bool IsForHeal 
    {
        get { return _isForHeal; }
        private set { _isForHeal = value; }
    }

    [SerializeField] private Sprite _image;
    public Sprite Image
    {
        get { return _image; }
        private set { _image = value; }
    }
}
