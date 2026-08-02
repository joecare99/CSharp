namespace RnzTrauer.Core.Domain;

/// <summary>RNZ notice categories and the legacy numeric values stored by MySQL.</summary>
public enum AdvertisementCategory
{
    Test = 1000,
    DeathNotice = 8050,
    DeathNoticeWithoutPlace = 8051,
    DeathNoticeWithoutBurial = 8052,
    Memorial = 8055,
    MemorialWithoutDeathNotice = 8056,
    Thanks = 8060,
    ThanksWithoutDeathNotice = 8061,
    PrivateObituary = 8070,
    PrivateObituaryWithoutDeathNotice = 8071,
    CorporateObituary = 8080,
    CorporateObituaryWithoutDeathNotice = 8081,
    Announcement = 8090,
    Advertisement = 8100,
}
