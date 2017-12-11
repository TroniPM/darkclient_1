public enum GlobalEvents : int
{
    ChangeHp = 1, //��ɫѪ���ı�
    ChangeLevel,  //��ɫ�ȼ��ı�
    ChangeForce,
    Death,        //��ɫ����
    GetItem,      //��ɫ��õ���
    GetTask,      
    FinishTask,
    OpenFunction,
    GetActivity,
    EnterInstance,
    LeaveInstance,
    OpenGUI,
    ButtonClick,
    SkillAvailable,
    TimeLimitEvent,
    Achiement,
    ArenicCredit
}

public enum SignalEvents : byte
{
    AnyClick = 1,
    ButtonClick =2,
    KillMonster =3,
    ClickItem = 4,
    ArrivePos = 5,
    FinishDialog = 6,
    Sleep = 7,
    StartGuide = 8
}

public enum InstanceIdentity : int
{
    TOWER = 20001,
    SANCTUARY_BEGIN = 40000,
    SANCTUARY_END   = 40010
}

public enum WindowName : int
{
    Tower = 1,
    Challenge,
    Arena,
    ArenaReward,
    Dialog
}

static public class UIEvent
{
    public readonly static string ChangePage = "Pager.ChangePage";
    public readonly static string SlipPage = "Slipper.ChangePage";
}

static public class InventoryEvent
{
    public readonly static string UseItems = "InventoryEvent.UseItems"; //ʹ�ø�����Ʒ
    public readonly static string DelItems = "InventoryEvent.DelItems"; //ɾ��������Ʒ
    public readonly static string DelAllItem = "InventoryEvent.DelAllItem"; //ɾ��������Ʒ
    public readonly static string SaleItems = "InventoryEvent.SaleItems"; //���۸�����Ʒ
    public readonly static string StrengthItem = "InventoryEvent.StrengthItem"; //ǿ��������Ʒ
    public readonly static string ShowItem = "InventoryEvent.ShowItem"; //չʾ������Ʒ
    public readonly static string SplitItem = "InventoryEvent.SplitItem"; //��ָ�����ƷΪ�������

    public readonly static string UpdateItemGrid = "InventoryEvent.UpdateItemGrid"; //���µ�������
    public readonly static string UpdateItemAllGrid = "InventoryEvent.UpdateItemAllGrid"; //����ȫ������



    public readonly static string OnChangeEquip = "InventoryEvent.OnChangeEquip";
    public readonly static string ShowTip = "InventoryEvent.ShowTip";
}

static public class SettingEvent
{
    public readonly static string MotityMusicVolume = "SettingEvent.MotityMusicVolume";
    public readonly static string MotitySoundVolume = "SettingEvent.MotitySoundVolume";
    public readonly static string SaveVolume = "SettingEvent.SaveVolume";

    public readonly static string UIUpPlaySound = "SettingEvent.UIUpPlaySound";
    public readonly static string UIDownPlaySound = "SettingEvent.UIDownPlaySound";

    public readonly static string BuildSoundEnvironment = "SettingEvent.BuildSoundEnvironment";
    public readonly static string FreeSoundEnvironment = "SettingEvent.FreeSoundEnvironment";

    public readonly static string PlayBackGroundMusic = "SettingEvent.PlayBackGroundMusic";
    public readonly static string ChangeMusic = "SettingEvent.ChangeMusic";

    public readonly static string LogicPlaySoundByID = "SettingEvent.LogicPlaySoundByID";
    public readonly static string LogicPlaySoundByClip = "SettingEvent.LogicPlaySoundByClip";
}

static public class VersionEvent
{
    public readonly static string AddMD5Content = "VersionEvent.AddMD5Content";
    public readonly static string GetContentMD5 = "VersionEvent.GetContentMD5";
}

static public class MarketEvent
{
    public readonly static string OpenBuy = "MarketEvent.OpenBuy";
    public readonly static string Buy = "MarketEvent.Buy";
    public readonly static string DownloadMarket = "MarketEvent.DownloadMarket";
    public readonly static string HotList = "MarketEvent.HotList";
    public readonly static string JewelList = "MarketEvent.JewelList";
    public readonly static string ItemList = "MarketEvent.ItemList";
    public readonly static string BuyNum = "MarketEvent.BuyNum";
    public readonly static string LigthArrow = "MarketEvent.LightArrow";
    public readonly static string OpenWithJewel = "MarketEvent.OpenWithJewel";
    public readonly static string OpenWithWing = "MarketEvent.OpenWithWing";
    public readonly static string DrawLevelPacks = "MarketEvent.DrawLevelPacks"; // ��ȡ�ȼ����
    public readonly static string WingList = "MarketEvent.WingList";
    public readonly static string Preview = "MarketEvent.Preview";
    public readonly static string PreviewBuy = "MarketEvent.PreviewBuy";
    public readonly static string WingBuy = "MarketEvent.WingBuy";

    public readonly static string DownloadLoginMarket = "MarketEvent.DownloadLoginMarket";
}