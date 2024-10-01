using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField]
    private Transform parentHealth;

    [SerializeField]
    private GameObject HealthPrefab;

    public void StartHealth(int maxHealth)
    {
        for (int i = 0; i < maxHealth; i++)
        {
            Instantiate(HealthPrefab, parentHealth);
        }
        UpdateHealth(maxHealth);
    }

    public void UpdateHealth(int currentHealth)
    {

        CleanUp();
        for (int i = 0; i < currentHealth; i++)
        {
            parentHealth.GetChild(i).GetComponent<Image>().DOFade(1,0);
            parentHealth.GetChild(i).gameObject.SetActive(true);
            parentHealth.GetChild(i).GetComponent<Image>().DOFade(0, 5).SetEase(Ease.OutFlash);
        }

    }

    private void CleanUp()
    {
        for (int i = 0; i < parentHealth.childCount; i++)
        {
            parentHealth.GetChild(i).gameObject.SetActive(false);
        }
    }

}
