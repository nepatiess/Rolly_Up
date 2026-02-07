using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGameStart;
    bool gameOver;
    int SceneIndex;
    [SerializeField] int completedPer;

    [SerializeField] GameObject[] Panels;

    // UI Elemanları
    [SerializeField] TextMeshProUGUI SumPoints;    // Toplam Puan
    [SerializeField] TextMeshProUGUI SumCoins;     // Toplam Coin
    [SerializeField] TextMeshProUGUI EarnedPoints;  // Sadece o levelda kazanılan
    [SerializeField] TextMeshProUGUI EarnedCoins;   // Sadece o levelda kazanılan
    [SerializeField] TextMeshProUGUI LevelStage;    // "Level 1", "Level 2" yazısı

    [SerializeField] AudioSource[] Musics;
    [SerializeField] Image[] ButtonPic;
    [SerializeField] Sprite[] SpriteObjects;

    private float lastCrashTime = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        isGameStart = false;
        gameOver = false;
        SceneIndex = SceneManager.GetActiveScene().buildIndex;

        ScenesFirstOptions();
    }

    void Start()
    {
        UpdateTotalUI();  // Kayıtlı puanları yükle
        UpdateLevelText(); // Level numarasını yazdır

        // Başlangıçta Level yazısını gizli tutalım (Start butonu basılana kadar)
        if (LevelStage != null) LevelStage.gameObject.SetActive(false);
    }

    // Level yazısını güncelleyen fonksiyon
    void UpdateLevelText()
    {
        if (LevelStage != null)
        {
            // Eğer ilk sahnen (index 0) Menu ise, Level 1 için Index'i kullanabilirsin.
            // Genelde Level 1 sahne indexi 1'dir.
            LevelStage.text = "Level " + SceneIndex.ToString();
        }
    }

    void UpdateTotalUI()
    {
        SumPoints.text = PlayerPrefs.GetInt("Record", 0).ToString();
        SumCoins.text = PlayerPrefs.GetInt("Coin", 0).ToString();
    }

    public void GameOver(int EarnedPer)
    {
        if (gameOver) return;
        gameOver = true;

        if (EarnedPer >= completedPer)
            Win(EarnedPer);
        else
            Lose();
    }

    void Win(int EarnedPer)
    {
        PlayAudio(7);
        OpenPanel(3);
        if (LevelStage != null) LevelStage.gameObject.SetActive(false); // Kazanma ekranında gizle

        int earnedPoints = EarnedPer * 2;
        int earnedCoins = earnedPoints / 10;

        int oldPoints = PlayerPrefs.GetInt("Record", 0);
        int oldCoins = PlayerPrefs.GetInt("Coin", 0);

        int totalPoints = oldPoints + earnedPoints;
        int totalCoins = oldCoins + earnedCoins;

        PlayerPrefs.SetInt("Record", totalPoints);
        PlayerPrefs.SetInt("Coin", totalCoins);
        PlayerPrefs.Save();

        EarnedPoints.text = "+" + earnedPoints.ToString();
        EarnedCoins.text = "+" + earnedCoins.ToString();
        SumPoints.text = totalPoints.ToString();
        SumCoins.text = totalCoins.ToString();
    }

    void Lose()
    {
        PlayAudio(6);
        OpenPanel(4);
        if (LevelStage != null) LevelStage.gameObject.SetActive(false); // Kaybetme ekranında gizle
        UpdateTotalUI();
    }

    public void PlayAudio(int Index)
    {
        if (Index < 0 || Index >= Musics.Length) return;
        if (Index == 4)
        {
            if (Time.time - lastCrashTime > 0.1f)
            {
                if (!Musics[Index].isPlaying) Musics[Index].Play();
                else Musics[Index].PlayOneShot(Musics[Index].clip);
                lastCrashTime = Time.time;
            }
        }
        else if (Index == 5)
        {
            if (!Musics[Index].isPlaying) Musics[Index].Play();
        }
        else { Musics[Index].Play(); }
    }

    public void ButtonSettings(string ButtonValue)
    {
        switch (ButtonValue)
        {
            case "MainMenu":
                Time.timeScale = 1;
                SceneManager.LoadScene(0); // MainMenu (index 0)
                break;

            case "Stop":
                Time.timeScale = 0;
                OpenPanel(2);
                PlayAudio(1);
                // Duraklatma menüsünde level yazısı kalsın istiyorsan buraya dokunma
                break;

            case "Continue":
                Time.timeScale = 1;
                ClosePanel(2);
                PlayAudio(1);
                break;

            case "StartGame":
                isGameStart = true;
                ClosePanel(0);
                OpenPanel(1);
                PlayAudio(1);
                if (LevelStage != null) LevelStage.gameObject.SetActive(true); // Oyun başlayınca göster
                break;

            case "Retry":
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneIndex); // Aynı sahne yüklenir, Start() tekrar çalışır
                break;

            case "NextLevel":
                Time.timeScale = 1;
                // Eğer bir sonraki sahne varsa yükle
                if (SceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
                    SceneManager.LoadScene(SceneIndex + 1);
                else
                    SceneManager.LoadScene(0); // Sahneler bittiyse menüye dön
                break;

            case "Quit":
                Time.timeScale = 0;      // Oyunu durdur
                OpenPanel(5);            // Emin misin paneli
                PlayAudio(1);
                break;

            case "Yes":
                Time.timeScale = 1;
                SceneManager.LoadScene(0); // MainMenu'ye dön
                break;

            case "No":
                ClosePanel(5);
                Time.timeScale = 1;
                PlayAudio(1);
                break;

            case "GameMusic": ToggleMusic(); break;
            case "GameEfect": ToggleEffects(); break;
        }
    }

    void ToggleMusic()
    {
        int state = PlayerPrefs.GetInt("GameMusic") == 1 ? 0 : 1;
        PlayerPrefs.SetInt("GameMusic", state);
        ApplyAudioSettings();
    }

    void ToggleEffects()
    {
        int state = PlayerPrefs.GetInt("GameEfect") == 1 ? 0 : 1;
        PlayerPrefs.SetInt("GameEfect", state);
        ApplyAudioSettings();
    }

    void ScenesFirstOptions()
    {
        if (!PlayerPrefs.HasKey("GameMusic")) PlayerPrefs.SetInt("GameMusic", 1);
        if (!PlayerPrefs.HasKey("GameEfect")) PlayerPrefs.SetInt("GameEfect", 1);
        ApplyAudioSettings();
    }

    void ApplyAudioSettings()
    {
        bool musicOn = PlayerPrefs.GetInt("GameMusic") == 1;
        Musics[0].mute = !musicOn;
        ButtonPic[0].sprite = musicOn ? SpriteObjects[0] : SpriteObjects[1];
        if (musicOn && !Musics[0].isPlaying) Musics[0].Play();

        bool effectOn = PlayerPrefs.GetInt("GameEfect") == 1;
        ButtonPic[1].sprite = effectOn ? SpriteObjects[2] : SpriteObjects[3];
        for (int i = 1; i < Musics.Length; i++) Musics[i].mute = !effectOn;
    }

    void OpenPanel(int Index) => Panels[Index].SetActive(true);
    void ClosePanel(int Index) => Panels[Index].SetActive(false);

    public void AddCoinUI(int value)
    {
        // Toplam coin
        int totalCoin = PlayerPrefs.GetInt("Coin", 0);
        SumCoins.text = totalCoin.ToString();

        // Bu levelda kazanılan coin
        if (EarnedCoins != null)
            EarnedCoins.text = "+" + value.ToString();
    }

}