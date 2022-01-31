using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LinkUIToObject : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.rotation = Camera.main.transform.rotation;
        rectTransform.localPosition = offset;
    }
}
