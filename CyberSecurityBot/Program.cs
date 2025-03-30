using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;

namespace CyberSecurityBot
{
    
    /// Cybersecurity Awareness Chatbot
    
    internal class Program
    {
        #region Constants and Fields
        
        /// Dictionary storing bot responses with case-insensitive matching
        
        private static readonly Dictionary<string, string> Responses = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase)
        {
            { "how are you", "I'm a bot, always ready to help SA citizens with cybersecurity!" },
            { "purpose", "I educate South Africans about phishing, password safety, and online security." },
            { "phishing", "SA Tip: Report phishing to reportphishing@cybersecurity.gov.za. Verify sender addresses!" },
            { "password", "Use ZA-specific tips: Combine English/Zulu words with numbers (e.g., 'Inyanga2024!')" },
            { "safe browsing", "For SA sites, always check .co.za domains and valid SSL certificates." }
        };

        
        /// ceation of  path to the welcome audio file in WAV format
        
        private const string AudioFilePath = @"Assets\welcome.wav";
        #endregion

        #region Main Entry Point
        
        /// Main method execution for the chatbot
        
        internal static void Main(string[] _)
        {
            PlayWelcomeAudio();
            DisplayAsciiArt();
            StartConversation();
        }
        #endregion

        #region Core Functionality
        
        /// Initialization of conversation method
        
        private static void StartConversation()
        {
            var userName = GetUserName();
            DisplayWelcomeMessage(userName);
            ProcessUserInput();
        }

        
        /// Method for handling continuous user input until exit command
        
        private static void ProcessUserInput()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nYou: ");
                Console.ResetColor();

                var input = Console.ReadLine()?.Trim().ToLower();
                if (input == "exit") break;

                Console.WriteLine();
                HandleUserInput(input);
            }
        }
        #endregion

        #region Audio Handling
        
        ///method for playing welcome audio with error handling for file issuess
        
        private static void PlayWelcomeAudio()
        {
            try
            {
                if (!File.Exists(AudioFilePath))
                {
                    throw new FileNotFoundException($"Missing file: {Path.GetFullPath(AudioFilePath)}");
                }

                using (var player = new SoundPlayer(AudioFilePath))
                {
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAUDIO ERROR: {ex.Message}");
                Console.WriteLine("Verify:");
                Console.WriteLine($"1. File exists at: {Path.GetFullPath(AudioFilePath)}");
                Console.WriteLine("2. Build action: 'Content', Copy: 'Copy if newer'");
                Console.ResetColor();
            }
        }
        #endregion

        #region User Interaction
        
        /// initialization of getters to validate and retrieve user name through console input
       
        private static string GetUserName()
        {
            string userName;
            do
            {
                Console.Write("Enter your name: ");
                userName = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(userName))
                {
                    Console.WriteLine("Name cannot be empty!");
                }
            } while (string.IsNullOrEmpty(userName));

            return userName;
        }

        
        /// Method for proccessing user input with validation and security checks
        
        private static void HandleUserInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter a valid question.");
                return;
            }

            if (input.Length > 100)
            {
                Console.WriteLine("Please ask shorter questions (max 100 chars).");
                return;
            }

            if (input.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
            {
                Console.WriteLine("Avoid special characters in questions.");
                return;
            }

            var response = Responses.FirstOrDefault(r => input.Contains(r.Key)).Value;
            DisplayResponse(response);
        }

        
        ///Method for displaying bot responses with typing animation and formatting
        
        private static void DisplayResponse(string response)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Bot: ");
            TypeText(string.IsNullOrEmpty(response)
                ? "I didn't understand. Try asking about:\n- Phishing\n- Passwords\n- Safe browsing"
                : response);
            Console.ResetColor();
            Console.WriteLine("\n" + new string('═', 50));
        }
        #endregion

        #region Display Utilities
        
        ///Method for creating typing animation effect for text display
        
        private static void TypeText(string text, int delay = 50)
        {
            foreach (var c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }

        
        ///Method for displaying a South African themed ASCII art header
        
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
    ║       /__/::::| \:\ /__/:/ /:/ / /__\/\:\:\ /__/:/ /:/\:\  ║
    ║       \  \:\~~\__\/ \  \:\/:/ /  \  \:\ \:\ \  \:\/:/~/:/  ║
    ║        \  \:\        \  \::/ /    \  \:\_\:\ \  \::/ /:/   ║
    ║         \  \:\        \  \:\/      \  \:\/:/  \__\/ /:/    ║
    ║          \  \:\        \  \:\       \  \::/     /__/:/     ║
    ║           \__\/         \__\/        \__\/      \__\/      ║
    ╚════════════════════════════════════════════════════════════╝
    ");
            Console.ResetColor();
        }

       
        ///Method for displaying personalized welcome message with SA topics
        
        private static void DisplayWelcomeMessage(string userName)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\nHello {userName}! What would you like to learn about?");
            TypeText("SA Cybersecurity Topics:\n- Phishing\n- Passwords\n- Safe browsing\n", 30);
            Console.ResetColor();
        }
        #endregion
    }
}