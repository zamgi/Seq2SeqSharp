
using AdvUtils;
using Microsoft.Extensions.Configuration;
using Seq2SeqSharp;
using Seq2SeqSharp._SentencePiece;
using Seq2SeqWebApps;

Logger.LogFile = $"{nameof(SeqWebApps)}_{GetTimeStamp(DateTime.Now)}.log";

var Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

if (String.IsNullOrEmpty(Configuration["Seq2Seq:ModelFilePath"]) == false)
{
    Logger.WriteLine($"Loading Seq2Seq model '{Configuration["Seq2Seq:ModelFilePath"]}'");

    var modelFilePath = Configuration["Seq2Seq:ModelFilePath"];
    var maxTestSrcSentLength = int.Parse(Configuration["Seq2Seq:MaxSrcTokenSize"]);
    var maxTestTgtSentLength = int.Parse(Configuration["Seq2Seq:MaxTgtTokenSize"]);
    var processorType = Configuration["Seq2Seq:ProcessorType"].ToEnum<ProcessorTypeEnums>();
    var deviceIds = Configuration["Seq2Seq:DeviceIds"];
    var tokenGenerationStrategy = Configuration["Seq2Seq:TokenGenerationStrategy"];
    var repeatPenalty = float.Parse(Configuration["Seq2Seq:RepeatPenalty"]);
    var topPSampling = float.Parse(Configuration["Seq2Seq:TopPSampling"]);

    SentencePiece? srcSpm = null;
    if (String.IsNullOrEmpty(Configuration["SourceSpm:ModelFilePath"]) == false)
    {
        srcSpm = new SentencePiece(Configuration["SourceSpm:ModelFilePath"]);
    }

    SentencePiece? tgtSpm = null;
    if (String.IsNullOrEmpty(Configuration["TargetSpm:ModelFilePath"]) == false)
    {
        tgtSpm = new SentencePiece(Configuration["TargetSpm:ModelFilePath"]);
    }

    Seq2SeqSharp.Utils.DecodingStrategyEnums decodingStrategyEnum = (Seq2SeqSharp.Utils.DecodingStrategyEnums)Enum.Parse(typeof(Seq2SeqSharp.Utils.DecodingStrategyEnums), tokenGenerationStrategy);

    Seq2SeqInstance.Initialization(modelFilePath, maxTestSrcSentLength, maxTestTgtSentLength, processorType, deviceIds, srcSpm, tgtSpm, decodingStrategyEnum, topPSampling, repeatPenalty);
}



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();


static string GetTimeStamp(DateTime timeStamp)
{
    return string.Format("{0:yyyy}_{0:MM}_{0:dd}_{0:HH}h_{0:mm}m_{0:ss}s", timeStamp);
}

/// <summary>
/// 
/// </summary>
internal static class Extensions
{
    public static T ToEnum<T>(this string s) where T : struct => Enum.Parse<T>(s, true);
    public static int ToInt(this string s) => int.Parse(s);
}