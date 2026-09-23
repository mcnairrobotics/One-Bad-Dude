using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public int health;
    public bool dead;
    private Animator anim;
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;

    [Header("Detection Settings")]
    public float visionRange = 8f;
    public float yVisionRange = 2f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 1.5f;
    public float bulletSpeed = 10f;
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundLayer;
    private bool isGrounded;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Transform player;
    private float nextDirectionChangeTime;
    private float nextShootTime;
    private AudioSource audio;
    public AudioClip[] clips;
    public Transform Eyes;
    public bool inMyMeleeRange;
    public bool hasMeleeAttack;
    public bool disableParentAnimator;
    public float deathTime = 1;
    private bool playerInSight = false;
    public string sceneToSwitch;
    public bool alwaysLookTowardsPlayer;
    public UnityEngine.Rendering.Universal.Light2D spriteLight;
    public SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        audio = gameObject.GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        PickRandomDirection();
        if(PlayerPrefs.GetInt("Difficulty") != 0 && health < 6)
            health *= Random.Range(1, 3);
        else if(PlayerPrefs.GetInt("Difficulty") == 1)
        {
            health *= 3;
            health /= 2;
        }
        else if(PlayerPrefs.GetInt("Difficulty") == 2)
        {
            health *= 2;
        }
    }
    void Update()
    {
        if(spriteLight != null)
            spriteLight.m_LightCookieSprite = sr.sprite;
        if (dead) return;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }
        if (alwaysLookTowardsPlayer)
        {
            if (Time.time >= nextDirectionChangeTime)
                PickRandomDirection();
        }
        // isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        anim.SetBool("Falling", !isGrounded);
        if (!isGrounded)
        {
            // Don’t try to walk while in air
            rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
            return;
        }
        DetectPlayer();

        if (playerInSight)
        {
            // Stop walking when attacking
            rb.linearVelocity = Vector2.zero;
            //FacePlayer();
            
            anim.SetTrigger("Attack");
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            //anim.Play("ThugAttack");
            /*if (Time.time >= nextShootTime)
            {
                ShootAtPlayer();
                nextShootTime = Time.time + shootCooldown;
            }*/
        }
        else
        {
            // Wander randomly
            if (Time.time >= nextDirectionChangeTime)
                PickRandomDirection();

            rb.linearVelocity = moveDirection * moveSpeed;
            anim.SetFloat("Speed", Mathf.Abs(moveDirection.x));
        }
    }

    void PickRandomDirection()
    {
        if (alwaysLookTowardsPlayer)
        {
            nextDirectionChangeTime = Time.time + changeDirectionTime;
            if(player.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            moveDirection = new Vector2(Random.Range(-1f, 1f),  rb.linearVelocity.y).normalized;
            nextDirectionChangeTime = Time.time + changeDirectionTime;
            if (moveDirection.x > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (moveDirection.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
        
    }

    void DetectPlayer()
    {
        Vector2 dirToPlayer = (player.position - Eyes.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);
        float yDistance = Mathf.Abs(transform.position.y - player.position.y);
        if(distance < 5f)
        {
            player.GetComponent<PlayerMovement2D>().Melee = true;
            inMyMeleeRange = true;
            if(hasMeleeAttack)
                anim.SetTrigger("Melee");
        }
        else
        {
            if(inMyMeleeRange == true)
                player.GetComponent<PlayerMovement2D>().Melee = false;
            inMyMeleeRange = false;
        }
        if (distance <= visionRange && yDistance < yVisionRange)
        {
            //print("within range");
            // Optional: add line of sight (no walls in the way)
            RaycastHit2D hit = Physics2D.Raycast(Eyes.position, dirToPlayer, visionRange, ~obstacleLayer);
            if (hit && hit.collider.CompareTag("Player"))
            {
                //print("spottted player");
                playerInSight = true;
                return;
            }
            
        }
        playerInSight = false;
    }

    void FacePlayer()
    {
        Vector2 lookDir = player.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void ShootAtPlayer()
    {
        if (bulletPrefab == null || firePoint == null) return;
        audio.PlayOneShot(clips[0]);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Vector3 scale = bullet.transform.localScale;
        scale.x *= transform.localScale.x;
        bullet.transform.localScale = scale;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Danger") && dead == false)
        {
            TakeDamage(3);
        }
        if (collision.gameObject.CompareTag("DangerSmall") && dead == false)
        {
            TakeDamage(1);
        }
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Melee")
        {
            audio.PlayOneShot(clips[5]);
            TakeDamage(1);
        }
        if(other.gameObject.name == "GodHitbox")
        {
            audio.PlayOneShot(clips[5]);
            TakeDamage(1);
        }

    }
    void OnDrawGizmosSelected()
    {
        // Draw vision range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health < 1 && dead == false)
        {
            player.GetComponent<PlayerMovement2D>().kills += 1;
            dead = true;
            audio.PlayOneShot(clips[Random.Range(2,5)]);
            anim.Play("ThugDie");
            if(disableParentAnimator)
                transform.parent.GetComponent<Animator>().enabled = false;
            if(inMyMeleeRange == true)
                player.GetComponent<PlayerMovement2D>().Melee = false;
            inMyMeleeRange = false;
            if(sceneToSwitch != "")
                Invoke("NewScene", deathTime);
            else
                Destroy(gameObject, deathTime);
        }
        else if(dead == false)
        {
            audio.PlayOneShot(clips[6]);
            anim.Play("ThugPain");
        }
    }
    void NewScene()
    {
        SceneManager.LoadScene(sceneToSwitch);
    }
}
