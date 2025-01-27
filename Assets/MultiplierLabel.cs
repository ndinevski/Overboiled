using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public class MultiplierLabel : MonoBehaviour
{
    private float timer;
    private TextMeshProUGUI _multiplierLabel;
    public MMF_Player feedbackPlayer;
    private RectTransform _rectTransform;
    public RectTransform _bottomLeftLimit;
    public RectTransform _topRightLimit;
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _multiplierLabel = GetComponent<TextMeshProUGUI>();
        timer = 0.0f;
    }

    void Update()
    {
        if (timer >= 0.0f)
        {
            var color = _multiplierLabel.color;
            if (timer < 1.0f)
            {
                color.a = Mathf.Lerp(_multiplierLabel.color.a, 0.0f, Time.deltaTime);
            }
            _multiplierLabel.color = color;
            timer -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ShowMultiplier(float multiplier, Color32 textColor)
    {
        var text = "x" + multiplier.ToString("0.0");
        _multiplierLabel.text = text;
        _multiplierLabel.color = textColor;
        var color = _multiplierLabel.color;
        color.a = 1;
        _multiplierLabel.color = color;
        timer = 3.0f;
        SpawnAtRandomPosition();
        feedbackPlayer.PlayFeedbacks();
    }

    public void SpawnAtRandomPosition()
    {
        var x = Random.Range(_bottomLeftLimit.anchoredPosition.x, _topRightLimit.anchoredPosition.x);
        var y = Random.Range(_bottomLeftLimit.anchoredPosition.y, _topRightLimit.anchoredPosition.y);
        var rotation = Random.Range(-30.0f, 30.0f);
        _rectTransform.rotation = Quaternion.Euler(_rectTransform.rotation.x, _rectTransform.rotation.y, rotation);
        _rectTransform.anchoredPosition = new Vector2(x, y);
        gameObject.SetActive(true);
    }
}
