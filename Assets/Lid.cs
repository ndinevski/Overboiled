using System;
using UnityEngine;

public class Lid : MonoBehaviour
{
    public float timer = 0;
    private Animator lidAnimator; 
    
    void Start()
    {
        lidAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            lidAnimator.SetBool("isActive", false);
            timer = 0;
        }
    }

    private void OnEnable()
    {
        // animacija
        timer = Config.Instance.POWERUP_LID_TIMER;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bubble"))
        {
            other.gameObject.GetComponent<Bubble>().DestroyBubble(0.2f);
        }
    }

    public void Activate()
    {
        timer = Config.Instance.POWERUP_LID_TIMER;
        lidAnimator.SetBool("isActive", true);
    }
}
