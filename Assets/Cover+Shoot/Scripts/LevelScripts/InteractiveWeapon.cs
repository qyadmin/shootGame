using UnityEngine;
using UnityEngine.UI;

// This class corresponds to any in-game weapon interactions.
public class InteractiveWeapon : MonoBehaviour
{
	public string label;                                      // 武器名称。无论游戏对象的名称如何，相同的名称将把武器视为相同的。
    public AudioClip shotSound, reloadSound,                  // 用于拍摄和重新加载的音频剪辑
        pickSound, dropSound;                                 // 选择和放下武器的音频剪辑。
    public Sprite sprite;                                     // 武器srpite显示在屏幕上的HUD。
    public Vector3 rightHandPosition;                         // 相对于球员右手的位置偏移量。
    public Vector3 relativeRotation;                          // 相对于玩家右手的旋转偏移量。
    public float bulletDamage = 10f;                          // 一枪伤害。
    public float recoilAngle;                                 // 武器反冲角度。
    public enum WeaponType                                    // 武器类型，与玩家的射击动画相关。
    {
		NONE,
		SHORT,
		LONG
	}
	public enum WeaponMode                                    // Weapon shooting modes.
	{
		SEMI,
		BURST,
		AUTO
	}
	public WeaponType type = WeaponType.NONE;                 // Default weapon type, change in Inspector.
	public WeaponMode mode = WeaponMode.SEMI;                 // Default weapon mode, change in Inspector.
	public int burstSize = 0;                                 // How many shot are fired on burst mode.
	[SerializeField]
	private int mag, totalBullets;                            // Current mag capacity and total amount of bullets being carried.
	private int fullMag, maxBullets;                          // Default mag capacity and total bullets for reset purposes.
	private GameObject player, gameController;                // References to the player and the game controller.
	private ShootBehaviour playerInventory;                   // Player's inventory to store weapons.
	private SphereCollider interactiveRadius;                 // In-game radius of interaction with player.
	private BoxCollider col;                                  // Weapon collider.
	private Rigidbody rbody;                                  // Weapon rigidbody.
	private WeaponUIManager weaponHud;                        // Reference to on-screen weapon HUD.
	private bool pickable;                                    // Boolean to store whether or not the weapon is pickable (player within radius).
	private Transform pickupHUD;                              // Reference to the weapon pickup in-game label.

	void Awake()
	{
		// Set up the references.
		this.gameObject.name = this.label;
		this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		foreach (Transform t in this.transform)
		{
			t.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
		player = GameObject.FindGameObjectWithTag("Player");
		playerInventory = player.GetComponent<ShootBehaviour>();
		gameController = GameObject.FindGameObjectWithTag("GameController");
		// Assert that exists a on-screen HUD.
		if (GameObject.Find("ScreenHUD") == null)
		{
			Debug.LogError("No ScreenHUD canvas found. Create ScreenHUD inside the GameController");
		}
		weaponHud = GameObject.Find("ScreenHUD").GetComponent<WeaponUIManager>();
		pickupHUD = gameController.transform.Find("PickupHUD");

		// Create physics components and radius of interaction.
		col = this.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
		CreateInteractiveRadius(col.center);
		this.rbody = this.gameObject.AddComponent<Rigidbody>();

		// Assert that an weapon slot is set up.
		if (this.type == WeaponType.NONE)
		{
			Debug.LogWarning("Set correct weapon slot ( 1 - small/ 2- big)");
			type = WeaponType.SHORT;
		}

		// Assert that the gun muzzle is exists.
		if(!this.transform.Find("muzzle"))
		{
			Debug.LogError(this.name+" muzzle is not present. Create a game object named 'muzzle' as a child of this game object");
		}

		// Set default values.
		fullMag = mag;
		maxBullets = totalBullets;
		pickupHUD.gameObject.SetActive(false);
	}

	// Create the sphere of interaction with player.
	private void CreateInteractiveRadius(Vector3 center)
	{
		interactiveRadius = this.gameObject.AddComponent<SphereCollider>();
		interactiveRadius.center = center;
		interactiveRadius.radius = 1f;
		interactiveRadius.isTrigger = true;
	}

	void Update()
	{
		// Handle player pick weapon action.
		if (this.pickable && Input.GetButtonDown(playerInventory.pickButton))
		{
			// Disable weapon physics.
			rbody.isKinematic = true;
			this.col.enabled = false;
			
			// Setup weapon and add in player inventory.
			playerInventory.AddWeapon(this);
			Destroy(interactiveRadius);
			this.Toggle(true);
			this.pickable = false;

			// Change active weapon HUD.
			TooglePickupHUD(false);
		}
	}


    public void Addweapon()
    {
            rbody.isKinematic = true;
            this.col.enabled = false;

            // Setup weapon and add in player inventory.
            playerInventory.AddWeapon(this);
            Destroy(interactiveRadius);
            this.Toggle(true);
            this.pickable = false;

            // Change active weapon HUD.
            TooglePickupHUD(false);        
    }

	// Handle weapon collision with environment.
	private void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.gameObject != player && Vector3.Distance(transform.position, player.transform.position) <= 5f)
		{
			AudioSource.PlayClipAtPoint(dropSound, transform.position, 0.5f);
		}
	}

	// Handle player exiting radius of interaction.
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == player)
		{
			pickable = false;
			TooglePickupHUD(false);
		}
	}

	// Handle player within radius of interaction.
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject == player && playerInventory && playerInventory.isActiveAndEnabled)
		{
			pickable = true;
			TooglePickupHUD(true);
		}
	}

	// Draw in-game weapon pickup label.
	private void TooglePickupHUD(bool toogle)
	{
		pickupHUD.gameObject.SetActive(toogle);
		if (toogle)
		{
			pickupHUD.position = this.transform.position + Vector3.up * 0.5f;
			Vector3 direction = player.GetComponent<BasicBehaviour>().playerCamera.forward;
			direction.y = 0f;
			pickupHUD.rotation = Quaternion.LookRotation(direction);
			pickupHUD.Find("Label").GetComponent<Text>().text = "Pick "+this.gameObject.name;
		}
	}

	// Manage weapon active status.
	public void Toggle(bool active)
	{
		if (active)
			AudioSource.PlayClipAtPoint(pickSound, transform.position, 0.5f);
		weaponHud.Toggle(active);
		UpdateHud();
	}

	// Manage the drop action.
	public void Drop()
	{
		this.gameObject.SetActive(true);
		this.transform.position += Vector3.up;
		rbody.isKinematic = false;
		this.transform.parent = null;
		CreateInteractiveRadius(col.center);
		this.col.enabled = true;
		weaponHud.Toggle(false);
	}

	// Start the reload action (called by shoot behaviour).
	public bool StartReload()
	{
		if (mag == fullMag || totalBullets == 0)
			return false;
		else if(totalBullets < fullMag - mag)
		{
			mag += totalBullets;
			totalBullets = 0; 
		}
		else
		{
			totalBullets -= fullMag - mag;
			mag = fullMag;
		}

		return true;
	}

	// End the reload action (called by shoot behaviour).
	public void EndReload()
	{
		UpdateHud();
	}

	// Manage shoot action.
	public bool Shoot()
	{
		if (mag > 0)
		{
			mag--;
			UpdateHud();
			return true;
		}
		return false;
	}

	// Reset the bullet parameters.
	public void ResetBullets()
	{
		mag = fullMag;
		totalBullets = maxBullets;
	}

	// Update weapon screen HUD.
	private void UpdateHud()
	{
		weaponHud.UpdateWeaponHUD(sprite, mag, fullMag, totalBullets);
	}
}
