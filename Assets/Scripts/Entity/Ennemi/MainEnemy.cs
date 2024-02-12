using System.Collections.Generic;
using UnityEngine;

public class MainNameEnemy : Entity, IComportement
{
    public EnemyStateMachine EnemyStateMachine;

    public List<Square> Path;

    [SerializeField] private Square _square;
    [SerializeField] private bool _canAttack;
    private int _minDistanceToPlayer;

    private Entity _target;
    private List<Spell> _canUseThisSpells;

    public void Start()
    {
        EnemyStateMachine = GetComponent<EnemyStateMachine>();

        GameManager.Instance.EntitiesInGame.Add(this);
        GameManager.Instance.EnemiesInGame.Add(this);
    }

    /// <summary>
    /// Choisi le joueur le + proche de l'ennemi
    /// </summary>
    public void ChosePlayer()
    {
        _minDistanceToPlayer = 100;

        for (int i = 0; i < BattleManager.Instance.PlayableEntitiesInBattle.Count; i++)
        {
            Path = AStarManager.Instance.CalculateShortestPathBetween(_startingSquare, BattleManager.Instance.PlayableEntitiesInBattle[i].SquareUnderTheEntity);

            int distance = Path.Count;

            if (distance < _minDistanceToPlayer)
            {
                _minDistanceToPlayer = distance;
                _target = BattleManager.Instance.PlayableEntitiesInBattle[i];
            }
        }

        ChoseAnAction();
    }

    /// <summary>
    /// Check quelle action l'ennemi peut effectuer avec ses PA et en fait une liste
    /// </summary>
    public void ChoseAnAction()
    {
        for (int i = 0; i < Spells.Count; i++)
        {
            if (Spells[i].SpellDatas.PaCost <= AP)
            {
                _canUseThisSpells.Add(Spells[i]);
            }
        }

        CheckCanAttack();
    }

    /// <summary>
    /// Check si l'ennemi est à coté du joueur, si c'est le cas il choisi une attack random sinon termine le tour
    /// </summary>
    public void CheckCanAttack()
    {
        int randomAction = Random.Range(0, _canUseThisSpells.Count);

        if (_minDistanceToPlayer <= _canUseThisSpells[randomAction].SpellDatas.Range)
        {
            Attack(_canUseThisSpells[randomAction], _target);
            DecreaseAP(_canUseThisSpells[randomAction].SpellDatas.PaCost);

            if (AP > 0)
            {
                ChoseAnAction();
            }
        }

        else if (_minDistanceToPlayer > _canUseThisSpells[randomAction].SpellDatas.Range && MP > 0)
        {
            MPToTarget();
        }

        else
        {
            _canUseThisSpells.Clear();
            EndOfTheTurn();
        }
    }

    /// <summary>
    /// Déplace l'ennemi vers sa target en fonction de ses MP
    /// </summary>
    public void MPToTarget()
    {
        if (_minDistanceToPlayer > MP)
        {
            _minDistanceToPlayer = MP;
        }
        else if (_minDistanceToPlayer <= MP)
        {
            _minDistanceToPlayer--;
        }

        DecreaseMP(_minDistanceToPlayer);
        StartFollowPath(Path); //enemy Move

        CheckCanAttack();
    }

    public override void ResetPoints()
    {
        MP = EntityDatas.MP;
        AP = EntityDatas.AP;
    }

    public void EndOfTheTurn()
    {
        BattleManager.Instance.NextEntityTurn();
        ResetPoints();
    }
}