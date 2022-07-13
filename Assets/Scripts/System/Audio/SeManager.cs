using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeManager : SingletonMonoBehaviour<SeManager>
{

    public const int TEXT_SOUND = 0;
    private readonly string SE_PATH = "Audio/SE/";
    private AudioSource[] audio_source_list;
    private AudioSetting setting;
    private bool is_init = false;
    private readonly Dictionary<int,string> SOUNDS = new Dictionary<int,string>(){
    {TEXT_SOUND,"TextSound"}};
    new private void Awake(){
        base.Awake();
        Initialize();
    }
    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        if(!is_init){
            base.Awake();
            setting = GameObject.Find("System/SoundManager").GetComponent<AudioSetting>();
            setting.Initialize();
            audio_source_list = new AudioSource[SOUNDS.Count];
            foreach(int index in SOUNDS.Keys){
                AddAudioSource(index);
            }
            is_init = true;
        }
    }
    /// <summary>
    /// 効果音を再生
    /// </summary>
    /// <param name="sound_id"></param>
    /// <param name="is_one_shot"></param>
    public void Play(int sound_id,bool is_one_shot = false){
        if (is_one_shot) audio_source_list[sound_id].PlayOneShot(audio_source_list[sound_id].clip);
        else audio_source_list[sound_id].Play();
    }
    /// <summary>
    /// 音声が鳴っている間は重複しない。
    /// </summary>
    /// <param name="sound_id"></param>
    public void PlayNotDuplicate(int sound_id){
        if(!audio_source_list[sound_id].isPlaying) audio_source_list[sound_id].Play();
    }
    /// <summary>
    /// 効果音を再生。ピッチを変更することが可能。
    /// </summary>
    /// <param name="sound_id"></param>
    /// <param name="pitch"></param>
    public void Play(int sound_id,float pitch){
        float temp = audio_source_list[sound_id].pitch;
        audio_source_list[sound_id].pitch = pitch;
        audio_source_list[sound_id].Play();
        StartCoroutine(Checking(audio_source_list[sound_id],()=>{
            audio_source_list[sound_id].pitch = temp;
        } ));
    }
    public void Play(int sound_id){
        audio_source_list[sound_id].Play();
    }
    private void AddAudioSource(int sound_id){
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio_source_list[sound_id] = GetComponents<AudioSource>()[sound_id];
        audio_source_list[sound_id].clip = Resources.Load<AudioClip>(SE_PATH + SOUNDS[sound_id]);
        audio.outputAudioMixerGroup = setting.Audio_mixer_se;
    }

    private IEnumerator Checking(AudioSource audio,UnityEngine.Events.UnityAction callback) {
        while(true) {
            yield return null;
            if (!audio.isPlaying) {
                callback();
                break;
            }
        }
    }
    public void SetVolume(float volume){
        setting.SetSe = volume;
    }

}