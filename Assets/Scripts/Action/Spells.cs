using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour
{
    [SerializeField] private SpellsData _actionBase;
    public SpellsData ActionBase
    {
        get { return _actionBase; }
        private set { _actionBase = value; }
    }
}
