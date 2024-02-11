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
    public int MP { get; protected set; }

    /// <summary>
    /// Action points which determines the total ammount of action points that an entity can use in one turn.
    /// </summary>
    public int AP { get; protected set; }

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
    public List<Spell> Spells { get; protected set; }

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
        Name = EntityDatas.Name;
        Class = EntityDatas.Class;
        MP = EntityDatas.MP;
        AP = EntityDatas.AP;
        HP = EntityDatas.MaxHP;
        Speed = EntityDatas.Speed;
        Spells = EntityDatas.Spells;
        _moveSpeed = EntityDatas.MoveSpeed;
    }

    /// <summary>
    /// Called to start following a path.
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
    private IEnumerator FollowThePath(List<Square> path)
    {
        SquareUnderTheEntity.LeaveSquare();

        Vector3[] pathToFollow = AStarManager.Instance.ConvertSquaresIntoPositions(path).ToArray();

        yield return transform.DOPath(pathToFollow, _moveSpeed * pathToFollow.Length, PathType.Linear);

        SquareUnderTheEntity = path[^1];
        SquareUnderTheEntity.SetEntity(this);
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
    /// Attack an entity with the spell given.
    /// </summary>
    /// <param name="attackedSquare"> Spell used. </param>
    /// <param name="attackingSpell"> Entity to attack. </param>
    public void Attack(Spell spell, Entity entityToAttack)
    {
        entityToAttack.TakeAttack(spell);
    }

    /// <summary>
    /// Recieves a spell and does the the effect of the spell.
    /// </summary>
    /// <param name="attackingSpell"></param>
    public void TakeAttack(Spell spellReceived)
    {
        if (spellReceived.SpellDatas.IsForHeal)
        {
            HealHP(spellReceived.SpellDatas.Damages);
        }
        else
        {
            TakeDamage(spellReceived.SpellDatas.Damages);
        }
    }

    /// <summary>
    /// Applies damages of a spell.
    /// </summary>
    /// <param name="damage"> Damages to take. </param>
    public void TakeDamage(int damages)
    {
        HP -= damages;

        if (HP < 0)
        {
            HP = 0;
        }
    }

    /// <summary>
    /// Applies the healing of a spell.
    /// </summary>
    /// <param name="heal"> HP to heal. </param>
    public void HealHP(int heal)
    {
        // Prevents the healing over the maximum of HP
        HP = Mathf.Clamp(HP + heal, 0, EntityDatas.MaxHP);
    }

    public abstract void ResetPoints();
}
