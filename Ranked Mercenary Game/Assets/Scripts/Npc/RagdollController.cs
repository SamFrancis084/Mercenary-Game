using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RagdollController : MonoBehaviour
{
    [SerializeField] Collider parentCollider;
    [SerializeField] List<Collider> rdCols = new List<Collider>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InitColliders();
    }

    void InitColliders()
    {
        parentCollider = GetComponentInParent<Collider>();

        Collider[] newCols = GetComponentsInChildren<Collider>();
        foreach (Collider col in newCols)
        {
            if (col.GetComponent<Rigidbody>() != null) col.GetComponent<Rigidbody>().isKinematic = true;
            col.enabled = false;
            rdCols.Add(col);
        }
    }

    public void DisableRagdoll()
    {
        parentCollider.enabled = true;

        for (int i = 0; i < rdCols.Count; i++)
        {
            if (rdCols[i].GetComponent<Rigidbody>() != null) rdCols[i].GetComponent<Rigidbody>().isKinematic = true;
            rdCols[i].enabled = false;
        }
    }
    public void EnableRagdoll(Vector3 dir = new Vector3(), float force = 0f)
    {
        parentCollider.enabled = false;

        List<Rigidbody> rbs = new List<Rigidbody>();
        for (int i = 0; i < rdCols.Count; i++)
        {
            rdCols[i].enabled = true;
            if (rdCols[i].GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = rdCols[i].GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rbs.Add(rb);
            }
        }

        //push rbs
        if (force > 0f)
        {
            foreach (Rigidbody rb in rbs)
            {
                rb.AddForce(dir * force, ForceMode.Impulse);
            }
        }
    }
}
