public class SetPlayerStatus
{
    public readonly int MaxHealth;
    public readonly int CurrentHealth;
    public readonly float MaxTankCapacity;
    public readonly float CurrentTankCapacity;

    //data‚ª-1‚¾‚ÆœŠO‚³‚ê‚é
    public SetPlayerStatus(int maxHealth = -1, int currentHealth = -1,
        float maxTankCapacity = -1, float currentTankCapacity = -1)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        MaxTankCapacity = maxTankCapacity;
        CurrentTankCapacity = currentTankCapacity;
    }
}
