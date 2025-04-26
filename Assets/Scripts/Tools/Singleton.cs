using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance;

    protected void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = (T)this;
        }
    }

    protected void OnDestroy()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }
    }
}
