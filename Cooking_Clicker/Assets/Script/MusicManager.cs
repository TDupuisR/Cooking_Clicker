using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource m_musicSource;
    [SerializeField] List<AudioClip> m_musicList;

    private void Update()
    {
        if (!m_musicSource.isPlaying)
        {
            m_musicSource.clip = m_musicList[Random.Range(0, m_musicList.Count)];
            m_musicSource.Play();
        }
    }
}
