using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private enum SpawnState {spawning, waiting, counting};
    [SerializeField] private SpawnState state = SpawnState.counting;
    [SerializeField] protected Transform enemiesParent;
    public GameController gC;

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int amount;
        public float rate;
    }
    public Wave[] waves;
    private int nextWave = 0;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;
    private float waveCountdown;
    private float searchCountdown = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        gC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (spawnPoints.Length == 0)
        {
            Debug.Log("Error, no spawn points referenced");
        }
        waveCountdown = timeBetweenWaves;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == SpawnState.waiting) // After enemies are spawned
        {
            if (!EnemyIsAlive() | (GameController.gC.gameTimer < 0 && nextWave + 1 != waves.Length)) // if no enemies are alive (searches once every second) or game timer reaches 0
            {
                WaveCompleted(); // the wave is complete
            }
            else
            {
                return; // otherwise repeat
            }
        }
        
        if (waveCountdown <= 0) // if wave countdown is complete
        {
            if (state != SpawnState.spawning) // if not in spawning state
            {
                StartCoroutine(SpawnWave(waves[nextWave])); // start spawning enemies
                // Wave Title
                gC.SetWaveTitle(nextWave + 1); // display the current wave
                // Newgrounds Medals
                if (nextWave + 1 == 6) NGHelper.nGIO.GetComponent<NGHelper>().unlockMedal(60342); // Newgrounds: Award Medal for Completion of 5 Waves
                if (nextWave + 1 == 10) NGHelper.nGIO.GetComponent<NGHelper>().unlockMedal(60343); // Newgrounds: Award Medal for Completion of 10 Waves
                gC.Invoke("DeleteWaveTitle", 3.75f); // delete the wave title after 3.75 seconds
                // Play Impact Sound + Music
                if (GameController.gC.waveSoundPlayed == false)
                {
                    SoundController.sC.Play(SoundController.sC.sEffectSource, PlayerController.pC.transform.position, SoundController.sC.waveImpactSounds, 1f);
                    GameController.gC.waveSoundPlayed = true;
                }
                if (SoundController.sC.musicSource.isPlaying == false)
                {
                    SoundController.sC.PlayMusic(SoundController.sC.arenaMusic[SoundController.sC.songCounter], 0.12f);
                    SoundController.sC.songCounter++;
                    if (SoundController.sC.songCounter > SoundController.sC.arenaMusic.Length - 1) SoundController.sC.songCounter = 0;
                }
                // Set Game Timer
                GameController.gC.gameTimer = 25f + (nextWave * 5f);
            }
        }
        else waveCountdown -= Time.deltaTime; // otherwise advance wave countdown
    }

    // Advances the wave count and initiates a wave countdown timer
    void WaveCompleted()
    {
        Debug.Log("Wave Completed!");
        state = SpawnState.counting;
        waveCountdown = timeBetweenWaves;
        GameController.gC.waveSoundPlayed = false;
        if(nextWave+1 > waves.Length -1)
        {
            Debug.Log("All waves complete!");
            gC.LoadScene(3); // Proceed to victory screen
        }
        else nextWave++;
    }

    // Checks if any enemies are alive in the scene, limited to once every search countdown
    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0)
        {
            searchCountdown = 0.5f; // reset search countdown
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    // Spawn the wave by calling SpawnEnemy at the specified rate
    IEnumerator SpawnWave(Wave _wave)
    {
        Debug.Log("spawning wave" + _wave.name);
        state = SpawnState.spawning;

        for (int i = 0; i < _wave.amount; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate); // normally "yield return null" advances the coroutine to the next frame, this advances the coroutine until the end of the specified time
        }

        state = SpawnState.waiting;

        yield break; // ends the coroutine, not actually needed since coroutines end automatically when code is fully executed
    }

    // Spawn the enemy by choosing a spawn point at random
    void SpawnEnemy (Transform _enemy)
    {
        Debug.Log("Spawning Enemy" + _enemy.name);
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, Quaternion.identity, enemiesParent);
    }
}
