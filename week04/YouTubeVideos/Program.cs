using System;
using System.Collections.Generic;

namespace YouTubeVideoTracker
{
    // Class for comment on a video.
    public class Comment
    {
        public string CommenterName { get; set; }
        public string CommentText { get; set; }

        public Comment(string commenterName, string commentText)
        {
            CommenterName = commenterName;
            CommentText = commentText;
        }
    }

    // Class for YouTube video.
    public class Video
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int LengthInSeconds { get; set; }
        private List<Comment> comments;

        public Video(string title, string author, int lengthInSeconds)
        {
            Title = title;
            Author = author;
            LengthInSeconds = lengthInSeconds;
            comments = new List<Comment>();
        }

        // Add a comment to the video.
        public void AddComment(Comment comment)
        {
            comments.Add(comment);
        }

        // Return the number of comments.
        public int GetCommentCount()
        {
            return comments.Count;
        }

        // Return the list of comments.
        public List<Comment> GetComments()
        {
            return comments;
        }
    }

    // Main Program class.
    class Program
    {
        static void Main(string[] args)
        {
            // Create a list to store videos.
            List<Video> videos = new List<Video>();

            // Create Video 1.
            Video video1 = new Video("Amazing Nature", "NatureChannel", 300);
            video1.AddComment(new Comment("Alice", "What a beautiful view!"));
            video1.AddComment(new Comment("Bob", "I love this video!"));
            video1.AddComment(new Comment("Charlie", "I want to visit."));
            videos.Add(video1);

            // Create Video 2.
            Video video2 = new Video("Tech Review: iphone-android hybrid", "TechGuru", 600);
            video2.AddComment(new Comment("Dave", "Great review."));
            video2.AddComment(new Comment("Eve", "I want this phone."));
            video2.AddComment(new Comment("Frank", "I can't beleive they did it!"));
            videos.Add(video2);

            // Create Video 3.
            Video video3 = new Video("Cooking 101: Pasta Recipe", "ChefMaster", 420);
            video3.AddComment(new Comment("Grace", "I can't wait to try this recipe!"));
            video3.AddComment(new Comment("Heidi", "Looks delicious."));
            video3.AddComment(new Comment("Ivan", "Perfect in a pinch."));
            videos.Add(video3);

            // Iterate through the list of videos and display the details.
            foreach (Video video in videos)
            {
                Console.WriteLine("Title: " + video.Title);
                Console.WriteLine("Author: " + video.Author);
                Console.WriteLine("Length (seconds): " + video.LengthInSeconds);
                Console.WriteLine("Number of Comments: " + video.GetCommentCount());
                Console.WriteLine("Comments:");

                foreach (Comment comment in video.GetComments())
                {
                    Console.WriteLine("  - " + comment.CommenterName + ": " + comment.CommentText);
                }

                Console.WriteLine(new string('-', 40)); // Separator between videos
            }

            // Pause the console before exiting.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
