using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DistanceBrain : Brain
{
    public override void InitBrain()
    {
        base.InitBrain();

        for (int i = 0; i < _spells.Count; i++)
        {
            if (_spells[i].SpellDatas.Type != Type.distance)
            {
                Debug.LogError(_spells[i].SpellDatas.Name + " is not a valid spell for " + _enemyMain.Name);
            }
        }
    }

    public override IEnumerator EnemyPattern()
    {
        // Waits to simulate reflexion 
        yield return new WaitForSeconds(3f);

        // Sorts spells by descending order of their range
        _spells = _spells.OrderByDescending(spell => spell.SpellDatas.MaxRange).ToList();

        // Creates a dictionary which will stocks reachable playable entities with a score of priority calculated with percentage of missing HP
        // and multiplied by a proximity coefficient
        Dictionary<Entity, int> playableEntities = PlayableEntitiesByOrderOfPriority(PlayableEntitiesByOrderOfDistance(true));

        if (playableEntities.Count > 0)
        {
            // Gets the playable entity to attack
            Entity playableEntityToAttack = playableEntities.ElementAt(0).Key;

            // Tries to attack the playable entity
            yield return StartCoroutine(TryToAttack(playableEntityToAttack, BestSpellToUse(_spells, playableEntityToAttack)));

            // If the enemy has enough AP to use the cheapest spell
            if (_enemyMain.AP > _spells[^1].SpellDatas.APCost)
            {
                // Restarts a patern
                StartCoroutine(EnemyPattern());
            }
            else
            {
                // If the enemy has remained MP then he tries to escape
                if (_enemyMain.MP > 0)
                {
                    // Waits to simulate reflexion 
                    yield return new WaitForSeconds(3f);

                    // Enemy tries to escape
                    yield return StartCoroutine(TryToEscape());
                }

                // Enemy end his turn
                _enemyMain.EndOfTheTurn();
            }
        }
        else if (_enemyMain.MP > 0)
        {
            // Enemy tries to move at a good distance from the weakest entity
            yield return StartCoroutine(MoveAtAGoodDistanceToTheWeakerPlayableEntity());

            // Enemy ends his turn
            _enemyMain.EndOfTheTurn();
        }
        else
        {
            // Enemy ends his turn
            _enemyMain.EndOfTheTurn();
        }
    }

    /// <summary>
    /// Called to get a dictionary of playable entities sorted in ascending order of their distance of the melee with a score of priority
    /// </summary>
    /// <returns></returns>
    private Dictionary<Entity, int> PlayableEntitiesByOrderOfDistance(bool itsToAttack)
    {
        // Creates a dictionary which will stocks playable entities with a score of priority
        Dictionary<Entity, int> playableEntities = new();
        List<Entity> playableEntitiesInBattle = new(BattleManager.Instance.PlayableEntitiesInBattle);

        // Gets a list of spell order by ascending order of their minimum range
        List<Spell> _spellsByAscendingOrderOfTheirMinRange = new(_spells);
        _spellsByAscendingOrderOfTheirMinRange = _spellsByAscendingOrderOfTheirMinRange.OrderBy(spell => spell.SpellDatas.MinRange).ToList();

        if (playableEntitiesInBattle.Count > 0)
        {
            // For each playable entity
            for (int i = 0; i < playableEntitiesInBattle.Count; i++)
            {
                Entity playableEntity = playableEntitiesInBattle[i];

                // Calculates the shortest path between the melee and the playable entity
                List<Square> shortestPathToThePlayableEntity = AStarManager.Instance.CalculateShortestPathToAnEntity(_enemyMain.SquareUnderTheEntity, playableEntity.SquareUnderTheEntity);

                // If the playable entity is reachable with the attack that has the greatest range or the lowest or if it's not for an attack
                if (!itsToAttack || 
                    shortestPathToThePlayableEntity.Count <= _enemyMain.MP + _spells[0].SpellDatas.MaxRange ||
                    shortestPathToThePlayableEntity.Count >= _spellsByAscendingOrderOfTheirMinRange[0].SpellDatas.MinRange + _enemyMain.MP)
                {
                    // Add the playable entity to the list of playable entities
                    playableEntities.Add(playableEntity, shortestPathToThePlayableEntity.Count);
                }
            }

            if (playableEntities.Count > 1)
            {
                // Sorts playable entities by ascending order of their distance to the melee
                playableEntities = playableEntities.OrderBy(enemies => enemies.Value).ToDictionary(enemies => enemies.Key, enemies => 0);

                // Attributes a distance coefficient for each playable entity
                for (int i = 0; i < playableEntities.Count; i++)
                {
                    playableEntities[playableEntities.ElementAt(i).Key] = playableEntities.Count - i;
                }
            }
            else if (playableEntities.Count == 1)
            {
                // Resets score to 0
                playableEntities[playableEntities.ElementAt(0).Key] = 0;
            }
        }

        return playableEntities;
    }

    /// <summary>
    /// Called to get a dictionary of playable entities sorted in ascending order of their score calculated with percentage of missing HP and multiplied by a proximity coefficient.
    /// </summary>
    /// <param name="playableEntities"> Playable entities to sort. </param>
    /// <returns></returns>
    private Dictionary<Entity, int> PlayableEntitiesByOrderOfPriority(Dictionary<Entity, int> playableEntities)
    {
        if (playableEntities.Count > 0)
        {
            // For each playable entity
            for (int i = 0; i < playableEntities.Count; i++)
            {
                Entity playableEntity = playableEntities.ElementAt(i).Key;

                // Calculates the percentage of missing HP and multiplies with the distance coefficient
                playableEntities[playableEntity] = (int)((playableEntity.EntityDatas.MaxHP - playableEntity.HP) / playableEntity.EntityDatas.MaxHP * 100f) * playableEntities[playableEntity];
            }

            if (playableEntities.Count > 1)
            {
                // Sorts playable entities by descending order of their score
                playableEntities = playableEntities.OrderByDescending(allies => allies.Value).ToDictionary(allies => allies.Key, allies => allies.Value);
            }
        }

        return playableEntities;
    }

    /// <summary>
    /// Called to get a dictionnary of combinations of spells ordered by their efficiency and by anticipating a move in advance.
    /// </summary>
    /// <param name="spells"> List of spells. </param>
    /// <param name="playableEntityToAttack"> The playable entity to attack. </param>
    /// <returns></returns>
    private Dictionary<List<Spell>, int> BestSpellToUse(List<Spell> spells, Entity playableEntityToAttack)
    {            
        // Creates the dictionary which will stocks combinations of two spells with the percentage of damages on the enemy to attack
        Dictionary<List<Spell>, int> spellsCombinationInOrderOfDamages = new();

        if (spells.Count > 1)
        {
            // Gets a copy of spells
            List<Spell> spellsToCheck = new(spells);

            // For each spell
            for (int i = 0; i < spells.Count; i++)
            {
                Spell currentStartingSpell = spells[i];

                // For each other spell which was not already used to start a combination
                for (int j = 0; j < spellsToCheck.Count; j++)
                {
                    Spell currentSpellToCheck = spellsToCheck[j];

                    // If the combination is a combination of the two same spells
                    if (currentStartingSpell == currentSpellToCheck)
                    {
                        // If the combination is possible
                        if (currentStartingSpell.SpellDatas.APCost + currentSpellToCheck.SpellDatas.APCost <= _enemyMain.AP)
                        {
                            // Calculates the percentage of damages that it represents
                            int percentageOfDamages = (int)(((currentStartingSpell.SpellDatas.Damages + currentSpellToCheck.SpellDatas.Damages) / (float)(playableEntityToAttack.EntityDatas.MaxHP)) * 100f);

                            // Adds the combination
                            spellsCombinationInOrderOfDamages.Add(new List<Spell>() { currentStartingSpell, currentSpellToCheck }, percentageOfDamages);
                        }
                        // Checks if the spell is possible alone
                        else if (currentStartingSpell.SpellDatas.APCost <= _enemyMain.AP)
                        {
                            // Calculates the percentage of damages that it represents
                            int percentageOfDamages = (int)((currentStartingSpell.SpellDatas.Damages / (float)(playableEntityToAttack.EntityDatas.MaxHP)) * 100f);

                            // Adds the spell as a combination
                            spellsCombinationInOrderOfDamages.Add(new List<Spell>() { currentStartingSpell }, percentageOfDamages);
                        }
                    }
                    // If it's not a combination of the two same spells and if the combination is possible
                    else if (currentStartingSpell.SpellDatas.APCost + currentSpellToCheck.SpellDatas.APCost <= _enemyMain.AP)
                    {
                        // Calculates the percentage of damages that it represents
                        int percentageOfDamages = (int)(((currentStartingSpell.SpellDatas.Damages + currentSpellToCheck.SpellDatas.Damages) / (float)(playableEntityToAttack.EntityDatas.MaxHP)) * 100f);

                        // Adds the combination
                        spellsCombinationInOrderOfDamages.Add(new List<Spell>() { currentStartingSpell, currentSpellToCheck }, percentageOfDamages);
                    }
                }

                // Removes the starting spell from the spell to check to prevent duplicates
                spellsToCheck.Remove(currentStartingSpell);
            }

            // Sorts combinations by descending order of their efficiency
            spellsCombinationInOrderOfDamages = spellsCombinationInOrderOfDamages.OrderByDescending(spell => spell.Value).ToDictionary(spell => spell.Key, spell => spell.Value);

            // Returns the dictionnary
            return spellsCombinationInOrderOfDamages;
        }
        else
        {
            spellsCombinationInOrderOfDamages.Add(new List<Spell>() { spells[0] }, 0);
            return spellsCombinationInOrderOfDamages;
        }
    }

    /// <summary>
    /// Called to try to attack a playable entity with a spell.
    /// </summary>
    /// <param name="playableEntityToAttack"> Playable entity to attack. </param>
    /// <param name="spellsCombinations"> Spells combination in the order of their efficiency. </param>
    /// <returns></returns>
    private IEnumerator TryToAttack(Entity playableEntityToAttack, Dictionary<List<Spell>, int> spellsCombinations)
    {
        // The list which stocks spells in the order of using
        List<Spell> spellsInOrderOfUsing = new ();

        // Convert the dictionnary into a list of spell in order of using
        for (int i = 0; i < spellsCombinations.Count; i++)
        {
            Spell spell = spellsCombinations.ElementAt(i).Key[0];

            if (!spellsInOrderOfUsing.Contains(spell))
            {
                spellsInOrderOfUsing.Add(spell);
            }
        }

        // For each spell
        for (int i = 0; i < spellsInOrderOfUsing.Count; i++)
        {
            Spell spellToUse = spellsInOrderOfUsing[i];

            // Gets the range of the spell
            List<Square> range = RangeManager.Instance.CalculateRange(_enemyMain.SquareUnderTheEntity, spellToUse.SpellDatas.MinRange, spellToUse.SpellDatas.MaxRange);

            // Checks if the playable entity to attack is in the range
            if (range.Contains(playableEntityToAttack.SquareUnderTheEntity))
            {
                // Attacks the playable entity
                UniTask attacking = _enemyMain.Attack(spellToUse, playableEntityToAttack);
                yield return new WaitUntil(() => attacking.Status != UniTaskStatus.Pending);
                break;
            }
            // If she is not
            else
            {
                // Calculates the distance to the playable entity
                Square[] distanceToPlayableEntity = AStarManager.Instance.CalculateASimpleDistance(_enemyMain.SquareUnderTheEntity, playableEntityToAttack.SquareUnderTheEntity).ToArray();

                // If the playable entity is too far
                if (distanceToPlayableEntity.Length > (spellToUse.SpellDatas.MaxRange))
                {
                    // Reduces the path by the range of the spell to save MP
                    List<Square> pathToGetCloser = distanceToPlayableEntity[..^(spellToUse.SpellDatas.MaxRange)].ToList();

                    // Waits until the enemy has moved
                    UniTask followingThePath = _enemyMain.FollowThePath(pathToGetCloser);
                    yield return new WaitUntil(() => followingThePath.Status != UniTaskStatus.Pending);

                    // Attacks the playable entity
                    UniTask attacking = _enemyMain.Attack(spellToUse, playableEntityToAttack);
                    yield return new WaitUntil(() => attacking.Status != UniTaskStatus.Pending);

                    break;
                }
                else if (distanceToPlayableEntity.Length < spellToUse.SpellDatas.MinRange)
                {
                    // Gets a new position to go back
                    Square squareWhereGoBack = FindAPositionToMoveBack(spellToUse.SpellDatas.MinRange - distanceToPlayableEntity.Length);

                    // The path to follow to get away from the playable entity
                    List<Square> pathToGetAway = AStarManager.Instance.CalculateShortestPathForAMovement(_enemyMain.SquareUnderTheEntity, squareWhereGoBack);

                    // Waits until the enemy has moved
                    UniTask followingThePath = _enemyMain.FollowThePath(pathToGetAway);
                    yield return new WaitUntil(() => followingThePath.Status != UniTaskStatus.Pending);

                    // Attacks the playable entity
                    UniTask attacking = _enemyMain.Attack(spellToUse, playableEntityToAttack);
                    yield return new WaitUntil(() => attacking.Status != UniTaskStatus.Pending);

                    break;
                }
            }
        }
    }

    /// <summary>
    /// Called to find a position to move back.
    /// </summary>
    /// <param name="range"> The distance to move back. </param>
    private Square FindAPositionToMoveBack(int range)
    {
        List<Square> positionsInTheRange = RangeManager.Instance.CalculateRange(_enemyMain.SquareUnderTheEntity, range, range);

        for (int i = 0; i < positionsInTheRange.Count; i++)
        {
            Square square = positionsInTheRange[i];

            // If the square is available and if the path go to is enough short
            if (square.EntityOnThisSquare == null && AStarManager.Instance.CalculateShortestPathForAMovement(_enemyMain.SquareUnderTheEntity, square).Count <= _enemyMain.MP)
            {
                return square;
            }
        }

        return null;
    }

    /// <summary>
    /// Called to move at a good distance to the weaker playable entity.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveAtAGoodDistanceToTheWeakerPlayableEntity()
    {
        // Creates a dictionary which will stocks reachable playable entities with a score of priority calculated with percentage of remaining HP
        // and multiplied by a proximity coefficient
        Dictionary<Entity, int> playableEntities = PlayableEntitiesByOrderOfPriority(PlayableEntitiesByOrderOfDistance(false));

        if (playableEntities.Count > 0)
        {
            // Gets the playable entity to attack
            Entity playableEntityToMoveTo = playableEntities.ElementAt(0).Key;

            // Calculates the path to go to the playable entity
            Square[] pathToPlayableEntity = AStarManager.Instance.CalculateShortestPathToAnEntity(_enemyMain.SquareUnderTheEntity, playableEntityToMoveTo.SquareUnderTheEntity).ToArray();

            // Reduces the path by one for the playable entity
            pathToPlayableEntity = pathToPlayableEntity[..^1];

            // The path to follow
            List<Square> pathToGetCloser;

            // Checks if the path is too long to reach the end
            if (pathToPlayableEntity.Length >= _enemyMain.MP)
            {
                // Reduces the path by the difference between the length of the path and the remaining MP
                pathToGetCloser = pathToPlayableEntity[..^(pathToPlayableEntity.Length - _enemyMain.MP)].ToList();
            }
            else
            {
                pathToGetCloser = pathToPlayableEntity.ToList();
            }

            if (pathToGetCloser.Count > 0)
            {
                // Wait until the enemy has moved
                UniTask followingThePath = _enemyMain.FollowThePath(pathToGetCloser);
                yield return new WaitUntil(() => followingThePath.Status != UniTaskStatus.Pending);
            }
        }
    }

    /// <summary>
    /// // Called to try to find a position at a certain distance from any playable entity.
    /// </summary>
    private IEnumerator TryToEscape()
    {
        // The list which contains all possible positions
        List<Square> possiblePositions = new();

        possiblePositions.Add(_enemyMain.SquareUnderTheEntity);

        // Add possible positions with remaining MP
        possiblePositions.AddRange(RangeManager.Instance.CalculateRange(_enemyMain.SquareUnderTheEntity, 1, _enemyMain.MP));

        // All playable entities in battle
        List<Entity> playableEntitiesInBattle = BattleManager.Instance.PlayableEntitiesInBattle;

        // Value indicating that the enemy has found a safe place
        bool HasFoundASafePlace = false;

        // Tries to find a safe position from the maximum distance to the minimum
        for (int i = 0; i < _enemyMain.MP; i++)
        {
            int minDistance = _enemyMain.MP - i;

            // For each possible position
            for (int j = 0; j < possiblePositions.Count; j++)
            {
                Square position = possiblePositions[j];
                bool isThisPositionSafe = true;

                // If there is no entity on it
                if (position.EntityOnThisSquare == null)
                {
                    // For each playable entity
                    for (int k = 0; k < playableEntitiesInBattle.Count; k++)
                    {
                        // Calculates distance from the playable entity
                        Entity playableEntity = playableEntitiesInBattle[k];
                        List<Square> path = AStarManager.Instance.CalculateASimpleDistance(position, playableEntity.SquareUnderTheEntity);
                        int distanceFromThisPlayableEntity = path.Count;

                        // If the distance is lower than the minimum distance then finds another position
                        if (distanceFromThisPlayableEntity < minDistance)
                        {
                            isThisPositionSafe = false;
                            break;
                        }
                    }
                }
                else
                {
                    isThisPositionSafe = false;
                }

                // If the position is safe then go to this position
                if (isThisPositionSafe)
                {
                    HasFoundASafePlace = true;
                    List<Square> path = AStarManager.Instance.CalculateShortestPathForAMovement(_enemyMain.SquareUnderTheEntity, position);

                    // Waits until the enemy has moved
                    UniTask followingThePath = _enemyMain.FollowThePath(path);
                    yield return new WaitUntil(() => followingThePath.Status != UniTaskStatus.Pending);
                    break;
                }
            }

            // If the enemy has found a safe place then stops the calculation
            if (HasFoundASafePlace)
            {
                break;
            }
        }
    }
}
