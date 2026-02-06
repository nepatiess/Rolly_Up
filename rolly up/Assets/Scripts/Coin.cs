using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Kayýtlý coin artýr
            int oldCoin = PlayerPrefs.GetInt("Coin", 0);
            int newCoin = oldCoin + coinValue;
            PlayerPrefs.SetInt("Coin", newCoin);

            // UI güncelle
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoinUI(coinValue);
                GameManager.Instance.PlayAudio(3); // coin sesi (indexi ayarla)
            }

            Destroy(gameObject);
        }
    }
}
