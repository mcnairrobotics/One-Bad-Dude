using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraSizeVolume : MonoBehaviour
{
    public Camera targetCamera;

    [Header("Camera")]
    public float targetSize = 3f;
    public float defaultSize = 5f;

    [Header("Blend")]
    public float blendDistance = 3f;

    private BoxCollider2D box;
    public float smoothTime = 0.25f;

private float velocity;

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;

        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (targetCamera == null)
            return;

        Transform player = targetCamera.transform;

        Bounds bounds = box.bounds;

        Vector3 closest = bounds.ClosestPoint(player.position);

        float distance = Vector2.Distance(player.position, closest);

        float t;

        if (bounds.Contains(player.position))
        {
            print("player in bounds");
            t = 1f;
        }
        else
        {
            t = Mathf.Clamp01(1f - (distance / blendDistance));
        }

        float desired = Mathf.Lerp(defaultSize, targetSize, t);

        targetCamera.orthographicSize =
            Mathf.SmoothDamp(
                targetCamera.orthographicSize,
                desired,
                ref velocity,
                smoothTime);
        }
}