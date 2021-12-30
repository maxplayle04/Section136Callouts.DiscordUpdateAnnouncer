// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using Section136Callouts.UpdateAnnouncer.Net6;
using System.Net;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

//WebhookHelper.Instance.SendUpdate("1", "1", out string? webhookFailed);

// *******************************************************************
// Setup the application, and set all security protocols as required.
// *******************************************************************

ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

HttpClient client = new();

while (true)
{
    Tick();
    Thread.Sleep(TimeSpan.FromMinutes(5));
}

void Tick()
{
    var version = GetLastVersion().GetAwaiter();
}

void printColoured(string msg, ConsoleColor color)
{
    ConsoleColor c = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine(msg);
    Console.ForegroundColor = c;
}

void print(string msg, params object?[]? args) => Console.WriteLine(msg, args);

async Task GetLastVersion()
{
    try
    {
        var r = await client.GetAsync("http://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=30221");

        dynamic returnedJson = JsonConvert.DeserializeObject(value: r.Content.ReadAsStringAsync().GetAwaiter().GetResult());

        Console.WriteLine(returnedJson);

        if (returnedJson.error != null && !string.IsNullOrEmpty(returnedJson.error.ToString()))
        {
            throw new Exception(returnedJson.error);
        }
        else
            Console.WriteLine("[OK] No errors detected whilst obtaining versioning information.");

        string fullVersionString = returnedJson.version.ToString();

        Console.WriteLine("Version number: {0}", returnedJson.version.ToString());

        Regex pattern = new Regex(@"\d+(\.\d+)+");
        Match m = pattern.Match(fullVersionString);
        string version = m.Value;

        Console.WriteLine("Regex returned version number: {0}", version);

        if (string.IsNullOrEmpty(KnownValues.Default.LastKnownUpdate))
        {
            printColoured("No version information is stored.", ConsoleColor.Red);
            Console.WriteLine("* Storing new data...");
            KnownValues.Default.LastKnownUpdate = version;
            KnownValues.Default.LastUpdateCheck = DateTime.Now;
            KnownValues.Default.Save();
            return;
        }
        else
            printColoured("App has stored version information", ConsoleColor.Green);

        bool canParseVersion = Version.TryParse(version, out Version serverVersion);
        bool canParseKnownVersion = Version.TryParse(version, out Version storedVersion);

        storedVersion = new Version(1, 0, 0, 0);

        Console.WriteLine("Can parse server? {0} \n Can parse local? {1} ", canParseVersion.ToString() + " " + serverVersion.ToString(), canParseKnownVersion.ToString() + " " + storedVersion);

        if (canParseVersion && canParseKnownVersion && serverVersion != null && storedVersion != null)
        {
            if (serverVersion != storedVersion)
            {
                printColoured("new update detected!", ConsoleColor.Green);
                print("Stored: {0}\nServer: {1}", storedVersion, serverVersion);
                WebhookHelper.Instance.SendUpdate(serverVersion.ToString(), fullVersionString, out string failReason);
                KnownValues.Default.LastKnownUpdate = serverVersion.ToString();
            }
            else
                print("No new version detected. (Received version info: {0})", serverVersion.ToString());
            KnownValues.Default.LastUpdateCheck = DateTime.Now;
            KnownValues.Default.Save();
        }
        else
        {
            printColoured("PARSE ERROR", ConsoleColor.Red);
            print("local parsed? {0}, server parsed? {1}", canParseKnownVersion, canParseVersion);
        }
        return;
    }
    catch (Exception e)
    {
        Console.WriteLine("Could not fetch most recent version.");
        Console.WriteLine(e);
    }
}

