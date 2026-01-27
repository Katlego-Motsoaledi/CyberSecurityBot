using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;

namespace CyberSecurityBot
{
    internal class Program
    {
        private static readonly Dictionary<string, List<string>> Responses =
            new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "how are you", new List<string> { "I'm a bot, always ready to help SA citizens with cybersecurity!" } },
                { "purpose", new List<string> { "I educate South Africans about phishing, password safety, and online security." } },
                { "phishing", new List<string>
                    {
                        "SA Tip: Report phishing to reportphishing@cybersecurity.gov.za.",
                        "Always verify email sender addresses before clicking links.",
                        "Avoid clicking suspicious links or downloading unknown attachments."
                    }
                },
                { "password", new List<string>
                    {
                        "Use a mix of English/Zulu and numbers, e.g., 'Inyanga2024!'.",
                        "Never reuse the same password across websites.",
                        "Enable two-factor authentication where possible."
                    }
                },
                { "safe browsing", new List<string>
                    {
                        "Only enter sensitive information on secure sites with HTTPS.",
                        "Avoid using public Wi-Fi for banking or shopping.",
                        "Install browser security plugins for added protection."
                    }
                }
            };

        private static readonly Dictionary<string, string> SentimentResponses =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "worried", "It's okay to feel concerned. Cyber threats are serious, but you're taking the right step by learning!" },
                { "scared", "Stay calm. Knowledge is power, and I'm here to help you stay safe online." },
                { "frustrated", "Take a deep breath. Let's tackle cybersecurity one topic at a time." },
                { "curious", "That's great! Curiosity helps us learn more and stay safe." }
            };

        private static string lastTopic = string.Empty;
        private static string userName = string.Empty;

        private const string AudioFilePath = @"Assets\welcome.wav";

        internal static void Main(string[] args)
        {
            try
            {
                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
                {
                    Console.WriteLine("\n\nExit command received. Shutting down the bot safely...");
                    e.Cancel = true;
                    Environment.Exit(0);
                };

                PlayWelcomeAudio();
                DisplayAsciiArt();
                StartConversation();

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nUNEXPECTED ERROR: " + ex.Message);
                Console.ResetColor();
            }
        }

        private static void StartConversation()
        {
            userName = GetUserName();
            DisplayWelcomeMessage(userName);
            ProcessUserInput();
        }

        private static void ProcessUserInput()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nYou: ");
                Console.ResetColor();

                string input = Console.ReadLine();
                if (input != null)
                {
                    input = input.Trim().ToLower();
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Please enter a valid question.");
                    continue;
                }

                if (input == "exit") break;

                if (input.Length > 100)
                {
                    Console.WriteLine("Please ask shorter questions (max 100 chars).");
                    continue;
                }

                if (input.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
                {
                    Console.WriteLine("Avoid special characters in your input.");
                    continue;
                }

                Console.WriteLine();
                HandleUserInput(input);
            }
        }

        private static void PlayWelcomeAudio()
        {
            try
            {
                if (!File.Exists(AudioFilePath))
                    throw new FileNotFoundException("Missing file: " + Path.GetFullPath(AudioFilePath));

                using (var player = new SoundPlayer(AudioFilePath))
                {
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nAUDIO ERROR: " + ex.Message);
                Console.WriteLine("Verify:\n1. File exists at: {0}\n2. Build action: 'Content', Copy: 'Copy if newer'", Path.GetFullPath(AudioFilePath));
                Console.ResetColor();
            }
        }

        private static string GetUserName()
        {
            string name;
            do
            {
                Console.Write("Enter your name: ");
                name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name))
                {
                    name = name.Trim();
                }

                if (string.IsNullOrEmpty(name))
                    Console.WriteLine("Name cannot be empty!");
            } while (string.IsNullOrEmpty(name));

            return name;
        }

        private static void HandleUserInput(string input)
        {
            foreach (var sentiment in SentimentResponses)
            {
                if (input.Contains(sentiment.Key))
                {
                    DisplayResponse(sentiment.Value);
                    return;
                }
            }

            if (input.Contains("tell me more") && !string.IsNullOrEmpty(lastTopic) && Responses.ContainsKey(lastTopic))
            {
                var moreResponse = GetRandomResponse(Responses[lastTopic]);
                DisplayResponse(moreResponse);
                return;
            }

            foreach (var key in Responses.Keys)
            {
                if (input.Contains(key) || key.Split(' ').Any(word => input.Contains(word)))
                {
                    lastTopic = key;
                    var reply = GetRandomResponse(Responses[key]);
                    DisplayResponse(reply);
                    return;
                }
            }

            DisplayResponse("I'm not sure I understand. Try asking about:\n- Phishing\n- Passwords\n- Safe browsing");
        }

        private static string GetRandomResponse(List<string> responses)
        {
            var rand = new Random();
            return responses[rand.Next(responses.Count)];
        }

        private static void DisplayResponse(string response)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Bot (" + userName + "): ");
            TypeText(response);
            Console.ResetColor();
            Console.WriteLine("\n" + new string('═', 50));
        }

        private static void TypeText(string text, int delay = 50)
        {
            foreach (var c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }

        private static void DisplayAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
    ╔══════════════════ SOUTH AFRICAN CYBER SHIELD ═══════════════╗
    ║     🇿🇦    PROTECTING DIGITAL SOUTH AFRICA    🇿🇦     ║
    ║            ___           ___                      ___       ║
    ║           /__/\         /  /\        ___         /  /\      ║
    ║          |  |::\       /  /:/_      /  /\       /  /:/_     ║
    ║          |  |:|:\     /  /:/ /\    /  /:/      /  /:/ /\    ║
    ║        __|__|:|\:\   /  /:/ /:/_  /__/::\     /  /:/ /::\   ║
    ║       /__/::::| \:\ /__/:/ /:/ / /__/\:\:\ /__/:/ /:/\:\  ║
    ║       \  \:\~~\__\/ \  \:\/:/ /  \  \:\ \:\ \  \:\/:/~/:/  ║
    ║        \  \:\        \  \::/ /    \  \:\_\:\ \  \::/ /:/   ║
    ║         \  \:\        \  \:\      \  \:\/:/  \__\/ /:/    ║
    ║          \  \:\        \  \:\       \  \::/     /__/:/     ║
    ║           \__\/         \__\/        \__\/      \__\/      ║
    ╚════════════════════════════════════════════════════════════╝
            ");
            Console.ResetColor();
        }

        private static void DisplayWelcomeMessage(string userName)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nHello " + userName + "! What would you like to learn about?");
            TypeText("SA Cybersecurity Topics:\n- Phishing\n- Passwords\n- Safe browsing\n", 30);
            Console.ResetColor();
        }
    }
}
