using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_FontAsset fontNotHover;
    public TMP_FontAsset fontOnHover;
    public TMP_Text text;

    private void Start()
    {
        text.font = fontNotHover;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.font = fontOnHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.font = fontNotHover;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        text.font = fontNotHover;

        StartCoroutine(ReactiveFont());
    }

    IEnumerator ReactiveFont()
    {
        yield return new WaitForSeconds(0.05f);
        
        text.font = fontOnHover;
    }
}
