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
	}

	void Update ()
	{
		//this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.Euler(targetRotation), 20 * Time.deltaTime);
	}

	public bool IsDead { get { return dead; } }

	public override void TakeDamage(Vector3 location, Vector3 direction, float damage)
	{
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

	private void UpdateHealthBar()
	{
		float scaleFactor = health / totalHealth;

		healthBar.sizeDelta = new Vector2(scaleFactor * originalBarScale, healthBar.sizeDelta.y);
	}
}
