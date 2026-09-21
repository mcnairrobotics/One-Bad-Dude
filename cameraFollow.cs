using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform player;
    private float startPosY;
    private float startPosY2;
    public int threshold;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosY = player.position.y;
        startPosY2 = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //
        if (player.position.y - startPosY / 2 > startPosY2 + threshold)
        {
            startPosY2 += (threshold - 3);
            startPosY += (threshold - 3);
        }
        else if (player.position.y - startPosY / 2 < startPosY2 - threshold)
        {
            startPosY2 -= (threshold - 3);
            startPosY -= (threshold - 3);
        }
        transform.position = new Vector3(player.position.x, (player.position.y - startPosY/2)/2 + startPosY2);
    }
}
