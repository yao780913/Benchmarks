namespace JsonOptionsBenchmark;

/// <summary>
/// 模擬 JobHub 的 CandidateSnapshot：name + 3 筆 nested experience，含中文。
/// </summary>
public sealed record CandidateSnapshot(
    string Name,
    string? Phone,
    string? Email,
    string? Region,
    IReadOnlyList<ExperienceSnapshot> Experiences);

public sealed record ExperienceSnapshot(
    string Company,
    string Position,
    string Duration);

/// <summary>
/// 模擬 EF Entity：含一個會走 HasConversion 序列化成 JSON 字串的 VO 欄位。
/// </summary>
public sealed class JobApplicationRow
{
    public Guid Id { get; set; }
    public string ApplicantName { get; set; } = "";
    public CandidateSnapshot Candidate { get; set; } = null!;
}

public static class TestData
{
    public static CandidateSnapshot SampleSnapshot() => new(
        Name:  "王小明",
        Phone: "0912345678",
        Email: "wang@example.com",
        Region: "台北市",
        Experiences: new[]
        {
            new ExperienceSnapshot("便利商店 A", "門市人員", "6 個月"),
            new ExperienceSnapshot("速食店 B",   "外場服務", "1 年"),
            new ExperienceSnapshot("咖啡廳 C",   "吧檯助手", "8 個月"),
        });

    public static JobApplicationRow SampleRow(int seed) => new()
    {
        Id            = Guid.NewGuid(),
        ApplicantName = $"應徵者 #{seed:D3}",
        Candidate     = SampleSnapshot(),
    };
}
