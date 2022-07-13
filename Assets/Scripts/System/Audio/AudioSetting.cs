using UnityEngine;
using UnityEngine.Audio;

public class AudioSetting : MonoBehaviour {
    private AudioMixerGroup[] audio_mixer_groups;

    public AudioMixerGroup Audio_mixer_bgm{
        get{return audio_mixer_groups[0];}
    }
    public AudioMixerGroup Audio_mixer_master{
        get{return audio_mixer_groups[1];}
    }
    public AudioMixerGroup Audio_mixer_se{
        get{return audio_mixer_groups[2];}
    }

    public float SetBgm{
        set{Audio_mixer_bgm.audioMixer.SetFloat("Master/BGM",Mathf.Clamp(ConvertVolume2dB(value), -80.0f, 0.0f));}
    }
    public float SetMaster{
        set{Audio_mixer_master.audioMixer.SetFloat("Master",Mathf.Clamp(ConvertVolume2dB(value), -80.0f, 0.0f));}
    }
    public float SetSe{
        set{Audio_mixer_se.audioMixer.SetFloat("Master/SE",Mathf.Clamp(ConvertVolume2dB(value), -80.0f, 0.0f));}
    }
    private bool is_init = false;
    float ConvertVolume2dB(float volume) => 20f * Mathf.Log10(Mathf.Clamp(volume, 0f, 1f));
    public void Initialize() {
        if(!is_init){
            audio_mixer_groups = Resources.LoadAll<AudioMixerGroup>("Audio/AudioMixer");
            is_init = true;
        }
    }
}