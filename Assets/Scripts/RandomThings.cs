using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomThings
{
    public static Vector3 Interpolate(float currtime,  Vector3 c0, Vector3 c1)
    {
        Vector3 c01 = c0 * (1f - currtime) + c1 * currtime;

        return c01;
    }
}
