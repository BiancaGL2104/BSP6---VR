using System.Collections.Generic;
using UnityEngine;

public class AudioDistractorManager : MonoBehaviour
{
    [Header("Audio Anchors")]
    public List<Transform> audioZones = new List<Transform>();

    [Header("Audio Clips")]
    public AudioClip predictableClip;
    public AudioClip unpredictableClip;

    [Header("Audio Settings")]
    public float volume = 0.7f;
    public bool loopPredictable = true;
    public bool loopUnpredictable = true;

    [Header("Runtime")]
    public AudioSource activeAudioSource;

    [Header("Unpredictable Orbit Settings")]
    public Transform orbitCenter;
    public float orbitRadius = 2f;
    public float orbitSpeed = 40f;
    private GameObject orbitingAudioObject;
    private float currentAngle = 0f;
    private bool isOrbiting = false;

    public void StopAudioDistractor()
    {
        isOrbiting = false;

        if (activeAudioSource != null)
        {
            activeAudioSource.Stop();
            activeAudioSource = null;
        }

        if (orbitingAudioObject != null)
        {
            Destroy(orbitingAudioObject);
            orbitingAudioObject = null;
        }

        Debug.Log("Audio distractor stopped");
    }

    public void PlayPredictableAtZone(int zoneIndex)
    {
        if (predictableClip == null) return;
        if (audioZones == null || audioZones.Count == 0) return;
        if (zoneIndex < 0 || zoneIndex >= audioZones.Count) return;

        StopAudioDistractor();

        Transform zone = audioZones[zoneIndex];

        GameObject audioObject = new GameObject("PredictableAudioSource");
        audioObject.transform.position = zone.position;
        audioObject.transform.parent = transform;

        activeAudioSource = audioObject.AddComponent<AudioSource>();
        audioObject.AddComponent<AudioDistractorLogger>();
        activeAudioSource.clip = predictableClip;
        activeAudioSource.volume = volume;
        activeAudioSource.loop = loopPredictable;
        activeAudioSource.spatialBlend = 1f;
        activeAudioSource.playOnAwake = false;
        activeAudioSource.minDistance = 1f;
        activeAudioSource.maxDistance = 10f;

        activeAudioSource.Play();

        Debug.Log("Predictable audio playing at zone: " + zone.name);
    }

    public void PlayUnpredictableOrbitAudio()
    {
        if (unpredictableClip == null) return;
        if (orbitCenter == null) return;

        StopAudioDistractor();

        orbitingAudioObject = new GameObject("UnpredictableOrbitAudioSource");
        orbitingAudioObject.transform.parent = transform;

        activeAudioSource = orbitingAudioObject.AddComponent<AudioSource>();
        orbitingAudioObject.AddComponent<AudioDistractorLogger>();
        activeAudioSource.clip = unpredictableClip;
        activeAudioSource.volume = volume;
        activeAudioSource.loop = true;
        activeAudioSource.spatialBlend = 1f;
        activeAudioSource.playOnAwake = false;
        activeAudioSource.minDistance = 1f;
        activeAudioSource.maxDistance = 10f;

        currentAngle = 0f;
        isOrbiting = true;
        activeAudioSource.Play();

        Debug.Log("Unpredictable orbit audio started");
    }

    private void Update()
    {
        if (isOrbiting && orbitingAudioObject != null && orbitCenter != null)
        {
            currentAngle += orbitSpeed * Time.deltaTime;

            float radians = currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * orbitRadius;
            orbitingAudioObject.transform.position = orbitCenter.position + offset;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayPredictableAtZone(1);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            StopAudioDistractor();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayUnpredictableOrbitAudio();
        }
    }





}


