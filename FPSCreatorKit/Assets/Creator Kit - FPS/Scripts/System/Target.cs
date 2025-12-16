using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Target : MonoBehaviour
{
    public float health = 5.0f;
    public int pointValue;

    public ParticleSystem DestroyedEffect;

    [Header("Audio")]
    public RandomPlayer HitPlayer;
    public AudioSource IdleSource;
    public RandomPlayer DeathPlayer;

    [Header("Enemy Drops")]
    public GameObject ammoDropPrefab;  // Ammo box to spawn when killed
    public float dropHeightOffset = 0.5f;  // Height above death position

    
    public bool Destroyed => m_Destroyed;

    bool m_Destroyed = false;
    float m_CurrentHealth;

    void Awake()
    {
        Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("Target"));
    }

    void Start()
    {
        if(DestroyedEffect)
            PoolSystem.Instance.InitPool(DestroyedEffect, 16);
        
        m_CurrentHealth = health;
        if(IdleSource != null)
            IdleSource.time = Random.Range(0.0f, IdleSource.clip.length);
    }


    public void Got(float damage)
    {
        m_CurrentHealth -= damage;
        
        if(HitPlayer != null)
            HitPlayer.PlayRandom();
        
        if(m_CurrentHealth > 0)
            return;

        Vector3 position = transform.position;

        Debug.Log($"[Target] {name} died at {position}");

        if (ammoDropPrefab == null)
            {
                Debug.LogWarning($"[Target] ammoDropPrefab is NULL on {name}");
            }
        else
            {
                Vector3 spawnPosition = position + Vector3.up * dropHeightOffset;
                GameObject drop = Instantiate(ammoDropPrefab, spawnPosition, Quaternion.identity);
                Debug.Log($"[Target] Spawned ammo drop: {drop.name} at {spawnPosition}");
            }
        
        if (HitPlayer != null)
        {
            var source = WorldAudioPool.GetWorldSFXSource();
            source.transform.position = position;
            source.pitch = HitPlayer.source.pitch;
            source.PlayOneShot(HitPlayer.GetRandomClip());
        }

        // death sound
        if (DeathPlayer != null)
        {
        var source = WorldAudioPool.GetWorldSFXSource();
        source.transform.position = position;
        source.pitch = 1.0f;
        source.PlayOneShot(DeathPlayer.GetRandomClip());
        }

        if (DestroyedEffect != null)
        {
            var effect = PoolSystem.Instance.GetInstance<ParticleSystem>(DestroyedEffect);
            effect.time = 0.0f;
            effect.Play();
            effect.transform.position = position;
        }

        m_Destroyed = true;
        
        gameObject.SetActive(false);
       
        GameSystem.Instance.TargetDestroyed(pointValue);
    }
}
