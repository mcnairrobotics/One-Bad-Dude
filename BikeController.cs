using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;
using Unity.VisualScripting;

[RequireComponent(typeof(SpriteRenderer))]
public class BikeController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float scaleSpeed = 8f;
    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite forwardSprite;
    public Sprite backwardSprite;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite[] specialSprites;
    public int specialSpriteNumber;
    public bool aimingBack;

    private SpriteRenderer sr;
    public SpriteRenderer shadowRender;
    private Rigidbody2D rb;
    private Vector2 movement;
    private float startingScale;
    private float startingYpos;
    public GameObject lightsource1;
    public GameObject lightsource2;
    private Animator animator;
    private AudioSource audio;
    public AudioClip[] clips;
    public int ammo;
    private int clipAmmo;
    public int clipSize;
    public TMPro.TMP_Text ammoText;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public Color hitColor;
    private Color normalColor;
    public GameObject flash;
    public bool Invulnerable = false;
    bool canBeHit = true;
    bool dead = false;
    public int health = 3;
    public GameObject[] healthIcons;
    private InputManager input;

    void Awake()
    {
        input = GetComponent<InputManager>();
        animator = GetComponent<Animator>();
        audio = gameObject.GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        startingYpos = transform.position.y;
        startingScale = transform.localScale.y;
        clipAmmo = clipSize;
    }
    void Start() {
        flash.SetActive(true);
        normalColor = sr.color;
    }

    void Update()
    {
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (health > i)
                healthIcons[i].SetActive(true);
            else
                healthIcons[i].SetActive(false);
        }
        if(dead)
        {
            movement.x = -1;
            movement.y = 0;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            UpdateSprite();
            return;
        }
        movement.x = input.Move.x;
        movement.y = input.Move.y;
        ammoText.text = "Clip " + clipAmmo +"i" +clipSize+ "\nAmmo " + ammo;
        if (input.FirePressed)
        {
            if(ammo < 0)
                ammo = 0;
            if (ammo > 0 && clipAmmo > 0)
            {
                animator.SetTrigger("Shooting");
            }
            else
            {
                PlayClip(2);
            }
            // moveInput = 0;
        }
        else if (input.ReloadPressed)//reload
        {
            if(ammo > 0)
            {
                animator.SetTrigger("Reload");
                PlayClip(1);
                if(ammo < clipSize)
                    clipAmmo = ammo;
                else
                    clipAmmo = clipSize;
            }
            
            // moveInput = 0;
        }
        UpdateSprite();
        input.ClearFrameInput();
    }

    void FixedUpdate()
    {
        
        Vector2 normalized = movement.normalized;
        normalized.x *= 2;
        if(dead)
            normalized.x *= 3;
        if(aimingBack == true)
            normalized /= 2;
        rb.MovePosition(rb.position + normalized * moveSpeed * Time.fixedDeltaTime);
        transform.localScale = new Vector3(startingScale - scaleSpeed*(transform.position.y - startingYpos), startingScale - scaleSpeed*(transform.position.y - startingYpos), transform.localScale.z);
        sr.sortingOrder = 38-(int)(transform.position.y*5);
    }

    void UpdateSprite()
    {
        shadowRender.sprite = sr.sprite;
        if(aimingBack == true)
        {
            sr.sprite = specialSprites[specialSpriteNumber];
            lightsource2.SetActive(false);
            lightsource1.SetActive(true);
        }
        else if (movement == Vector2.zero)
        {
            sr.sprite = idleSprite;
            lightsource2.SetActive(false);
            lightsource1.SetActive(true);
            return;
        }
        
        // Prioritize whichever direction has the larger input
        else if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            lightsource2.SetActive(false);
            lightsource1.SetActive(true);
            if (movement.x > 0)
                sr.sprite = forwardSprite;
            else
                sr.sprite = backwardSprite;
        }
        else
        {
            if (movement.y < 0)
            {
                sr.sprite = downSprite;
                lightsource2.SetActive(true);
                lightsource1.SetActive(false);
            }
            else
            {
                sr.sprite = upSprite;
                lightsource2.SetActive(false);
                lightsource1.SetActive(true);
            }
        }
    }
    public void PlayClip(int clipNum)
    {
        if (clipNum == 0)
        {
            ammo -= 1;
            clipAmmo -= 1;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Vector3 scale = bullet.transform.localScale;
            scale.x *= transform.localScale.x;
            bullet.transform.localScale = scale;
        }
        audio.PlayOneShot(clips[clipNum]);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Hitbox"){
            takeDmg();
        }
    }
    public void takeDmg()
    {
        if(canBeHit && !Invulnerable)
            StartCoroutine("takeDamage");
    }
    IEnumerator takeDamage()
    {
        canBeHit = false;
        if (!dead)
        {
           health -= 1;
            animator.Play("Pain");
            audio.PlayOneShot(clips[3]);
            if (health < 1)
            {
                dead = true;
                animator.Play("Die");
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            } 
        }
        sr.color = hitColor;
        yield return new WaitForSeconds(0.6f / (PlayerPrefs.GetInt("Difficulty")*2+1));
        canBeHit = true;
        sr.color = normalColor;
        
    }

}