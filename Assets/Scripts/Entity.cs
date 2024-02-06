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
    //public List<int> ActionPool;    //TODO : fait Action

    public void InitEntity(EntityData data)
    {
        Name = data.Name;
        Class = data.Class;

        MPs = data.MPs;
        APs = data.APs;
        HP = data.MaxHP;
        Speed = data.Speed;

        //TODO : Ajout de Action
    }

    //public void Move()

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
    public void AttackEntity(Entity entity)
    {
        entity.TakeDamage(5);   
    }


    public void TakeDamage(int damage)
    {
        HP -= damage;
    }

    public void HealHP(int heal)
    {
        //TODO : Check PVs Max
        HP += heal;
    }

    public abstract void ResetPoints();
}
