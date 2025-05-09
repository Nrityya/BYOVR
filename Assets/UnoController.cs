using System;
using Fusion;
using UnityEngine;

public class UnoController : NetworkBehaviour
{
    public GameObject joinButton;
    public GameObject exitButton;

    public bool LocalPlaying { get; private set; } = false;
    public bool IsSpawned { get; private set; } = false;

    public const int DECK_SIZE = 108;

    [Networked]
    [Capacity(DECK_SIZE)]
    public NetworkLinkedList<UnoCard> DrawDeck { get; }

    [Networked]
    [Capacity(DECK_SIZE)]
    public NetworkLinkedList<UnoCard> DiscardDeck { get; }

    [Networked]
    int PlayerCount { get; set; } = 0;

    readonly static string tooltip = "X - Draw/Play Card\nA - Select Left Card\nB - Select Right Card";

    public override void Spawned()
    {
        IsSpawned = true;
        ResetDecks();
    }

    public void LocalPlayerJoin()
    {
        exitButton.SetActive(true);
        joinButton.SetActive(false);
        LocalPlaying = true;
        PlayerNetworkController.localPlayer.movementEnabled = false;
        ToolTipController.PushTooltip(tooltip);
        RpcPlayerJoin();
    }

    public void LocalPlayerExit()
    {
        exitButton.SetActive(false);
        joinButton.SetActive(true);
        LocalPlaying = false;
        PlayerNetworkController.localPlayer.movementEnabled = true;
        var cards = PlayerNetworkController.localPlayer.unoHand.Clear();
        ToolTipController.PopTooltip(tooltip);
        RpcPlayerExit(cards.ToArray());
    }

    public UnoCard DrawCard()
    {
        var idx = UnityEngine.Random.Range(0, DrawDeck.Count);
        var card = DrawDeck[idx];
        RpcDrawCard(card);
        return card;
    }

    public void PlayCard(UnoCard card)
    {
        RpcDiscardCard(card);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcPlayerJoin()
    {
        PlayerCount++;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcPlayerExit(UnoCard[] cards)
    {
        PlayerCount--;
        foreach (var card in cards)
        {
            DrawDeck.Add(card);
        }
        if (PlayerCount == 0) ResetDecks();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcDrawCard(UnoCard card)
    {
        DrawDeck.Remove(card);
        if (DrawDeck.Count == 0)
        {
            ResetDecks(true);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcDiscardCard(UnoCard card)
    {
        DiscardDeck.Add(card);
    }

    void ResetDecks(bool fromDiscard = false)
    {
        if (fromDiscard)
        {
            foreach (var card in DiscardDeck)
            {
                DrawDeck.Add(card);
            }
        }
        else
        {
            DrawDeck.Clear();
            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
                {
                    if (color < CardColor.WILD && cardType >= CardType.PLUS_FOUR) continue;
                    if (color == CardColor.WILD && cardType < CardType.PLUS_FOUR) continue;
                    int amount = 1;
                    if (color != CardColor.WILD)
                    {
                        if (cardType != CardType.ZERO) amount = 2; // Normal Cards
                    }
                    else
                    {
                        amount = 4; // Wilds
                    }
                    for (int i = 0; i < amount; i++)
                    {
                        DrawDeck.Add(new UnoCard()
                        {
                            color = color,
                            type = cardType,
                        });
                    }
                }
            }
        }
        DiscardDeck.Clear();
    }
}

public enum CardColor
{
    RED = 0,
    YELLOW = 1,
    BLUE = 2,
    GREEN = 3,
    WILD = 4,
}

public enum CardType
{
    ZERO = 0,
    ONE = 1,
    TWO = 2,
    THREE = 3,
    FOUR = 4,
    FIVE = 5,
    SIX = 6,
    SEVEN = 7,
    EIGHT = 8,
    NINE = 9,
    SKIP = 10,
    REVERSE = 11,
    PLUS_TWO = 12,
    PLUS_FOUR = 13,
    WILD = 14,
}

public struct UnoCard : INetworkStruct
{
    public CardColor color;
    public CardType type;
}