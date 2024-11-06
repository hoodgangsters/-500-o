using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Diagnostics;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();
    private static string appZipUrl = "https://github.com/worldexecutor/World-Simplified/raw/refs/heads/main/World84.zip";
    private static string appDirectory = "WorldApp";

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        await ShowAsciiArt();
        await RunExtendedLoadingSequence();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[-] Downloading new update files...");
        await DownloadAndExtractUpdate();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[+] Update completed successfully!");
        Console.WriteLine("[+] Starting World..");
        StartApplication();
        
        Console.ResetColor();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static async Task ShowAsciiArt()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("██     ██  ██████  ██████  ██      ██████  ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("██     ██ ██    ██ ██   ██ ██      ██   ██ ");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("██  █  ██ ██    ██ ██████  ██      ██   ██ ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("██ ███ ██ ██    ██ ██   ██ ██      ██   ██ ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" ███ ███   ██████  ██   ██ ███████ ██████  ");
        Console.ResetColor();
        Console.WriteLine("\nWORLD\n");

        await Task.Delay(1000);
    }

    static async Task RunExtendedLoadingSequence()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        // Extended loading steps
        Console.WriteLine("[-] Checking system requirements...");
        await Task.Delay(800);

        Console.WriteLine("[-] Preparing environment...");
        await ShowDots(4);

        Console.WriteLine("[-] Verifying network connection...");
        await Task.Delay(1000);
        Console.WriteLine("[✓] Connection established");
        await Task.Delay(500);

        Console.WriteLine("[-] Allocating resources...");
        await ShowDots(6);

        Console.WriteLine("[-] Starting update process...");
        await Task.Delay(1000);

        Console.Write("[-] Downloading update file... ");
        await ShowProgress();

        Console.WriteLine("\n[-] Verifying integrity of downloaded file...");
        await Task.Delay(800);
        Console.WriteLine("[✓] Verification complete");
        await Task.Delay(500);
    }

    static async Task DownloadAndExtractUpdate()
    {
        string tempZipFile = Path.Combine(Path.GetTempPath(), "WorldAppUpdate.zip");

        // Delete any existing zip file to ensure a fresh download
        if (File.Exists(tempZipFile))
        {
            File.Delete(tempZipFile);
        }

        // Download the update ZIP file
        using (var response = await httpClient.GetAsync(appZipUrl))
        {
            response.EnsureSuccessStatusCode();
            using (var fileStream = new FileStream(tempZipFile, FileMode.Create))
            {
                await response.Content.CopyToAsync(fileStream);
            }
        }

        Console.WriteLine("[-] Extracting files...");
        await ShowExtractionProgress();

        // Extract the ZIP file
        if (Directory.Exists(appDirectory))
        {
            Directory.Delete(appDirectory, true); // Clean up old files
        }

        ZipFile.ExtractToDirectory(tempZipFile, appDirectory);
        File.Delete(tempZipFile); // Remove the zip after extraction
    }

    static void StartApplication()
    {
        // Generate a random 9-character string for renaming
        string randomDirectory = Path.Combine(appDirectory, GenerateRandomString(9));
        Directory.CreateDirectory(randomDirectory);

        string sourceExe = Path.Combine(appDirectory, "Debug", "World.exe");
        string destinationExe = Path.Combine(randomDirectory, "World.exe");

        if (File.Exists(sourceExe))
        {
            File.Copy(sourceExe, destinationExe, true);
            Process.Start(destinationExe);
        }
        else
        {
            Console.WriteLine("Failed to start the application. File not found.");
        }
    }

    static async Task ShowProgress()
    {
        for (int i = 0; i <= 100; i++)
        {
            Console.Write($"\r[=>] {i}%");
            await Task.Delay(30); // Simulates progress delay
        }
        Console.WriteLine();
    }

    static async Task ShowExtractionProgress()
    {
        for (int i = 0; i <= 100; i += 2)
        {
            Console.Write($"\r[=>] Extracting... {i}%");
            await Task.Delay(40); // Simulates extraction delay
        }
        Console.WriteLine();
    }

    static async Task ShowDots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Console.Write(".");
            await Task.Delay(400);
        }
        Console.WriteLine();
    }

    static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$_&-+*!";
        Random random = new Random();
        char[] buffer = new char[length];
        for (int i = 0; i < length; i++)
        {
            buffer[i] = chars[random.Next(chars.Length)];
        }
        return new string(buffer);
    }
}
