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

    [SerializeField] AudioSource[] Musics;
    [SerializeField] Image[] ButtonPic;
    [SerializeField] Sprite[] SpriteObjects;


    private float lastCrashTime = 0f;

    private void Awake()
    {
        ScenesFirstOptions();
        // Singleton yapýsýný saðlama alýyoruz
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Eðer sahnede baþka bir GameManager varsa yenisini sil
            return;
        }

        // Sahne baþladýðýnda oyun duruyor olmalý
        isGameStart = false;
        SceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Diðer kodlarýn (GameOver, ButtonSettings vs.) ayný kalabilir.


    public void GameOver(int EarnedPoints)
    {
        gameOver = true;

        if (EarnedPoints >= completedPer)
            Win(EarnedPoints);
        else
            Lose();
    }

    void Win(int EarnedPer)
    {
        PlayAudio(7);
        OpenPanel(3);
        PlayerPrefs.SetInt("Record", PlayerPrefs.GetInt("Record") + EarnedPer * 2);
        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + (EarnedPer * 2) / 10);
        Debug.Log("Earned Points: " + EarnedPer * 2);
    }

    void Lose()
    {
        PlayAudio(6);
        OpenPanel(4);
        Debug.Log("Lose....");
    }

    public void PlayAudio(int Index)
    {
        // Güvenlik kontrolü (Index listenin dýþýna çýkmasýn)
        if (Index < 0 || Index >= Musics.Length) return;

        // EÐER ÇALINACAK SES "ÇARPMA SESÝ" ÝSE (Index 4)
        if (Index == 4)
        {
            // Þu anki zaman - Son çalýnan zaman farký 0.1 saniyeden büyük mü?
            // Yani "En son 0.1 saniye önce mi çaldým?" kontrolü.
            if (Time.time - lastCrashTime > 0.1f)
            {
                // PlayOneShot kullanýyoruz ki sesler üst üste binerse patlamasýn, daha doðal dursun
                if (!Musics[Index].isPlaying)
                {
                    Musics[Index].Play();
                }
                else
                {
                    // Zaten çalýyorsa bile efekt olarak üstüne ekle (daha tok ses verir)
                    // Ama Play() diyip baþa sarmýyoruz!
                    Musics[Index].PlayOneShot(Musics[Index].clip);
                }

                lastCrashTime = Time.time; // Zamaný kaydet
            }
        }
        // DÝÐER SESLER (FÝNÝSH, TOPLAMA VS.) NORMAL ÇALSIN
        else
        {
            if (Index == 5) // Finish sesi gibi uzun sesler için
            {
                if (!Musics[Index].isPlaying) Musics[Index].Play();
            }
            else
            {
                Musics[Index].Play();
            }
        }
    }

    public void ButtonSettings(string ButtonValue)
    {
        switch (ButtonValue)
        {
            case "Stop":
                PlayAudio(1);
                OpenPanel(2);
                Time.timeScale = 0;
                break;

            case "Continue":
                PlayAudio(1);
                ClosePanel(2);
                Time.timeScale = 1;
                break;

            case "StartGame":
                PlayAudio(1);
                ClosePanel(0);
                OpenPanel(1);
                isGameStart = true;
                break;

            case "Retry":
                PlayAudio(1);
                SceneManager.LoadScene(SceneIndex);
                Time.timeScale = 1;
                break;

            case "NextLevel":
                PlayAudio(1);
                SceneManager.LoadScene(SceneIndex + 1);
                Time.timeScale = 1;
                break;

            case "Quit":
                PlayAudio(1);
                OpenPanel(5);
                break;

            case "Yes":
                PlayAudio(1);
                Application.Quit();
                break;

            case "No":
                PlayAudio(1);
                ClosePanel(5);
                break;


            case "GameMusic":
                PlayAudio(1);
                if (PlayerPrefs.GetInt("GameMusic") == 1)
                {
                    PlayerPrefs.SetInt("GameMusic", 0);
                    ButtonPic[0].sprite = SpriteObjects[1];
                    Musics[0].mute = true;
                }
                else
                {
                    PlayerPrefs.SetInt("GameMusic", 1);
                    ButtonPic[0].sprite = SpriteObjects[0];
                    Musics[0].mute = false;
                }
                break;

            case "GameEfect":
                PlayAudio(1);
                if (PlayerPrefs.GetInt("GameEfect") == 1)
                {
                    PlayerPrefs.SetInt("GameEfect", 0);
                    ButtonPic[1].sprite = SpriteObjects[3];

                    for (int i = 1; i < Musics.Length; i++)
                    {
                        Musics[i].mute = true;
                    }
                }
                else
                {
                    PlayerPrefs.SetInt("GameEfect", 1);
                    ButtonPic[1].sprite = SpriteObjects[2];

                    for (int i = 1; i < Musics.Length; i++)
                    {
                        Musics[i].mute = false;
                    }
                }
                break;

        }
    }
    void ScenesFirstOptions()
    {
        // Ýlk oyun açýlýþýnda varsayýlan deðerleri ayarla
        if (!PlayerPrefs.HasKey("GameMusic"))
        {
            PlayerPrefs.SetInt("GameMusic", 1); // Varsayýlan olarak açýk
        }

        if (!PlayerPrefs.HasKey("GameEfect"))
        {
            PlayerPrefs.SetInt("GameEfect", 1); // Varsayýlan olarak açýk
        }

        // Müzik ayarlarýný uygula
        if (PlayerPrefs.GetInt("GameMusic") == 1)
        {
            ButtonPic[0].sprite = SpriteObjects[0];
            Musics[0].mute = false;
            if (!Musics[0].isPlaying)
            {
                Musics[0].Play(); // Müziði baþlat
            }
        }
        else
        {
            ButtonPic[0].sprite = SpriteObjects[1];
            Musics[0].mute = true;
        }

        // Efekt sesleri ayarlarýný uygula
        if (PlayerPrefs.GetInt("GameEfect") == 1)
        {
            ButtonPic[1].sprite = SpriteObjects[2];

            for (int i = 1; i < Musics.Length; i++)
            {
                Musics[i].mute = false;
            }
        }
        else
        {
            ButtonPic[1].sprite = SpriteObjects[3];

            for (int i = 1; i < Musics.Length; i++)
            {
                Musics[i].mute = true;
            }
        }
    }

    void OpenPanel(int Index)
    {
        Panels[Index].SetActive(true);
    }

    void ClosePanel(int Index)
    {
        Panels[Index].SetActive(false);
    }
}