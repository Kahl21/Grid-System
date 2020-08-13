using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomThings
{
    //global interpolation math
    public static Vector3 Interpolate(float currtime, Vector3 c0, Vector3 c1)
    {
        Vector3 c01 = c0 * (1f - currtime) + c1 * currtime;

        return c01;
    }

    public static float Interpolate(float currtime, float c0, float c1)
    {
        float c01 = c0 * (1f - currtime) + c1 * currtime;

        return c01;
    }
}
