public interface IUIData
{
}
public class NoneUIData:IUIData
{
    public static NoneUIData noneUIData;
    
    static NoneUIData()
    {
        noneUIData = new NoneUIData();
    }
}