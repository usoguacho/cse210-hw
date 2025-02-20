using System;
using System.Collections.Generic;

namespace FitnessTracker
{
    // Base
    public abstract class Activity
    {
        private DateTime _date;
        private int _duration;  // in minutes

        public Activity(DateTime date, int duration)
        {
            _date = date;
            _duration = duration;
        }

        public DateTime Date => _date;
        public int Duration => _duration;

        // calculate distance (miles), speed (mph) and pace (min per mile)
        public abstract double GetDistance();
        public abstract double GetSpeed(); // mph
        public abstract double GetPace();  // minutes per mile

        
        public virtual string GetSummary()
        {
            string formattedDate = Date.ToString("dd MMM yyyy");
            string activityType = GetType().Name;
            // Format: Running (30 min) - Distance 3.0 miles, Speed 6.0 mph, Pace: 10.0 min per mile
            return $"{formattedDate} {activityType} ({Duration} min) - " +
                   $"Distance {GetDistance():0.0} miles, " +
                   $"Speed {GetSpeed():0.0} mph, " +
                   $"Pace: {GetPace():0.0} min per mile";
        }
    }

    // Running activity: user provides the distance (in miles)
    public class Running : Activity
    {
        private double _distance;  // miles

        public Running(DateTime date, int duration, double distance)
            : base(date, duration)
        {
            _distance = distance;
        }

        public override double GetDistance()
        {
            return _distance;
        }

        public override double GetSpeed()
        {
            // Speed (mph) = (distance / duration_in_minutes) * 60
            return (_distance / Duration) * 60;
        }

        public override double GetPace()
        {
            // Pace (min per mile) = duration / distance
            return Duration / _distance;
        }
    }

    // Cycling activity: user provides the speed (in mph)
    public class Cycling : Activity
    {
        private double _speed; // mph (user's set value)

        public Cycling(DateTime date, int duration, double speed)
            : base(date, duration)
        {
            _speed = speed;
        }

        public override double GetSpeed()
        {
            return _speed;
        }

        public override double GetDistance()
        {
            // Distance (miles) = speed (mph) * (duration / 60)
            return _speed * (Duration / 60.0);
        }

        public override double GetPace()
        {
            // Pace (min per mile) = 60 / speed
            return 60 / _speed;
        }
    }

    // Swimming activity: user provides the number of laps. Each lap is 50 meters.
    public class Swimming : Activity
    {
        private int _laps;

        public Swimming(DateTime date, int duration, int laps)
            : base(date, duration)
        {
            _laps = laps;
        }

        public override double GetDistance()
        {
            // Distance (miles) = (laps * 50 meters) / 1000 => km then convert km to miles (1 km = 0.62 miles)
            double km = (_laps * 50) / 1000.0;
            return km * 0.62;
        }

        public override double GetSpeed()
        {
            // Speed (mph) = distance (miles) / (duration/60)
            return GetDistance() / (Duration / 60.0);
        }

        public override double GetPace()
        {
            // Pace (min per mile) = duration / distance
            return Duration / GetDistance();
        }
    }

    // Main Program class.
    class Program
    {
        static void Main(string[] args)
        {
            // Create one instance of each activity.
            Activity running = new Running(new DateTime(2022, 11, 03), 30, 3.0);   
            Activity cycling = new Cycling(new DateTime(2022, 11, 03), 45, 12.0);  
            Activity swimming = new Swimming(new DateTime(2022, 11, 03), 30, 20);   

            // Add them to a list
            List<Activity> activities = new List<Activity> { running, cycling, swimming };

            // Iterate through the list and display the summary for each activity.
            foreach (Activity act in activities)
            {
                Console.WriteLine(act.GetSummary());
            }
        }
    }
}
