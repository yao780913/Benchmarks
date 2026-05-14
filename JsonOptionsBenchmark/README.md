# JsonOptionsBenchmark

驗證「`JsonSerializerOptions` 重用 vs 每次 `new`」對效能的實際影響——特別針對 EF Core `HasConversion(...)` 把 VO 序列化成 JSON 字串落 DB 的場景。

---

## 來源

來自一份 tech note 的論點驗證：

- 文件：`system-text-json-chinese-escape.html`（PART II §07 / §08）
- 問題：替 JobHub.Backend 的 `DefaultJsonOptions` 寫共享單例 pattern 時撞到 `MakeReadOnly()` 的 `TypeInfoResolver` 強制檢查，順帶思考：
  - 沒鎖（不呼叫 `MakeReadOnly()`）會發生什麼
  - 每次都 `Create()` 一份新的 options 又會發生什麼
- 文獻說「每次 `new` 慢 20–100×」「list page 50 筆從 1 ms → 50 ms」，數字看起來很誇張，**直接跑跑看**

---

## 跑法

```powershell
# 必須 Release，Debug 跑出來的數字沒意義
dotnet run -c Release
```

跑完約 12–15 分鐘，產出三份報告到 `BenchmarkDotNet.Artifacts/results/`：

- `*-report-github.md` — markdown 表格（貼 PR、wiki）
- `*-report.csv` — Excel / 出圖
- `*-report.html` — 瀏覽器直接看

---

## 測什麼

### Group 1 — 純 JSON round-trip（`PureJsonBenchmark.cs`）

對 `CandidateSnapshot`（含中文 name + 3 筆 nested experience）做一次 serialize + deserialize，比較 5 種 options 策略：

| Case | 寫法 |
|---|---|
| `Shared_Options` | 重用 `static readonly` + `MakeReadOnly()` 單例（baseline） |
| `Default_NoOptions` | `JsonSerializer.Serialize(obj)` 不傳 options |
| `New_PerCall_Plain` | 每次 `new JsonSerializerOptions { ... }`（無 converter） |
| `New_PerCall_WithConverter` | 同上 + `JsonStringEnumConverter`（**最壞情境**） |
| `Newtonsoft_Baseline` | `JsonConvert.SerializeObject` 對照 |

### Group 2 — EF Core 真實場景（`EfConverterBenchmark.cs`）

SQLite in-memory 預塞 N 筆 `JobApplicationRow`（每筆含序列化的 `CandidateSnapshot`），`ToList()` 讀完全部，比較兩種 `HasConversion` lambda：

| Case | `HasConversion` 寫法 |
|---|---|
| `Ef_Shared` | 兩個 lambda 都引用 `static readonly` options（baseline） |
| `Ef_NewPerCall` | lambda 內 `new JsonSerializerOptions { ... }`（含 enum converter） |

Row count 掃描：`[Params(1, 10, 50, 200, 1000)]`

---

## 結果

**環境**：BenchmarkDotNet 0.14.0 · .NET 10.0.8 · x64 RyuJIT AVX2 · Windows 11

### Group 1 — 純 JSON

| Method | Mean | Ratio | Allocated | Alloc Ratio |
| ------ | ----:| -----:| ---------:| -----------:|
| Shared_Options            |   2.88 μs |  **1.00×** |  2.40 KB |  1.00× |
| Default_NoOptions         |   3.46 μs |  1.21× |  2.73 KB |  1.14× |
| New_PerCall_Plain         |   3.64 μs |  1.27× |  2.79 KB |  1.16× |
| New_PerCall_WithConverter |  86.37 μs | **30.19×** | 66.09 KB | **27.55×** |
| Newtonsoft_Baseline       |   3.69 μs |  1.29× |  7.49 KB |  3.12× |

### Group 2 — EF Core HasConversion

| Rows | Ef_Shared | Ef_NewPerCall | Ratio | Allocated (Fresh) | Alloc Ratio |
| ----:| ---------:| -------------:| -----:| -----------------:| -----------:|
|    1 |    52 μs |    110 μs |   2.12× |    92 KB |   1.55× |
|   10 |    80 μs |    506 μs |   6.35× |   410 KB |   4.96× |
|   50 |   197 μs |   2.38 ms | **12.13×** |  1.82 MB |   9.87× |
|  200 |   615 μs |   9.73 ms | **15.85×** |  7.11 MB |  12.56× |
| 1000 |  3.71 ms |  62.38 ms | **17.36×** | **35.34 MB** | 13.61× |

### Ratio curve

```
Ratio
  ↑
33× ┤ ╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌  (純 JSON 理論上限)
    │
17× ┤                          •━━━━━━•   ← EF 實境 plateau
    │                  ╱━━━━━╯
12× ┤             •━━╱
    │        ╱
 6× ┤   •╱
    │ ╱
 1× •━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━→ rows
    1     10      50      200      1000
```

---

## 結論

1. **裸 `new JsonSerializerOptions` 只慢 1.4×；加 converter 飆到 30×。**
   真正的兇手是 converter chain 重建，不是 options 物件本身的 allocation。所以 `DefaultJsonOptions` 含 `JsonStringEnumConverter`、又被 per-call 重建的話，必然踩到 30× 那條路徑，沒有「只慢一點點」的可能。

2. **EF 實境的 ratio plateau 落在 17×，不是純 JSON 的 33×。**
   EF 自己的 materialization overhead（建 entity、value reader、change tracker）也隨 row count 線性成長，把比例壓平了。所以「在 EF 內 per-call 比理論值好一點」是真的——但好的有限。

3. **「列表頁性能斷崖」位置在 50 rows 附近。**
   Ratio 跨過 10× 大約在 row count = 50——正好是「典型分頁大小」。小於 50 不太有感（detail page 1 row 只慢 2×），大於 50 開始明顯。

4. **記憶體炸更兇。**
   1000 rows 的 list 讀取，per-call `Create()` 每次**多配 33 MB**。10 個並發 request 就是 330 MB 額外 alloc，GC pause 直接看得到。
   Allocation 差距比 latency 差距更難 hide——你 SQL profiler 看不到、APM tracing 看不到，只能看到「GC pause 變多」。

5. **.NET 10 的 STJ 跟 Newtonsoft 在小 payload 上已經打成平手。**
   差距只剩 1.29×。早期那種「STJ 快 2-3 倍」的 narrative 已經過時——STJ 的真正優勢在 AOT、source gen、`Utf8JsonWriter` 對大 payload 的 streaming。對「小 DTO 多次序列化」的 web API 場景，效能跟 Newtonsoft 是同一個 quadrant 的事。

### 最該記得的一句話

> `DefaultJsonOptions` + `MakeReadOnly()` + 重用 `Instance` 這套組合不是 over-engineering。
> 規範本身零成本（`static readonly` 一條、`.MakeReadOnly()` 一行、`.ToJson()` extension 一個）；
> 但沒這套規範，**每一支 list API 都在繳這 12–17× 的稅**。

---

## 注意事項與限制

- **SQLite in-memory** 的純 I/O 成本接近零，所以 EF overhead 被放大、JSON 成本占比被放大。換成真實 SQL Server（多 RTT + network），EF/JSON 占整體 latency 的比例會降低，但**絕對 latency 差距不變**——該繳的稅還是繳。
- **EF Core 的 `HasConversion` lambda 走 expression compile**，所以 `new JsonSerializerOptions()` 在 lambda 內是「每次 read row 都新建一次」，跟 dev 直覺一致。
- **沒測 source generator JSON**（`[JsonSerializable]`）。預期它會把 `Shared_Options` 那條再往下壓 1.5–2×，是另一個獨立題目。
- **沒測高並發場景**。這份 benchmark 是單執行緒測 throughput；高並發下 GC pressure 會放大 per-call 的成本，差距預期更大。

---

## 檔案

```
JsonOptionsBenchmark/
├── README.md                  ← 本檔
├── JsonOptionsBenchmark.csproj
├── Program.cs                 ← BenchmarkRunner 進入點
├── Models.cs                  ← CandidateSnapshot / JobApplicationRow / TestData
├── JsonOptionsFactory.cs      ← Shared 單例 + FreshPerCall 工廠
├── PureJsonBenchmark.cs       ← Group 1
├── Db.cs                      ← SharedDbContext + FreshDbContext
├── EfConverterBenchmark.cs    ← Group 2
└── BenchmarkDotNet.Artifacts/
    ├── BenchmarkRun-*.log     ← 完整 log（含 histogram、statistics）
    └── results/
        ├── *-report-github.md
        ├── *-report.csv
        └── *-report.html
```
