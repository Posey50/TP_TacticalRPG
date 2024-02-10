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

    protected EntityData _entityData;

    private void Awake()
    {
        _currentSquare = _startingSquare;
        _currentSquare.SetEntity(this);
        transform.position = _currentSquare.transform.position;
    }

    public void InitEntity()
    {
        Name = _entityData.Name;
        Class = _entityData.Class;

        MPs = _entityData.MPs;
        APs = _entityData.APs;
        HP = _entityData.MaxHP;
        Speed = _entityData.Speed;

        Spells = _entityData.Spells;

        Debug.Log($"{Name} has been initialized");
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
    public void Attack(Square attackedSquare, SpellsData attackingSpell)
    {
        //Attack Square 
        Debug.Log($"{Name} uses {attackingSpell.Name} to attack {attackedSquare.name}");

        attackedSquare.TargetEntity(attackingSpell);
    }

    public void TakeAttack(SpellsData attackingSpell)
    {
        Debug.Log($"{Name} recieves {attackingSpell.Name} on the face");

        if (attackingSpell.IsForHeal)
        {
            HealHP(attackingSpell.Damages);
        }
        else
        {
            TakeDamage(attackingSpell.Damages);
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage > 0)
        {
            HP -= damage;

            Debug.Log($"{Name} takes {damage} damage");
        }
    }

    public void HealHP(int heal)
    {
        if (heal > 0)
        {
            HP = Mathf.Clamp(HP + heal, 0, _entityData.MaxHP);  //Can't heal higher than your maxHP

            Debug.Log($"{Name} recieves {heal} of healing");
        }
    }

    public abstract void ResetPoints();
}
