using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class AStar
{
    public List<Tile> CalculateShortestPath(Transform departure, Transform arrival)
    {
        List<Tile> result = new List<Transform>();
        List<Tile> shortestPath = new();

        return 
    }

    private List<Transform> ShortestPath(Transform departure, Transform arrival, List<Transform> shortestPath)
    {
        //If the departure is not the arrival, close it and open it linked and open waypoints
        if (_departure != _arrival)
        {
            //Remove the departure from open waypoints because it is closed
            if (waypointsOpen.Contains(_departure))
            {
                waypointsOpen.Remove(_departure);
            }
            //Set the bool isOpen to false in the waypoint's script
            Waypoint departure = _departure.GetComponent<Waypoint>();
            departure.isOpen = false;

            //If the departure has waypoints next to it...
            if (departure.directWaypoints.Count > 0)
            {
                //...foreach waypoint...
                foreach (Transform waypoint in departure.directWaypoints)
                {
                    //...if it is open then add this waypoint to the open waypoints list, calcul it distance and assign it previous waypoint to access it
                    if (waypoint.GetComponent<Waypoint>().isOpen)
                    {
                        waypointsOpen.Add(waypoint);
                        CalculDistanceBetween(_departure, waypoint);
                        waypoint.GetComponent<Waypoint>().previousWaypoint = _departure;
                    }
                }

                //Finally, perform again this action with the closest open waypoint as a departure
                NewNearestPath(ClosestOpenedWaypoint(), _arrival);
            }
            else
            {
                //If the departure doesn't have waypoints next to it, perform again this action with the other closest open waypoint as a departure
                NewNearestPath(ClosestOpenedWaypoint(), _arrival);
            }
        }
        else
        {
            //Clear the path to follow
            pathToFollow.Clear();

            //Set the new path to follow
            pathToFollow.Add(_arrival);
            ReturnNearestPath(_arrival);
            pathToFollow.Reverse();

            //Reset all values in waypoints after the calcul
            ResetAllWaypoints();
        }
    }
}
