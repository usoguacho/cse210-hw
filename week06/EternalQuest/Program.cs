using System;
using System.Collections.Generic;
using System.IO;

namespace EternalQuest
{
    // =============================
    //           Base Class
    // =============================
    public abstract class Goal
    {
        private string _name;
        private string _description;
        private int _points;

        protected Goal(string name, string description, int points)
        {
            _name = name;
            _description = description;
            _points = points;
        }

        public string Name => _name;
        public string Description => _description;
        public int Points => _points;

        // Whether the goal is complete or not
        public abstract bool IsComplete { get; }

        // This method is called when the user records an event for the goal.
        // It returns how many points to award the user.
        public abstract int RecordEvent();

        // Returns a string like "[ ]" or "[X]" to show completion status in a list.
        public abstract string GetStatusString();

        // Used for saving/loading goals to a file. Subclasses override to add extra info.
        public virtual string Serialize()
        {
            // Format: GoalType|Name|Description|Points|... (subclass data)
            return $"{GetType().Name}|{_name}|{_description}|{_points}";
        }

        // Each subclass must have a way to deserialize from a string array.
        public static Goal Deserialize(string[] parts)
        {
            // parts[0] = type, parts[1] = name, parts[2] = description, parts[3] = points
            string goalType = parts[0];
            string name = parts[1];
            string description = parts[2];
            int points = int.Parse(parts[3]);

            switch (goalType)
            {
                case "SimpleGoal":
                    bool completed = bool.Parse(parts[4]);
                    return new SimpleGoal(name, description, points, completed);
                case "EternalGoal":
                    return new EternalGoal(name, description, points);
                case "ChecklistGoal":
                    int timesCompleted = int.Parse(parts[4]);
                    int timesRequired = int.Parse(parts[5]);
                    int bonusPoints = int.Parse(parts[6]);
                    return new ChecklistGoal(name, description, points, bonusPoints, timesRequired, timesCompleted);
                default:
                    throw new Exception("Unknown goal type in file: " + goalType);
            }
        }
    }

    // =============================
    //      Derived Class: SimpleGoal
    // =============================
    // A simple goal can be completed exactly once. Once done, it's marked complete.
    public class SimpleGoal : Goal
    {
        private bool _completed;

        public SimpleGoal(string name, string description, int points, bool completed = false)
            : base(name, description, points)
        {
            _completed = completed;
        }

        public override bool IsComplete => _completed;

        public override int RecordEvent()
        {
            if (!_completed)
            {
                _completed = true;
                return Points;  // Award the base points once when completed
            }
            // If it's already completed, no additional points
            return 0;
        }

        public override string GetStatusString()
        {
            return _completed ? "[X]" : "[ ]";
        }

        public override string Serialize()
        {
            // Add _completed as part of the data
            return base.Serialize() + $"|{_completed}";
        }
    }

    // =============================
    //     Derived Class: EternalGoal
    // =============================
    // An eternal goal never ends. Each time the user records progress, they get points, but it never completes.
    public class EternalGoal : Goal
    {
        public EternalGoal(string name, string description, int points)
            : base(name, description, points)
        {
        }

        public override bool IsComplete => false;

        public override int RecordEvent()
        {
            // Each record awards the base points, no completion
            return Points;
        }

        public override string GetStatusString()
        {
            // Eternal goals never complete
            return "[∞]";
        }

        public override string Serialize()
        {
            // No extra data to store beyond the base
            return base.Serialize();
        }
    }

    // =============================
    //    Derived Class: ChecklistGoal
    // =============================
    // A checklist goal requires a certain number of completions. Each completion awards some points,
    // and on the final completion, a bonus is awarded.
    public class ChecklistGoal : Goal
    {
        private int _timesCompleted;
        private int _timesRequired;
        private int _bonusPoints;

        public ChecklistGoal(
            string name,
            string description,
            int points,
            int bonusPoints,
            int timesRequired,
            int timesCompleted = 0)
            : base(name, description, points)
        {
            _bonusPoints = bonusPoints;
            _timesRequired = timesRequired;
            _timesCompleted = timesCompleted;
        }

        public override bool IsComplete => _timesCompleted >= _timesRequired;

        public override int RecordEvent()
        {
            if (!IsComplete)
            {
                _timesCompleted++;
                // If this event completes the goal, award bonus
                if (IsComplete)
                {
                    return Points + _bonusPoints;
                }
                else
                {
                    return Points; // Normal points each time
                }
            }
            // If it's already complete, no additional points
            return 0;
        }

        public override string GetStatusString()
        {
            // For example: "[X] Completed 3/5 times" or "[ ] Completed 2/5 times"
            string checkMark = IsComplete ? "[X]" : "[ ]";
            return $"{checkMark} Completed {_timesCompleted}/{_timesRequired} times";
        }

        public override string Serialize()
        {
            // Additional data: _timesCompleted, _timesRequired, _bonusPoints
            return base.Serialize() + $"|{_timesCompleted}|{_timesRequired}|{_bonusPoints}";
        }
    }

    // =============================
    //        Goal Manager
    // =============================
    // This class manages the list of goals and the user's total score.
    public class GoalManager
    {
        private List<Goal> _goals;
        private int _score;

        public GoalManager()
        {
            _goals = new List<Goal>();
            _score = 0;
        }

        public void CreateGoal()
        {
            Console.WriteLine("\nChoose the type of goal to create:");
            Console.WriteLine("1. Simple Goal");
            Console.WriteLine("2. Eternal Goal");
            Console.WriteLine("3. Checklist Goal");
            Console.Write("Option: ");
            string choice = Console.ReadLine();

            Console.Write("Enter a short name for the goal: ");
            string name = Console.ReadLine();

            Console.Write("Enter a description: ");
            string description = Console.ReadLine();

            Console.Write("Enter the amount of points for each completion: ");
            int points = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case "1":
                    // Simple Goal
                    _goals.Add(new SimpleGoal(name, description, points));
                    break;
                case "2":
                    // Eternal Goal
                    _goals.Add(new EternalGoal(name, description, points));
                    break;
                case "3":
                    // Checklist Goal
                    Console.Write("How many times does this goal need to be accomplished? ");
                    int timesRequired = int.Parse(Console.ReadLine());

                    Console.Write("Enter the bonus points upon completion: ");
                    int bonusPoints = int.Parse(Console.ReadLine());

                    _goals.Add(new ChecklistGoal(name, description, points, bonusPoints, timesRequired));
                    break;
                default:
                    Console.WriteLine("Invalid option. No goal created.");
                    break;
            }
        }

        public void ListGoals()
        {
            Console.WriteLine("\nGoals:");
            int i = 1;
            foreach (Goal g in _goals)
            {
                Console.WriteLine($"{i}. {g.GetStatusString()} {g.Name} ({g.Description})");
                i++;
            }
        }

        public void RecordEvent()
        {
            Console.WriteLine("\nWhich goal did you accomplish?");
            ListGoals();
            Console.Write("Enter the goal number: ");
            int index = int.Parse(Console.ReadLine()) - 1;
            if (index < 0 || index >= _goals.Count)
            {
                Console.WriteLine("Invalid goal number.");
                return;
            }

            Goal goal = _goals[index];
            int pointsEarned = goal.RecordEvent();
            _score += pointsEarned;
            Console.WriteLine($"You earned {pointsEarned} points! Your total score is now {_score}.");
        }

        public void ShowScore()
        {
            Console.WriteLine($"\nYour current score is: {_score} points.");
        }

        public void SaveGoals()
        {
            Console.Write("Enter a filename to save to: ");
            string filename = Console.ReadLine();
            using (StreamWriter writer = new StreamWriter(filename))
            {
                // First line: user's score
                writer.WriteLine(_score);

                // Then, each goal on its own line
                foreach (Goal g in _goals)
                {
                    writer.WriteLine(g.Serialize());
                }
            }
            Console.WriteLine("Goals saved successfully!");
        }

        public void LoadGoals()
        {
            Console.Write("Enter the filename to load from: ");
            string filename = Console.ReadLine();
            if (!File.Exists(filename))
            {
                Console.WriteLine("File not found.");
                return;
            }

            string[] lines = File.ReadAllLines(filename);
            _goals.Clear(); // Clear current list
            _score = int.Parse(lines[0]); // First line is score
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] parts = line.Split('|');
                Goal goal = Goal.Deserialize(parts);
                _goals.Add(goal);
            }
            Console.WriteLine("Goals loaded successfully!");
        }
    }

    // =============================
    //           Program
    // =============================
    class Program
    {
        static void Main(string[] args)
        {
            GoalManager manager = new GoalManager();
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nEternal Quest Menu");
                Console.WriteLine("1. Create New Goal");
                Console.WriteLine("2. List Goals");
                Console.WriteLine("3. Save Goals");
                Console.WriteLine("4. Load Goals");
                Console.WriteLine("5. Record Event");
                Console.WriteLine("6. Show Score");
                Console.WriteLine("7. Quit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        manager.CreateGoal();
                        break;
                    case "2":
                        manager.ListGoals();
                        break;
                    case "3":
                        manager.SaveGoals();
                        break;
                    case "4":
                        manager.LoadGoals();
                        break;
                    case "5":
                        manager.RecordEvent();
                        break;
                    case "6":
                        manager.ShowScore();
                        break;
                    case "7":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            Console.WriteLine("Goodbye!");
        }
    }
}
