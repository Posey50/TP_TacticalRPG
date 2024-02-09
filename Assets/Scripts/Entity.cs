using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public string Name;
    public string Class;

    public int MPs;
    public int APs;
    public int HP;
    public int Speed;       //Used to make the overall turn order

    public List<Spells> Actions;

    public Square _currentSquare;

    

    public void InitEntity(EntityData data)
    {
        Name = data.Name;
        Class = data.Class;

        MPs = data.MPs;
        APs = data.APs;
        HP = data.MaxHP;
        Speed = data.Speed;

        Actions = data.Actions;
    }

    public virtual async void Move(Square destination)
    {
        //Astar ici
        //Foreach -> MoveTo -> Wait
        _currentSquare = destination;
        
    }

    /// <summary>
    /// Decreases PMs of Entity by the amount given
    /// </summary>
    /// <param name="amount"></param>
    public void DecreasePM(int amount)
    {
        if (amount <= MPs)
        {
            MPs -= amount;
        }
    }

    /// <summary>
    /// Decreases PAs of Entity by the amount given
    /// </summary>
    /// <param name="amount"></param>
    public void DecreasePA(int amount)
    {
        if (amount <= APs)
        {
            APs -= amount;
        }
    }

    //TODO
    public void Attack(Square entity)
    {
        //Attack Square 
    }


    public void TakeDamage(int damage)
    {
        //TODO : Check Damage if correct
        HP -= damage;
    }

    public void HealHP(int heal)
    {
        //TODO : Check PVs Max
        HP += heal;
    }

    public abstract void ResetPoints();
}
