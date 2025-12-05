using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Invader : MonoBehaviour
{

    /// <summary>
    /// Animation sprites
    /// </summary>
    public Sprite[] animationSprites;

    /// <summary>
    /// time between animations
    /// </summary>
    public float animationTime = 1.0f;

    /// <summary>
    /// sound byte
    /// </summary>
    public AudioSource pew;
    
    /// <summary>
    /// sprite renderer
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// frame
    /// </summary>
    private int animationFrame;

    /// <summary>
    /// Kill status
    /// </summary>
    public System.Action killed;

    // Start is called before the first frame update
    void Start()
    {
        pew = GetComponent<AudioSource>();
        pew.playOnAwake = false;
        InvokeRepeating(nameof(AnimateSprite), animationTime, animationTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Animates sprites
    private void AnimateSprite()
    {
        animationFrame++;

        if (animationFrame >= animationSprites.Length)
        {
            animationFrame = 0;
        }

        spriteRenderer.sprite = animationSprites[animationFrame];
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            pew.PlayOneShot(pew.clip);
            this.killed.Invoke();
            this.gameObject.SetActive(false);
            ScoreKeeper.AddToScore(1);
        }

    }
}
