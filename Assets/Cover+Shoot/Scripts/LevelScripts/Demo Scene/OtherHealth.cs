using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        TargetLink();
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
                        i.localEulerAngles = new Vector3(0, 0, 0);
                    }
                }
                break;
        }
    }


    public override void TargetLink()
    {
        base.TargetLink();
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
    public override void Hiteffect(Ray ray, RaycastHit raycastHit)
    {
        Transform[] allchild;
        allchild = transform.gameObject.GetComponentsInChildren<Transform>();
        base.Hiteffect(ray, raycastHit);     
        if (toggleSound)
            AudioSource.PlayClipAtPoint(toggleSound,ray.GetPoint(0));

        switch (targetType)
        {
            case TargetType.MolotovCocktails:
                foreach (Transform i in allchild)
                {
                    if(i.name == "Obj")
                    {
                        //i.GetComponent<MeshExploder>().Explode();
                        i.GetComponent<MeshCollider>().enabled = false;
                        i.GetComponent<MeshRenderer>().enabled = false;
                        GameObject particle = Instantiate(Resources.Load<GameObject>("Particle/explosion"));
                        GameObject particle2 = Instantiate(Resources.Load<GameObject>("Particle/SmokeFire"),this.transform);
                        particle.transform.position = this.transform.position + new Vector3(0,0.5f,0);
                        particle2.transform.position = this.transform.position;
                        particle2.transform.name = "SmokeFire";
                        for (int j =0;j<i.transform.childCount;j++)
                        {
                            Destroy(i.transform.GetChild(j).gameObject);
                        }
                    }
                }
                break;
            case TargetType.balloon:
                foreach (Transform i in allchild)
                {
                    if (i.name == "Obj")
                    {                     
                        i.GetComponent<CapsuleCollider>().enabled = false;
                        i.GetComponent<MeshRenderer>().enabled = false;
                        GameObject particle = Instantiate(Resources.Load<GameObject>("Particle/explosion_qq"));
                        particle.transform.position = this.transform.position + new Vector3(0, 1.3f, 0);
                        for (int j = 0; j < i.transform.childCount; j++)
                        {
                            Destroy(i.transform.GetChild(j).gameObject);
                        }
                    }
                }
                break;
            case TargetType.coke:
                GameObject effect = Instantiate(raycastHit.collider.gameObject, raycastHit.collider.gameObject.transform.position, raycastHit.collider.gameObject.transform.rotation);
                effect.AddComponent<Rigidbody>().AddForceAtPosition(ray.direction * raycastHit.distance, raycastHit.point, ForceMode.Impulse);

                EffectGroup.Add(effect);

                for (int i = 0; i < raycastHit.collider.gameObject.transform.childCount; i++)
                {
                    Destroy(raycastHit.collider.gameObject.transform.GetChild(i).gameObject);
                }

                raycastHit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                raycastHit.collider.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                break;
            case TargetType.watermelon:
                foreach (Transform i in allchild)
                {
                    if (i.name == "Obj")
                    {
                        //i.GetComponent<MeshExploder>().Explode();
                        i.GetComponent<CapsuleCollider>().enabled = false;
                        i.GetComponent<MeshRenderer>().enabled = false;
                        GameObject particle = Instantiate(Resources.Load<GameObject>("Particle/explosion"));
                        particle.transform.position = this.transform.position + new Vector3(0, 0.2f, 0);
                        for (int j = 0; j < i.transform.childCount; j++)
                        {
                            Destroy(i.transform.GetChild(j).gameObject);
                        }
                    }
                }
                break;
        }
    }
    public void TargetEnd()
    {
        StopAllCoroutines();
         Transform[] allchild;
        allchild = transform.gameObject.GetComponentsInChildren<Transform>();
        switch (targetType)
        {         
            case TargetType.MolotovCocktails:
                for (int i = 0;i< transform.childCount;i++)
                {
                    if (transform.GetChild(i).name == "Obj")
                    {
                        transform.GetChild(i).GetComponent<MeshCollider>().enabled = true;
                        transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
                    }
                    if (transform.GetChild(i).name == "SmokeFire")
                    {
                        Destroy(transform.GetChild(i).gameObject);
                    }
                }
                break;
            case TargetType.balloon:
                foreach (Transform i in allchild)
                {
                    if (i.name == "Obj")
                    {
                        i.GetComponent<CapsuleCollider>().enabled = true;
                        i.GetComponent<MeshRenderer>().enabled = true;
                    }                    
                }
                break;
            case TargetType.coke:
                foreach (Transform i in allchild)
                {
                    if (i.name == "Obj")
                    {
                        i.GetComponent<CapsuleCollider>().enabled = true;
                        i.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
               
                for (int i = 0; i < EffectGroup.Count; i++)
                {
                    Destroy(EffectGroup[i].gameObject);
                }
                EffectGroup.Clear();
                break;
            case TargetType.watermelon:
                foreach (Transform i in allchild)
                {
                    if (i.name == "Obj")
                    {
                        i.GetComponent<CapsuleCollider>().enabled = true;
                        i.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
                break;
        }
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
        movePos.GetComponent<NavMeshObstacle>().enabled = false;
        movePos.GetComponentInChildren<BoxCollider>().enabled = false;
        Quaternion q = Quaternion.identity;
        q.eulerAngles = new Vector3(0, movePos.rotation.eulerAngles.y-126, 0);
        while (Mathf.Abs(movePos.localEulerAngles.y - 225) > 10)
        {      
            movePos.rotation = Quaternion.Lerp(movePos.rotation, q, 10*Time.deltaTime);
            yield return 0;
        }
        movePos.GetComponent<NavMeshObstacle>().enabled = true;
        movePos.GetComponentInChildren<BoxCollider>().enabled = true;

        if (LinkObj)
        LinkObj.TargetLink();
    }
}
