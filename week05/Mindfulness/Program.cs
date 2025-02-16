using System;
using System.Collections.Generic;
using System.Threading;

namespace MindfulnessApp
{
    // Base class containing common properties and methods for activities.
    abstract class MindfulnessActivity
    {
        protected string name;
        protected string description;
        protected int duration; // in seconds

        public MindfulnessActivity(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        // Prompts the user to set the duration.
        protected void SetDuration()
        {
            Console.Write("Enter the duration (in seconds) for this activity: ");
            if (!int.TryParse(Console.ReadLine(), out duration))
            {
                duration = 30; // default duration if input is invalid
            }
        }

        // Displays the starting message common to all activities.
        protected void DisplayStartingMessage()
        {
            Console.Clear();
            Console.WriteLine($"Welcome to the {name} Activity.");
            Console.WriteLine(description);
            SetDuration();
            Console.WriteLine("Get ready...");
            DisplaySpinner(3);
        }

        // Displays the ending message common to all activities.
        protected void DisplayEndingMessage()
        {
            Console.WriteLine("Well done!");
            DisplaySpinner(3);
            Console.WriteLine($"You have completed the {name} Activity for {duration} seconds.");
            DisplaySpinner(3);
        }

        // Displays a spining animation 
        protected void DisplaySpinner(int seconds)
        {
            DateTime endTime = DateTime.Now.AddSeconds(seconds);
            while (DateTime.Now < endTime)
            {
                Console.Write("|");
                Thread.Sleep(250);
                Console.Write("\b \b");

                Console.Write("/");
                Thread.Sleep(250);
                Console.Write("\b \b");

                Console.Write("-");
                Thread.Sleep(250);
                Console.Write("\b \b");

                Console.Write("\\");
                Thread.Sleep(250);
                Console.Write("\b \b");
            }
            Console.WriteLine();
        }

        // Displays a countdown (number decreases each second).
        protected void DisplayCountdown(int seconds)
        {
            for (int i = seconds; i > 0; i--)
            {
                Console.Write(i);
                Thread.Sleep(1000);
                Console.Write("\b \b");
            }
            Console.WriteLine();
        }

        // Each activity implements its own RunActivity method.
        public abstract void RunActivity();
    }

    // Breathing activity
    class BreathingActivity : MindfulnessActivity
    {
        public BreathingActivity()
            : base("Breathing", "This activity will help you relax by guiding you through slow, deep breathing. Clear your mind and focus on your breathing.")
        {
        }

        public override void RunActivity()
        {
            DisplayStartingMessage();
            int breathCycle = 5; 
            int elapsed = 0;
            while (elapsed < duration)
            {
                Console.Write("Breathe in... ");
                DisplayCountdown(breathCycle);
                elapsed += breathCycle;
                if (elapsed >= duration)
                    break;

                Console.Write("Breathe out... ");
                DisplayCountdown(breathCycle);
                elapsed += breathCycle;
            }
            DisplayEndingMessage();
        }
    }

    // Reflection activity: prompts the user with a reflective prompt and then displays random follow-up questions.
    class ReflectionActivity : MindfulnessActivity
    {
        private List<string> prompts = new List<string>
        {
            "Think of a time when you stood up for someone else.",
            "Think of a time when you did something really difficult.",
            "Think of a time when you helped someone in need.",
            "Think of a time when you did something truly selfless."
        };

        private List<string> questions = new List<string>
        {
            "Why was this experience meaningful to you?",
            "Have you ever done anything like this before?",
            "How did you get started?",
            "How did you feel when it was complete?",
            "What made this time different than other times?",
            "What is your favorite thing about this experience?",
            "What could you learn from this experience?",
            "What did you learn about yourself through this experience?",
            "How can you keep this experience in mind in the future?"
        };

        public ReflectionActivity()
            : base("Reflection", "This activity will help you reflect on times when you have shown strength and resilience, helping you recognize your inner power.")
        {
        }

        public override void RunActivity()
        {
            DisplayStartingMessage();

            // Display a random reflective prompt.
            Random rand = new Random();
            string chosenPrompt = prompts[rand.Next(prompts.Count)];
            Console.WriteLine(chosenPrompt);
            Console.WriteLine("When you have something in mind, press enter to continue.");
            Console.ReadLine();

            // Show random reflection questions until the activity duration is reached.
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddSeconds(duration);
            while (DateTime.Now < endTime)
            {
                string question = questions[rand.Next(questions.Count)];
                Console.WriteLine("> " + question);
                // Pause with a spinner for 5 seconds between questions.
                DisplaySpinner(5);
            }
            DisplayEndingMessage();
        }
    }

    // Listing activity: asks the user to list as many items as possible based on a prompt.
    class ListingActivity : MindfulnessActivity
    {
        private List<string> prompts = new List<string>
        {
            "Who are people that you appreciate?",
            "What are personal strengths of yours?",
            "Who are people that you have helped this week?",
            "When have you felt the Holy Ghost this month?",
            "Who are some of your personal heroes?"
        };

        public ListingActivity()
            : base("Listing", "This activity will help you reflect on the good things in your life by having you list as many items as you can in a certain area.")
        {
        }

        public override void RunActivity()
        {
            DisplayStartingMessage();

            // Display a random prompt.
            Random rand = new Random();
            string chosenPrompt = prompts[rand.Next(prompts.Count)];
            Console.WriteLine(chosenPrompt);

            Console.WriteLine("You will have a few seconds to think about it...");
            DisplayCountdown(5);

            Console.WriteLine("Now, list as many items as you can. Press enter after each item. When you are done, press enter on an empty line.");

            List<string> items = new List<string>();
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddSeconds(duration);

            // Since Console.ReadLine() is blocking, this allow the user to list items until they press enter on an empty line.
            
            while (DateTime.Now < endTime)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    break;
                items.Add(input);
            }

            Console.WriteLine($"You listed {items.Count} items.");
            DisplayEndingMessage();
        }
    }

    // Main Program class containing the menu system.
    class Program
    {
        static void Main(string[] args)
        {
            bool keepRunning = true;
            while (keepRunning)
            {
                Console.Clear();
                Console.WriteLine("Mindfulness App Menu");
                Console.WriteLine("1. Breathing Activity");
                Console.WriteLine("2. Reflection Activity");
                Console.WriteLine("3. Listing Activity");
                Console.WriteLine("4. Quit");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                MindfulnessActivity activity = null;
                switch (choice)
                {
                    case "1":
                        activity = new BreathingActivity();
                        break;
                    case "2":
                        activity = new ReflectionActivity();
                        break;
                    case "3":
                        activity = new ListingActivity();
                        break;
                    case "4":
                        keepRunning = false;
                        break;
                    default:
                        Console.WriteLine("Wrong choice. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }

                if (activity != null)
                {
                    activity.RunActivity();
                    Console.WriteLine("Press any key to return to the main menu.");
                    Console.ReadKey();
                }
            }
        }
    }
}
