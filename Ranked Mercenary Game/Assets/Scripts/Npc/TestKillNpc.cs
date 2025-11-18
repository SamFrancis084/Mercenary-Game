using UnityEngine;

public class TestKillNpc : MonoBehaviour
{
    [SerializeField] BaseNPC targetNpc;
    [SerializeField] KeyCode killKey;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(killKey)) targetNpc.Die();
    }
}
