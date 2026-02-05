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
    [SerializeField] TextMeshProUGUI SumPoints; // Toplam Puan
    [SerializeField] TextMeshProUGUI SumCoins;  // Toplam Coin
    [SerializeField] TextMeshProUGUI EarnedPoints; // Sadece o levelda kazanılan
    [SerializeField] TextMeshProUGUI EarnedCoins;  // Sadece o levelda kazanılan

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
        // ÖNEMLİ: PlayerPrefs.DeleteAll() satırını sildik! 
        // Artık veriler oyun kapansa da silinmeyecek.

        UpdateTotalUI(); // Oyun başlar başlamaz eski puanları ekrana yazdırıyoruz.
    }

    // Toplam puanları ekranda güncelleyen yardımcı fonksiyon
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

        // Bu levelda kazanılanlar
        int earnedPoints = EarnedPer * 2;
        int earnedCoins = earnedPoints / 10;

        // Eski verileri çek
        int oldPoints = PlayerPrefs.GetInt("Record", 0);
        int oldCoins = PlayerPrefs.GetInt("Coin", 0);

        // Yeni verileri topla
        int totalPoints = oldPoints + earnedPoints;
        int totalCoins = oldCoins + earnedCoins;

        // Kaydet
        PlayerPrefs.SetInt("Record", totalPoints);
        PlayerPrefs.SetInt("Coin", totalCoins);
        PlayerPrefs.Save();

        // UI Güncelleme
        EarnedPoints.text = "+" + earnedPoints.ToString();
        EarnedCoins.text = "+" + earnedCoins.ToString();
        SumPoints.text = totalPoints.ToString();
        SumCoins.text = totalCoins.ToString();
    }

    void Lose()
    {
        PlayAudio(6);
        OpenPanel(4);
        UpdateTotalUI(); // Kaybetse bile toplam puanı görsün
    }

    // --- DİĞER FONKSİYONLAR (Aynı Kalıyor) ---
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
            case "Stop": Time.timeScale = 0; OpenPanel(2); PlayAudio(1); break;
            case "Continue": Time.timeScale = 1; ClosePanel(2); PlayAudio(1); break;
            case "StartGame": isGameStart = true; ClosePanel(0); OpenPanel(1); PlayAudio(1); break;
            case "Retry": Time.timeScale = 1; SceneManager.LoadScene(SceneIndex); break;
            case "NextLevel": Time.timeScale = 1; SceneManager.LoadScene(SceneIndex + 1); break;
            case "Quit": OpenPanel(5); PlayAudio(1); break;
            case "Yes": Application.Quit(); break;
            case "No": ClosePanel(5); PlayAudio(1); break;
            case "GameMusic": ToggleMusic(); break;
            case "GameEfect": ToggleEffects(); break;
        }
    }

    // Müzik ve Efekt kodlarını daha temiz hale getirmek için ayırdım
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
}