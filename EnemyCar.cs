using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    public int maxHealth;
    private int health;
    private bool dead;
    public GameObject[] damages;
    public GameObject carSploshion;
    void Start()
    {
        if(PlayerPrefs.GetInt("Difficulty") == 0)
        {
            maxHealth /= 3;
        }
        else if(PlayerPrefs.GetInt("Difficulty") == 1)
        {
            maxHealth /= 2;
        }
        health = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health < 1 && dead == false)
        {
            dead = true;
            /*audio.PlayOneShot(clips[Random.Range(2,5)]);
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
            */
            Instantiate(carSploshion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else if(dead == false)
        {
            //audio.PlayOneShot(clips[6]);
            for (int i = 0; i < damages.Length; i++){
                if((i+1)*10 < maxHealth - health)
                {
                    damages[i].SetActive(true);
                }
            }
        }
    }
}
