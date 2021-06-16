public static class GameUpdate
{

    public delegate void UpdateFunctions(); //delegate for events

    static event UpdateFunctions PlayerUpdate;
    public static UpdateFunctions PlayerSubscribe { get { return PlayerUpdate; } set { PlayerUpdate = value; } }

    static event UpdateFunctions ObjectUpdate;
    public static UpdateFunctions ObjectSubscribe { get { return ObjectUpdate; } set { ObjectUpdate = value; } }

    static event UpdateFunctions UIupdate;
    public static UpdateFunctions UISubscribe { get { return UIupdate; } set { UIupdate = value; } }

    // Update is called once per frame
    public static void CheckUpdate()
    {
        PlayerUpdate?.Invoke();
        UIupdate?.Invoke();
        ObjectUpdate?.Invoke();
    }

    public static void ClearPlayerSubscriptions()
    {
        PlayerUpdate = null;
    }

    public static void ClearUISubscriptions()
    {
        UIupdate = null;
    }

    public static void ClearWorldSubscriptions()
    {
        ObjectUpdate = null;
    }
    public static void ClearAllSubscriptions()
    {
        PlayerUpdate = null;
        UIupdate = null;
        ObjectUpdate = null;
    }

}
