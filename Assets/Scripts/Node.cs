using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    [SerializeField]
    private Color startColor;
    [SerializeField]
    private Color endColor;

    [SerializeField]
    private Material outlineMaterial;
    [SerializeField]
    private float outlineWidth;
    [SerializeField]
    private Color outlineColor;

    private float mFade = 0.0f;
    private Renderer sphereRenderer;
    private Renderer outlineRenderer;
    private Coroutine sphereCoroutine;

    private Vector3 screenPoint;
    private Vector3 offset;


    private void Start()
    {
        GameObject test = new GameObject("Test");
        test.SetActive(false);

        test.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
        test.AddComponent<MeshRenderer>().material = outlineMaterial;
        test.transform.SetPositionAndRotation(transform.position, transform.rotation);
        test.transform.parent = transform;

        test.SetActive(true);


        sphereRenderer = GetComponent<Renderer>();
        sphereRenderer.material.SetColor("_Color", startColor);

        outlineRenderer = test.GetComponent<Renderer>();
        outlineRenderer.material.SetFloat("_Opacity", 0.0f);
        outlineRenderer.material.SetFloat("_OutlineWidth", 0.0f);
        outlineRenderer.material.SetColor("_OutlineColor", outlineColor);

    }

    private void OnMouseEnter()
    {
        StartFadeCoroutine("in");
    }

    private void OnMouseExit()
    {
        StartFadeCoroutine("out");
    }

    private void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    private void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    private void StartFadeCoroutine(string fadeType)
    {
        if (sphereCoroutine != null)
            StopCoroutine(sphereCoroutine);
        sphereCoroutine = StartCoroutine(FadeColorAndOutline(fadeType, 0.01f));
    }

    private IEnumerator FadeColorAndOutline(string fadeType, float repeatRate)
    {
        while(true)
        {
            if (fadeType == "in" && mFade >= 1.0f)
                yield break;
            if (fadeType == "out" && mFade <= 0.0f)
                yield break;

            if (fadeType == "in")
                mFade += 0.05f;
            else
                mFade -= 0.05f;

            Color lerpedColor = Color.Lerp(startColor, endColor, mFade);
            sphereRenderer.material.SetColor("_BaseColor", lerpedColor);

            outlineRenderer.material.SetFloat("_Opacity", mFade);
            outlineRenderer.material.SetFloat("_OutlineWidth", mFade * outlineWidth);

            yield return new WaitForSeconds(repeatRate);
        }
    }
}
