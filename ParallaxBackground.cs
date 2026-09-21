using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Camera & Speed Settings")]
    public Camera cam;

    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    [Header("Three Background Pieces (Left, Middle, Right)")]
    public SpriteRenderer leftRenderer;
    public SpriteRenderer midRenderer;
    public SpriteRenderer rightRenderer;

    [Header("Settings")]
    public bool autoScroll = false;

    private float spriteWidth;
    private Transform camTransform;

    private float camX;

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

        // Get width from the middle sprite.
        spriteWidth = midRenderer.bounds.size.x;

        // Make sure the three sprites are correctly ordered.
        SortRenderersByX();

        // Force them into a perfect three-tile formation.
        SetupSpritePositions();

        // Initialize parallax position.
        camX = camTransform.position.x * parallaxFactor;
        transform.position = new Vector3(
            camX,
            transform.position.y,
            transform.position.z
        );
    }

    void Update()
    {
        // -----------------------------------------
        // Move the parallax layer
        // -----------------------------------------

        if (!autoScroll)
        {
            camX = camTransform.position.x * parallaxFactor;
        }
        else
        {
            camX -= Time.deltaTime * parallaxFactor;
        }

        transform.position = new Vector3(
            camX,
            transform.position.y,
            transform.position.z
        );

        // -----------------------------------------
        // Check for looping
        // -----------------------------------------

        CheckAndRepositionSprites();
    }

    void CheckAndRepositionSprites()
    {
        /*
         * Convert the camera position into the parallax
         * object's local coordinate space.
         *
         * This is much more stable than checking the camera's
         * viewport edges against sprite edges.
         */
        float cameraLocalX =
            camTransform.position.x - transform.position.x;

        /*
         * Camera has moved far enough to the RIGHT.
         *
         * Example:
         *
         *     LEFT    MID    RIGHT
         *      -W      0       +W
         *
         * Once the camera passes halfway between MID and RIGHT,
         * recycle LEFT to the far RIGHT.
         */
        if (cameraLocalX > spriteWidth * 0.5f)
        {
            MoveLeftToRight();
        }

        /*
         * Camera has moved far enough to the LEFT.
         *
         * Recycle RIGHT to the far LEFT.
         */
        else if (cameraLocalX < -spriteWidth * 0.5f)
        {
            MoveRightToLeft();
        }
    }

    void MoveLeftToRight()
    {
        // Remember the current left sprite.
        SpriteRenderer oldLeft = leftRenderer;

        // Shift references.
        leftRenderer = midRenderer;
        midRenderer = rightRenderer;
        rightRenderer = oldLeft;

        // Put recycled sprite exactly one tile beyond the new right.
        rightRenderer.transform.position =
            midRenderer.transform.position +
            Vector3.right * spriteWidth;
    }

    void MoveRightToLeft()
    {
        // Remember the current right sprite.
        SpriteRenderer oldRight = rightRenderer;

        // Shift references.
        rightRenderer = midRenderer;
        midRenderer = leftRenderer;
        leftRenderer = oldRight;

        // Put recycled sprite exactly one tile beyond the new left.
        leftRenderer.transform.position =
            midRenderer.transform.position +
            Vector3.left * spriteWidth;
    }

    void SetupSpritePositions()
    {
        /*
         * Keep the middle sprite where it currently is,
         * then place the other two exactly one width away.
         */

        Vector3 middlePosition = midRenderer.transform.position;

        leftRenderer.transform.position =
            middlePosition + Vector3.left * spriteWidth;

        rightRenderer.transform.position =
            middlePosition + Vector3.right * spriteWidth;
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