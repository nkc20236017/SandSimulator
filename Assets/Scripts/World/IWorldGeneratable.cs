using WorldCreation;
public interface IWorldGeneratable
{
    public int[, ] Execute(CaveLayer[] worldLayers);
}