using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        Debug.Log("MemoryCard awake on: " + gameObject.name);
        interactable = GetComponent<XRSimpleInteractable>();

        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.selectEntered.AddListener(OnSelectEntered);
        }
        else
        {
            Debug.LogWarning("No XRSimpleInteractable found on " + gameObject.name);
        }
    }

    private void Start()
    {
        ShowBack();
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("MOUSE CLICKED CARD: " + gameObject.name);
        SelectCard();
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("XR HOVER ENTERED: " + gameObject.name);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("XR SELECT ENTERED: " + gameObject.name);
        SelectCard();
    }

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