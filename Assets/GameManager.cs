using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance;
    public bool timeTrial;

    [SerializeField]
    GameObject door;

    private GameObject player;

    private void Awake()
    {
        _Instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
    }


    private void Start()
    {
        StartCoroutine(delate());
    }
    private void StartGame()
   {
        timeTrial = true;
        player.GetComponent<CapsuleCollider>().enabled = true;
        door.GetComponent<Animator>().SetTrigger("open");
        door.GetComponent<AudioSource>().Play();
   }

    IEnumerator delate()
    {
        yield return new WaitForSeconds(5);
        StartGame();
    }
}
