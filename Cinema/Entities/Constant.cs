namespace Cinema.Entities;

/// <summary>
/// 性别
/// </summary>
public enum Gender_
{
    /// <summary>
    /// 男
    /// </summary>
    male = 0,

    /// <summary>
    /// 女
    /// </summary>
    female = 1,

    /// <summary>
    /// 武装直升机
    /// </summary>
    apache = 2,
}

/// <summary>
/// 电影票状态
/// </summary>
public enum TicketState
{
    /// <summary>
    /// 正常
    /// </summary>
    normal = 0,

    /// <summary>
    /// 已退票
    /// </summary>
    refunded,

    /// <summary>
    /// 已过期
    /// </summary>
    expired
}