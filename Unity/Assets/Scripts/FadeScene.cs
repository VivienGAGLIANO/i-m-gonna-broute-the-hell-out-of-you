using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScene : MonoBehaviour
{
    Image fadeImage;
    public float fadeTime;
    private float startTime;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        while (fadeImage.color.a > 0f)
        {
            Color tmp = fadeImage.color;
            float a = (Time.time - startTime) / fadeTime;
            tmp.a = Mathf.SmoothStep(1f, 0f, a);
            fadeImage.color = tmp;
            yield return 0;
        }

    }
}
