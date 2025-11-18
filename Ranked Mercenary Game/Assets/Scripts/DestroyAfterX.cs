using UnityEngine;

public class DestroyAfterX : MonoBehaviour
{
    [SerializeField] float lifeTime = 3f;
    float destroytimer = 0f;
    // Update is called once per frame
    void Update()
    {
        destroytimer += Time.deltaTime;

        if (destroytimer > lifeTime)
        {
            Destroy(gameObject); 
        }
    }
}
