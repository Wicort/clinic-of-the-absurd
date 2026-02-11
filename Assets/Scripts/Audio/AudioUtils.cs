using UnityEngine;

public static class AudioUtils
{
    // Быстрые методы для часто используемых звуков
    
    public static void PlayGagSound(HumorType gagType)
    {
        AudioManager.PlayGagSound(gagType);
    }
    
    public static void PlayPatientReaction(PatientReactionType reactionType)
    {
        AudioManager.PlayPatientReactionSound(reactionType);
    }
    
    public static void PlayClick()
    {
        AudioManager.PlayButtonClickSound();
    }
    
    public static void PlayDoor()
    {
        AudioManager.PlayDoorOpenSound();
    }
    
    public static void PlayReward()
    {
        AudioManager.PlayRewardSound();
    }
    
    public static void PlayVictory()
    {
        AudioManager.PlayVictorySound();
    }
    
    // Музыка
    public static void PlayMenuMusic()
    {
        AudioManager.PlayMainMenuMusic();
    }
    
    public static void PlayGameMusic()
    {
        AudioManager.PlayGameplayMusic();
    }
    
    public static void PlayBossMusic()
    {
        AudioManager.PlayBossMusic();
    }
    
    public static void StopMusic()
    {
        AudioManager.StopMusic();
    }
    
    // Настройки громкости
    public static void SetMusicVolume(float volume01)
    {
        AudioManager.SetMusicVolume(volume01);
    }
    
    public static void SetSFXVolume(float volume01)
    {
        AudioManager.SetSFXVolume(volume01);
    }
}
