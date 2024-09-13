class InputTank
{
    private IInputTank inputTank;

    public InputTank(IInputTank inputTank)
    {
        this.inputTank = inputTank;
    }

    public void AddTankCommand(MineralType mineralType)
    {
        inputTank.InputAddTank(mineralType);
    }

    public void RemoveTankCommand(MineralType mineralType)
    {
        inputTank.InputRemoveTank(mineralType);
    }

}

