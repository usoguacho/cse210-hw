using System;
using System.Collections.Generic;
using System.Linq;

namespace ScriptureMemorizer
{
    // Represents the reference for a scripture
    public class ScriptureReference
    {
        private string book;
        private int chapter;
        private int verseStart;
        private int verseEnd;

        // Constructor for a single verse.
        public ScriptureReference(string book, int chapter, int verse)
        {
            this.book = book;
            this.chapter = chapter;
            this.verseStart = verse;
            this.verseEnd = verse;
        }

        // Constructor for a verse range.
        public ScriptureReference(string book, int chapter, int verseStart, int verseEnd)
        {
            this.book = book;
            this.chapter = chapter;
            this.verseStart = verseStart;
            this.verseEnd = verseEnd;
        }

        // Returns a string representation of the reference.
        public override string ToString()
        {
            if (verseStart == verseEnd)
            {
                return $"{book} {chapter}:{verseStart}";
            }
            else
            {
                return $"{book} {chapter}:{verseStart}-{verseEnd}";
            }
        }
    }

    // Represents a single word in the scripture text.
    public class Word
    {
        private string text;
        private bool isHidden;

        public Word(string text)
        {
            this.text = text;
            isHidden = false;
        }

        // Returns the word for display; if the word is hidden, it returns underscores.
        public string GetDisplay()
        {
            if (isHidden)
            {
                return new string('_', text.Length);
            }
            else
            {
                return text;
            }
        }

        // Hides the word.
        public void Hide()
        {
            isHidden = true;
        }

        // Indicates whether the word is hidden.
        public bool IsHidden
        {
            get { return isHidden; }
        }
    }

    // Represents the full scripture, including its reference and text.
    public class Scripture
    {
        private ScriptureReference reference;
        private List<Word> words;

        // The constructor splits the provided text into words.
        public Scripture(ScriptureReference reference, string scriptureText)
        {
            this.reference = reference;
            words = new List<Word>();

            // Splitting on space
            foreach (string word in scriptureText.Split(' '))
            {
                // Preserve punctuation attached to a word.
                words.Add(new Word(word));
            }
        }

        // Returns a complete string with the reference and the scripture text (words displayed with underscores if hidden).
        public string GetDisplayText()
        {
            string displayWords = string.Join(" ", words.Select(w => w.GetDisplay()));
            return reference.ToString() + "\n" + displayWords;
        }

        // Returns true if every word in the scripture is hidden.
        public bool AllWordsHidden()
        {
            return words.All(w => w.IsHidden);
        }

        // Hides a specified number of random words that are not yet hidden.
        public void HideRandomWords(int count)
        {
            // Get a list of indices for words that are not hidden.
            List<int> notHiddenIndices = new List<int>();
            for (int i = 0; i < words.Count; i++)
            {
                if (!words[i].IsHidden)
                {
                    notHiddenIndices.Add(i);
                }
            }

            // If no words are left, nothing to hide.
            if (notHiddenIndices.Count == 0)
            {
                return;
            }

            Random rnd = new Random();

            // Hide "count" words or as many as remain unhidden.
            count = Math.Min(count, notHiddenIndices.Count);
            for (int i = 0; i < count; i++)
            {
                int randomIndex = rnd.Next(notHiddenIndices.Count);
                int wordIndex = notHiddenIndices[randomIndex];
                words[wordIndex].Hide();
                notHiddenIndices.RemoveAt(randomIndex);
            }
        }
    }

    // The main program class.
    class Program
    {
        static void Main(string[] args)
        {
            // Create a scripture reference.
            ScriptureReference reference = new ScriptureReference("Proverbs", 3, 5, 6);

            // The scripture text
            string scriptureText = "Trust in the Lord with all thine heart; and lean not unto thine own understanding. In all thy ways acknowledge him, and he shall direct thy paths.";

            // Create the scripture.
            Scripture scripture = new Scripture(reference, scriptureText);

            // Loop until the user quits or all words have been hidden.
            while (true)
            {
                Console.Clear();
                
                Console.WriteLine(scripture.GetDisplayText());

                // If every word is hidden, end the program.
                if (scripture.AllWordsHidden())
                {
                    Console.WriteLine("\nAll words have been hidden. Press any key to exit.");
                    Console.ReadKey();
                    break;
                }

                Console.WriteLine("\nPress Enter to hide more words or type 'quit' to exit:");
                string input = Console.ReadLine();

                if (input.ToLower() == "quit")
                {
                    break;
                }

                
                scripture.HideRandomWords(3);
            }
        }
    }
}
