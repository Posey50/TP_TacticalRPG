using System.Collections.Generic;
using UnityEngine;

public class MainNameEnnemy : Entity, IComportement
{
    public EntityDatas EntityData;
    public EnemyStateMachine EnemyStateMachine;

    public List<Square> path;
    public Square startSquare;
    public Material startMaterial;

    public Square Square;
    private int _minDistanceToPlayer;
    private SpellDatas _spellsData; 

    public bool CanAttack;

    public void Start()
    {
        startSquare.GetComponent<MeshRenderer>().material = startMaterial;
        ChosePlayer();
    }

    /// <summary>
    /// Choisi le joueur le + proche de l'ennemi
    /// </summary>
    public void ChosePlayer()
    {
        for (int i = 0; i < 3; i++)
        {
            path = AStarManager.Instance.CalculateShortestPathBetween(startSquare, Square /*� remplacer par EntityOnThisSquare*/); //--> renvoie liste de square entre le point A et B

            int distance = path.Count;

            if (distance < _minDistanceToPlayer)
            {
                _minDistanceToPlayer = distance;
            }
        }

        MPToPlayer();
    }

    /// <summary>
    /// Check si ennemy MP > ou = � la distance du joueur le plus proche
    /// </summary>
    public void MPToPlayer()
    {
        if (_minDistanceToPlayer > EntityData.MP)
        {
            _minDistanceToPlayer = EntityData.MP;
        }
        else if (_minDistanceToPlayer == EntityData.MP)
        {
            _minDistanceToPlayer--;
        }

        startSquare = path[_minDistanceToPlayer];

        DecreaseMP(_minDistanceToPlayer);
        //Move(startSquare);    Erreur de truc, Rip bozo

        ChoseAnAction();
    }


    /// <summary>
    /// Check si l'ennemi est � cot� du joueur, si c'est le cas il choisi une attack random sinon termine le tour
    /// </summary>
    public void ChoseAnAction()
    {
        for (int i = 0; i <= Square.Neighbors.Count; i++)
        {
            if (path[_minDistanceToPlayer] = Square.Neighbors[i])
            {
                CanAttack = true;
            }
            else if (Square.Neighbors.Count == i)
            {
                BattleManager.Instance.NextEntityTurn();
            }
        }

        if (CanAttack)
        {
            for (int i = 0; i <= EntityData.Spells.Count; i++)
            {
                int randomAction = Random.Range(0, EntityData.Spells.Count);

                if (EntityData.Spells[randomAction].SpellDatas.PaCost <= EntityData.AP)
                {
                    Debug.Log($"Ennemy effectue {EntityData.Spells[randomAction].SpellDatas.Name}");

                    //Attack(path[_minDistanceToPlayer], EntityData.Spells[randomAction]);
                    DecreaseAP(EntityData.Spells[randomAction].SpellDatas.PaCost);
                }
                else if (EntityData.Spells.Count == i)
                {
                    Debug.Log("Ennemy n'a pu assez de PA");
                    BattleManager.Instance.NextEntityTurn();
                }
            }
        }
    }

    public override void ResetPoints()
    {
        MP = EntityData.MP;
        AP = EntityData.AP;
    }
}

