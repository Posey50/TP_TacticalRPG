using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public string Name { get; protected set; }
    public string Class { get; protected set; }

    public int MPs { get; protected set; }
    public int APs { get; protected set; }
    public int HP { get; protected set; }
    public int Speed { get; protected set; }       //Used to make the overall turn order

    public List<Spells> Spells { get; protected set; }

    public Square _currentSquare { get; protected set; }

    [SerializeField]
    protected Square _startingSquare;

    [SerializeField]
    protected EntityData _entityData;

    private void Awake()
    {
        _currentSquare = _startingSquare;
        transform.position = _currentSquare.transform.position;
    }

    protected void InitEntity()
    {
        Name = _entityData.Name;
        Class = _entityData.Class;

        MPs = _entityData.MPs;
        APs = _entityData.APs;
        HP = _entityData.MaxHP;
        Speed = _entityData.Speed;

        Spells = _entityData.Spells;
    }

    public virtual async void Move(Square destination)
    {
        //Astar ici
        //Foreach -> MoveTo -> Wait
        _currentSquare.LeaveSquare();

        _currentSquare = destination;
        transform.position = _currentSquare.transform.position;

        _currentSquare.SetEntity(this);
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
    public void Attack(Square attackedSquare, Spells attackingSpell)
    {
        //Attack Square 
        Debug.Log($"Attack {attackedSquare.name}");
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
