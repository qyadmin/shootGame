using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherHealth : HealthManager
{
    public AudioClip toggleSound;

    [SerializeField]
    TargetType targetType;


    private void Awake()
    {

    }

    public void Revive()
    {
        TargetAwake();
    }

    public void End()
    {
        StopAllCoroutines();
    }

    public override void TargetResets()
    {
        base.TargetResets();
        StopAllCoroutines();
        Transform[] allchild;
        allchild = transform.gameObject.GetComponentsInChildren<Transform>();
        switch (targetType)
        {         
            case TargetType.DoorOrWindow:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        i.localEulerAngles = new Vector3(0,0,0);
                    }
                }
                break;


        }

    }


    public override void TargetAwake()
    {
        base.TargetAwake();
        StopAllCoroutines();
        Transform[] allchild;
        allchild = transform.gameObject.GetComponentsInChildren<Transform>();
        switch (targetType)
        {
            case TargetType.Rotate_single:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        IEnumerator rotate = RotateMove(i);
                        StartCoroutine(rotate);
                    }
                }
                break;
            case TargetType.Rotate_double:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        IEnumerator rotate = RotateMove(i);
                        StartCoroutine(rotate);
                    }
                }
                break;
            case TargetType.UDMove:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        IEnumerator udmove = UDMove(i);
                        StartCoroutine(udmove);
                    }
                }
                break;
            case TargetType.LRMove:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        IEnumerator lrmove = LRMove(i);
                        StartCoroutine(lrmove);
                    }
                }
                break;
            case TargetType.Slide_single:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        IEnumerator slidemove = Slide_single(i);
                        StartCoroutine(slidemove);
                    }
                }
                break;
            case TargetType.Slide_double:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        IEnumerator slidemove = Slide_single(i);
                        StartCoroutine(slidemove);
                    }
                }
                break;
            case TargetType.DoorOrWindow:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        IEnumerator doormove = door_open(i);
                        StartCoroutine(doormove);
                    }
                }
                break;


        }
    }

    public void TargetEnd()
    {
        StopAllCoroutines();
    }
    float Rotate_speed = 30;
    float Move_speed = 1;
    IEnumerator RotateMove(Transform movePos)
    {
        while (true)
        {
            movePos.Rotate(new Vector3(0, Rotate_speed * Time.deltaTime, 0));
            yield return null;
        }
    }

    IEnumerator UDMove(Transform movePos)
    {
        while (movePos.localPosition.y > -0.5f)
        {
            movePos.transform.Translate(Vector3.up * -Move_speed * Time.deltaTime);
            yield return null;
        }
        while (movePos.localPosition.y < 0.5f)
        {
            movePos.transform.Translate(Vector3.up * Move_speed * Time.deltaTime);
            yield return null;
        }
        IEnumerator udmove = UDMove(movePos);
        StartCoroutine(udmove);
    }

    IEnumerator LRMove(Transform movePos)
    {
        while (Mathf.Abs(movePos.eulerAngles.z - 300) > 2)
        {
            movePos.Rotate(new Vector3(0, 0, -Rotate_speed * 2 * Time.deltaTime));
            yield return null;
        }
        while (Mathf.Abs(movePos.eulerAngles.z - 60) > 2)
        {
            movePos.Rotate(new Vector3(0, 0, Rotate_speed * 2 * Time.deltaTime));
            yield return null;
        }
        IEnumerator lrmove = LRMove(movePos);
        StartCoroutine(lrmove);
    }

    IEnumerator Slide_single(Transform movePos)
    {
        while (movePos.localPosition.x < 2.5f)
            movePos.transform.Translate(Vector3.left * Move_speed * Time.deltaTime);
        yield return null;
    }

    IEnumerator door_open(Transform movePos)
    {       
        while (Mathf.Abs(movePos.localEulerAngles.y -225) > 2)
        {
            Debug.Log(movePos.localEulerAngles.y);
            movePos.Rotate(new Vector3(0, -Rotate_speed * 10 * Time.deltaTime, 0));
            yield return null;
        }
        if(LinkObj)
        LinkObj.TargetAwake();
    }
}
