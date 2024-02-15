using System.Collections.Generic;
using UnityEngine;

public class RangeManager : MonoBehaviour
{
    // Singleton
    private static RangeManager _instance = null;

    public static RangeManager Instance => _instance;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
    }

    /// <summary>
    /// Called to get a complexe range with a minimum and a maximum.
    /// </summary>
    /// <param name="departure"> Center of the range. </param>
    /// <param name="minRange"> Minimum of the range. </param>
    /// <param name="maxRange"> Maximum of the range. </param>
    /// <returns></returns>
    public List<Square> CalculateComplexeRange(Square departure, int minRange, int maxRange)
    {
        List<Square> squaresNotInRange = new();
        List<Square> squaresInRange = new();
        List<Square> squaresOpen = new();

        squaresOpen.Add(departure);

        // For the first layers which are not in the range
        for (int i = 0; i < minRange; i++)
        {
            // List of squares to check in the layer
            List<Square> squaresToCheck = new(squaresOpen);

            // For each square in the layer
            for (int j = 0; j < squaresToCheck.Count; j++)
            {
                // Current square to check
                Square currentSquaresToCheck = squaresToCheck[j];

                // For each neighbor of the current square to check
                for (int k = 0; k < currentSquaresToCheck.Neighbors.Count; k++)
                {
                    // Current neighbor
                    Square currentNeighbor = currentSquaresToCheck.Neighbors[k];

                    // Checks if the neighbor is already stocked in the "not in range"
                    if (currentNeighbor != null && !squaresNotInRange.Contains(currentNeighbor) && currentNeighbor != departure)
                    {
                        squaresNotInRange.Add(currentNeighbor);
                        squaresOpen.Add(currentNeighbor);
                    }
                }

                // Remove the current square to the open squares list
                squaresOpen.Remove(currentSquaresToCheck);
            }

            // Clear the list of squares to check in the layer
            squaresToCheck.Clear();
        }

        // For layers until the maximum range
        for (int i = 0; i < maxRange - minRange; i++)
        {
            // List of squares to check in the layer
            List<Square> squaresToCheck = new(squaresOpen);

            // For each square in the layer
            for (int j = 0; j < squaresToCheck.Count; j++)
            {
                // Current square to check
                Square currentSquaresToCheck = squaresToCheck[j];

                // For each neighbor of the current square to check
                for (int k = 0; k < currentSquaresToCheck.Neighbors.Count; k++)
                {
                    // Current neighbor
                    Square currentNeighbor = currentSquaresToCheck.Neighbors[k];

                    // Checks if the neighbor is already stocked in the range
                    if (currentNeighbor != null && !squaresInRange.Contains(currentNeighbor) && !squaresNotInRange.Contains(currentNeighbor))
                    {
                        squaresInRange.Add(currentNeighbor);
                        squaresOpen.Add(currentNeighbor);
                    }
                }

                // Remove the current square to the open squares list
                squaresOpen.Remove(currentSquaresToCheck);
            }

            // Clear the list of squares to check in the layer
            squaresToCheck.Clear();
        }

        return squaresInRange;
    }
}
