using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public string Name;
    public string Classe;

    public int MovementPoints;
    public int ActionPoints;
    public int HP;
    public int Speed;       //Used to make the overall turn order
    public List<int> ActionPool;    //TODO : Remplacer int par Action

}
