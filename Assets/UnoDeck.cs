using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnoDeck : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDynamicSelectable
{
    public UnoController unoController;
    public MeshRenderer topCardRenderer;
    public bool isDrawDeck = true;
    public Material backMaterial;
    public Material[] cardMaterials;

    float initialYScale;
    float initialYPos;

    MeshRenderer deckRenderer;
    Outline outlineComponent;

    public void Awake()
    {
        initialYScale = transform.localScale.y;
        initialYPos = transform.position.y;
        topCardRenderer.material = backMaterial;
        deckRenderer = GetComponent<MeshRenderer>();

        outlineComponent = gameObject.GetComponent<Outline>();
        outlineComponent.enabled = false;
        outlineComponent.OutlineWidth = 10f;
        outlineComponent.OutlineColor = Color.yellow;
        outlineComponent.OutlineMode = Outline.Mode.OutlineVisible;
    }

    public void Update()
    {
        if (!unoController.IsSpawned) return;

        var deckSize = isDrawDeck ? unoController.DrawDeck.Count : unoController.DiscardDeck.Count;
        var percent = deckSize / (float)UnoController.DECK_SIZE;
        ChangeHeight(percent);

        if (!isDrawDeck)
        {
            var deckList = unoController.DiscardDeck;
            if (deckList.Count == 0)
            {
                ToggleVisibility(false);
            }
            else
            {
                ToggleVisibility(true);
                UpdateCardMaterial(deckList.Last());
            }
        }
    }

    void ToggleVisibility(bool visible)
    {
        if (visible)
        {
            if (!deckRenderer.enabled) deckRenderer.enabled = true;
            // if (!topCardRenderer.enabled) topCardRenderer.enabled = true;
        }
        else
        {
            if (deckRenderer.enabled) deckRenderer.enabled = false;
            // if (topCardRenderer.enabled) topCardRenderer.enabled = false;
        }
    }

    void ChangeHeight(float percent)
    {
        transform.localScale = new Vector3(transform.localScale.x, percent * initialYScale, transform.localScale.z);
        float newY = initialYPos - initialYScale * (1 - percent) * 0.5f;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var player = PlayerNetworkController.localPlayer;
        if (isDrawDeck)
        {
            var card = unoController.DrawCard();
            player.unoHand.AddCard(card);
        }
        else if (player.unoHand.CardCount > 0)
        {
            var card = player.unoHand.PlayCard();
            unoController.PlayCard(card);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outlineComponent.enabled = unoController.LocalPlaying;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outlineComponent.enabled = false;
    }

    public bool ShouldBeSelected(PointerEventData pointerEventData)
    {
        return unoController.LocalPlaying && (Input.GetKeyDown(KeyCode.E) || ControllerInputHelper.IsXButtonDown());
    }

    void UpdateCardMaterial(UnoCard card)
    {
        topCardRenderer.material = cardMaterials[(int)card.type];
        topCardRenderer.material.color = card.color switch
        {
            CardColor.BLUE => Color.cyan,
            CardColor.RED => Color.red,
            CardColor.YELLOW => Color.yellow,
            CardColor.GREEN => Color.green,
            _ => Color.white,
        };
    }
}
