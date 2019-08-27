using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // Config params
    [SerializeField] AudioClip breakSound;
    [SerializeField] GameObject blockSparklesVFX;
    [SerializeField] Sprite[] hitSprites;

    // Cached reference;
    Level level;
    GameSession gameStatus;
    // Forma vieja
    // [SerializeField] Level level; // Y luego enlazar desde el inspector

    // State variables
    [SerializeField] int timesHit; // Only serialized for debug purposes

    private void Start()
    {
        // En lugar de [SerializeField], podemos usar esto para obtener la instancia del objeto de forma más limpia
        level = FindObjectOfType<Level>();
        gameStatus = FindObjectOfType<GameSession>();
        CountBreakableBlocks();
    }

    private void CountBreakableBlocks()
    {
        if (tag.Equals("Breakable"))
        {
            level.CountBreakableBlocks();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("Ball") && tag.Equals("Breakable"))
        {
            HandleHit();
        }
    }

    private void HandleHit()
    {
        timesHit++;
        int maxHits = hitSprites == null ? 1 : hitSprites.Length + 1;
        if (timesHit >= maxHits)
        {
            DestroyBlock();
        } else
        {
            ShowNextHitSprite();
        }
    }

    private void ShowNextHitSprite()
    {
        int spriteIndex = timesHit - 1;
        if (hitSprites.Length > spriteIndex)
        {
            GetComponent<SpriteRenderer>().sprite = hitSprites[spriteIndex];
        } else
        {
            Debug.LogError(gameObject.name + ": Block sprite is missing from array");
        }
    }

    private void DestroyBlock()
    {
        gameStatus.AddToScore();
        AudioSource.PlayClipAtPoint(breakSound, Camera.main.transform.position);
        Destroy(gameObject);
        level.BlockDestroyed();
        TriggerSparklesVFX();
    }

    private void TriggerSparklesVFX()
    {
        GameObject sparkles = Instantiate(blockSparklesVFX, transform.position, transform.rotation);
        // Para no ocupar memoria, destruimos el objeto
        Destroy(sparkles, 2f);
    }
}
