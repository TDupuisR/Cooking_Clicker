using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private GameObject m_noisePrefab;

    public void SpawnSound(AudioClip clip)
    {
        GameObject newSound = Instantiate(m_noisePrefab, Vector3.zero, Quaternion.identity);
        newSound.GetComponent<AudioSource>().clip = clip;
        newSound.GetComponent<AudioSource>().Play();
        Destroy(newSound, clip.length);
    }
}
