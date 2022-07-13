using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// BGMは最大3つまでキャッシュされ、よく使われるBGMに関してはキャッシュが保たれる。
/// 新しい楽曲が読み込まれた時、使用回数がすくないBGMは破棄される仕組み。
/// </summary>
public class BgmManager : SingletonMonoBehaviour<BgmManager>{

    /// <summary>
    /// 使用する楽曲の詳細
    /// </summary>
    public class BgmInfo{
        public int GetId{get;}
        public string GetBgmName{get;}
        public float GetIntroStart{get;}
        public float GetIntroEnd{get;}
        public float GetLoopEnd{get;}
        /// <summary>
        /// intro_start_time秒で始まりloop_end秒になると
        /// intro_start_time秒からBGMが開始される。
        /// </summary>
        /// <param name="id">BGMID regionのBGM情報で設定されたものを使用すること。</param>
        /// <param name="bgm_name">楽曲の名前</param>
        /// <param name="intro_start_time">イントロの始まる秒数。設定しない場合は-1</param>
        /// <param name="intro_end_time">イントロの終わり秒数。設定しない場合は0</param>
        /// <param name="loop_end">指定された秒数になったときに強制的にintro_start_timeから始まる。設定しない場合は-1</param>
        public BgmInfo(int id,string bgm_name,float intro_start_time,float intro_end_time,float loop_end){
            this.GetId = id;
            this.GetBgmName = bgm_name;
            this.GetIntroStart = intro_start_time;
            this.GetIntroEnd = intro_end_time;
            this.GetLoopEnd = loop_end;
        }
    }
    [System.Serializable]
    public class PlayBgm{
        public string BgmName;
        public float PlayTime;
        public int CallCount;
        public float GetIntroEnd;
        public float GetIntroStart;
        public float GetLoopEnd;
        /// <summary>
        /// 最後に再生されたものか
        /// </summary>
        public bool IsLastPlay;
        public AudioSource AudioSource;
        // public string BgmName{get;set;}
        // public float PlayTime{get;set;}
        // public int CallCount{get;set;}
        // public float GetIntroEnd{get;private set;}
        // public float GetIntroStart{get;private set;}
        // public float GetLoopEnd{get;private set;}
        // public AudioSource AudioSource{get;set;}
        public void Reset(){
            IsLastPlay = false;
            BgmName = "";
            PlayTime = 0;
            CallCount = 0;
            GetIntroEnd = 0;
            GetIntroStart = 0;
            GetLoopEnd = 0;
            AudioSource.Stop();
        }
        public PlayBgm(string name, float intro_start,float intro_end,float loop_end){
            BgmName = name;
            GetLoopEnd = loop_end;
            GetIntroEnd = intro_end;
            GetIntroStart = intro_start;
        }
    }
    private readonly string BGM_PATH = "Audio/BGM/";

    #region BGM情報
    public readonly int Horror = 0;
    public readonly BgmInfo[] BgmInfos = new BgmInfo[]{

    };
    #endregion
    private AudioSetting setting;
    [SerializeField]
    private List<PlayBgm> play_bgms;    
    [SerializeField]
    private PlayBgm now_play = null;
    private Coroutine play_routine = null;
    public void SetVolume(float volume){
        setting.SetBgm = volume;
    }
    public void Initialize(){
        base.Awake();
        setting = GameObject.Find("System/SoundManager").GetComponent<AudioSetting>();
        play_bgms = new List<PlayBgm>();
    }
    private BgmInfo GetBgmInfo(int bgm){
        return BgmInfos[bgm];
    }
    /// <summary>
    /// 曲を再生します。
    /// それまでかかっていた曲は塗り替えて再生します。
    /// </summary>
    /// <param name="bgm_info">bgm</param>
    /// <param name="is_fead_in">フェードインするか</param>
    /// <param name="is_load">途中から再生するか</param>
    public void Play(int bgm_id,bool is_fead_in,bool is_load){
        Stop(is_fead_in);
        BgmInfo bgm_info = GetBgmInfo(bgm_id);
        //BGMセット
        PlayBgm play_bgm = play_bgms
        .Where(x => x.BgmName == bgm_info.GetBgmName).Select(x => x).FirstOrDefault();
        if(play_bgms.Count <= 0 || play_bgm == null){
            play_bgm = new PlayBgm(
                bgm_info.GetBgmName,
                bgm_info.GetIntroStart,
                bgm_info.GetIntroEnd,
                bgm_info.GetLoopEnd
            );
            SetAudioClip(play_bgm);
            play_bgms.Add(play_bgm);
        }

        PlayProcess(play_bgm,is_fead_in,is_load);
    }
    /// <summary>
    /// 音声を止める
    /// </summary>
    /// <param name="is_fead_out">フェードアウトするか</param>
    public void Stop(bool is_fead_out){
        if(play_bgms.Count <= 0) return;
        StopRoutine();
        PlayBgm stop_bgm = play_bgms.Last();
        stop_bgm.PlayTime = stop_bgm.AudioSource.time;
        stop_bgm.AudioSource.Stop();
        now_play = null;
    }
/// <summary>
/// replay_back回前に再生した曲を再生する
/// </summary>
/// <param name="is_fead_in">フェードイン</param>
/// <param name="is_load">ロード</param>
/// <param name="replay_back">1＝直前 2=2つ前回前の曲を参照</param>
    public void Replay(bool is_fead_in,bool is_load,int replay_back = 1){
        if(play_bgms.Count <= 0) return;
        Stop(is_fead_in);
        PlayBgm replay_bgm = null;
        if(play_bgms.Count == 1){
            replay_bgm = play_bgms[0];
        }else{
            replay_bgm = play_bgms[play_bgms.Count - replay_back];
        }
        PlayProcess(replay_bgm,is_fead_in,is_load);
    }
    private void StopRoutine(){
        if(play_routine != null){
            StopCoroutine(play_routine);
            play_routine = null;
        }
    }

    private void PlayProcess(PlayBgm play,bool is_fead_in,bool is_load){
        play_bgms.ForEach(every_bgm => {
            if(play != every_bgm){
                every_bgm.IsLastPlay = false;
            }
        });
        play.IsLastPlay = true;
        StopRoutine();
        if(play != play_bgms.Last()){
            play_bgms.Remove(play);
            play_bgms.Insert(play_bgms.Count,play);
        }
        play.CallCount++;
        if(is_load)play.AudioSource.time = play.PlayTime;
        else play.AudioSource.time = 0;
        if(is_fead_in){
            DOTween.To(
                () => play.AudioSource.volume,
                num => play.AudioSource.volume = num,
                1,
                5f
            );
        }
        now_play = play;
        play.AudioSource.Play();
        if(now_play.GetIntroEnd != -1 && now_play.GetLoopEnd != -1){
            play_routine = StartCoroutine(IntroAndLoop());
        }
    }
    private IEnumerator IntroAndLoop(){
        while(true){
            if(now_play != null && now_play.AudioSource.time >= now_play.GetLoopEnd){
                now_play.AudioSource.time = now_play.GetIntroEnd;
            }
            yield return null;
        }
    }

    private AudioSource CreateAudioSource(){
        AudioSource audio = null;
        if(play_bgms.Count < 4){
            audio = gameObject.AddComponent<AudioSource>();
            audio.outputAudioMixerGroup = setting.Audio_mixer_bgm;
        }else{
            PlayBgm reset =  play_bgms.OrderBy(x => x.CallCount).Where(x => !x.IsLastPlay).FirstOrDefault();
            audio = reset.AudioSource;
            reset.Reset();
            play_bgms.Remove(reset);
        }
        return audio;
    }
    private void SetAudioClip(PlayBgm play_bgm){
        AudioSource audio = CreateAudioSource();
        play_bgm.AudioSource = audio;
        audio.clip = Resources.Load<AudioClip>(BGM_PATH + play_bgm.BgmName);
        play_bgm.AudioSource.loop = true;
        audio.playOnAwake = false;
    }
}