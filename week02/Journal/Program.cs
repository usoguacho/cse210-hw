using System;
using System;
using System.Collections.Generic;
using System.IO;

namespace JournalApp
{
    // Class representing a single journal entry.
    public class JournalEntry
    {
        public string Date { get; set; }
        public string Prompt { get; set; }
        public string Response { get; set; }

        // Constructor that initializes a journal entry.
        public JournalEntry(string date, string prompt, string response)
        {
            Date = date;
            Prompt = prompt;
            Response = response;
        }

        // Returns a formatted string representation of the journal entry.
        public override string ToString()
        {
            return $"Date: {Date}\nPrompt: {Prompt}\nResponse: {Response}\n";
        }
    }

    // Class that manages the journal (a collection of journal entries).
    public class Journal
    {
        private List<JournalEntry> entries;

        public Journal()
        {
            entries = new List<JournalEntry>();
        }

        // Adds a new entry to the journal.
        public void AddEntry(JournalEntry entry)
        {
            entries.Add(entry);
        }

        // Displays all journal entries to the console.
        public void DisplayEntries()
        {
            if (entries.Count == 0)
            {
                Console.WriteLine("No entries to display.");
                return;
            }

            foreach (JournalEntry entry in entries)
            {
                Console.WriteLine(entry.ToString());
            }
        }

        // Saves all journal entries to a file using StreamWriter in a using block.
        public void SaveToFile(string filename)
        {
            try
            {
                // The using block ensures that the StreamWriter is closed and disposed properly.
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    // We use a pipe (|) as a separator for the fields.
                    foreach (JournalEntry entry in entries)
                    {
                        writer.WriteLine($"{entry.Date}|{entry.Prompt}|{entry.Response}");
                    }
                }
                Console.WriteLine("Journal saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving journal: {ex.Message}");
            }
        }

        // Loads journal entries from a file using File.ReadAllLines().
        // This replaces any entries currently stored in the journal.
        public void LoadFromFile(string filename)
        {
            try
            {
                // Read the entire file into an array of strings.
                string[] lines = File.ReadAllLines(filename);
                List<JournalEntry> loadedEntries = new List<JournalEntry>();

                // Process each line.
                foreach (string line in lines)
                {
                    // Each line is expected to have 3 parts separated by a pipe.
                    string[] parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        string date = parts[0];
                        string prompt = parts[1];
                        string response = parts[2];
                        loadedEntries.Add(new JournalEntry(date, prompt, response));
                    }
                }

                entries = loadedEntries; // Replace current entries with the loaded entries.
                Console.WriteLine("Journal loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading journal: {ex.Message}");
            }
        }
    }

    // Class that provides a random prompt from a pre-defined list.
    public static class PromptGenerator
    {
        private static List<string> prompts = new List<string>()
        {
            "Who did I help today?",
            "What was my favorite meal today?",
            "How did I see the hand of the Lord in my life today?",
            "What was the strongest emotion I felt today?",
            "What happened today that I want to happen tomorrow?"
        };

        // Returns a random prompt from the list.
        public static string GetRandomPrompt()
        {
            Random random = new Random();
            int index = random.Next(prompts.Count);
            return prompts[index];
        }
    }

    // The Program class provides the main menu and interacts with the user.
    class Program
    {
        static void Main(string[] args)
        {
            Journal journal = new Journal();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\nJournal Menu:");
                Console.WriteLine("1. Write a new entry");
                Console.WriteLine("2. Display the journal");
                Console.WriteLine("3. Save the journal to a file");
                Console.WriteLine("4. Load the journal from a file");
                Console.WriteLine("5. Quit");
                Console.Write("Select an option (1-5): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        WriteNewEntry(journal);
                        break;
                    case "2":
                        journal.DisplayEntries();
                        break;
                    case "3":
                        SaveJournal(journal);
                        break;
                    case "4":
                        LoadJournal(journal);
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Nope, Please choose between 1 and 5.");
                        break;
                }
            }
        }

        // Prompts the user to write a new entry.
        static void WriteNewEntry(Journal journal)
        {
            // Get a random prompt.
            string prompt = PromptGenerator.GetRandomPrompt();
            Console.WriteLine($"\nPrompt: {prompt}");
            Console.Write("Your response: ");
            string response = Console.ReadLine();

            // Get the current date as a string using ToShortDateString().
            string date = DateTime.Now.ToShortDateString();

            // Create a new JournalEntry and add it to the journal.
            JournalEntry entry = new JournalEntry(date, prompt, response);
            journal.AddEntry(entry);

            Console.WriteLine("Your entry is recorded!");
        }

        // Prompts the user for a filename and saves the journal.
        static void SaveJournal(Journal journal)
        {
            Console.Write("Enter the filename to save the journal: ");
            string filename = Console.ReadLine();
            journal.SaveToFile(filename);
        }

        // Prompts the user for a filename and loads the journal from that file.
        static void LoadJournal(Journal journal)
        {
            Console.Write("Enter the filename to load the journal: ");
            string filename = Console.ReadLine();
            journal.LoadFromFile(filename);
        }
    }
}
