using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static 
{
    private static Static instance;

    public static Static Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Static();
            }
            return instance;
        }
    }


    public SceneType sceneType;
}
