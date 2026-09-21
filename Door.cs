using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject thugToSpawn;
    public GameObject gunnerToSpawn;
    public int gunnerSpawnChance;
    private Transform player;
    public float spawnDist;
    public int randomChanceToSpawn = 5;
    private Animator anim;
    public Transform spawnPoint;
    private PlayerMovement2D pm;
    private bool playerInDoor = false;
    public bool avaliable = false;
    public bool giveAmmo = true;
    public bool giveHealth = false;
    public bool giveMagic = false;
    public bool giveMachinegun;
    public GameObject magicLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pm = player.gameObject.GetComponent<PlayerMovement2D>();
        if(PlayerPrefs.GetInt("Difficulty") == 2)
        {
            randomChanceToSpawn += 1;
            gunnerSpawnChance += 1;
        }
        InvokeRepeating("CheckToSpawn", .5f, .5f);
    }

    void CheckToSpawn()
    {
        if(pm.Invulnerable)
            return;
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < spawnDist && distance > 3 && Random.Range(0, 10) < randomChanceToSpawn && anim.GetCurrentAnimatorStateInfo(0).IsName("DoorChill"))
        {
            if(Random.Range(0,10) < gunnerSpawnChance)
                anim.Play("DoorGunner");
            else
                anim.Play("Door");
        }
    }
    void Update()
    {
        if(pm.Invulnerable)
            anim.Play("DoorChill");
        if(avaliable == false) return;
        float distance = Vector2.Distance(transform.position, player.position);
        float upDownInput = Input.GetAxisRaw("Vertical");
        if(playerInDoor == false && pm.isGrounded == true && distance < 1.5f && upDownInput > 0 && pm.idling == true)
        {
            //playerBait.SetActive(true)
            playerInDoor = true;
            if(giveAmmo == true)
            {
                giveAmmo = false;
                if(PlayerPrefs.GetInt("Difficulty") == 2)
                {
                    pm.ammo += Random.Range(4, 10);
                }
                else
                {
                    pm.ammo += Random.Range(5, 12);
                }
                gameObject.GetComponent<AudioSource>().Play();
            }
            if(giveHealth == true && pm.health < 3){
                giveHealth = false;
                pm.health += 1;
                gameObject.GetComponent<AudioSource>().Play();
            }
            if(giveMagic == true){
                giveMagic = false;
                pm.magic += 2;
                gameObject.GetComponent<AudioSource>().Play();
            }
            if(giveMachinegun == true){
                giveMachinegun = false;
                pm.machinegunPowerup();
                gameObject.GetComponent<AudioSource>().Play();
            }
            pm.Invoke("AmmoUpdate", 0.15f);
            player.gameObject.SetActive(false);
            anim.Play("DoorOpenPlayer");
            
        }else if(playerInDoor == true && upDownInput < 0)
        {
            anim.Play("DoorPlayer");
        }
    }
    public void SpawnDude(int type)
    {
        if(type == 0)
            Instantiate(thugToSpawn, spawnPoint.position, spawnPoint.rotation);
        if(type == 1)
            Instantiate(gunnerToSpawn, spawnPoint.position, spawnPoint.rotation);
        if(type == -1)
        {
            playerInDoor = false;
            player.gameObject.SetActive(true);
            player.position = new Vector3(transform.position.x, transform.position.y, player.position.z);
            if(magicLight != null)
                magicLight.SetActive(false);

        }
    }
}
