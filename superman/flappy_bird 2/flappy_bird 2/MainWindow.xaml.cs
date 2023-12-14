using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace flappy_bird_2
{
    public partial class MainWindow : Window
    {
        // Timer for game events
        DispatcherTimer GameTimer = new DispatcherTimer();
        // Initial speed of the game
        double speed = 1.0;
        // Player's score
        double Points;
        // Force of gravity
        int Gravity = 8;
        // Flag indicating if the game is over
        bool Over;
        // Elapsed time
        double elapsedTime = 0;
        // Rectangle representing the player (superman)
        Rect supermanBox;
        // Pause the game
        bool IsPaused = true;
        // Game started flag
        bool GameStarted = false;

        public MainWindow()
        {
            InitializeComponent();
            // Subscribe to the timer tick event
            GameTimer.Tick += MainEventTimer;
            // Set the timer interval
            GameTimer.Interval = TimeSpan.FromMilliseconds(20);
            // Start the game
            StartGame();
        }

        // Event handler for the main game timer. Manages game logic and updates UI.
        private void MainEventTimer(object sender, EventArgs e)
        {
            if (!IsPaused)
            {
                // Update the points display
                txtPoints.Content = "Points: " + Points;

                // Update the player's position
                supermanBox = new Rect(Canvas.GetLeft(superman), Canvas.GetTop(superman), superman.Width - 10, superman.Height);
                Canvas.SetTop(superman, Canvas.GetTop(superman) + Gravity);

                // Check if the player goes out of bounds
                if (Canvas.GetTop(superman) < -30 || Canvas.GetTop(superman) > 460)
                {
                    // End the game and reset speed
                    EndGame();
                    speed = 1.0;
                }
                // Ensure speed doesn't exceed the limit
                speed = Math.Min(speed, 8.0);

                // Move pipes and clouds
                foreach (var x in Mygame.Children.OfType<Image>())
                {
                    if ((string)x.Tag == "obs1" || (string)x.Tag == "obs2" || (string)x.Tag == "obs3")
                    {
                        // Move pipes to the left
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - (5 * speed));

                        // Check if the pipe is out of bounds
                        if (Canvas.GetLeft(x) < -100)
                        {
                            // Reset pipe position
                            Canvas.SetLeft(x, 800);
                            // Increase speed and update points
                            speed += 0.02;
                            Points += .5;
                        }

                        // Rectangle representing the pipe for collision detection
                        Rect PillarHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                        // Check for collision with the player
                           if (supermanBox.IntersectsWith(PillarHitBox))
                        {
                            // Reset speed and end the game
                            speed = 1.0;
                            EndGame();
                        }

                        // Move clouds to the left
                        if ((string)x.Tag == "cloud")
                        {
                            Canvas.SetLeft(x, Canvas.GetLeft(x) - 8);

                            // Check if the cloud is out of bounds
                            if (Canvas.GetLeft(x) < -250)
                            {
                                // Reset cloud position
                                Canvas.SetLeft(x, 550);
                                // Update points
                                Points += .5;
                            }
                        }
                    }
                }
            }
        }

        // Handles the key down event for controlling the character's movement.
        private void Down(object sender, KeyEventArgs e)
        {
            // If the game is paused and the space key is pressed, start the game and unpause it
            if (IsPaused && e.Key == Key.Space)
            {
                StartGame();
                IsPaused = false;
                GameStarted = true;
            }

            // Rotate the player and change gravity when space key is pressed
            if (e.Key == Key.Space && !IsPaused)
            {
                Start.Visibility = Visibility.Collapsed;
                superman.RenderTransform = new RotateTransform(-20, superman.Width / 2, superman.Height / 2);
                Gravity = -8;
            }

            // Toggle pause/resume when 'P' key is pressed
            if (e.Key == Key.P && GameStarted)
            {
                TogglePause();
            }
            // Restart the game when 'R' key is pressed and the game is over
            if (e.Key == Key.R && Over == true)
            {
                Ending.Visibility = Visibility.Collapsed;
                FinalPoints.Visibility = Visibility.Collapsed;
                reset.Visibility = Visibility.Collapsed;
                StartGame();
            }
        }

        // Handles the key up event for controlling the character's movement.
        private void Up(object sender, KeyEventArgs e)
        {
            // Reset the rotation and gravity when the space key is released
            superman.RenderTransform = new RotateTransform(5, superman.Width / 2, superman.Height / 2);
            Gravity = 8;
        }

        // Starts the game by setting initial values and positions.
        private void StartGame()
        {
            Mygame.Focus();
            int temp = 300;
            Points = 0;
            Over = false;
            Canvas.SetTop(superman, 190);


            foreach (var x in Mygame.Children.OfType<Image>())
            {
                // Set initial positions for pipes and clouds
                if ((string)x.Tag == "obs1")
                {
                    Canvas.SetLeft(x, 500);
                }
                if ((string)x.Tag == "obs2")
                {
                    Canvas.SetLeft(x, 800);
                }
                if ((string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x, 1100);
                }
                if ((string)x.Tag == "clouds")
                {
                    Canvas.SetLeft(x, 400 + temp);
                    temp = 800;
                }
            }

            // Start the game timer
            GameTimer.Start();
        }

        // Ends the game and displays the game over screen.
        private void EndGame()
        {
            Ending.Visibility = Visibility.Visible;
            FinalPoints.Visibility = Visibility.Visible;
            FinalPoints.Content = $"Best Scores: {Points}";
            reset.Visibility = Visibility.Visible;
            GameTimer.Stop();
            Over = true;
        }

        private void TogglePause()
        {
            if (!Over)
            {
                IsPaused = !IsPaused;

                if (IsPaused)
                {
                    // Stop the game timer when paused
                    GameTimer.Stop();
                }
                else
                {
                    // Resume the game timer when resumed
                    GameTimer.Start();
                }
            }
        }
    }
}