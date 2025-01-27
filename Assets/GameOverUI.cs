using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private Image _logo;
    [SerializeField] private Image _screen;
    [SerializeField] private GameObject _container;
    [SerializeField] private AudioSource gameOverSoundSource;
    [SerializeField] private AudioSource gameOverSFXSource;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip gameOverSFXClip;
    [SerializeField] private Image fadeOutImage;
    

    void Start()
    {
        gameOverSoundSource.clip = gameOverSound;
        gameOverSFXSource.clip = gameOverSFXClip;
        AudioManager.Instance.PlaySFXSource(gameOverSFXSource);
        AudioManager.Instance.PlayGameOverSource(gameOverSoundSource);
        StartCoroutine(ShowGameOverSequence());
        scoreDisplay.text =     _container.GetComponent<Container>()._scoreManager.Score.ToString();

    }

    private IEnumerator ShowGameOverSequence()
    {
        // Initialize logo's alpha to 0
        UnityEngine.Color logoColor = _logo.color;
        logoColor.a = 0;
        _logo.color = logoColor;

        _logo.gameObject.SetActive(true);

        float elapsedTime = 0f;
        float duration = 2.5f;

        // Gradually increase the alpha of the logo
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            logoColor.a = alpha;
            _logo.color = logoColor;
            yield return null;
        }

        logoColor.a = 1;
        _logo.color = logoColor;

        // Fade in the fadeOutImage over 1 second
        elapsedTime = 0f;
        UnityEngine.Color fadeColor = fadeOutImage.color;
        fadeColor.a = 0;
        fadeOutImage.color = fadeColor;

        fadeOutImage.gameObject.SetActive(true);
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / 0.5f);
            fadeColor.a = alpha;
            fadeOutImage.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 1;
        fadeOutImage.color = fadeColor;

        _screen.gameObject.SetActive(true); 

        elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsedTime / 0.5f);
            fadeColor.a = alpha;
            fadeOutImage.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 0;
        fadeOutImage.color = fadeColor;

        // Destroy the container object
        if (_container != null)
        {
            Destroy(_container);
        }
    }

}