using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PowerUpsUI : MonoBehaviour
{
    [SerializeField] public Button lidButton;
    [SerializeField] public Button popAllButton;
    [SerializeField] public Button lowerLevelButton;

    [SerializeField] public List<Sprite> lidButtonSpries;
    [SerializeField] public List<Sprite> popAllButtonSprites;
    [SerializeField] public List<Sprite> lowerLevelButtonSprites;

    [SerializeField] public TextMeshProUGUI lidPowerUpCounter;
    [SerializeField] public TextMeshProUGUI popAllPowerUpCounter;
    [SerializeField] public TextMeshProUGUI lowerLevelButtonCounter;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
