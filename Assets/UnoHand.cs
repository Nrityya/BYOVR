using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnoHand : MonoBehaviour
{
    public GameObject cardPrefab;
    public Canvas handCanvas;
    public Texture[] cardTextures;

    public int CardCount => unoCards.Count;

    List<UnoCard> unoCards = new();
    List<RawImage> cardImages = new();
    int selectedCard = 0;

    void Update()
    {
        if (cardImages.Count == 0 || PlayerNetworkController.localPlayer.movementEnabled) return;

        int cardChange = 0;
        if (ControllerInputHelper.IsYButtonDown() || Input.GetKeyDown(KeyCode.D)) cardChange -= 1;
        if (ControllerInputHelper.IsADown() || Input.GetKeyDown(KeyCode.A)) cardChange += 1;
        selectedCard += cardChange;
        if (selectedCard >= cardImages.Count) selectedCard = 0;
        if (selectedCard < 0) selectedCard = cardImages.Count - 1;

        var angleRange = Math.Min(180, (cardImages.Count - 1) * 15);
        for (int i = 0; i < cardImages.Count; i++)
        {
            var card = cardImages[i];
            var z = Mathf.Lerp(-angleRange, angleRange, (i + 1) / (float)cardImages.Count);
            var angles = card.transform.rotation.eulerAngles;
            card.transform.rotation = Quaternion.Euler(angles.x, angles.y, z);
            card.transform.localPosition = new Vector3(50,-75,1);
            float alpha = 0.75f;
            if (i == selectedCard)
            {
                card.transform.localPosition = new Vector3(50,-75,-30);
                card.transform.SetAsLastSibling();
                alpha = 1;
            }
            card.color = new Color(card.color.r, card.color.g, card.color.b, alpha);
        }
    }

    public void AddCard(UnoCard card)
    {
        var obj = Instantiate(cardPrefab, handCanvas.gameObject.transform);
        var img = obj.GetComponent<RawImage>();
        img.texture = cardTextures[(int)card.type];
        img.color = card.color switch
        {
            CardColor.BLUE => Color.cyan,
            CardColor.RED => Color.red,
            CardColor.YELLOW => Color.yellow,
            CardColor.GREEN => Color.green,
            _ => Color.white,
        };
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0.6f);
        unoCards.Add(card);
        cardImages.Add(img);
    }

    public UnoCard PlayCard()
    {
        var card = unoCards[selectedCard];
        unoCards.RemoveAt(selectedCard);
        Destroy(cardImages[selectedCard]);
        cardImages.RemoveAt(selectedCard);
        return card;
    }

    public List<UnoCard> Clear()
    {
        var cards = unoCards;
        cardImages = new();
        foreach (var img in cardImages)
        {
            Destroy(img);
        }
        unoCards = new();
        return cards;
    }
}
