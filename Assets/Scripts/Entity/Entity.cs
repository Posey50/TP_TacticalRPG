using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using System.Collections;

public abstract class Entity : MonoBehaviour
{
    /// <summary>
    /// The database which represents this entity.
    /// </summary>
    public EntityData EntityData { get; protected set; }

    /// <summary>
    /// Name of the entity.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Class of the entity
    /// </summary>
    public string Class { get; protected set; }

    /// <summary>
    /// Movement points which determines the number of movement that an entity cans do in one turn.
    /// </summary>
    public int MPs { get; protected set; }

    /// <summary>
    /// Action points which determines the total ammount of action points that an entity can use in one turn.
    /// </summary>
    public int APs { get; protected set; }

    /// <summary>
    /// Health points of the entity.
    /// </summary>
    public int HP { get; protected set; }

    /// <summary>
    /// Speed of the entity which determines the order of action in a turn.
    /// </summary>
    public int Speed { get; protected set; }

    /// <summary>
    /// List of spells that the entity can use.
    /// </summary>
    public List<Spells> Spells { get; protected set; }

    /// <summary>
    /// Square on which the entity is located.
    /// </summary>
    public Square SquareUnderTheEntity { get; protected set; }

    /// <summary>
    /// Speed at which the entity moves.
    /// </summary>
    protected float _moveSpeed;


    [SerializeField]
    protected Square _startingSquare;

    private void Awake()
    {
        SquareUnderTheEntity = _startingSquare;
        SquareUnderTheEntity.SetEntity(this);
        transform.position = SquareUnderTheEntity.transform.position;
    }

    /// <summary>
    /// Called to hydrate the entity with their datas.
    /// </summary>
    public void InitEntity()
    {
        Name = EntityData.Name;
        Class = EntityData.Class;
        MPs = EntityData.MPs;
        APs = EntityData.APs;
        HP = EntityData.MaxHP;
        Speed = EntityData.Speed;
        _moveSpeed = EntityData.MoveSpeed;
        Spells = EntityData.Spells;
    }

    /// <summary>
    /// Starts the Coroutine FollowThePath
    /// </summary>
    public void StartFollowPath(List<Square> path)
    {
        StartCoroutine(FollowThePath(path));
    }

    /// <summary>
    /// Called to make an entity following a path.
    /// </summary>
    /// <param name="path"> Path to follow. </param>
    private IEnumerator FollowThePath(List<Square> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            yield return StartCoroutine(Move(path[i]));
        }
    }

    /// <summary>
    /// Called to move to a destination
    /// </summary>
    /// <param name="destination"> Destination to reach. </param>
    /// <returns></returns>
    private IEnumerator Move(Square destination)
    {
        SquareUnderTheEntity.LeaveSquare();

        yield return transform.DOMove(destination.transform.position, 0.2f); //_moveSpeed * Time.deltaTime
        yield return new WaitForSeconds(0.2f);

        SquareUnderTheEntity = destination;
        //transform.position = SquareUnderTheEntity.transform.position;

        SquareUnderTheEntity.SetEntity(this);
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

    /// <summary>
    /// Sends a spell to a specified Square. 
    /// </summary>
    /// <param name="attackedSquare"></param>
    /// <param name="attackingSpell"></param>
    public void Attack(Square attackedSquare, SpellsData attackingSpell)
    {
        //Attack Square 
        Debug.Log($"{Name} uses {attackingSpell.Name} to attack {attackedSquare.name}");

        attackedSquare.TargetEntity(attackingSpell);
    }

    /// <summary>
    /// Recieves a spell and correctly takes it (heal or damage)
    /// </summary>
    /// <param name="attackingSpell"></param>
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
    
    /// <summary>
    /// Applies the damage of spell
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (damage > 0)
        {
            HP -= damage;

            Debug.Log($"{Name} takes {damage} damage");
        }
    }

    /// <summary>
    /// Applies the healing of a spell
    /// </summary>
    /// <param name="heal"></param>
    public void HealHP(int heal)
    {
        if (heal > 0)
        {
            HP = Mathf.Clamp(HP + heal, 0, EntityData.MaxHP);  //Can't heal higher than your maxHP

            Debug.Log($"{Name} recieves {heal} of healing");
        }
    }

    public abstract void ResetPoints();
}
