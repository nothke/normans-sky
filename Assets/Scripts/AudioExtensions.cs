// All these extension are initiated from an AudioClip instance
// eg. to play once: 
//          someAudioClip.PlayOnce(Vector3.zero);
//
// or to create a source:
//          AudioSource source = someAudioClip.CreateSource(Vector3.zero)

using UnityEngine;
//using System.Collections;
//using System;

namespace AudioExtensions
{
    public static class AudioExtensions
    {
        /// <summary>
        /// Plays a clip at position with random pitch
        /// </summary>
        public static void PlayOnce(this AudioClip clip, Vector3 position, Vector2 randomPitchRange, float volume = 1, float spread = 90, float minDistance = 1)
        {
            clip.PlayOnce(position, volume, Random.Range(randomPitchRange.x, randomPitchRange.y), spread, minDistance);
        }

        /// <summary>
        /// Plays a clip at a position.
        /// Creates a GameObject with AudioSource, plays the clip and destroys the object after playing finishes
        /// </summary>
        public static void PlayOnce(this AudioClip clip, Vector3 position, float volume = 1, float pitch = 1, float spread = 90, float minDistance = 1)
        {
            GameObject go = new GameObject("AudioTemp");
            go.transform.position = position;

            AudioSource source = go.AddComponent<AudioSource>();

            source.spatialBlend = 1; // makes the source 3d
            source.minDistance = minDistance; // magic value

            source.loop = false;
            source.clip = clip;
            source.volume = volume;

            source.pitch = pitch;
            pitch = source.pitch == 0 ? pitch = 0.001f : pitch = source.pitch;

            source.spread = spread;

            source.dopplerLevel = 0;

            source.Play();

            GameObject.Destroy(go, clip.length * (1 / pitch));
        }

        public static AudioSource CreateSource(this AudioClip clip, Transform at, bool loop = true, bool playAtStart = true,
                                        float minDistance = 1, float volume = 1, float pitch = 1, float spread = 1, float spatialBlend = 1)
        {
            GameObject go = new GameObject("AudioLoop");
            go.transform.parent = at;
            go.transform.localPosition = Vector3.zero;

            AudioSource source = go.AddComponent<AudioSource>();

            source.loop = loop;
            source.clip = clip;

            source.spatialBlend = spatialBlend;
            source.spread = spread;
            source.minDistance = minDistance;

            source.playOnAwake = playAtStart;

            return source;
        }
    }
}
