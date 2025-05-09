using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action AfterSceneLoad;

    public static void CallAfterSceneLoad()
    {
        AfterSceneLoad?.Invoke();
    }

    public static event Action<Vector3> PlayerMove;

    public static void CallPlayerMove(Vector3 targetPos)
    {
        PlayerMove?.Invoke(targetPos);
    }
}
