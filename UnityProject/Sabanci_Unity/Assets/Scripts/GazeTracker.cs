using System.Collections.Generic;
using UnityEngine;

public static class GazeTracker
{
    private static Dictionary<Transform, int> lookers = new Dictionary<Transform, int>();

    // Called when a student starts looking at an object
    public static void AddLooker(Transform obj)
    {
        if (obj == null) return;

        if (lookers.ContainsKey(obj))
        {
            lookers[obj]++;
        }
        else
        {
            lookers[obj] = 1;
        }
    }

    // Called when a student stops looking at an object
    public static void RemoveLooker(Transform obj)
    {
        if (obj == null) return;

        if (lookers.ContainsKey(obj))
        {
            lookers[obj]--;

            if (lookers[obj] <= 0)
            {
                lookers.Remove(obj);
            }
        }
    }

    // Check if an object is still being looked at
    public static bool IsBeingLookedAt(Transform obj)
    {
        return lookers.ContainsKey(obj) && lookers[obj] > 0;
    }
}
