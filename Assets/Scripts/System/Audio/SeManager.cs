using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeManager : SingletonMonoBehaviour<SeManager>
{

    public const int WALK = 0;
    private readonly string SE_PATH = "Audio/SE/";
    private AudioSource[] audio_source_list;
    private AudioSetting setting;
    private bool is_init = false;
    private readonly Dictionary<int,string> SOUNDS = new Dictionary<int,string>(){
    {WALK,"Walk"}};
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
    public AudioSource Play(Transform tf,int sound_id,bool is_one_shot = false){
        GameObject obj = new GameObject();
        obj.transform.parent = tf;
        obj.transform.localPosition = Vector3.zero;
        AudioSource audio = obj.AddComponent<AudioSource>();
        audio.outputAudioMixerGroup = setting.Audio_mixer_se;
        audio.clip = audio_source_list[sound_id].clip;
        audio.volume = 0.5f;
        audio.maxDistance = 10;
        if (is_one_shot) audio.PlayOneShot(audio_source_list[sound_id].clip);
        else audio.Play();
        StartCoroutine(Checking(audio,()=>{
            if(obj != null) Destroy(obj);
        } ));
        return audio;
    }
    public void Stop(AudioSource stop_audio){
        stop_audio.Stop();
        Destroy(stop_audio.gameObject);
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
            
            if (audio == null || !audio.isPlaying) {
                callback();
                break;
            }
        }
    }
    public void SetVolume(float volume){
        setting.SetSe = volume;
    }

}