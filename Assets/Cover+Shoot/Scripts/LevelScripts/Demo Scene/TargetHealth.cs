using System.Collections;
using UnityEngine;

// This class is created for the example scene. There is no support for this script.
public class TargetHealth : HealthManager
{
	public bool boss;
	public AudioClip toggleSound;

	private Vector3 targetRotation;
	private float health, totalHealth = 20;
	private RectTransform healthBar;
	private float originalBarScale;
	private bool dead;

    

    [SerializeField]
    TargetType targetType;

    [SerializeField]
    bool isStatic = false;

	void Awake ()
	{
		targetRotation = this.transform.localEulerAngles;
        if (!isStatic)
            targetRotation.x = -90;
		if (boss)
		{
			healthBar = this.transform.Find("Health/Bar").GetComponent<RectTransform>();
			healthBar.parent.gameObject.SetActive(false);
			originalBarScale = healthBar.sizeDelta.x;
		}
		dead = true;
		health = totalHealth;
        TargetAwake();
    }

	void Update ()
	{
		//this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.Euler(targetRotation), 20 * Time.deltaTime);
	}

	public bool IsDead { get { return dead; } }

	public override void TakeDamage(Vector3 location, Vector3 direction, float damage)
	{
        Debug.Log("中弹了");
		if (boss)
		{
			health -= damage;
			UpdateHealthBar();
			if (health <= 0 && (int)this.transform.localEulerAngles.x == 0)
			{
				Kill();
               
            }
            AudioSource.PlayClipAtPoint(toggleSound, transform.position);
        }
		else if ((int)this.transform.localEulerAngles.x >= -15 && !dead)
		{
            health -= damage;
            if (health <= 0 && (int)this.transform.localEulerAngles.x == 0)
            {
                Kill();
                
            }
            AudioSource.PlayClipAtPoint(toggleSound, transform.position);
        }
	}

	public void Kill()
	{
		if(boss)
			healthBar.parent.gameObject.SetActive(false);
		dead = true;

        if(LinkObj)
        LinkObj.TargetStart();
		//targetRotation.x = -90;
		
	}

	public void Revive()
	{

        health = totalHealth;
        if (boss)
		{
			
			healthBar.parent.gameObject.SetActive(true);
			UpdateHealthBar();
		}
		dead = false;
        if (!isStatic)
            targetRotation.x = 0;
		//AudioSource.PlayClipAtPoint(toggleSound, transform.position);
	}

    public void End()
    {
        TargetEnd();
    }

	private void UpdateHealthBar()
	{
		float scaleFactor = health / totalHealth;

		healthBar.sizeDelta = new Vector2(scaleFactor * originalBarScale, healthBar.sizeDelta.y);
	}
    public override void TargetResets()
    {
        base.TargetResets();
    }

    public override void TargetAwake()
    {
        base.TargetAwake();
        Transform[] allchild;
        allchild = transform.gameObject.GetComponentsInChildren<Transform>();
        switch (targetType)
        {
            case TargetType.Rotate_single:
                foreach (Transform i in allchild)
                {
                    if (i.name == "TargetGourp")
                    {
                        GameObject[] childCount = new GameObject[i.childCount];
                        for (int j = 0; j < i.childCount; j++)
                        {
                            childCount[j] = i.GetChild(j).gameObject;
                        }
                        RotatePos = childCount;
                    }
                }
                break;
            case TargetType.Rotate_double:
                foreach (Transform i in allchild)
                {
                    if (i.name == "TargetGourp")
                    {
                        GameObject[] childCount = new GameObject[i.childCount];
                        for (int j = 0; j < i.childCount; j++)
                        {
                            childCount[j] = i.GetChild(j).gameObject;
                        }
                        RotatePos = childCount;
                    }
                }
                break;
        }
    }

    public override void Hiteffect(Ray ray,RaycastHit raycastHit)
    {
        base.Hiteffect(ray,raycastHit);

        GameObject effect;
        switch (targetType)
        {
            case TargetType.Rotate_single:
                effect = Instantiate(raycastHit.collider.gameObject, raycastHit.collider.gameObject.transform.position, raycastHit.collider.gameObject.transform.rotation);
                effect.GetComponent<Rigidbody>().AddForceAtPosition(ray.direction * raycastHit.distance,raycastHit.point,ForceMode.Impulse);
                raycastHit.collider.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(ray.direction * raycastHit.distance, raycastHit.point, ForceMode.Impulse);



                for (int i = 0; i < raycastHit.collider.gameObject.transform.childCount; i++)
                {
                    Destroy(raycastHit.collider.gameObject.transform.GetChild(i).gameObject);
                }

                raycastHit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                raycastHit.collider.gameObject.GetComponent<MeshCollider>().enabled = false;
                raycastHit.collider.gameObject.GetComponent<Rigidbody>().mass = 0;
                break;
            case TargetType.Rotate_double:
                effect = Instantiate(raycastHit.collider.gameObject, raycastHit.collider.gameObject.transform.position, raycastHit.collider.gameObject.transform.rotation);
                effect.GetComponent<Rigidbody>().AddForceAtPosition(ray.direction * raycastHit.distance, raycastHit.point, ForceMode.Impulse);
                raycastHit.collider.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(ray.direction * raycastHit.distance, raycastHit.point, ForceMode.Impulse);


                for (int i = 0; i < raycastHit.collider.gameObject.transform.childCount; i++)
                {
                    Destroy(raycastHit.collider.gameObject.transform.GetChild(i).gameObject);
                }

                raycastHit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                raycastHit.collider.gameObject.GetComponent<MeshCollider>().enabled = false;
                raycastHit.collider.gameObject.GetComponent<Rigidbody>().mass = 0;
                break;
        }
    }



    public override void TargetStart()
    {
        base.TargetStart();
        StopAllCoroutines();
        Transform[] allchild;
        allchild = transform.gameObject.GetComponentsInChildren<Transform>();
        switch (targetType)
        {          
            case TargetType.Rotate_single:
                foreach (GameObject i in RotatePos)
                {
                    i.GetComponent<MeshRenderer>().enabled = true;
                    i.GetComponent<MeshCollider>().enabled = true;
                    i.GetComponent<Rigidbody>().mass = 1;
                }
                break;
            case TargetType.Rotate_double:
                foreach (GameObject i in RotatePos)
                {
                    i.GetComponent<MeshRenderer>().enabled = true;
                    i.GetComponent<MeshCollider>().enabled = true;
                    i.GetComponent<Rigidbody>().mass = 1;
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
               
                break;
            case TargetType.Slide_double:
               
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
            case TargetType.Rotate_single:
                foreach (GameObject i in RotatePos)
                {
                    i.GetComponent<MeshRenderer>().enabled = true;
                    i.GetComponent<MeshCollider>().enabled = true;
                    i.GetComponent<Rigidbody>().mass = 1;
                }
                break;
            case TargetType.Rotate_double:
                foreach (GameObject i in RotatePos)
                {
                    i.GetComponent<MeshRenderer>().enabled = true;
                    i.GetComponent<MeshCollider>().enabled = true;
                    i.GetComponent<Rigidbody>().mass = 1;
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
            movePos.Rotate(new Vector3(0,Rotate_speed*Time.deltaTime,0));
            yield return null;
        }
    }

    IEnumerator UDMove(Transform movePos)
    {
        while (movePos.localPosition.y > -0.5f)
        {
            movePos.transform.Translate(Vector3.up* -Move_speed *Time.deltaTime );
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
        while (Mathf.Abs(movePos.eulerAngles.z - 300)>2)
        {       
            movePos.Rotate(new Vector3(0,0, -Rotate_speed*2 * Time.deltaTime));           
            yield return null;
        }
        while (Mathf.Abs(movePos.eulerAngles.z - 60) > 2)
        {
            movePos.Rotate(new Vector3(0,0 ,Rotate_speed*2 * Time.deltaTime));
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
}
