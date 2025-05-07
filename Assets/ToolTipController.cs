using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ToolTipController : MonoBehaviour
{
    private static readonly List<string> tooltipStack = new();
    private static readonly List<ToolTipController> instances = new();

    public Canvas tooltipCanvas;
    public TextMeshProUGUI tooltipText;

    void Start()
    {
        instances.Add(this);
        if (tooltipStack.Count == 0)
        {
            tooltipStack.Add(tooltipText.text);
        }
    }

    void OnDestroy()
    {
        instances.Remove(this);
    }

    void Update()
    {
        var buttonDown = Input.GetKeyDown(KeyCode.K) || ControllerInputHelper.IsMenuUp();
        if (!buttonDown) return;
        var isActive = tooltipCanvas.gameObject.activeSelf;
        tooltipCanvas.gameObject.SetActive(!isActive);
    }

    public static void PushTooltip(string tooltip)
    {
        tooltipStack.Add(tooltip);
        UpdateTextAll();
    }

    public static void PopTooltip()
    {
        PopTooltip(null);
    }

    public static void PopTooltip(string tooltip)
    {
        var idx = -1;
        if (string.IsNullOrEmpty(tooltip)) idx = tooltipStack.Count - 1;
        else
        {
            for (int i = tooltipStack.Count - 1; i >= 0; i--)
            {
                if (tooltipStack[i] == tooltip)
                {
                    idx = i;
                    break;
                }
            }
        }
        if (idx >= 0)
        {
            tooltipStack.RemoveAt(idx);
            UpdateTextAll();
        }
    }

    static void UpdateTextAll()
    {
        foreach (var i in instances)
        {
            i.UpdateText();
        }
    }

    void UpdateText()
    {
        tooltipText.text = tooltipStack.Last();
    }
}
