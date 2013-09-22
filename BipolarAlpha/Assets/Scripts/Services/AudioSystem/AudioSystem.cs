//Made By: Engana
using UnityEngine;
using System.Collections.Generic;

enum Fade { NONE, OUT, IN }

/// <summary>
/// The Audio system is reponsible for playing all sounds during gameplay, be them sound effects or music
/// </summary>
public class AudioSystem : MonoBehaviour
{
  //Libraries loaded with all audio clips placed in SFX and Music folders on the Resources directory
  private Dictionary<string, AudioClip> _sfxLibrary = new Dictionary<string,AudioClip>();
  private Dictionary<string, AudioClip> _musicLibrary = new Dictionary<string,AudioClip>();

  //Curently instanced looping sound objects, for ambience sounds.
  //Key: <<SourceObject,sfxName>, SoundSourceObject>
  private Dictionary<KeyValuePair<Transform, string>, GameObject> _currentlyPlayingAudioObjects = new Dictionary<KeyValuePair<Transform, string>, GameObject>();

  //AudioMananger AudioSource Component, for playing background music
  private AudioSource _musicSource;

  private string _currentlyPlayingMusic = "";
  private string _nextMusicInQueue = "";
  private Fade _currentFadeEffect = Fade.NONE;

  [@SerializeField]
  private float _fadeSpeed = 1f;

  #region Public Play Sound Methods
  /// <summary>
  /// Plays a quick non-looping sound effect
  /// </summary>
  public void PlayQuickSFX(string sfxName, Vector3 onPosition = default(Vector3), float volume = 1f)
  {
    if (!_sfxLibrary.ContainsKey(sfxName))
    {
      throw new BipolarExceptionAudioClipNotFound("Audio Clip " + sfxName + " not found in SFX sound Library");
    }
    AudioSource.PlayClipAtPoint(_sfxLibrary[sfxName], onPosition, volume);
  }

  /// <summary>
  /// Plays a given sound effect in a loop until instructed otherwise
  /// </summary>
  public void PlayLoopingSFX(string sfxName, Transform objectEmittingSound, float volume = 1f)
  {
    if (!_sfxLibrary.ContainsKey(sfxName))
    {
      throw new BipolarExceptionAudioClipNotFound("Audio Clip " + sfxName + " not found in SFX sound Library");
    }

    //If the sound is already looping, do nothing
    if (_currentlyPlayingAudioObjects.ContainsKey(new KeyValuePair<Transform, string>(objectEmittingSound, sfxName)))
    {
      return;
    }

    //Creates an empty gameObject with only an AudioSource Component and places it at the objectEmmitingSound position as it's child object
    //This way we can have a looping sound effect without needing to mess with the source gameobject adding an audioSource
    GameObject source = ServiceLocator.GetResourceSystem().InstanceOf("Prefabs/SoundSource/AudioSource", objectEmittingSound.position);
    source.transform.position = objectEmittingSound.position;
    source.transform.parent = objectEmittingSound;

    source.name = sfxName;

    _currentlyPlayingAudioObjects[new KeyValuePair <Transform,string>(objectEmittingSound,sfxName)] = source;

    source.GetComponent<AudioSource>().clip = _sfxLibrary[sfxName];
    source.GetComponent<AudioSource>().Play();
  }

  /// <summary>
  /// Stops a previously set sfx loop
  /// </summary>
  public void StopLoopingSFX(string sfxName, Transform objectEmittingSound)
  {
    if (!_currentlyPlayingAudioObjects.ContainsKey(new KeyValuePair<Transform, string>(objectEmittingSound, sfxName)))
    {
      return;
    }
    //Destroys the previously instanced object made to play the audio clip
    GameObject.Destroy(_currentlyPlayingAudioObjects[new KeyValuePair<Transform, string>(objectEmittingSound, sfxName)]);
    _currentlyPlayingAudioObjects.Remove(new KeyValuePair<Transform, string>(objectEmittingSound, sfxName));
  }

  /// <summary>
  /// Plays background music
  /// </summary>
  public void PlayMusic(string musicName)
  {
    if (!_musicLibrary.ContainsKey(musicName))
    {
      throw new BipolarExceptionAudioClipNotFound("Audio Clip " + musicName + " not found in Music Library");
    }
    if (_currentlyPlayingMusic == musicName)
    {
      return;
    }

    if (_currentlyPlayingMusic == "")
    {
      //First song to be played
      //Sets the song as next in queue, and plays it, followed by a fade-in effect in case the volume is set to 0
      _nextMusicInQueue = musicName;
      PlayNextMusic();
      _currentFadeEffect = Fade.IN;
    }
    else
    {
      //Sets the audio manager to fade out the current tune, while scheduling the new one to play next
      _nextMusicInQueue = musicName;
      _currentFadeEffect = Fade.OUT;
    }
  }

  #endregion

  #region Startup private functions
  /// <summary>
  /// Loads up both the music and sfx libraries with all AudioClips in folders Sound/Music and Sound/SFX
  /// </summary>
  private void loadLibraries()
  {
    Object[] musicClips = Resources.LoadAll("Sound/Music", typeof(AudioClip));
    foreach (AudioClip clip in musicClips)
    {
      _musicLibrary.Add(clip.name, clip);
    }

    Object[] sfxClips = Resources.LoadAll("Sound/SFX", typeof(AudioClip));
    foreach (AudioClip clip in sfxClips)
    {
      _sfxLibrary.Add(clip.name, clip);
    }
  }

  /// <summary>
  /// Switches the currently playing track with the next in queue
  /// </summary>
  private void PlayNextMusic()
  {
    _currentlyPlayingMusic = _nextMusicInQueue;
    _musicSource.Stop();
    _musicSource.clip = _musicLibrary[_currentlyPlayingMusic];
    _musicSource.Play();
  }

  /// <summary>
  /// Manages the fade effects in case there are any currently in order
  /// If we're fading out and we reach 0, then we should switch to the next track in queue
  /// </summary>
  private void ApplyFade()
  {
    if (_currentFadeEffect != Fade.NONE)
    {
      _musicSource.volume = _musicSource.volume + _fadeSpeed * Time.deltaTime *
        (_currentFadeEffect == Fade.OUT ? -1 : 1);

      if (_musicSource.volume <= 0)
      {
        PlayNextMusic();
        _currentFadeEffect = Fade.IN;
      }
      if (_musicSource.volume >= 1)
      {
        _currentFadeEffect = Fade.NONE;
      }
    }
  }

  void Start()
  {
    _musicSource = GetComponent<AudioSource>();
    loadLibraries();
    ServiceLocator.ProvideAudioSystem(this);
  }

  void Update()
  {
    ApplyFade();
  }
  #endregion
}

