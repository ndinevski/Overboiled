using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private OvenUI _ovenUI;

    [SerializeField]
    private LevelSwitch levelSwitch;
    
    [SerializeField]
    private MultiplierLabel multiplierLabel;
    
    [SerializeField]
    private Renderer liquidRenderer;
    
    [SerializeField] AudioSource gasAudioSource;
    [SerializeField] ParticleSystem gasParticleSystem;
    [SerializeField] Container container;
    public float multiplier;
    public Bubble bubble;
    public float multiplierTimer;
    public float levelTimer;
    public bool gameStarted = false;
    private Level _level;
    public Level Level
    {
        get => _level;
        set
        {
            if (value.level == 4)
            {
                _level = value;
                return;
            }
            if (_level?.level < value.level && _level != null) 
            {
                levelSwitch.TargetAngle = value.rotationAngle;
            }

            if (_level?.level > value.level && _level != null)
            {
                levelSwitch.TargetAngle = value.rotationAngle;
            }

            var main = gasParticleSystem.main; 
            main.startLifetime = value.gasLifetime;

            if (value.gasSound != null)
            {
                gasAudioSource.clip = value.gasSound;
                AudioManager.Instance.PlayGasSource(gasAudioSource);
            }
            else
            {
                gasAudioSource.Stop();
            }
            
            liquidRenderer.material.SetFloat("_WaveSpeed", value.liquidSpeed);
            levelTimer = value.levelTimer;
            _level = value;
            _ovenUI.LevelDisplayCounter.text = _level.level.ToString();
        }
    }

    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            _ovenUI.ScoreDisplayCounter.text = _score.ToString();
        }
    }

    void Awake()
    {
        bubble.scoreManager = this;
    } 
    void Start()
    {
        multiplier = Config.Instance.STARTING_MULTIPLIER;
        Score = 0;
        multiplierTimer = 0f;
        Level = Config.Instance.LEVELS[0];
        levelTimer = _level.levelTimer;
        _ovenUI.LevelDisplayCounter.text = _level.level.ToString();
        _ovenUI.ScoreDisplayCounter.text = _score.ToString();
    }

    void Update()
    {
        if(!gameStarted || container._gameOver) return;
        ManageMultiplierTimer();
        ManageLevelTimer();
    }

    private void ManageMultiplierTimer()
    {
        if (multiplierTimer > 0)
        {
            multiplierTimer -= Time.deltaTime;
        }
        else
        {
            multiplierTimer = 0;
            multiplier = Config.Instance.STARTING_MULTIPLIER;
        }
    }
    
    private void ManageLevelTimer()
    {
        if (levelTimer > 0)
        {
            levelTimer -= Time.deltaTime;
        }
        else
        {
            IncreaseLevel();
            levelTimer = _level.levelTimer;
        }
    }

    private void IncreaseLevel()
    {
        if (_level.level == Config.Instance.MAX_LEVELS)
        {
            return;
        }
        
        Level = Config.Instance.LEVELS[_level.level + 1];
    }

    public void AddScore(int points)
    {
        Score += (int) Math.Round(points * multiplier);
    }

    public void IncreaseMultiplier(Color32 popColor)
    {
        multiplierTimer = Config.Instance.MULTIPLIER_TIMER;
        multiplier *= Config.Instance.MULTIPLIER_INCREASE;
        multiplierLabel.ShowMultiplier(multiplier, popColor);
    }

    public void StartGame()
    {
        Debug.Log("Game started");
        if (Level.level == 0)
        {
            gameStarted = true;
            IncreaseLevel();
        }
    }
}
