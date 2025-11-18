using UnityEngine;

public class NpcChunks : MonoBehaviour
{
    public Rigidbody[] rbs;

    public float testForce = 1000f;
    public Transform testT;


    public void MoveChunks(Vector3 attackDir)
    {

        foreach (Rigidbody rb in rbs)
        {
            rb.AddForce(attackDir * testForce + Vector3.up * 20, ForceMode.Impulse);
        }
    }
}
