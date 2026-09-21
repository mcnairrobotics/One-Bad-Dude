using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public Transform[] points;
    private int i = 0;
    public float delay;
    private bool startWait = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = points[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f && startWait == false)
        {
            Invoke("updatePos", delay);
            startWait = true;
        }
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }
    void updatePos()
    {
        i++;
        i %= points.Length;
        startWait = false;

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.SetParent(transform);
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        collision.transform.SetParent(null);
        
    }
}
