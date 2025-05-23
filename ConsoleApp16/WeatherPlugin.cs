using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ConsoleApp16;

/// <summary>
/// 天気を取得するプラグイン
/// </summary>
/// <param name="client"></param>
public class WeatherPlugin(HttpClient client)
{
    /// <summary>
    /// URI で識別されるリソースへのリクエスト送信およびレスポンス受信に使用される HTTP クライアントです。
    /// </summary>
    /// <remarks>
    /// このフィールドは <see cref="HttpClient"/> のインスタンスで初期化され、HTTP リクエストの送信に使用されます。
    /// readonly としてマークされており、クラスのライフタイム全体で同じインスタンスが使用されます。
    /// </remarks>
    private readonly HttpClient _client = client;

    /// <summary>
    /// 天気を取得する場所コードを取得します
    /// </summary>
    /// <param name="place">天気を取得する場所</param>
    /// <returns>天気コード</returns>
    /// <exception cref="ArgumentException">対応してない地域の場合</exception>
    [KernelFunction, Description("""
        天気を取得する場所コードを取得します。
        対応している場所コードは下記のとおりです。
        下記に含まれない場所の場合は近くの場所の天気を代わりに取得してください。
        東京
        群馬
        埼玉
        千葉
        横浜
        名古屋
        京都
        静岡
        福井
        新潟
        富山
        金沢
        岐阜
        長野
        高山
        松本
        大津
        大阪
        札幌
        仙台
        福岡
        那覇
        ----
        以上、ここに含まれない場合は近くの場所の代わりに取得してください。
        """)]
    public static int GetPlaceId([Description("天気を取得する場所")] string place)
    {
        var res = place switch
        {
            "札幌" => 016000,
            "群馬" => 100000,
            "埼玉" => 110000,
            "千葉" => 120000,
            "東京" => 130000,
            "横浜" => 140000,
            "名古屋" => 230000,
            "京都" => 260000,
            "静岡" => 220000,
            "福井" => 180000,
            "新潟" => 150000,
            "富山" => 160000,
            "金沢" => 170000,
            "岐阜" => 210000,
            "長野" => 200000,
            "高山" => 190000,
            "松本" => 200000,
            "大津" => 250000,
            "大阪" => 270000,
            "仙台" => 040000,
            "福岡" => 400000,
            "那覇" => 471000,
            _ => throw new ArgumentException("対応していない地域です。")
        };
        return res;
    }

    /// <summary>
    /// 場所コードの地域の天気を返す
    /// </summary>
    /// <param name="place">場所コード</param>
    /// <returns>天気情報</returns>
    [KernelFunction, Description("場所コードの地域の天気を返す")]
    public async Task<string> Weather([Description("場所コード")] int place)
    {
        var response = await _client.GetAsync($"https://www.jma.go.jp/bosai/forecast/data/forecast/{place}.json");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

}
