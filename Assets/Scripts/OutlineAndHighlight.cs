using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class OutlineAndHighlight : MonoBehaviour
{
    [SerializeField]
    private bool highlightOnHover = true;
    [SerializeField]
    private bool outlineOnHover = true;

    // Node fade colors
    [SerializeField]
    private Color startColor = Color.white;
    [SerializeField]
    private Color endColor = new Color(1f, 0.42f, 0f);

    // Node outline parameters
    [SerializeField]
    private Material outlineMaterial;
    [SerializeField]
    private float outlineWidth = 0.025f;
    [SerializeField]
    private Color outlineColor = Color.white;
    [SerializeField]
    private Color outlineSelectedColor = new Color(1f, 0.35f, 0f);

    // Node fade parameters
    private float mFade = 0.0f;
    private Renderer sphereRenderer;
    private Renderer outlineRenderer;
    private Coroutine sphereCoroutine;

    private void Start()
    {
        if (highlightOnHover)
        {
            // Initialise node colour
            sphereRenderer = GetComponent<Renderer>();
            sphereRenderer.material.SetColor("_Color", startColor);
        }

        if (outlineOnHover)
        {
            // Create node outline object
            GameObject test = new GameObject("Test");
            test.SetActive(false);

            // Supply with key components
            test.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
            test.AddComponent<MeshRenderer>().material = outlineMaterial;
            test.transform.SetPositionAndRotation(transform.position, transform.rotation);
            test.transform.parent = transform;

            test.SetActive(true);

            // Initialise outline parameters
            outlineRenderer = test.GetComponent<Renderer>();
            outlineRenderer.material.SetFloat("_Opacity", 0.0f);
            outlineRenderer.material.SetFloat("_OutlineWidth", 0.0f);
            outlineRenderer.material.SetColor("_OutlineColor", outlineColor);
        }
    }
    private void OnMouseEnter()
    {
        if (highlightOnHover || outlineOnHover)
            StartFadeCoroutine("in");
    }

    private void OnMouseExit()
    {
        if (highlightOnHover || outlineOnHover)
            StartFadeCoroutine("out");
    }

    private void OnMouseDown()
    {
        if (outlineOnHover)
            outlineRenderer.material.SetColor("_OutlineColor", outlineSelectedColor);
    }

    private void OnMouseUp()
    {
        if (outlineOnHover)
            outlineRenderer.material.SetColor("_OutlineColor", outlineColor);
    }

    private void StartFadeCoroutine(string fadeType)
    {
        if (sphereCoroutine != null)
            StopCoroutine(sphereCoroutine);
        sphereCoroutine = StartCoroutine(FadeColorAndOutline(fadeType, 0.01f));
    }

    private IEnumerator FadeColorAndOutline(string fadeType, float repeatRate)
    {
        float fadeAmount = 0.05f;
        if (fadeType == "out")
            fadeAmount = -0.05f;

        while (true)
        {
            if (fadeType == "in" && mFade >= 1.0f)
                yield break;
            if (fadeType == "out" && mFade <= 0.0f)
                yield break;

            mFade += fadeAmount;

            if (highlightOnHover)
            {
                Color lerpedColor = Color.Lerp(startColor, endColor, mFade);
                sphereRenderer.material.SetColor("_BaseColor", lerpedColor);
            }
            
            if (outlineOnHover)
            {
                outlineRenderer.material.SetFloat("_Opacity", mFade);
                outlineRenderer.material.SetFloat("_OutlineWidth", mFade * outlineWidth);
            }

            yield return new WaitForSeconds(repeatRate);
        }
    }
}
