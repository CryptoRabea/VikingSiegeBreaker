using UnityEngine;
using System.Collections.Generic;

namespace VikingSiegeBreaker.Managers
{
    /// <summary>
    /// Central audio management system - handles music and SFX playback.
    /// Supports volume control, muting, and audio pooling.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private GameObject sfxSourcePrefab; // For pooling multiple SFX

        [Header("Audio Clips")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameplayMusic;

        [Header("Volume")]
        [SerializeField][Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField][Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;

        [Header("Settings")]
        [SerializeField] private bool musicMuted = false;
        [SerializeField] private bool sfxMuted = false;
        [SerializeField] private int sfxPoolSize = 5;

        // SFX Pool
        private List<AudioSource> sfxPool = new List<AudioSource>();
        private Dictionary<string, AudioClip> sfxLibrary = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            // Initialize SFX pool
            InitializeSFXPool();

            // Load audio clips from Resources
            LoadAudioClips();

            // Load volume settings
            LoadVolumeSettings();

            // Play menu music
            PlayMusic("Menu");
        }

        #region Initialization

        private void InitializeSFXPool()
        {
            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject obj = new GameObject($"SFX_Pool_{i}");
                obj.transform.SetParent(transform);
                AudioSource source = obj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                sfxPool.Add(source);
            }
        }

        private void LoadAudioClips()
        {
            // Load all SFX from Resources/Audio/SFX
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/SFX");
            foreach (var clip in clips)
            {
                sfxLibrary[clip.name] = clip;
            }

            Debug.Log($"[AudioManager] Loaded {sfxLibrary.Count} SFX clips");

            // Load music
            if (menuMusic == null)
                menuMusic = Resources.Load<AudioClip>("Audio/Music/MenuTheme");

            if (gameplayMusic == null)
                gameplayMusic = Resources.Load<AudioClip>("Audio/Music/GameplayTheme");
        }

        #endregion

        #region Music

        /// <summary>
        /// Plays a music track by name.
        /// </summary>
        public void PlayMusic(string trackName)
        {
            if (musicMuted) return;

            AudioClip clip = trackName.ToLower() switch
            {
                "menu" => menuMusic,
                "gameplay" => gameplayMusic,
                _ => null
            };

            if (clip != null)
            {
                if (musicSource.clip == clip && musicSource.isPlaying)
                    return; // Already playing

                musicSource.clip = clip;
                musicSource.volume = musicVolume * masterVolume;
                musicSource.Play();

                Debug.Log($"[AudioManager] Playing music: {trackName}");
            }
            else
            {
                Debug.LogWarning($"[AudioManager] Music track not found: {trackName}");
            }
        }

        /// <summary>
        /// Stops the current music.
        /// </summary>
        public void StopMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        /// <summary>
        /// Pauses the current music.
        /// </summary>
        public void PauseMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }

        /// <summary>
        /// Resumes paused music.
        /// </summary>
        public void ResumeMusic()
        {
            if (!musicSource.isPlaying)
            {
                musicSource.UnPause();
            }
        }

        #endregion

        #region SFX

        /// <summary>
        /// Plays a sound effect by name.
        /// </summary>
        public void PlaySFX(string sfxName)
        {
            if (sfxMuted) return;

            if (sfxLibrary.TryGetValue(sfxName, out AudioClip clip))
            {
                PlaySFX(clip);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] SFX not found: {sfxName}");
            }
        }

        /// <summary>
        /// Plays a sound effect from an AudioClip.
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null || sfxMuted) return;

            // Get available audio source from pool
            AudioSource source = GetAvailableSFXSource();
            if (source != null)
            {
                source.clip = clip;
                source.volume = sfxVolume * masterVolume * volumeMultiplier;
                source.Play();
            }
        }

        /// <summary>
        /// Plays a one-shot SFX (doesn't interrupt other sounds).
        /// </summary>
        public void PlayOneShot(string sfxName, float volumeMultiplier = 1f)
        {
            if (sfxMuted) return;

            if (sfxLibrary.TryGetValue(sfxName, out AudioClip clip))
            {
                sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeMultiplier);
            }
        }

        private AudioSource GetAvailableSFXSource()
        {
            // Find an available source
            foreach (var source in sfxPool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // If all busy, use the first one (will interrupt)
            return sfxPool[0];
        }

        #endregion

        #region Volume Control

        /// <summary>
        /// Sets the master volume.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
        }

        /// <summary>
        /// Sets the music volume.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume * masterVolume;
            PlayerPrefs.SetFloat(Core.SaveSystem.KEY_MUSIC_VOLUME, musicVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the SFX volume.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(Core.SaveSystem.KEY_SFX_VOLUME, sfxVolume);
            PlayerPrefs.Save();
        }

        private void UpdateAllVolumes()
        {
            musicSource.volume = musicVolume * masterVolume;
        }

        private void LoadVolumeSettings()
        {
            musicVolume = PlayerPrefs.GetFloat(Core.SaveSystem.KEY_MUSIC_VOLUME, 0.7f);
            sfxVolume = PlayerPrefs.GetFloat(Core.SaveSystem.KEY_SFX_VOLUME, 1f);
            UpdateAllVolumes();
        }

        #endregion

        #region Muting

        /// <summary>
        /// Mutes/unmutes music.
        /// </summary>
        public void SetMusicMuted(bool muted)
        {
            musicMuted = muted;
            musicSource.mute = muted;
        }

        /// <summary>
        /// Mutes/unmutes SFX.
        /// </summary>
        public void SetSFXMuted(bool muted)
        {
            sfxMuted = muted;
            foreach (var source in sfxPool)
            {
                source.mute = muted;
            }
        }

        /// <summary>
        /// Mutes/unmutes all audio.
        /// </summary>
        public void SetMuteAll(bool muted)
        {
            SetMusicMuted(muted);
            SetSFXMuted(muted);
        }

        #endregion

        #region Public Getters

        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;
        public bool IsMusicMuted() => musicMuted;
        public bool IsSFXMuted() => sfxMuted;

        #endregion
    }
}
