using UnityEngine;

public class breakableWall : MonoBehaviour
{
    public int health = 1;
    private Transform player;
    private bool inMyMeleeRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(health > 0)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        if(health < 1) return;
        float distance = Vector2.Distance(transform.position, player.position);
        if(distance < 4f)
        {
            player.GetComponent<PlayerMovement2D>().Melee = true;
            inMyMeleeRange = true;
        }
        else
        {
            if(inMyMeleeRange == true)
                player.GetComponent<PlayerMovement2D>().Melee = false;
            inMyMeleeRange = false;
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health < 1)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponentInChildren<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponentInChildren<ParticleSystem>().Play();
            if(inMyMeleeRange == true)
                player.GetComponent<PlayerMovement2D>().Melee = false;
            inMyMeleeRange = false;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Melee")
        {
            TakeDamage(1);
        }
        if(other.gameObject.name == "GodHitbox")
        {
            TakeDamage(2);
        }

    }
}
