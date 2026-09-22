using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Camera & Speed Settings")]
    public Camera cam;

    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    [Header("Three Background Pieces")]
    public SpriteRenderer leftRenderer;
    public SpriteRenderer midRenderer;
    public SpriteRenderer rightRenderer;

    [Header("Settings")]
    public bool autoScroll = false;

    private float spriteWidth;
    private Transform camTransform;

    private float parallaxX;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("LoopingParallax: No camera assigned!");
            enabled = false;
            return;
        }

        camTransform = cam.transform;

        if (leftRenderer == null ||
            midRenderer == null ||
            rightRenderer == null)
        {
            Debug.LogError("LoopingParallax: Assign all three SpriteRenderers!");
            enabled = false;
            return;
        }

        // Width of one background piece.
        spriteWidth = midRenderer.bounds.size.x;

        // Make sure references are actually left/middle/right.
        SortRenderersByX();

        // Establish a perfect three-piece arrangement.
        SetupSpritePositions();

        // Initial parallax position.
        parallaxX = camTransform.position.x * parallaxFactor;

        transform.position = new Vector3(
            parallaxX,
            transform.position.y,
            transform.position.z
        );
    }

    void Update()
    {
        // -----------------------------------------
        // PARALLAX MOVEMENT
        // -----------------------------------------

        if (!autoScroll)
        {
            parallaxX =
                camTransform.position.x * parallaxFactor;
        }
        else
        {
            parallaxX -=
                Time.deltaTime * parallaxFactor;
        }

        transform.position = new Vector3(
            parallaxX,
            transform.position.y,
            transform.position.z
        );

        // -----------------------------------------
        // LOOP BACKGROUND
        // -----------------------------------------

        CheckAndRepositionSprites();
    }

    void CheckAndRepositionSprites()
    {
        float cameraX = camTransform.position.x;

        float cameraHalfWidth =
            cam.orthographicSize * cam.aspect;

        float cameraLeft =
            cameraX - cameraHalfWidth;

        float cameraRight =
            cameraX + cameraHalfWidth;

        float leftEdge =
            leftRenderer.bounds.max.x;

        float rightEdge =
            rightRenderer.bounds.min.x;

        // -----------------------------------------
        // CAMERA MOVED RIGHT
        // -----------------------------------------

        if (cameraLeft > leftEdge)
        {
            MoveLeftToRight();
        }

        // -----------------------------------------
        // CAMERA MOVED LEFT
        // -----------------------------------------

        else if (cameraRight < rightEdge)
        {
            MoveRightToLeft();
        }
    }

    void MoveLeftToRight()
    {
        // Save the current left sprite.
        SpriteRenderer recycled = leftRenderer;

        // Shift references.
        leftRenderer = midRenderer;
        midRenderer = rightRenderer;
        rightRenderer = recycled;

        // Put recycled sprite exactly after the right sprite.
        rightRenderer.transform.position =
            new Vector3(
                midRenderer.transform.position.x + spriteWidth,
                rightRenderer.transform.position.y,
                rightRenderer.transform.position.z
            );
    }

    void MoveRightToLeft()
    {
        // Save the current right sprite.
        SpriteRenderer recycled = rightRenderer;

        // Shift references.
        rightRenderer = midRenderer;
        midRenderer = leftRenderer;
        leftRenderer = recycled;

        // Put recycled sprite exactly before the left sprite.
        leftRenderer.transform.position =
            new Vector3(
                midRenderer.transform.position.x - spriteWidth,
                leftRenderer.transform.position.y,
                leftRenderer.transform.position.z
            );
    }

    void SetupSpritePositions()
    {
        Vector3 middlePosition =
            midRenderer.transform.position;

        leftRenderer.transform.position =
            new Vector3(
                middlePosition.x - spriteWidth,
                leftRenderer.transform.position.y,
                leftRenderer.transform.position.z
            );

        rightRenderer.transform.position =
            new Vector3(
                middlePosition.x + spriteWidth,
                rightRenderer.transform.position.y,
                rightRenderer.transform.position.z
            );
    }

    void SortRenderersByX()
    {
        SpriteRenderer[] arr =
        {
            leftRenderer,
            midRenderer,
            rightRenderer
        };

        System.Array.Sort(
            arr,
            (a, b) =>
                a.transform.position.x.CompareTo(
                    b.transform.position.x
                )
        );

        leftRenderer = arr[0];
        midRenderer = arr[1];
        rightRenderer = arr[2];
    }
}