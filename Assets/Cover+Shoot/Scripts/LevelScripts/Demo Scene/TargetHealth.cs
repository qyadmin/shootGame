using System.Collections;
using System.Collections.Generic;
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

    private bool getshoot;
    [SerializeField]
    TargetType targetType;

    public TargetType getTargetType
    {
        get
        {
            return targetType;
        }
    }

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
        getshoot = true;
		health = totalHealth;
        TargetAwake();
    }

	void Update ()
	{
		//this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.Euler(targetRotation), 20 * Time.deltaTime);
	}

	public bool IsDead { get { return dead; } }
    public bool IsGetshoot { get { return getshoot; } }
    public class paper_target
    {
        public int A = 0;
        public int C = 0;
        public int D = 0;
        public int Other = 0;
        public int Though = 0;
    }

    List<paper_target> m_target = new List<paper_target>();

    public List<paper_target> get_target_resoult
    {
        get { return m_target; }
    }

    public override void TakeDamage(Ray ray, RaycastHit hit, float damage,bool isthough)
	{
        getshoot = true;
        if (isthough)
            m_target[0].Though++;
        switch (targetType)
        {
            case TargetType.IDPA:
                
                if (hit.collider.gameObject.name == "A")
                    m_target[0].A++;
                if (hit.collider.gameObject.name == "C")
                    m_target[0].C++;
                if (hit.collider.gameObject.name == "D")
                    m_target[0].D++;

                if (m_target[0].A + m_target[0].C + m_target[0].D >= 2)
                    Kill();
                break;
            case TargetType.IPSC:
                if (hit.collider.gameObject.name == "A")
                    m_target[0].A++;
                if (hit.collider.gameObject.name == "C")
                    m_target[0].C++;
                if (hit.collider.gameObject.name == "D")
                    m_target[0].D++;

                if (m_target[0].A + m_target[0].C + m_target[0].D >= 2)
                    Kill();
                break;

            case TargetType.LRMove:
                if (hit.collider.gameObject.name == "A")
                    m_target[0].A++;
                if (hit.collider.gameObject.name == "C")
                    m_target[0].C++;
                if (hit.collider.gameObject.name == "D")
                    m_target[0].D++;

                if (m_target[0].A + m_target[0].C + m_target[0].D >= 2)
                    Kill();
                break;
            case TargetType.UDMove:
                if (hit.collider.gameObject.name == "A")
                    m_target[0].A++;
                if (hit.collider.gameObject.name == "C")
                    m_target[0].C++;
                if (hit.collider.gameObject.name == "D")
                    m_target[0].D++;

                if (m_target[0].A + m_target[0].C + m_target[0].D >= 2)
                    Kill();
                break;

            case TargetType.Rotate_single:
                m_target[0].Other++;
                if (m_target[0].Other >= 6)
                    Kill();                
                break;
            case TargetType.Rotate_double:
                m_target[0].Other++;
                if (m_target[0].Other >= 8)
                    Kill();
                break;
            case TargetType.Slide_single:
                if (hit.collider.gameObject.name == "A")
                    m_target[0].A++;
                if (hit.collider.gameObject.name == "C")
                    m_target[0].C++;
                if (hit.collider.gameObject.name == "D")
                    m_target[0].D++;

                if (m_target[0].A + m_target[0].C + m_target[0].D >= 2)
                    Kill();
                break;
            case TargetType.Slide_double:
                if (hit.collider.gameObject.name == "A1")
                    m_target[0].A++;
                if (hit.collider.gameObject.name == "C1")
                    m_target[0].C++;
                if (hit.collider.gameObject.name == "D1")
                    m_target[0].D++;

                if (hit.collider.gameObject.name == "A2")
                    m_target[1].A++;
                if (hit.collider.gameObject.name == "C2")
                    m_target[1].C++;
                if (hit.collider.gameObject.name == "D2")
                    m_target[1].D++;

                if ((m_target[0].A + m_target[0].C + m_target[0].D >= 2)&& (m_target[1].A + m_target[1].C + m_target[1].D >= 2))
                    Kill();
                break;
            default:
                m_target[0].Other++;
                if (m_target[0].Other >= 1)
                    Kill();
                break;

        }

  //      Debug.Log("中弹了");
		//if (boss)
		//{
		//	health -= damage;
		//	UpdateHealthBar();
		//	if (health <= 0 && (int)this.transform.localEulerAngles.x == 0)
		//	{
		//		Kill();              
  //          }
  //          AudioSource.PlayClipAtPoint(toggleSound, transform.position);
  //      }
		//else if ((int)this.transform.localEulerAngles.x >= -15 && !dead)
		//{
  //          health -= damage;
  //          if (health <= 0 && (int)this.transform.localEulerAngles.x == 0)
  //          {
  //              Kill();
                
  //          }
  //          AudioSource.PlayClipAtPoint(toggleSound, transform.position);
  //      }
	}

	public void Kill()
	{
		if(boss)
			healthBar.parent.gameObject.SetActive(false);
		dead = true;
        if(LinkObj)
        LinkObj.TargetLink();
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
        getshoot = false;
        if (!isStatic)
            targetRotation.x = 0;

        ResetTarget();
        //AudioSource.PlayClipAtPoint(toggleSound, transform.position);
    }

    public void ResetTarget()
    {
        if (targetType == TargetType.Slide_double)
        {
            paper_target target1 = new paper_target();
            paper_target target2 = new paper_target();
            m_target.Clear();
            m_target.Add(target1);
            m_target.Add(target2);
        }
        else
        {
            paper_target target1 = new paper_target();
            m_target.Clear();
            m_target.Add(target1);
        }

        if (ProhibitShooting || InvalidItem)
            Kill();
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
            case TargetType.BarSteel:

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

                EffectGroup.Add(effect);

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

                EffectGroup.Add(effect);

                for (int i = 0; i < raycastHit.collider.gameObject.transform.childCount; i++)
                {
                    Destroy(raycastHit.collider.gameObject.transform.GetChild(i).gameObject);
                }

                raycastHit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                raycastHit.collider.gameObject.GetComponent<MeshCollider>().enabled = false;
                raycastHit.collider.gameObject.GetComponent<Rigidbody>().mass = 0;
                break;
            case TargetType.BarSteel:
                effect = Instantiate(raycastHit.collider.gameObject, raycastHit.collider.gameObject.transform.position, raycastHit.collider.gameObject.transform.rotation);
                Destroy(effect.GetComponent<TargetHealth>());                
                effect.AddComponent<Rigidbody>().AddForceAtPosition(ray.direction * raycastHit.distance, raycastHit.point, ForceMode.Impulse);

                EffectGroup.Add(effect);

                for (int i = 0; i < raycastHit.collider.gameObject.transform.childCount; i++)
                {
                    Destroy(raycastHit.collider.gameObject.transform.GetChild(i).gameObject);
                }

                raycastHit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                raycastHit.collider.gameObject.GetComponent<MeshCollider>().enabled = false;

                break;
            case TargetType.SliceSteel:               
                foreach (Transform i in transform)
                {
                    if (i.name == "movePos")
                    {
                        IEnumerator slicesteel = SliceSteel(i);
                        StartCoroutine(slicesteel);
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
                //foreach (GameObject i in RotatePos)
                //{
                //    i.GetComponent<MeshRenderer>().enabled = true;
                //    i.GetComponent<MeshCollider>().enabled = true;
                //    i.GetComponent<Rigidbody>().mass = 1;
                //}
                break;
            case TargetType.Rotate_double:
                //foreach (GameObject i in RotatePos)
                //{
                //    i.GetComponent<MeshRenderer>().enabled = true;
                //    i.GetComponent<MeshCollider>().enabled = true;
                //    i.GetComponent<Rigidbody>().mass = 1;
                //}
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
                        IEnumerator slide = Slide(i);
                        StartCoroutine(slide);
                    }
                }
                break;
            case TargetType.Slide_double:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        IEnumerator slide = Slide(i);
                        StartCoroutine(slide);
                    }
                }
                break;
            case TargetType.BarSteel:
                //foreach (GameObject i in RotatePos)
                //{
                //    i.GetComponent<MeshRenderer>().enabled = true;
                //    i.GetComponent<MeshCollider>().enabled = true;
                //}
                break;


        }
    }

    public void TargetEnd()
    {
        StopAllCoroutines();
        Transform[] allchild;

        allchild = transform.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < allchild.Length; i++)
        {
            if (allchild[i].name == "BulletHole")
                Destroy(allchild[i].gameObject);
        }
        switch (targetType)
        {
            case TargetType.Rotate_single:
                foreach (GameObject i in RotatePos)
                {
                    i.GetComponent<MeshRenderer>().enabled = true;
                    i.GetComponent<MeshCollider>().enabled = true;
                    i.GetComponent<Rigidbody>().mass = 1;
                }
                for (int i = 0; i < EffectGroup.Count; i++)
                {
                    Destroy(EffectGroup[i].gameObject);
                }
                EffectGroup.Clear();
                break;
            case TargetType.Rotate_double:
                foreach (GameObject i in RotatePos)
                {
                    i.GetComponent<MeshRenderer>().enabled = true;
                    i.GetComponent<MeshCollider>().enabled = true;
                    i.GetComponent<Rigidbody>().mass = 1;
                }
                for (int i = 0; i < EffectGroup.Count; i++)
                {
                    Destroy(EffectGroup[i].gameObject);
                }
                EffectGroup.Clear();
                break;
            case TargetType.BarSteel:               
                    transform.GetComponent<MeshRenderer>().enabled = true;
                    transform.GetComponent<MeshCollider>().enabled = true;
                for (int i = 0; i < EffectGroup.Count; i++)
                {
                    Destroy(EffectGroup[i].gameObject);
                }
                EffectGroup.Clear();
                break;
            case TargetType.SliceSteel:             
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        i.localEulerAngles = new Vector3(0,0,0);
                    }
                }
                break;
            case TargetType.Slide_single:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        i.localPosition = new Vector3(-2.556f, 1.149f, -0.07200003f);
                    }
                }
                break;
            case TargetType.Slide_double:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        i.localPosition = new Vector3(-2.556f, 1.149f, -0.07200003f);
                    }
                }
                break;
            case TargetType.UDMove:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        i.localPosition = new Vector3(-0.0317173f, -0.7f, -0.08000182f);
                    }
                }
                break;
            case TargetType.LRMove:
                foreach (Transform i in allchild)
                {
                    if (i.name == "movePos")
                    {
                        i.localPosition = new Vector3(-0.0317173f, -0.7f, -0.08000182f);
                        i.localEulerAngles = new Vector3(0,0,0);
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
            movePos.Rotate(new Vector3(0,Rotate_speed*Time.deltaTime,0));
            yield return null;
        }
    }

    IEnumerator UDMove(Transform movePos)
    {
        while (movePos.localPosition.y > -0.7f)
        {
            movePos.transform.Translate(Vector3.up* -Move_speed*(1- Mathf.Abs(-0.1f - movePos.localPosition.y)*1.5f)*3 *Time.deltaTime );
            yield return null;
        }
        while (movePos.localPosition.y < 0.5f)
        {
            movePos.transform.Translate(Vector3.up * Move_speed * (1 - Mathf.Abs(-0.1f - movePos.localPosition.y)*1.5f)*3 * Time.deltaTime);
            yield return null;
        }
        IEnumerator udmove = UDMove(movePos);
        StartCoroutine(udmove);
    }

    IEnumerator LRMove(Transform movePos)
    {
        while (movePos.localPosition.y < 0.1f)
        {
            movePos.transform.Translate(Vector3.up * Move_speed *3 * Time.deltaTime);
            yield return null;
        }

        while (Mathf.Abs(movePos.eulerAngles.z - 300)>2)
        {
            //Debug.Log((1 - ((360 - movePos.eulerAngles.z) > 100 ? Mathf.Abs((360 - movePos.eulerAngles.z) - 360) : (360 - movePos.eulerAngles.z)) / 65));
            movePos.Rotate(new Vector3(0,0, -Rotate_speed*5*(1-((360- movePos.eulerAngles.z)>100? Mathf.Abs((360 - movePos.eulerAngles.z)-360): (360 - movePos.eulerAngles.z)) /65)* Time.deltaTime));           
            yield return null;
        }
        while (Mathf.Abs(movePos.eulerAngles.z - 60) > 2)
        {
            //Debug.Log((1 - ((360 - movePos.eulerAngles.z) > 100 ? Mathf.Abs((360 - movePos.eulerAngles.z) - 360) : (360 - movePos.eulerAngles.z)) / 65));

            movePos.Rotate(new Vector3(0,0 ,Rotate_speed*5* (1 - ((360 - movePos.eulerAngles.z) > 100 ? Mathf.Abs((360 - movePos.eulerAngles.z) - 360) : (360 - movePos.eulerAngles.z)) / 65) * Time.deltaTime));
            yield return null;
        }
        IEnumerator lrmove = LRMove(movePos);
        StartCoroutine(lrmove);
    }

    IEnumerator Slide(Transform movePos)
    {
        float speed = 0;
        while (movePos.localPosition.x < 2.6f)
        {          
            movePos.transform.Translate(Vector3.right *speed* Time.deltaTime);
            speed += 0.03f;
            yield return null;
        }
        //while (movePos.localPosition.x > -2.56)
        //{
        //    movePos.transform.Translate(Vector3.left * Move_speed*3 * Time.deltaTime);
        //    yield return null;
        //}
        //IEnumerator slider = Slide(movePos);
        //StartCoroutine(slider);
    }

    IEnumerator SliceSteel(Transform movePos)
    {
        while (Mathf.Abs(movePos.localEulerAngles.x - 265) > 15)
        {
            Debug.Log(movePos.localEulerAngles.x);
            movePos.Rotate(new Vector3(-Rotate_speed * 10 * Time.deltaTime, 0, 0));
            yield return null;
        }     
    }
}
