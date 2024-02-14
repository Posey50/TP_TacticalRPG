using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    // Singleton
    private static AStarManager _instance = null;

    public static AStarManager Instance => _instance;

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
    /// Called to get the shortest path between two squares.
    /// </summary>
    /// <param name="departure"> Departure of the path. </param>
    /// <param name="arrival"> Arrival of the path. </param>
    /// <param name="itsToReachAnEntity"> A value indicating if the calculation is to reach an entity. </param>
    /// <returns></returns>
    public List<Square> CalculateShortestPathBetween(Square departure, Square arrival, bool itsToReachAnEntity)
    {
        return ShortestPath(departure, arrival, itsToReachAnEntity, new(), new(), new());
    }

    /// <summary>
    /// Main loop which goes through the squares from neighbor to neighbor.
    /// </summary>
    /// <param name="departure"> Square from which we choose the next one. </param>
    /// <param name="arrival"> Arrival of the path. </param>
    /// <param name="itsToReachAnEntity"> A value indicating if the calculation is to reach an entity. </param>
    /// <param name="openSquares"> List of open squares that must be browsed. </param>
    /// <param name="shortestPath"> Shortest path to complete at the end of the calculation. </param>
    /// <param name="squaresUsedInTheCalculation"> Squares used in the calculation. </param>
    /// <returns></returns>
    private List<Square> ShortestPath(Square departure, Square arrival, bool itsToReachAnEntity, List<Square> openSquares, List<Square> shortestPath, List<Square> squaresUsedInTheCalculation)
    {
        if (departure != null)
        {
            // If the departure is not the arrival, closes itsef and opens its neighbors
            if (departure != arrival)
            {
                if (!openSquares.Contains(departure))
                {
                    squaresUsedInTheCalculation.Add(departure);
                }

                // Removes the departure from open squares because it is closed
                if (!departure.IsClosed)
                {
                    departure.IsClosed = true;
                    if (openSquares.Contains(departure))
                    {
                        openSquares.Remove(departure);
                    }
                }

                // If the departure has squares next to it...
                if (departure.Neighbors.Count > 0)
                {
                    // ...foreach neighbors...
                    for (int i = 0; i < departure.Neighbors.Count; i++)
                    {
                        // ...if the neighbor is not already open or closed, then add this square to the open squares list,
                        // calculates its distance to the arrival and assignes its previous square to access to it
                        Square neighbor = departure.Neighbors[i];

                        if (!openSquares.Contains(neighbor) && !neighbor.IsClosed)
                        {
                            // If the calculation is not to reach an entity and if there is no entity on the neighbor
                            if (!itsToReachAnEntity && neighbor.EntityOnThisSquare == null)
                            {
                                if (!squaresUsedInTheCalculation.Contains(neighbor))
                                {
                                    squaresUsedInTheCalculation.Add(neighbor);
                                }

                                openSquares.Add(neighbor);
                                CalculateDistanceBetween(neighbor, arrival);
                                neighbor.PreviousSquare = departure;
                            }
                            // If it's to reach an entity and if there is an entity and if it's the arrival, ignores the entity
                            else if (itsToReachAnEntity && (neighbor.EntityOnThisSquare == null || neighbor.EntityOnThisSquare == arrival.EntityOnThisSquare))
                            {
                                if (!squaresUsedInTheCalculation.Contains(neighbor))
                                {
                                    squaresUsedInTheCalculation.Add(neighbor);
                                }

                                openSquares.Add(neighbor);
                                CalculateDistanceBetween(neighbor, arrival);
                                neighbor.PreviousSquare = departure;
                            }
                        }
                    }

                    // Finally, performs again this action with the closest open square as the departure
                    return ShortestPath(ClosestOpenSquare(openSquares), arrival, itsToReachAnEntity, openSquares, shortestPath, squaresUsedInTheCalculation);
                }
                else
                {
                    // If the departure doesn't have neighbors, performs again this action with the other closest open square as the departure
                    return ShortestPath(ClosestOpenSquare(openSquares), arrival, itsToReachAnEntity, openSquares, shortestPath, squaresUsedInTheCalculation);
                }
            }
            else
            {
                // Returns the shortest path to follow
                shortestPath.Add(arrival);
                return ReturnShortestPath(arrival, shortestPath, squaresUsedInTheCalculation);
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Sets F which is a score representating the distance between a square given and the arrival.
    /// </summary>
    /// <param name="square"> Score square to calculate. </param>
    /// <param name="arrival"> Arrival of the path. </param>
    private void CalculateDistanceBetween(Square square, Square arrival)
    {
        // H represents the distance as the crow flies between the square given and the arrival
        float h = Vector3.Distance(square.transform.position, arrival.transform.position);

        // G is equal to the number of squares between the square given and the departure
        if (square.PreviousSquare != null)
        {
            square.StepsForAccess = square.PreviousSquare.StepsForAccess + 1;
        }
        else
        {
            square.StepsForAccess = 0;
        }

        int g = square.StepsForAccess;

        square.F = h + g;
    }

    /// <summary>
    /// Returns the closest open square depending of its "distance score" (F).
    /// </summary>
    /// <param name="openSquares"> List of the open squares. </param>
    /// <returns></returns>
    private Square ClosestOpenSquare(List<Square> openSquares)
    {
        if (openSquares.Count > 0)
        {
            int closestOpenSquareIndex = 0;

            for (int i = 1; i < openSquares.Count; i++)
            {
                if (openSquares[i].F < openSquares[closestOpenSquareIndex].F)
                {
                    closestOpenSquareIndex = i;
                }
            }

            return openSquares[closestOpenSquareIndex];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the shortest path starting from the arrival and going through the previous squares which allow to reach it.
    /// </summary>
    /// <param name="arrival"> Arrival of the path. </param>
    /// <param name="shortestPath"> Shortest path to complete. </param>
    /// <param name="squaresUsedInTheCalculation"> Squares used in the calculation. </param>
    /// <returns></returns>
    private List<Square> ReturnShortestPath(Square arrival, List<Square> shortestPath, List<Square> squaresUsedInTheCalculation)
    {
        if (arrival.PreviousSquare != null)
        {
            shortestPath.Add(arrival.PreviousSquare);
            return ReturnShortestPath(arrival.PreviousSquare, shortestPath, squaresUsedInTheCalculation);
        }
        else
        {
            shortestPath.Reverse();
            ResetAllSquaresUsed(squaresUsedInTheCalculation);
            return shortestPath.ToArray()[1..].ToList();
        }
    }

    /// <summary>
    /// Called at the end of a calculation to reset all squares used in the calculation.
    /// </summary>
    /// <param name="squaresUsedInTheCalculation"> Squares to reset. </param>
    private void ResetAllSquaresUsed(List<Square> squaresUsedInTheCalculation)
    {
        for (int i = 0; i < squaresUsedInTheCalculation.Count; i++)
        {
            squaresUsedInTheCalculation[i].ResetSquare();
        }
    }

    /// <summary>
    /// Return a list of squares into a list of positions
    /// </summary>
    /// <param name="squares"> Squares to convert. </param>
    /// <returns></returns>
    public List<Vector3> ConvertSquaresIntoPositions(List<Square> squares)
    {
        List <Vector3> path = new ();
        
        for (int i = 0; i < squares.Count; i++)
        {
            path.Add(squares[i].transform.position);
        }

        return path;
    }
}