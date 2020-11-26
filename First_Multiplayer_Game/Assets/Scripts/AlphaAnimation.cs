using System.Collections;
using TMPro;
using UnityEngine;

public class AlphaAnimation : MonoBehaviour
{
    TextMeshProUGUI _text;

    // Start is called before the first frame update
    void Start()
    {
        _text = transform.GetComponent<TextMeshProUGUI>();
        StartCoroutine("FadeInOut");
    }

    private IEnumerator FadeInOut()
    {
        float alpha = 0.1f;
        Color tmpColor = _text.GetComponent<TextMeshProUGUI>().color;
        tmpColor.a = alpha;
        _text.GetComponent<TextMeshProUGUI>().color = tmpColor;  // Reset the alpha to 0.1


        while (alpha < 1.1)
        {
            GetComponent<TextMeshProUGUI>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, alpha); //startAlpha = 0 <-- value is in tmp.a  // Mathf.Lerp(tmp.a, 255, _progress)
            alpha += Time.deltaTime * 1.25f;
            //Debug.Log("alpha: " + alpha);
            yield return null;
        }

        while (alpha > 0.1)
        {
            GetComponent<TextMeshProUGUI>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, alpha); //startAlpha = 1 <-- value is in tmp.a  // Mathf.Lerp(tmp.a, 0, _progress)
            alpha -= Time.deltaTime * 1.25f;
            //Debug.Log("alpha: " + alpha);
            yield return null;
        }

        StopCoroutine("FadeInOut");
        StartCoroutine("FadeInOut");
    }
}