using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextGoBrr : MonoBehaviour
{
    private TextMeshPro textMeshPro; 
    public float minScale = 0.8f; 
    public float maxScale = 1.2f; 
    public float flickerSpeed = 10f; 

    private Vector3 originalScale;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();

        originalScale = transform.localScale; 
    }

    void Update()
    {
        float scale = Mathf.Lerp(minScale, maxScale, Mathf.PerlinNoise(Time.time * flickerSpeed, 0));
        transform.localScale = originalScale * scale; 
    }
}
