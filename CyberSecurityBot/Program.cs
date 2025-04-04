using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;

namespace CyberSecurityBot
{
    /// <summary>
    /// Cybersecurity Awareness Chatbot with South African focus
    /// </summary>
    internal class Program
    {
        #region Constants and Fields

        // Application paths
        private const string AudioFilePath = @"Assets\welcome.wav";
        private const string LogoPath = @"Assets\logo.bmp";

        // ASCII character gradient for image conversion (dark to light)
        private const string AsciiChars = "@%#*+=-:. ";

        // List of bot responses with triggers and answers
        private static readonly List<Response> Responses = new List<Response>
        {
            new Response("how are you", "I'm a bot, always ready to help SA citizens with cybersecurity!"),
            new Response("purpose", "I educate South Africans about phishing, password safety, and online security."),
            new Response("phishing", "SA Tip: Report phishing to reportphishing@cybersecurity.gov.za. Verify sender addresses!"),
            new Response("password", "Use ZA-specific tips: Combine English/Zulu words with numbers (e.g., 'Inyanga2024!')"),
            new Response("safe browsing", "For SA sites, always check .co.za domains and valid SSL certificates.")
        };

        // Input validation rules with error messages
        private static readonly List<ValidationRule> ValidationRules = new List<ValidationRule>
        {
            new ValidationRule(input => string.IsNullOrWhiteSpace(input), "Please enter a valid question."),
            new ValidationRule(input => input.Length > 100, "Please ask shorter questions (max 100 chars)."),
            new ValidationRule(input => input.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)),
                "Avoid special characters in questions.")
        };

        #endregion

        #region Supporting Classes

        /// <summary>
        /// Represents a chatbot response with trigger phrase and answer
        /// </summary>
        public class Response
        {
            public string Trigger { get; }
            public string Answer { get; }

            public Response(string trigger, string answer)
            {
                Trigger = trigger;
                Answer = answer;
            }
        }

        /// <summary>
        /// Represents an input validation rule with validation function and error message
        /// </summary>
        public class ValidationRule
        {
            public Func<string, bool> Validate { get; }
            public string Message { get; }

            public ValidationRule(Func<string, bool> validate, string message)
            {
                Validate = validate;
                Message = message;
            }
        }

        #endregion

        #region Main Program Flow

        /// <summary>
        /// Main entry point for the application
        /// </summary>
        internal static void Main(string[] _)
        {
            PlayWelcomeAudio();
            DisplayBitmapAsciiArt(LogoPath);
            StartConversation();
        }

        #endregion

        #region Conversation Management

        /// <summary>
        /// Starts the conversation flow with user interaction
        /// </summary>
        private static void StartConversation()
        {
            var userName = GetUserName();
            DisplayWelcomeMessage(userName);
            ProcessUserInput();
        }

        /// <summary>
        /// Handles continuous user input until exit command
        /// </summary>
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

        #region Input Handling

        /// <summary>
        /// Processes and validates user input, providing appropriate responses
        /// </summary>
        /// <param name="input">User's question/input</param>
        private static void HandleUserInput(string input)
        {
            // Check validation rules
            foreach (var rule in ValidationRules)
            {
                if (rule.Validate(input))
                {
                    Console.WriteLine(rule.Message);
                    return;
                }
            }

            // Find matching response using case-insensitive search
            var response = Responses.FirstOrDefault(r =>
                input.IndexOf(r.Trigger, StringComparison.OrdinalIgnoreCase) >= 0)?.Answer;

            DisplayResponse(response);
        }

        #endregion

        #region Display Utilities

        /// <summary>
        /// Displays bitmap image as ASCII art with error handling
        /// </summary>
        /// <param name="imagePath">Path to bitmap image</param>
        /// <param name="targetWidth">Width of ASCII output (default 80)</param>
        private static void DisplayBitmapAsciiArt(string imagePath, int targetWidth = 80)
        {
            try
            {
                if (!File.Exists(imagePath))
                {
                    throw new FileNotFoundException($"Missing logo: {Path.GetFullPath(imagePath)}");
                }

                using var bitmap = new Bitmap(imagePath);
                var asciiArt = ConvertBitmapToAscii(bitmap, targetWidth);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(asciiArt);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nLOGO ERROR: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Converts bitmap image to ASCII art using brightness values
        /// </summary>
        /// <param name="bitmap">Source image</param>
        /// <param name="targetWidth">Desired output width</param>
        /// <returns>ASCII art string</returns>
        private static string ConvertBitmapToAscii(Bitmap bitmap, int targetWidth)
        {
            // Calculate aspect ratio-preserved dimensions
            double aspectRatio = (double)bitmap.Height / bitmap.Width;
            int resizedWidth = targetWidth;
            int resizedHeight = (int)(targetWidth * aspectRatio);

            // Resize image for ASCII conversion with explicit namespace
            using var resizedBitmap = new Bitmap(bitmap, new System.Drawing.Size(resizedWidth, resizedHeight));
            var asciiArt = new System.Text.StringBuilder();

            for (int y = 0; y < resizedBitmap.Height; y++)
            {
                for (int x = 0; x < resizedBitmap.Width; x++)
                {
                    Color pixel = resizedBitmap.GetPixel(x, y);
                    double brightness = (0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B) / 255.0;
                    int charIndex = (int)(brightness * (AsciiChars.Length - 1));
                    asciiArt.Append(AsciiChars[charIndex]);
                }
                asciiArt.AppendLine();
            }
            return asciiArt.ToString();
        }

        /// <summary>
        /// Displays text with typing animation effect
        /// </summary>
        private static void TypeText(string text, int delay = 50)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Displays chatbot response with formatting
        /// </summary>
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

        /// <summary>
        /// Displays personalized welcome message
        /// </summary>
        private static void DisplayWelcomeMessage(string userName)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\nHello {userName}! What would you like to learn about?");
            TypeText("SA Cybersecurity Topics:\n- Phishing\n- Passwords\n- Safe browsing\n", 30);
            Console.ResetColor();
        }

        #endregion

        #region Audio Handling

        /// <summary>
        /// Plays welcome audio with error handling
        /// </summary>
        private static void PlayWelcomeAudio()
        {
            try
            {
                if (!File.Exists(AudioFilePath))
                {
                    throw new FileNotFoundException($"Missing audio file: {Path.GetFullPath(AudioFilePath)}");
                }

                using var player = new SoundPlayer(AudioFilePath);
                player.PlaySync();
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

        /// <summary>
        /// Gets and validates user name through console input
        /// </summary>
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

        #endregion
    }
}