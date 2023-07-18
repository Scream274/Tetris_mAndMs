using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Block activeBlock { get; private set; }

    public TetrisFiguresData[] figures;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    //Pause settings
    private bool isPaused = false;
    public GameObject pausePanel;
    public AudioClip pauseSound;

    //Music
    private AudioSource globalAudioSource;
    private AudioSource audioSource;

    //Lvl and Score
    private int score = 0;
    public TMPro.TextMeshProUGUI scoreText;
    private int lvl = 1;

    //Speed up per lvl
    public float initialStepDelay = 1f;
    private float currentStepDelay;
    public float CurrentStepDelay { get { return currentStepDelay; } }
    public float fallDelayDecreasePerLevel = 0.1f;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activeBlock = GetComponentInChildren<Block>();

        for (int i = 0; i < figures.Length; i++)
        {
            figures[i].Initialize();
        }

    }

    private void Start()
    {
        currentStepDelay = initialStepDelay;
        GameObject targetObject = GameObject.Find("MusicLayer");
        globalAudioSource = targetObject.GetComponent<AudioSource>();

        audioSource = GetComponent<AudioSource>();
        SpawnBlock();
        UpdateScoreText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void SpawnBlock()
    {
        int random = Random.Range(0, figures.Length);
        TetrisFiguresData data = figures[random];

        activeBlock.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activeBlock, spawnPosition))
        {
            Set(activeBlock);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void Set(Block block)
    {
        for (int i = 0; i < block.cells.Length; i++)
        {
            Vector3Int tilePosition = block.cells[i] + block.position;
            tilemap.SetTile(tilePosition, block.data.tile);
        }
    }

    public void Clear(Block block)
    {
        for (int i = 0; i < block.cells.Length; i++)
        {
            Vector3Int tilePosition = block.cells[i] + block.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Block block, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < block.cells.Length; i++)
        {
            Vector3Int tilePosition = block.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }


    //Clear lines logic
    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int linesCleared = 0;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                linesCleared++;
            }
            else
            {
                row++;
            }
        }

        if (linesCleared > 0)
        {
            AddScore(linesCleared);
            UpdateScoreText();
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

    //Pause logic
    public void TogglePause()
    {
        isPaused = !isPaused;
        audioSource.PlayOneShot(pauseSound);

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            globalAudioSource.Pause();
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            globalAudioSource.UnPause();
        }
    }

    //Score adding logic
    public void AddScore(int linesCleared)
    {
        switch (linesCleared)
        {
            case 1:
                score += 75;
                break;
            case 2:
                score += 150;
                break;
            case 3:
                score += 250;
                break;
            case 4:
                score += 400;
                break;
        }

        if (score >= lvl * 1000)
        {
            lvl++;
            currentStepDelay -= fallDelayDecreasePerLevel;
            if (currentStepDelay < 0.1)
            {
                currentStepDelay = 0.1f;
            }
        }

        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        if (score.Equals(0))
        {
            scoreText.text = "SCORE: 0\nLEVEL: " + lvl.ToString();
        }
        else
        {
            scoreText.text = "SCORE: " + score.ToString() + "\nLEVEL: " + lvl.ToString();
        }
    }
}