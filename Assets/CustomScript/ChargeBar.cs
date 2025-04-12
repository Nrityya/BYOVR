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

    void Start()
    {
        chargeBarPrefab = Resources.Load<GameObject>("ChargeBar");
    }

    public void Setup(Camera camera)
    {
        bar = Instantiate(chargeBarPrefab, transform.position, Quaternion.identity, transform);
        bar.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f) / transform.localScale.x;
        chargeBarComponent = bar.transform.GetComponentsInChildren<Image>()[2];
        playerCamera = camera;
    }

    public void UpdateCharge(float percent)
    {
        if (bar == null) throw new Exception("Charge bar must be setup before it can be updated");

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
        if (bar)
        {
            Destroy(bar);
            bar = null;
        }
        chargeBarComponent = null;
        playerCamera = null;
    }
}
