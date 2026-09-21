using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;          // How fast the bullet travels
    public float lifetime = 3f;        // How long before it destroys itself
    public int damage = 1;             // How much damage it does to enemies

    private Rigidbody2D rb;
    public AudioClip[] clips;
    public GameObject bulletImpact;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Make sure the bullet moves forward
        rb.linearVelocity = transform.right * speed * transform.localScale.x;

        // Destroy the bullet after its lifetime expires
        Destroy(gameObject, lifetime);
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.name == "GodHitbox")
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
           
        // Example: check if the bullet hits an enemy
        if (other.CompareTag("Enemy"))
        {
            GameObject bullet2 = Instantiate(bulletImpact, transform.position, transform.rotation);
            Vector3 scale = bullet2.transform.localScale;
            scale.x *= transform.localScale.x;
            bullet2.transform.localScale = scale;     
            // Example damage system (requires enemy script)
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null){
                enemy.TakeDamage(damage);
                bullet2.GetComponent<AudioSource>().clip = clips[2];
                bullet2.GetComponent<Animator>().Play("BloodHit");
                Destroy(bullet2, 1);
            }  
            else{
                EnemyCar enemyCar = other.GetComponent<EnemyCar>();
                if (enemyCar != null)
                    enemyCar.TakeDamage(damage);
                bullet2.GetComponent<Animator>().Play("BulletHole");
                bullet2.transform.parent = other.transform;
            }
            bullet2.GetComponent<AudioSource>().Play();
            Destroy(gameObject); // Destroy bullet on impact
            
        }
        else if (other.CompareTag("Player"))
        {
            // Example damage system (requires enemy script)
            //
            PlayerMovement2D player = other.GetComponent<PlayerMovement2D>();
            if (player != null)
            {
                print("playerhit");
                player.takeDmg();
            }
            GameObject bullet2 = Instantiate(bulletImpact, transform.position, transform.rotation);
            Vector3 scale = bullet2.transform.localScale;
            scale.x *= transform.localScale.x;
            bullet2.transform.localScale = scale;
            bullet2.GetComponent<AudioSource>().clip = clips[2];
            bullet2.GetComponent<AudioSource>().Play();
            bullet2.GetComponent<Animator>().Play("BloodHit");
            Destroy(bullet2, 1);
            Destroy(gameObject); // Destroy bullet on impact
        }
        else if (other.gameObject.GetComponentInParent<breakableWall>())
        {
            // Example damage system (requires enemy script)
            breakableWall bw = other.GetComponent<breakableWall>();
            if (bw != null)
                bw.TakeDamage(damage);
            GameObject bullet2 = Instantiate(bulletImpact, transform.position, transform.rotation);
            Vector3 scale = bullet2.transform.localScale;
            scale.x *= transform.localScale.x;
            bullet2.transform.localScale = scale;
            bullet2.GetComponent<AudioSource>().clip = clips[Random.Range(0, 2)];
            bullet2.GetComponent<AudioSource>().Play();
            Destroy(bullet2, 1);
            Destroy(gameObject); // Destroy bullet on impact
        }
        // Optionally destroy bullet on hitting walls
        else if (!other.CompareTag("Sign"))
        {
            GameObject bullet2 = Instantiate(bulletImpact, transform.position, transform.rotation);
            Vector3 scale = bullet2.transform.localScale;
            scale.x *= transform.localScale.x;
            bullet2.transform.localScale = scale;
            bullet2.GetComponent<AudioSource>().clip = clips[Random.Range(0, 2)];
            bullet2.GetComponent<AudioSource>().Play();
            Destroy(bullet2, 1);
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject bullet2 = Instantiate(bulletImpact, transform.position, transform.rotation);
            Vector3 scale = bullet2.transform.localScale;
            scale.x *= transform.localScale.x;
            bullet2.transform.localScale = scale;
            bullet2.GetComponent<AudioSource>().clip = clips[Random.Range(0, 2)];
            bullet2.GetComponent<AudioSource>().Play();
            Destroy(bullet2, 1);
            Destroy(gameObject);
        }
            
    }
}