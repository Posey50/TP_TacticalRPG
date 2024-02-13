using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public abstract class Entity : MonoBehaviour
{
    /// <summary>
    /// The database which represents this entity.
    /// </summary>
    public EntityDatas EntityDatas { get; protected set; }

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
    [field: SerializeField]
    public int MP { get; protected set; }

    /// <summary>
    /// Action points which determines the total ammount of action points that an entity can use in one turn.
    /// </summary>
    [field: SerializeField]
    public int AP { get; protected set; }

    /// <summary>
    /// Health points of the entity.
    /// </summary>
    [field: SerializeField]
    public int HP { get; protected set; }

    /// <summary>
    /// Speed of the entity which determines the order of action in a turn.
    /// </summary>
    public int Speed { get; protected set; }

    /// <summary>
    /// List of spells that the entity can use.
    /// </summary>
    public List<Spell> Spells { get; protected set; } = new ();

    /// <summary>
    /// Square on which the entity is located.
    /// </summary>
    [field: SerializeField]
    public Square SquareUnderTheEntity { get; set; }

    /// <summary>
    /// A value indicating if the entity is moving.
    /// </summary>
    public bool IsMoving { get; private set; }

    /// <summary>
    /// Speed at which the entity moves.
    /// </summary>
    protected float _moveSpeed;


    // Observer
    public delegate void EntityDelegate();

    public event EntityDelegate TurnIsEnd;

    /// <summary>
    /// Called to hydrate the entity with their datas.
    /// </summary>
    public void InitEntity()
    {
        Name = EntityDatas.Name;
        Class = EntityDatas.Class;
        MP = EntityDatas.MP;
        AP = EntityDatas.AP;
        HP = EntityDatas.MaxHP;
        Speed = EntityDatas.Speed;
        Spells = EntityDatas.Spells;
        _moveSpeed = EntityDatas.MoveSpeed;

        Debug.Log(Name + "is init");
    }

    /// <summary>
    /// Called to start following a path. (Used for playable entities)
    /// </summary>
    /// <param name="path"> Path to follow. </param>
    public void StartFollowPath(List<Square> path)
    {
        StartCoroutine(FollowThePath(path));
    }

    /// <summary>
    /// Makes the entity following a path.
    /// </summary>
    /// <param name="path"> Path to follow. </param>
    public IEnumerator FollowThePath(List<Square> path)
    {
        IsMoving = true;

        SquareUnderTheEntity.LeaveSquare();

        Vector3[] pathToFollow = AStarManager.Instance.ConvertSquaresIntoPositions(path).ToArray();

        yield return transform.DOPath(pathToFollow, _moveSpeed * pathToFollow.Length, PathType.Linear).OnWaypointChange((int i) => path[i].ResetMaterial()).WaitForCompletion();

        DecreaseMP(path.Count);

        SquareUnderTheEntity = path[^1];
        SquareUnderTheEntity.SetEntity(this);

        IsMoving = false;
    }

    /// <summary>
    /// Decreases MP of the entity by the amount given.
    /// </summary>
    /// <param name="amount"> MP to decrease. </param>
    public void DecreaseMP(int amount)
    {
        if (amount <= MP)
        {
            MP -= amount;
        }
    }

    /// <summary>
    /// Decreases AP of the entity by the amount given.
    /// </summary>
    /// <param name="amount"> AP to decrease. </param>
    public void DecreaseAP(int amount)
    {
        if (amount <= AP)
        {
            AP -= amount;
        }
    }

    /// <summary>
    /// Called to start attacking an entity. (Used for playable entities)
    /// </summary>
    /// <param name="spell"> Spell used. </param>
    /// <param name="entityToAttack"> Entity to attack. </param>
    public void StartAttack(Spell spell, Entity entityToAttack)
    {
        StartCoroutine(Attack(spell, entityToAttack));
    }

    /// <summary>
    /// Attack an entity with the spell given.
    /// </summary>
    /// <param name="attackedSquare"> Spell used. </param>
    /// <param name="attackingSpell"> Entity to attack. </param>
    public IEnumerator Attack(Spell spell, Entity entityToAttack)
    {
        yield return StartCoroutine(entityToAttack.TakeAttack(spell));

        DecreaseAP(spell.SpellDatas.PaCost);
    }

    /// <summary>
    /// Recieves a spell and does the the effect of the spell.
    /// </summary>
    /// <param name="attackingSpell"></param>
    public IEnumerator TakeAttack(Spell spellReceived)
    {
        if (spellReceived.SpellDatas.IsForHeal)
        {
            yield return StartCoroutine(HealHP(spellReceived.SpellDatas.Damages));
        }
        else
        {
            yield return StartCoroutine(TakeDamage(spellReceived.SpellDatas.Damages));
        }
    }

    /// <summary>
    /// Applies damages of a spell.
    /// </summary>
    /// <param name="damage"> Damages to take. </param>
    public IEnumerator TakeDamage(int damages)
    {
        HP -= damages;

        if (HP < 0)
        {
            HP = 0;
            BattleManager.Instance.EntityDeath(this);
        }

        yield return null;
    }

    /// <summary>
    /// Applies the healing of a spell.
    /// </summary>
    /// <param name="heal"> HP to heal. </param>
    public IEnumerator HealHP(int heal)
    {
        // Prevents the healing over the maximum of HP
        HP = Mathf.Clamp(HP + heal, 0, EntityDatas.MaxHP);

        yield return null;
    }
    
    /// <summary>
    /// Resets MP and AP of the entity.
    /// </summary>
    public void ResetPoints()
    {
        MP = EntityDatas.MP;
        AP = EntityDatas.AP;
    }

    /// <summary>
    /// Called when the entity has end its turn.
    /// </summary>
    public void EndOfTheTurn()
    {
        TurnIsEnd?.Invoke();

        BattleManager battleManager = BattleManager.Instance;

        battleManager.EntitiesInActionOrder.Remove(this);
        battleManager.NextEntityTurn();
        ResetPoints();
    }
}
