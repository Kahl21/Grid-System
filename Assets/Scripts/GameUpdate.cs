using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUpdate
{

    public delegate void UpdateFunctions(); //delegate for events
    
    static event UpdateFunctions UIupdate;
    public static UpdateFunctions Subscribe { get { return UIupdate; } set { UIupdate = value; } }

    // Update is called once per frame
    public static void CheckUpdate()
    {
        UIupdate?.Invoke();
    }
}
