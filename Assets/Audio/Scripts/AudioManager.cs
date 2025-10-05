using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

/// <summary>
/// Mostly manages the pool of SoundEmitters, and I needed somewhere to put a "mute all audio" button!
/// </summary>
public class AudioManager : PersistentSingleton<AudioManager>
{
    IObjectPool<SoundEmitter> soundEmitterPool;
    readonly List<SoundEmitter> activeSoundEmitters = new();
    public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new();

    [Header("Pool Settings")]
    [SerializeField] SoundEmitter soundEmitterPrefab;
    [SerializeField] bool collectionCheck;
    [SerializeField] int defaultCapacity = 10;
    [SerializeField] int maxPoolSize = 100;
    public int maxSoundInstances = 10;

    public bool muteAllAudio;

    protected override void Awake()
    {
        base.Awake();

        if (soundEmitterPrefab == null) {
            Debug.LogWarning("Sound emitter prefab is null");
            return;
        }
        InitializePool();
    }

    public SoundEmitter Get()
    {
        return soundEmitterPool.Get();
    }

    public void ReturnToPool(SoundEmitter soundEmitter)
    {
        soundEmitterPool.Release(soundEmitter);
    }

    public void StopAllSounds()
    {
        foreach (var soundEmitter in activeSoundEmitters) { soundEmitter.Stop(); }
        FrequentSoundEmitters.Clear();
    }

    SoundEmitter CreateSoundEmitter()
    {
        var soundEmitter = Instantiate(soundEmitterPrefab);
        soundEmitter.gameObject.SetActive(false);
        return soundEmitter;
    }

    void OnTakeFromPool(SoundEmitter soundEmitter)
    {

        soundEmitter.gameObject.SetActive(true);
        activeSoundEmitters.Add(soundEmitter);
    }

    void OnReturnedToPool(SoundEmitter soundEmitter)
    {

        if (soundEmitter.Node != null)
        {
            FrequentSoundEmitters.Remove(soundEmitter.Node);
            soundEmitter.Node = null;
        }

        soundEmitter.gameObject.SetActive(false);
        activeSoundEmitters.Remove(soundEmitter);
    }

    void OnDestroyPoolObject(SoundEmitter soundEmitter)
    {
        if (soundEmitter != null) 
        Destroy(soundEmitter.gameObject);
    }

    void InitializePool()
    {
        soundEmitterPool = new ObjectPool<SoundEmitter>(
            CreateSoundEmitter,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize
            );
    }
}
