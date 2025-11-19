using UnityEngine;

public static class AudioTools
{
    public static void PlayClipAtPoint(AudioClip clip, Vector3 point, float volume = 1.0f, float pitch = 1.0f, float spatialBlend = 0.0f)
    {
        GameObject gameObject = new GameObject("One Shot Audio");
        gameObject.transform.position = point;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = spatialBlend;
        audioSource.Play();
        Object.Destroy(gameObject, clip.length * pitch);
    }
}
