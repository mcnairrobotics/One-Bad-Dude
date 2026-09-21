using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    [Range(0f, 1f)] public float airControlFactor = 0.3f; // how much control in the air

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask groundLayerSolid;
    public bool idling;

    private Rigidbody2D rb;
    private Animator animator;
    [HideInInspector]
    public bool isGrounded;
    private bool cantDrop;
    private float moveInput;
    public bool canMove;
    private float upDownInput;
    public bool justGotOffGround;
    private bool facingRight = true;
    public bool dead;
    private AudioSource audio;
    public AudioClip[] clips;
    public int ammo;
    private int clipAmmo;
    public int clipSize;
    public TMPro.TMP_Text ammoText;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public int health = 3;
    public GameObject[] healthIcons;
    public GameObject[] magicIcons;
    public LocalizedText signText;
    public GameObject StatScreen;
    private int shotsTaken = 0;
    public int Keys = 0;
    public int kills = 0;
    public string[] ranks = { "Peaceful", "Self Defense",  "Survivor", "Killer", "Viscious Killer!", "Utter Terror!", "COMPLETE ANIMAL!", "Unstoppable"};
    public TMPro.TMP_Text shotsText;
    public TMPro.TMP_Text killsText;
    public TMPro.TMP_Text timeText;
    public bool Melee;
    bool canBeHit = true;
    public TMPro.TMP_Text ranksText;
    private SpriteRenderer sr;
    public Color hitColor;
    private Color normalColor;
    public Camera cam;
    public Animator bikeAnim;
    public GameObject uiKey;
    public GameObject flash;
    public bool Invulnerable = false;
    public int magic = 6;
    public bool machinegunMode = false;
    public bool waitTillNextFire = false;
    [SerializeField]
    private RuntimeAnimatorController standardController;
    [SerializeField]
    private RuntimeAnimatorController machinegunController;
    private InputManager input;

    void Start()
    {
        input = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        normalColor = sr.color;
        animator = GetComponent<Animator>();
        audio = gameObject.GetComponent<AudioSource>();
        clipAmmo = clipSize;
        flash.SetActive(true);
    }
    public void PlayClip(int clipNum)
    {
        if (clipNum == 0)
        {
            ammo -= 1;
            clipAmmo -= 1;
            shotsTaken += 1;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Vector3 scale = bullet.transform.localScale;
            scale.x *= transform.localScale.x;
            bullet.transform.localScale = scale;
        }
        if (clipNum == 7)
        {
            magic -= 2;
        }
        audio.PlayOneShot(clips[clipNum]);
    }

    void Update()
    {
        if(Time.timeScale == 0.0f)
            return;
        AmmoUpdate();
        /**/
        if (dead) return;
        // Input
        moveInput = input.Move.x;
        upDownInput = input.Move.y;
        animator.SetBool("Melee", Melee);
        animator.SetBool("Machinegun", machinegunMode);
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        cantDrop = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerSolid);
        // Jump
        if(input.FireHeld && machinegunMode && waitTillNextFire == false)
        {
            if(ammo < 0)
                ammo = 0;
            if(isGrounded || upDownInput < 0)
            {
                if ((ammo > 0 && clipAmmo > 0) || Melee == true)
                {
                    animator.SetBool("Melee", Melee);
                    animator.SetTrigger("Shooting");
                }
                else
                {
                    revertToPistol();
                    //PlayClip(2);
                }
            }
            
        }
        if (input.FirePressed && isGrounded && !machinegunMode)
        {
            if(ammo < 0)
                ammo = 0;
            if ((ammo > 0 && clipAmmo > 0) || Melee == true)
            {
                animator.SetBool("Melee", Melee);
                animator.SetTrigger("Shooting");
            }
            else
            {
                PlayClip(2);
            }
            // moveInput = 0;
        }
        else if (input.SpecialPressed && isGrounded && magic > 0 && canMove)
        {
            //if(ammo < 0)
            //    ammo = 0;
            //if ((ammo > 0 && clipAmmo > 0) || Melee == true)
            //{
            animator.SetTrigger("Magic1");
            //}
            //else
            //{
            //    PlayClip(2);
            //}
            // moveInput = 0;
        }
        else if (input.ReloadPressed && isGrounded && !machinegunMode)//reload
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
        else if (input.JumpPressed && isGrounded)
        {
            animator.ResetTrigger("JumpKick");
            audio.PlayOneShot(clips[4]);
            if (moveInput == 0 && upDownInput > 0)
                rb.linearVelocity = new Vector2(0, jumpForce * 1.7f);
            else if (moveInput == 0 && upDownInput < 0 && !cantDrop)
            {
                //dip through platform
                groundLayer &= ~(1 << 6);
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                rb.linearVelocity = new Vector2(0, -jumpForce * 1.7f);
                Invoke("resetLayer", 0.15f);
            }
            else
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 1.2f);
        }
        else if (input.FirePressed && !isGrounded)//jump kick
        {
            if (!facingRight && moveInput > 0) Flip();
            else if (facingRight && moveInput < 0) Flip();
            if ((ammo > 0 && clipAmmo > 0) && upDownInput < 0)
            {
                animator.SetTrigger("Shooting");
            }
            else
            {
                animator.SetTrigger("JumpKick");
                rb.linearVelocity = new Vector2(jumpForce*2*transform.localScale.x, -jumpForce * 1.7f);
            }
            
        }

        // Flip sprite only when on ground
        if (isGrounded)
        {
            if (justGotOffGround)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y * 0.7f);
                groundLayer |= (1 << 6);
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
                justGotOffGround = false;
            }
            if (!facingRight && moveInput > 0) Flip();
            else if (facingRight && moveInput < 0) Flip();
        }
        else
        {
            justGotOffGround = true;
        }

        // Animator parameters
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("Ducking", upDownInput * -1);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if animator is currently in the "Idle" state
        bool isIdle = stateInfo.IsName("Idle");
        // Check if animator is NOT transitioning
        bool isNotTransitioning = !animator.IsInTransition(0);
        if (isIdle && isNotTransitioning)
        {
            idling = true;
        }
        else
        {
            idling = false;
        }
        input.ClearFrameInput();
    }
    public void revertToPistol()
    {
        machinegunMode = false;
        animator.runtimeAnimatorController = standardController;
        clipSize = 10;
    }
    public void machinegunPowerup()
    {
        machinegunMode = true;
        animator.runtimeAnimatorController = machinegunController;
        clipSize = 25;
        clipAmmo = 25;
        ammo += 25;
    }
    public IEnumerator win()
    {
        timeText.text = "Time " + (int)Time.time;
        killsText.text = "Kills " + kills;
        shotsText.text = "Shots " + shotsTaken;
        int rankValue = (kills+9)/10;
        if(rankValue > ranks.Length - 1)
            rankValue = ranks.Length - 1;
        ranksText.text = ranks[rankValue];
        StatScreen.SetActive(true);
        dead = true;
        yield return new WaitForSeconds(6);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1);

    }
    public void disappear()
    {
        dead = true;
        cam.gameObject.SetActive(false);
        bikeAnim.Play("BikeHopOn");
    }
    void resetLayer()
    {
        groundLayer |= (1 << 6);
    }
    void FixedUpdate()
    {
        if (dead) return;
        if (!canMove)
            moveInput = 0;
        if (isGrounded)
        {
            // Full movement on ground
            if (upDownInput >= 0 && !(animator.GetCurrentAnimatorStateInfo(0).IsName("Duck") || animator.GetCurrentAnimatorStateInfo(0).IsName("DuckShoot") || animator.GetCurrentAnimatorStateInfo(0).IsName("DuckKick")))
                rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Reduced air control (momentum dominant)
            float targetXVel = moveInput * moveSpeed;
            float smoothedXVel = Mathf.Lerp(rb.linearVelocity.x, targetXVel, airControlFactor * Time.fixedDeltaTime * 10f);
            rb.linearVelocity = new Vector2(smoothedXVel, rb.linearVelocity.y);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Danger") && dead == false)
        {
            health = 1;
            takeDmg();
        }
        if (collision.gameObject.CompareTag("DangerSmall") && dead == false)
        {
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
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ammo"))
        {
            ammo += Random.Range(6, 9);
            Destroy(collision.gameObject);
            PlayClip(1);
        }
        if (collision.gameObject.CompareTag("Key"))
        {
            Keys += 1;
            uiKey.SetActive(true);
            Destroy(collision.gameObject);
            PlayClip(5);
        }
        if (collision.gameObject.CompareTag("LockedDoor") && Keys > 0)
        {
            Keys -= 1;
            uiKey.SetActive(false);
            collision.gameObject.GetComponent<Animator>().enabled = true;
            //Destroy(collision.gameObject);
            PlayClip(6);
        }
        if (collision.gameObject.CompareTag("Sign"))
        {
            signText.updateKey(collision.gameObject.name); //;
        }
        if (collision.gameObject.name == ("Win"))
        {
            animator.Play("Win");
        }
        if (collision.gameObject.name == ("WinBike"))
        {
            animator.Play("PlayerDisappear");
        }
    }
    private void OnEnable() {
        justGotOffGround = true;
        sr.color = normalColor;
        canBeHit = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Sign"))
        {
            signText.clearText();
            //signText.text = "";
        }
    }
    public void AmmoUpdate()
    {
        if (LocalizationManager.Instance == null)
            return;
        ammoText.text = LocalizationManager.Instance.GetText("CLIP") + " " + clipAmmo +"i" +clipSize+ "\n"+ LocalizationManager.Instance.GetText("AMMO") + " "  + ammo;
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (health > i)
                healthIcons[i].SetActive(true);
            else
                healthIcons[i].SetActive(false);

        }
        for (int i = 0; i < magicIcons.Length; i++)
        {
            if (magic > i*2)
                magicIcons[i].SetActive(true);
            else
                magicIcons[i].SetActive(false);

        }
        
    }
}
