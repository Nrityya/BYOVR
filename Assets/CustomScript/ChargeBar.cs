using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ChargeBar : MonoBehaviour
{
    private GameObject chargeBarPrefab;
    private GameObject bar = null;
    private Image chargeBarComponent = null;
    private Camera playerCamera = null;
    private AudioSource audioSource;
    bool maxChargeReached;

    void Start()
    {
        chargeBarPrefab = Resources.Load<GameObject>("ChargeBar");
    }

    public void Setup(Camera camera)
    {
        maxChargeReached = false;
        bar = Instantiate(chargeBarPrefab, transform.position, Quaternion.identity, transform);
        bar.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f) / transform.localScale.x;
        chargeBarComponent = bar.transform.GetComponentsInChildren<Image>()[2];
        playerCamera = camera;
        audioSource = bar.GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>("Audio/charge");
        audioSource.volume = 0.7f;
        audioSource.Play();
        bar.GetComponentInChildren<ParticleSystem>().Stop();
    
    }

    public void UpdateCharge(float percent)
    {   
        
        if (bar == null) throw new Exception("Charge bar must be setup before it can be updated");

        if(!maxChargeReached && percent >= 0.95f){
            audioSource.Stop();
            audioSource.clip = Resources.Load<AudioClip>("Audio/max charge");
            audioSource.pitch = 2;
            audioSource.Play();
            maxChargeReached = true;
            bar.GetComponentInChildren<ParticleSystem>().Play();
        }

        float t = Mathf.Lerp(1, 0, percent);
        float shakeSpeed = 0.008f * (1 - t);
        bar.transform.position = transform.position + playerCamera.transform.right * Random.Range(-shakeSpeed, shakeSpeed) + playerCamera.transform.up * Random.Range(-shakeSpeed, shakeSpeed);
        bar.transform.LookAt(playerCamera.transform.position);
        bar.transform.Rotate(0, 180, 0);

        Color c = Color.yellow;
        c.g = t;
        chargeBarComponent.color = c;
        chargeBarComponent.fillAmount = 1 - t;

    }

    public void Cleanup()
    {
        audioSource.Stop();
        if (bar)
        {
            Destroy(bar);
            bar = null;
        }
        chargeBarComponent = null;
        playerCamera = null;
    }
}
