using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Container : MonoBehaviour
{
    [SerializeField] private Transform bubbleTransform;
    [SerializeField] public ScoreManager _scoreManager;
    [SerializeField] private PowerUpsUI _powerUpsUI;
    [SerializeField] private Lid lid;
    [SerializeField] private AudioSource gameplayMusicSource;
    [SerializeField] private AudioClip gamePlayAudioClip;
    [SerializeField] private Transform liquidMask;
    [SerializeField] private GameOverUI _gameOverUI;
    [SerializeField] private AudioClip boilingWaterClip;
    [SerializeField] private AudioMixerGroup boilingWaterGroup;
    [SerializeField] private AudioSource boilingWaterAudioSource;
    private float elapsedTime = 0f;
    private Bubble bubble;
    public List<Bubble> bubbles = new List<Bubble>();

    private bool _gameStarted;
    private int popAllCounter;
    private int coolOffCounter;
    private int lidCounter;
    public bool _gameOver;

    public int PopAllCounter
    {
        get => popAllCounter;
        set
        {
            popAllCounter = value;
            var popAllSprite = value > 0 ? _powerUpsUI.popAllButtonSprites[1] : _powerUpsUI.popAllButtonSprites[0];
            _powerUpsUI.popAllButton.image.sprite = popAllSprite;
            _powerUpsUI.popAllPowerUpCounter.text = value.ToString();
        }
    }

    public int CoolOffCounter
    {
        get => coolOffCounter;
        set
        {
            coolOffCounter = value;
            var coolOffSprite = value > 0 ? _powerUpsUI.lowerLevelButtonSprites[1] : _powerUpsUI.lowerLevelButtonSprites[0];
            _powerUpsUI.lowerLevelButton.image.sprite = coolOffSprite; 
            _powerUpsUI.lowerLevelButtonCounter.text = value.ToString();
        }
    }

    public int LidCounter
    {
        get => lidCounter;
        set
        {
            lidCounter = value;
            var lidSprite = value > 0 ? _powerUpsUI.lidButtonSpries[1] : _powerUpsUI.lidButtonSpries[0];
            _powerUpsUI.lidButton.image.sprite = lidSprite;
            _powerUpsUI.lidPowerUpCounter.text = value.ToString();
        }
    }

    void Awake()
    {
        gameplayMusicSource.clip = gamePlayAudioClip;
        boilingWaterAudioSource.clip = boilingWaterClip;
        AudioManager.Instance.PlayBoilingWaterSource(boilingWaterAudioSource);
        AudioManager.Instance.PlayGamePlaySource(gameplayMusicSource);
    }

    void Start()
    {
        var scale = liquidMask.transform.localScale;
        scale.y = 0.0f;
        liquidMask.transform.localScale = scale;
        _gameStarted = false;
        LidCounter = 2;
        PopAllCounter = 3;
        CoolOffCounter = 3;
        bubble = bubbleTransform.GetComponent<Bubble>();
        StartCoroutine(StartGame());
    }

    void Update()
    {
        if(!_gameStarted) return;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= _scoreManager.Level.spawnInterval)
        {
            elapsedTime = 0f;
            int randomInt = Random.Range(0, 100);

            if (randomInt < _scoreManager.Level.spawnChance)
            {
                bubble.container = this;
                var newBubble = Instantiate(bubbleTransform, Vector3.zero, Quaternion.identity);
                bubbles.Add(newBubble.GetComponent<Bubble>());
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if(_gameOver) return;
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider)
            {
                if (hit.collider.CompareTag("Bubble"))
                {
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    hit.collider.gameObject.GetComponent<Bubble>()?.DestroyBubbleOnTap();
                }
            }
        }
        
    }
    
    public void GameOver()
    {
        if(_gameOver) return;
        _gameOver = true;
        _scoreManager.Level = Config.Instance.GameOverLevel;
        _gameOverUI.gameObject.SetActive(true);
    }

    public void DeleteBubbles(HashSet<Bubble> deleteBubbles)
    {
        foreach (var b in deleteBubbles)
        {
            b.DestroyBubble(0.3f);
        }
    }
    
    // POWER UPS
    public void PopAllFromColor()
    {
        if(_gameOver) return;
        if (PopAllCounter == 0)
        {
            return;
        }
        var color = bubbles[Random.Range(0, bubbles.Count)].GetColor();
        var destroyBubbles = bubbles.FindAll(b => b.GetColor() == color);

        foreach (var b in destroyBubbles)
        {
            _scoreManager.AddScore(Config.Instance.SCORE_FOR_POPPED_BELOW_HORIZON);
            b.DestroyBubble(0.3f);
        }
        
        _scoreManager.IncreaseMultiplier(UnityEngine.Color.yellow);
        
        PopAllCounter -=  1;
    }
    
    // LOWER LEVEL
    public void LowerLevel()
    {
        if(_gameOver) return;
        if (CoolOffCounter == 0)
        {
            return;
        }
        if (_scoreManager.Level.level != 0)
        {
            _scoreManager.Level = Config.Instance.LEVELS[_scoreManager.Level.level - 1];
        }
        
        CoolOffCounter -= 1;
    }
    
    // PUT LID ON
    public void putLidOn()
    {
        if(_gameOver) return;
        if (LidCounter == 0 || lid.timer>0)
        {
            return;
        }
        lid.Activate();
        LidCounter -= 1;
    }

    public IEnumerator StartGame()
    {
        float elapsedTime = 0f;
        float targetScale = 1f;
        Vector3 initialScale = liquidMask.transform.localScale;

        yield return new WaitForSeconds(0.2f);
        while (elapsedTime < 2f)
        {
            float normalizedTime = Mathf.Clamp01(elapsedTime / 2f); 

            float newScaleY = Mathf.Lerp(initialScale.y, targetScale, normalizedTime);

            liquidMask.transform.localScale = new Vector3(initialScale.x, newScaleY, initialScale.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        liquidMask.transform.localScale = new Vector3(initialScale.x, targetScale, initialScale.z);

        _gameStarted = true;
        _scoreManager.StartGame();
    }

}