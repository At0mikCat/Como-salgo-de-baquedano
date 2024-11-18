using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InstancingEntities : MonoBehaviour
{
    [SerializeField] int sizeX = 30; 
    [SerializeField] int sizeZ = 30;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;

    private Matrix4x4[] matrices;

    [SerializeField] Vector3 offset; 
    [SerializeField] float spacing = 2.0f;
    [SerializeField] float randomOffsetRange = 0.5f;

    private Vector3[] randomOffsets;

    private void OnValidate()
    {
        int totalInstances = sizeX * sizeZ;
        matrices = new Matrix4x4[totalInstances];
        randomOffsets = new Vector3[totalInstances];

        for (int i = 0; i < totalInstances; i++)
        {
            float randomX = Random.Range(-randomOffsetRange, randomOffsetRange);
            float randomZ = Random.Range(-randomOffsetRange, randomOffsetRange);
            randomOffsets[i] = new Vector3(randomX, 0, randomZ);
        }
    }
    void Update()
    {
        matrices = new Matrix4x4[sizeX * sizeZ];
        int i = 0;

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                Vector3 basePosition = new Vector3(x * spacing, 0, z * spacing) + offset;
                Vector3 positionWithOffset = basePosition + randomOffsets[i];
                matrices[i] = Matrix4x4.TRS(positionWithOffset, Quaternion.identity, Vector3.one);
                i++;
            }
        }

        Graphics.DrawMeshInstanced(mesh, 0, material, matrices);
    }
}
