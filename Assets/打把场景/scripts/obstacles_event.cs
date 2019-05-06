using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacles_event : MonoBehaviour
{
    [SerializeField]
    Transform objects;

    [SerializeField]
    float speed;

    [SerializeField]
    public bool swich = false;

    private void Start()
    {
        StartCoroutine(move());
    }


    IEnumerator move()
    {
        while (objects.localPosition.y >=-4)
        {
            if(swich)
                objects.transform.Translate(Vector3.up*-speed*Time.deltaTime);
            yield return null;
        }
        while (objects.localPosition.y <= 0)
        {
            if (swich)
                objects.transform.Translate(Vector3.up * +speed * Time.deltaTime);
            yield return null;
        }
        yield return null;
        StartCoroutine(move());
    }
}
