using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LinkUIToObject : MonoBehaviour
{
    public GameObject parent;

    [SerializeField]
    private Vector2 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(parent.transform.position) + new Vector3(offset.x, offset.y, 0f);
    }
}
