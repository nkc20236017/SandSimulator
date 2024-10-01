using UnityEngine;

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
    }

    public void UpdateHealth(int currentHealth)
    {
        CleanUp();
        for (int i = 0; i < currentHealth; i++)
        {
            parentHealth.GetChild(i).gameObject.SetActive(true);
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
