using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        public Form1()
        {
            InitializeComponent();

            //Set settings to default
            new Settings();

            //Set game speed and start timer
            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            //Start new game
            StartGame();
        }

        private void StartGame()
        {
            lblGameOver.Visible = false;
            //Set settings to default
            new Settings();

            //Create new player object
            Snake.Clear();
            Circle head = new Circle { x = 10, y = 5 };
            Snake.Add(head);

            labelScore.Text = Settings.Score.ToString();
            GenerateFood();
        }

        //Place random food object
        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle { x = random.Next(0, maxXPos), y = random.Next(0, maxYPos) };
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            //Check for Game Over
            if (Settings.GameOver)
            {
                //Check if enter is pressed
                if (Inputs.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Inputs.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                    Settings.direction = Direction.Right;
                else if (Inputs.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                    Settings.direction = Direction.Left;
                else if (Inputs.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                    Settings.direction = Direction.Up;
                else if (Inputs.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                    Settings.direction = Direction.Down;

                MovePlayer();
            }

            pbCanvas.Invalidate();
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                //Set colour of snake

                //Draw snake
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColour;
                    if (i == 0)
                        snakeColour = Brushes.Black;    //Draw head
                    else
                        snakeColour = Brushes.Green;    //Rest of body

                    //Draw snake
                    canvas.FillEllipse(snakeColour,
                        new Rectangle(Snake[i].x * Settings.Width,
                                      Snake[i].y * Settings.Height,
                                      Settings.Width, Settings.Height));

                    //Draw food
                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(food.x * Settings.Width,
                            food.y * Settings.Height, Settings.Width, Settings.Height));
                }
            }
            else
            {
                string gameOver = "Game over \nYour final score is: " + Settings.Score + "\nPress Enter to try again";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
            }
        }

        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                //Move head
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].x++;
                            break;
                        case Direction.Left:
                            Snake[i].x--;
                            break;
                        case Direction.Up:
                            Snake[i].y--;
                            break;
                        case Direction.Down:
                            Snake[i].y++;
                            break;
                    }

                    //Get maximum X and Y Pos
                    int maxXPos = pbCanvas.Size.Width / Settings.Width;
                    int maxYPos = pbCanvas.Size.Height / Settings.Height;

                    //Detect collisions with game borders
                    if(Snake[i].x<0 || Snake[i].y<0
                        || Snake[i].x >= maxXPos || Snake[i].y >= maxYPos)
                    {
                       Die();
                    }

                    //Detect collisions with body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if(Snake[i].x==Snake[j].x &&
                            Snake[i].y == Snake[j].y)
                        {
                           Die();
                        }
                    }

                    //Detect collisions with food pieces
                    if (Snake[0].x == food.x && Snake[0].y == food.y)
                    {
                        Eat();
                    }
                }
                else
                {
                    Snake[i].x = Snake[i - 1].x;
                    Snake[i].y = Snake[i - 1].y;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Inputs.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Inputs.ChangeState(e.KeyCode, false);
        }

        private void Eat()
        {
            //Add circle to body
            Circle circle = new Circle
            {
                x = Snake[Snake.Count - 1].x,
                y = Snake[Snake.Count - 1].y
            };
            Snake.Add(circle);

            //Update score
            Settings.Score += Settings.Points;
            labelScore.Text = Settings.Score.ToString();

            GenerateFood();
        }

        private void Die()
        {
            Settings.GameOver = true;
        }
    }
}
