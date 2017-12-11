public class MFUIQueueUnit
{
    public System.Action act;

    public uint UnitID;
    public uint UnitPriority = 0;
    public MFUIManager.MFUIID BaseUIID;

    public void JustDoIt()
    {
        if (act != null)
        {
            act();
        }
    }
}
