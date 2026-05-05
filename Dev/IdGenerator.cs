namespace Buildings.Utils;

public interface IIdGenerator<out T>
    where T : IComparable, IComparable<T>, IEquatable<T>, IFormattable, ISpanFormattable, IUtf8SpanFormattable,
    ISpanParsable<T>, IUtf8SpanParsable<T>
{
    public T NextId();
}

public sealed class SnowflakeIdGenerator : IIdGenerator<long>
{
    ///起始的时间戳
    public const long StartStamp = 1480166465631L;

    private long _lastStamp = -1L; //上一次时间戳
    private long _sequence; //序列号

    public SnowflakeIdGenerator()
    {
        var option = new SnowflakeIdGeneratorOptions();

        SequenceBit = option.SequenceBit;
        MachineBit = option.MachineBit;
        DatacenterBit = option.DatacenterBit;

        if (option.MachineId > MaxMachineNum || option.MachineId < 0)
            throw new Exception(nameof(option.MachineId));
        if (option.DatacenterId > MaxDatacenterNum || option.DatacenterId < 0)
            throw new Exception(nameof(option.DatacenterId));

        DatacenterId = option.DatacenterId;
        MachineId = option.MachineId;
    }

    ///序列号占用的位数
    public int SequenceBit { get; }

    ///机器标识占用的位数
    public int MachineBit { get; }

    ///数据中心占用的位数
    public int DatacenterBit { get; }

    public long MaxDatacenterNum => -1L ^ (-1L << DatacenterBit);
    public long MaxMachineNum => -1L ^ (-1L << MachineBit);
    public long MaxSequence => -1L ^ (-1L << SequenceBit);


    public int MachineLeft => SequenceBit;
    public int DatacenterLeft => SequenceBit + MachineBit;
    public int TimestampLeft => DatacenterLeft + DatacenterBit;

    public long DatacenterId { get; }
    public long MachineId { get; }

    /// <summary>
    ///     产生下一个ID
    /// </summary>
    /// <returns></returns>
    public long NextId()
    {
        var currStamp = GetNewStamp();
        if (currStamp < _lastStamp) throw new Exception("时钟倒退，Id生成失败！");

        if (currStamp == _lastStamp)
        {
            //相同毫秒内，序列号自增
            _sequence = (_sequence + 1) & MaxSequence;
            //同一毫秒的序列数已经达到最大
            if (_sequence == 0L) currStamp = GetNextMill();
        }
        else
        {
            //不同毫秒内，序列号置为0
            _sequence = 0L;
        }

        _lastStamp = currStamp;

        return ((currStamp - StartStamp) << TimestampLeft) //时间戳部分
               | (DatacenterId << DatacenterLeft) //数据中心部分
               | (MachineId << MachineLeft) //机器标识部分
               | _sequence; //序列号部分
    }

    private long GetNextMill()
    {
        var mill = GetNewStamp();
        while (mill <= _lastStamp) mill = GetNewStamp();

        return mill;
    }

    private static long GetNewStamp()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }
}

public class SnowflakeIdGeneratorOptions
{
    public int SequenceBit { get; set; } = 12; //序列号占用的位数
    public int MachineBit { get; set; } = 5; //机器标识占用的位数
    public int DatacenterBit { get; set; } = 5; //数据中心占用的位数
    public long DatacenterId { get; set; } = 0;
    public long MachineId { get; set; } = 1;
}

public sealed class GuidGenerator : IIdGenerator<Guid>
{
    public Guid NextId()
    {
        return Guid.CreateVersion7();
    }
}