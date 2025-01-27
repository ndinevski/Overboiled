using UnityEngine;
using UnityEngine.Serialization;

public class LevelSwitch : MonoBehaviour
{
    [FormerlySerializedAs("Transform")] public Transform SwitchTransform;
    [SerializeField] private AudioSource LevelSwitchSource;
    [SerializeField] private AudioClip LevelSwitchSound;
    public float rotationAngle;
    public float targetAngle;
    public float TargetAngle
    {
        get => targetAngle;
        set
        {
            value += 4.0f;
            targetAngle = value;
            if (!Mathf.Approximately(rotationAngle, targetAngle))
            {
                LevelSwitchSource.clip = LevelSwitchSound;
                AudioManager.Instance.PlaySFXSource(LevelSwitchSource);
            }
        }
    }

    void Awake()
    {
        LevelSwitchSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        rotationAngle = 4.0f;
        targetAngle = 4.0f;
    }

    // Update is called once per frame
    void Update()
    {
        var rotation = SwitchTransform.localRotation;
    

        if (!Mathf.Approximately(rotationAngle, targetAngle))
        {
            rotationAngle = Mathf.LerpAngle(rotationAngle, targetAngle, Time.deltaTime * 7.0f);
        }
        SwitchTransform.localRotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotationAngle);
    }
}
