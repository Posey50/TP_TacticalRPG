using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

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

    public Vector3 YOffset { get; private set; }

    /// <summary>
    /// Speed at which the entity moves.
    /// </summary>
    protected float _moveSpeed;

    //Events
    public event Action<int> DamageRecieved;
    public event Action<int> HealRecieved;
    public event Action<int> UpadateMpUI;
    public event Action<int> UpadateApUI;
    public event Action<bool> IsMove;
    public event Action StartAttack;
    public event Action EndAttack;

    // Observer
    public delegate void EntityDelegate();

    public event EntityDelegate TurnIsEnd;

    // Visual feedback
    private SpriteRenderer _spriteRenderer;

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

        _spriteRenderer = GetComponent<SpriteRenderer>();

        YOffset = new Vector3(0, (SquareUnderTheEntity.GetComponent<Collider>().bounds.size.y / 2f) + (_spriteRenderer.bounds.size.y / 6f), 0);
        UpadateMpUI?.Invoke(MP);
        UpadateApUI?.Invoke(AP);
    }

    /// <summary>
    /// Makes the entity following a path.
    /// </summary>
    /// <param name="path"> Path to follow. </param>
    public async UniTask FollowThePath(List<Square> path)
    {
        if(path != null)
        {
            IsMoving = true;
            IsMove?.Invoke(IsMoving);

            SquareUnderTheEntity.LeaveSquare();

            Vector3[] pathToFollow = AStarManager.Instance.ConvertSquaresIntoPositions(path).ToArray();

            for (int i = 0; i < pathToFollow.Length; i++)
            {
                pathToFollow[i] += YOffset;
            }

            await transform.DOPath(pathToFollow, _moveSpeed * pathToFollow.Length, PathType.Linear, PathMode.Full3D).SetEase(Ease.Linear).OnWaypointChange((int i) => { if (i > 0) path[i - 1].ResetMaterial(); }).AsyncWaitForCompletion();

            DecreaseMP(path.Count);

            SquareUnderTheEntity = path[^1];
            SquareUnderTheEntity.SetEntity(this);

            IsMoving = false;
            IsMove?.Invoke(IsMoving);
        }
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
            UpadateMpUI?.Invoke(MP);
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
            UpadateApUI?.Invoke(AP);
        }
    }

    /// <summary>
    /// Attack an entity with the spell given.
    /// </summary>
    /// <param name="spell"> Spell used. </param>
    /// <param name="entityToAttack"> Entity to attack. </param>
    public void Attack(Spell spell, Entity entityToAttack)
    {
        StartAttack?.Invoke();
        Debug.Log(Name + " attacks " + entityToAttack.Name + " with " + spell.SpellDatas.Name);

        entityToAttack.TakeAttack(spell);

        DecreaseAP(spell.SpellDatas.PaCost);
        StartAttack?.Invoke();
    }

    /// <summary>
    /// Recieves a spell and does the the effect of the spell.
    /// </summary>
    /// <param name="spellReceived"> Spell received by the entity. </param>
    public void TakeAttack(Spell spellReceived)
    {
        if (spellReceived.SpellDatas.Type == Type.heal)
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
    /// <param name="damages"> Damages to take. </param>
    public void TakeDamage(int damages)
    {
        Debug.Log(Name + " looses " + damages + "HP");

        HP -= damages;

        DOTween.Sequence()
            .Append(
                _spriteRenderer.DOColor(Color.red, 0.1f)
            )
            .AppendInterval(0.05f)
            .Append(
                _spriteRenderer.DOColor(Color.white, 0.1f)
            );

        if (HP <= 0)
        {
            HP = 0;
            BattleManager.Instance.EntityDeath(this);
        }

        DamageRecieved?.Invoke(damages);
    }

    /// <summary>
    /// Applies the healing of a spell.
    /// </summary>
    /// <param name="heal"> HP to heal. </param>
    public void HealHP(int heal)
    {
        Debug.Log(Name + " heals " + heal + "HP");
        // Prevents the healing over the maximum of HP
        HP = Mathf.Clamp(HP + heal, 0, EntityDatas.MaxHP);

        DOTween.Sequence()
            .Append(
                _spriteRenderer.DOColor(Color.green, 0.1f)
            )
            .AppendInterval(0.05f)
            .Append(
                _spriteRenderer.DOColor(Color.white, 0.1f)
            );

        HealRecieved?.Invoke(heal);
    }
    
    /// <summary>
    /// Resets MP and AP of the entity.
    /// </summary>
    public void ResetPoints()
    {
        MP = EntityDatas.MP;
        AP = EntityDatas.AP;

        UpadateMpUI?.Invoke(MP);
        UpadateApUI?.Invoke(AP);
    }

    /// <summary>
    /// Called when the entity has end its turn.
    /// </summary>
    public void EndOfTheTurn()
    {
        Debug.Log("fin du tour");
        TurnIsEnd?.Invoke();

        BattleManager battleManager = BattleManager.Instance;

        battleManager.EntitiesInActionOrder.Remove(this);
        battleManager.NextEntityTurn();
        ResetPoints();
    }
}
