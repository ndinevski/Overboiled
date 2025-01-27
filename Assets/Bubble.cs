using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using Random = System.Random;

public class Bubble : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool IsDestroyed;
    [SerializeField] private Collider2D liquidArea;    
    private SpriteRenderer _spriteRenderer;
    private Transform _transform;
    private Color _color;
    private bool _hasReachedHorizon;
    private float _gravity;
    private float _size;
    private float _mass;
    private Collider2D _collider;
    public Guid _bubbleId;
    public Container container;
    public ScoreManager scoreManager;
    public MMF_Player _feedbackPlayer;
    [SerializeField] public List<AudioClip> bubblePops;
    [SerializeField] public AudioSource bubbleAudioSource;
    
    private void Awake()
    {
        IsDestroyed = false;
        liquidArea = GameObject.Find("PlayZone").GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _mass = scoreManager.Level.mass;
        _color = Config.Instance.colors[new Random().Next(0, Config.Instance.colors.Count)];
        _hasReachedHorizon = false;
        _gravity = UnityEngine.Random.Range(Config.Instance.GRAVITY_RANGE.Item1, Config.Instance.GRAVITY_RANGE.Item2);
        _size = UnityEngine.Random.Range(Config.Instance.SIZE_RANGE.Item1, Config.Instance.SIZE_RANGE.Item2);
        _bubbleId = Guid.NewGuid();
    }

    void Start()
    {
        _spriteRenderer.color = _color.BubbleColor;
        _transform.localScale = new Vector3(_size, _size, _size);
        
        var vector3 = _transform.position;
        vector3.y = Config.Instance.LEFT_SPAWN_LIMIT.position.y;
        _transform.position = vector3;

        var position = _transform.position;
        position.x = UnityEngine.Random.Range(Config.Instance.LEFT_SPAWN_LIMIT.position.x, Config.Instance.RIGHT_SPAWN_LIMIT.position.x);
        _transform.position = position;
        
        rb.mass = _mass;
        rb.gravityScale = _gravity;
        
        Collider2D[] colliders = GetComponents<Collider2D>();
        if (colliders.Length > 0)
        {
            _collider = colliders[2];
        }
    }
    private float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        
        if (elapsedTime >= 0.25f && !_hasReachedHorizon)
        {
            Vector2 dirVector;
            float direction = UnityEngine.Random.Range(0f, 100f);
            if (direction < 50f)
            {
                dirVector = Vector2.right * (5f * _mass);
            }
            else
            {
                dirVector = Vector2.left * (5f * _mass);
            }
            elapsedTime = 0f;
            rb.AddForce(dirVector, ForceMode2D.Force);
        }

        CollideBubbles();

    }

    private void PlayRandomSound() {
        int randomIndex = UnityEngine.Random.Range(0, bubblePops.Count);
        var clip = bubblePops[randomIndex];
        bubbleAudioSource.clip = clip;
        AudioManager.Instance.PlaySFXSource(bubbleAudioSource);
    }
    
    public void DestroyBubbleOnTap() {
        if (!_hasReachedHorizon && !IsDestroyed)
        {
            scoreManager.AddScore(Config.Instance.SCORE_FOR_POPPED_BELOW_HORIZON);
            DestroyBubble(0.15f);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other == liquidArea)
        {
            rb.gravityScale = _gravity;
            rb.mass = _mass;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == liquidArea)
        {
            _hasReachedHorizon = true;
            rb.gravityScale = Config.Instance.HORIZON_GRAVITY;
            rb.mass = Config.Instance.MASS.Item1;
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public HashSet<Bubble> GetAllContactingColliders()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        
        List<Collider2D> contacts = new List<Collider2D>();
        _collider.Overlap(contactFilter, contacts);

        var collidingSameColorBubbles = new HashSet<Bubble>();
        collidingSameColorBubbles.Add(this);

        foreach (var contact in contacts)
        {
            Bubble bubble = contact.GetComponent<Bubble>();
            if (bubble != null &&
                bubble._bubbleId != this._bubbleId &&
                bubble._color.Equals(_color) && 
                _color.IsPoppable)
            {
                collidingSameColorBubbles.Add(bubble);
            }
        }

        return collidingSameColorBubbles;
    }

    private void CollideBubbles()
    {
        if(IsDestroyed || container._gameOver) return;
        var collidingSameColorBubbles = GetAllContactingColliders();
        if (collidingSameColorBubbles.Count >= Config.Instance.NUMBER_OF_COLLISIONS_TO_POP)
        {
            scoreManager.AddScore(collidingSameColorBubbles.Count * Config.Instance.SCORE_FOR_POPPED_ABOVE_HORIZON);
            container.DeleteBubbles(collidingSameColorBubbles);
            scoreManager.IncreaseMultiplier(_color.BubbleColor);
        }  
    }

    public void DestroyBubble(float destroyDelay)
    {
        if(IsDestroyed) return;
        IsDestroyed = true;
        PlayRandomSound();
        _feedbackPlayer.PlayFeedbacks();
        container.bubbles.Remove(this);
        Destroy(gameObject, destroyDelay);
    }
    
    public Color GetColor()
    {
        return _color;
    }
}
