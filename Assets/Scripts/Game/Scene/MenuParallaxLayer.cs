using UnityEngine;

public class MenuParallaxLayer : MonoBehaviour
{
    [Header("Parallax Settings")]
    [SerializeField] private float scrollSpeed = 0.5f;// Speed at which the parallax layer scrolls, can be set in the Inspector
    [SerializeField] private Transform firstSprite;// Reference to the first sprite in the parallax layer, used to determine when to loop the background
    [SerializeField] private Transform secondSprite;// Reference to the second sprite in the parallax layer, used to determine when to loop the background

    private float spriteWith;// Width of the sprite, calculated in Start() based on the first sprite's renderer bounds

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpriteRenderer spriteRenderer = firstSprite.GetComponent<SpriteRenderer>();// Get the SpriteRenderer component from the first sprite to calculate the width of the sprite

        spriteWith = spriteRenderer.bounds.size.x;// Calculate the width of the sprite using the bounds of the SpriteRenderer, which will be used to determine when to loop the background
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.left * scrollSpeed * Time.unscaledDeltaTime;// Calculate the movement vector for this frame based on the scroll speed and unscaled delta time to ensure consistent scrolling even when the game is paused

        firstSprite.position += movement;// Move the first sprite by the calculated movement vector
        secondSprite.position += movement;// Move the second sprite by the calculated movement vector

        // Check if the first sprite has moved completely off the left side of the screen
        if (firstSprite.position.x <= -spriteWith)
        {
            firstSprite.position = new Vector3(secondSprite.position.x + spriteWith, firstSprite.position.y, firstSprite.position.z);// If the first sprite has moved completely off the left side of the screen, reposition it to the right of the second sprite to create a looping effect
        }

        // Check if the second sprite has moved completely off the left side of the screen
        if (secondSprite.position.x <= -spriteWith)
        {
            secondSprite.position = new Vector3(firstSprite.position.x + spriteWith, secondSprite.position.y, secondSprite.position.z);// If the second sprite has moved completely off the left side of the screen, reposition it to the right of the first sprite to create a looping effect
        }
    }
}
