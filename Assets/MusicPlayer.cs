using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright (C) Xenfinity LLC - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bilal Itani <bilalitani1@gmail.com>, June 2017
 */

public class MusicPlayer : MonoBehaviour {

    #region Attributes

    // All Audio Clips that can be used as music tracks.
    public List<AudioClip> musicClips = new List<AudioClip>();

    public MusicQueue musicQueue;

    // 2D music player.
    public static AudioSource music;

    // Audio currently playing.
    AudioClip currentTrack;

    // Length of the current track; used to know when to play next
    private float length;

    private Coroutine musicLoop;

    private AudioSource musicSource;

    #endregion

    void Start()
    {
        musicQueue = new MusicQueue(musicClips);

        musicSource = GetComponent<AudioSource>();

        StartMusic();
    }

    #region Audio Playing

    public void PlayMusicClip(AudioClip music)
    {
        musicSource.Stop();
        musicSource.clip = music;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicLoop != null)
            StopCoroutine(musicLoop);

        music.Stop();
    }

    public void StartMusic()
    {
        musicLoop = StartCoroutine(musicQueue.LoopMusic(this, 0, PlayMusicClip));
    }

    #endregion
}

// Queue of multiple audioclips that automatically randomizes and may repeat with given delay
public class MusicQueue
{
    List<AudioClip> clips;

    public MusicQueue(List<AudioClip> clips)
    {
        this.clips = clips;
    }

    public IEnumerator LoopMusic(MonoBehaviour player, float delay, System.Action<AudioClip> playFunction)
    {
        while(true)
        {
            yield return player.StartCoroutine(Run(RandomizeList(clips), delay, playFunction));
        }
    }

    // Runs all music clips, then repeats if desired
    public IEnumerator Run(List<AudioClip> tracks,
        float delay, System.Action<AudioClip> playFunction)
    {        
        // Run all clips
        foreach (AudioClip clip in tracks)
        {
            // play
            playFunction(clip);

            // Wait until the clip is done, and delay between clips is over
            yield return new WaitForSeconds(clip.length + delay);
        }
    }

    public List<AudioClip> RandomizeList(List<AudioClip> list)
    {
        List<AudioClip> copy = new List<AudioClip>(list);

        int n = copy.Count;

        // what we do here is grab any random track,
        // then set the last track in the copy to be that track,
        // then we remove the last track from the list of tracks we need to change.

        // basically, we move from largest index to smallest,
        // setting the current index to a random clip from the smallest index
        // and up to the largest index that has not been set
        while (n > 1)
        {
            n--;

            // exclusive int range, add one since we remove one earlier
            int k = Random.Range(0, n + 1);

            // store temporary value
            AudioClip value = copy[k];

            // swap without overwrite
            copy[k] = copy[n];
            copy[n] = value;
        }

        return copy;
    }
}
