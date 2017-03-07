package plugin.albert.audiocenter;

import java.io.IOException;
import java.util.HashSet;
import java.util.Set;

import android.app.Activity;
import android.content.res.AssetFileDescriptor;

import android.media.AudioManager;
import android.media.SoundPool;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

public class AudioCenter extends SoundPool {

    private Activity activity;
    private Set<Integer> soundsSet = new HashSet<Integer>();

    public AudioCenter(int maxStreams) {
        super(maxStreams, AudioManager.STREAM_MUSIC, 0);
        this.activity = UnityPlayer.currentActivity;
        setOnLoadCompleteListener(new OnLoadCompleteListener() {

            @Override
            public void onLoadComplete(SoundPool soundPool, int sampleId, int status) {
                soundsSet.add(sampleId);
            }

        });
    }

    private void Play(int soundId, float volume) {
        if (soundsSet.contains(soundId)) {
            play(soundId, volume, volume, 1, 0, 1.0f);
        }
    }

    public void PlaySound(final int soundId, final float volume) {
        if (!soundsSet.contains(soundId) || soundId == 0) {
            Log.e("SoundPluginUnity", "File has not been loaded!");
        }
        activity.runOnUiThread(new Runnable() {

            @Override
            public void run() {
                Play(soundId, volume);
            }

        });
    }

    public int LoadSound(String soundName) {
        AssetFileDescriptor afd = null;
        Log.d("SoundPluginUnity", "soundName:" + soundName);
        try {
            afd = activity.getAssets().openFd(soundName);
        } catch (IOException e) {
            Log.e("SoundPluginUnity", "File does not exist! " + e.toString());
            return -1;
        }
        int soundId = load(afd, 1);
        soundsSet.add(soundId);
        return soundId;
    }

    public void UnloadSound(int soundId) {
        if (unload(soundId)) {
            soundsSet.remove(soundId);
        } else {
            Log.e("SoundPluginUnity", "File has not been loaded!");
        }
    }
}
