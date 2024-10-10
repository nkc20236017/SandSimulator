public class PlayerStatusData
{
    public readonly int MaxHealth;
    public readonly int CurrentHealth;
    public readonly float MaxTankCapacity;
    public readonly float CurrentTankCapacity;

    public PlayerStatusData(int maxHealth, int currentHealth, float maxTankCapacity, float currentTankCapacity)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        MaxTankCapacity = maxTankCapacity;
        CurrentTankCapacity = currentTankCapacity;
    }

    public PlayerStatusData(PlayerDatas playerData)
    {
        MaxHealth = playerData.MaxHealth;
        CurrentHealth = playerData.CurrentHealth;
        MaxTankCapacity = playerData.MaxTankCapacity;
        CurrentTankCapacity = playerData.CurrentTankCapacity;
    }

}
