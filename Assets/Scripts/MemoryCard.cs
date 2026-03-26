using UnityEngine;

public class MemoryCard : MonoBehaviour
{
    [Header("Card Data")]
    public int cardId;
    public bool isFlipped = false;
    public bool isMatched = false;

    [Header("Visuals")]
    public Renderer cardRenderer;
    public Material backMaterial;
    public Material[] frontMaterials;

    [Header("References")]
    public MemoryGameManager gameManager;

    private void Start()
    {
        ShowBack();
    }

    // This is the method your debug buttons should call
    public void SelectCard()
    {
        Debug.Log("CARD SELECTED: " + gameObject.name);

        if (gameManager != null)
        {
            gameManager.SelectCard(this);
        }
        else
        {
            Debug.LogWarning("No gameManager assigned on " + gameObject.name);
        }   
    }

    public void Flip()
    {
        if (isMatched) return;

        isFlipped = !isFlipped;

        if (isFlipped)
        {
            ShowFront();
        }
        else
        {
            ShowBack();
        }
    }

    public void ShowFront()
    {
        if (cardRenderer != null && frontMaterials != null)
        {
            if (cardId >= 0 && cardId < frontMaterials.Length)
            {
                cardRenderer.material = frontMaterials[cardId];
            }
        }
    }

    public void ShowBack()
    {
        if (cardRenderer != null && backMaterial != null)
        {
            cardRenderer.material = backMaterial;
        }
    }

    public void SetMatched()
    {
        isMatched = true;
    }

    public void ResetCard()
    {
        isFlipped = false;
        isMatched = false;
        ShowBack();
    }
}