using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;



namespace HCI_1
{
    public partial class Form1 : Form
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private Thread receiveThread;
        private bool isSnakeActive = true;
        private bool isFoxActive = false;
        private bool isTigerActive = false;
        private float handToScreenScale = 3.0f; // Adjust as needed (3x scaling)


        Bitmap background = new Bitmap("mainbg.jpeg");
        Bitmap happyface = new Bitmap("Happy.jpeg");
        Bitmap neturalface = new Bitmap("neutral.jpg");
        Bitmap digustface = new Bitmap("Disgust.jpeg");
        Bitmap fearface = new Bitmap("fear.jpeg");
        Bitmap sadface = new Bitmap("sad.jpeg");
        Bitmap suprizedface = new Bitmap("suprized.jpeg");
        Bitmap angrayface = new Bitmap("Angry.jpeg");


        // SNAKE Elephant
        Bitmap snake = new Bitmap("snake.jpeg");
        Bitmap checkmarksnake = new Bitmap("checkmark.jpeg");
        int xsnake = 150;
        int ysnake = 50;
        bool ballTouchedsnake = false;
        bool snakeapper = true;
        // The ball that helps detect that the snake is in the cage
        int xBall = 900;
        int yBall = 600;
        int ballRadius = 50;
        int checkx1 = 850;
        int checky1 = 500;
        bool movesnake = false;

        //-------------------------------------------------------------------------------------
        //Tiger Alligator
        Bitmap Tiger = new Bitmap("Tiger.jpeg");
        Bitmap checkmarkTiger = new Bitmap("checkmark.jpeg");
        int xTiger = 800;
        int yTiger = 50;
        bool ballTouchedTiger = false;
        bool Tigerapper = true;
        // The ball that helps detect that the Tiger is in the cage
        int xBallT = 600;
        int yBallT = 600;
        int ballRadiusT = 50;
        int checkx1T = 550;
        int checky1T = 500;
        bool moveTiger = false;
        //-------------------------------------------------------------------------------------
        //fox cheetah
        Bitmap Fox = new Bitmap("fox.jpeg");
        Bitmap checkmarkFox = new Bitmap("checkmark.jpeg");
        int xfox = 500;
        int yfox = 50;
        bool ballTouchedfox = false;
        bool foxapper = true;
        // The ball that helps detect that the Fox is in the cage
        int xBallF = 300;
        int yBallF = 600;
        int ballRadiusF = 50;
        int checkx1F = 250;
        int checky1F = 500;
        bool movefox = false;
        //---------------------------------------------------------------------------------------
        //winng picture
        Bitmap winning = new Bitmap("Winngbg.jpeg");
        bool c1 = false;
        bool c2 = false;
        bool c3 = false;
        //----------------------------------------------------------------------------------------
        public int flagE;
        public string data;
        BufferedGraphicsContext context;
        BufferedGraphics graphics;

        public Form1()
        {
            InitializeComponent();

            this.Size = new Size(background.Width, background.Height);

            this.Paint += new PaintEventHandler(Form1_Paint);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            context = BufferedGraphicsManager.Current;
            graphics = context.Allocate(this.CreateGraphics(), this.DisplayRectangle);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConnectToServer("localhost", 5050);

            receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawScene(graphics.Graphics);
            graphics.Render(e.Graphics);
        }

        private void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            g.DrawImage(background, 0, 0, background.Width, background.Height);
            g.DrawString("Fox", new Font("Arial", 60), Brushes.Red, new PointF(250, 450));
            g.DrawString("Tiger", new Font("Arial", 60), Brushes.Red, new PointF(550, 450));
            g.DrawString("Snake", new Font("Arial", 60), Brushes.Red, new PointF(800, 450));
            g.DrawString($"Welcome {data} Good luck ", new Font("Arial", 30), Brushes.Red, new PointF(30, 13));

            if (snakeapper)
            {
                g.DrawImage(snake, xsnake, ysnake, snake.Width / 5, snake.Height / 5);
            }
            else
            {

                g.DrawImage(checkmarksnake, checkx1, checky1, checkmarksnake.Width / 5, checkmarksnake.Height / 5);
                movesnake = false;
                c1 = true;
            }
            if (Tigerapper)
            {
                g.DrawImage(Tiger, xTiger, yTiger, Tiger.Width / 5, Tiger.Height / 5);
            }
            else
            {
                g.DrawImage(checkmarkTiger, checkx1T, checky1T, checkmarkTiger.Width / 5, checkmarkTiger.Height / 5);
                moveTiger = false;
                c2 = true;
            }
            if (foxapper)
            {
                g.DrawImage(Fox, xfox, yfox, Fox.Width / 5, Fox.Height / 5);
            }
            else
            {
                g.DrawImage(checkmarkFox, checkx1F, checky1F, checkmarkFox.Width / 5, checkmarkFox.Height / 5);
                movefox = false;
                c3 = true;
            }
            if (flagE == 0)
            {
                g.DrawImage(neturalface, 20, 40, neturalface.Width / 3, neturalface.Height / 3);
            }
            if (flagE == 1)
            {
                g.DrawImage(happyface, 20, 40, happyface.Width / 2, happyface.Height / 2);
            }
            if (flagE == 2)
            {
                g.DrawImage(angrayface, 20, 40, angrayface.Width / 2, angrayface.Height / 2);
            }
            if (flagE == 3)
            {
                g.DrawImage(digustface, 20, 40, digustface.Width / 2, digustface.Height / 2);
            }
            if (flagE == 4)
            {
                g.DrawImage(suprizedface, 20, 40, suprizedface.Width / 2, suprizedface.Height / 2);
            }
            if (flagE == 5)
            {
                g.DrawImage(sadface, 20, 40, sadface.Width / 2, sadface.Height / 2);
            }
            if (flagE == 6)
            {
                g.DrawImage(fearface, 20, 40, fearface.Width / 2, fearface.Height / 2);
            }
            if (c1 == true && c2 == true && c3 == true)
            {
                g.DrawImage(winning, 0, 0, winning.Width, winning.Height);
            }



            //DrawBall1(g, xBallT, yBallT, ballRadiusT);
            //DrawBall2(g, xBallF, yBallF, ballRadiusF);

        }

        private void ConnectToServer(string host, int portNumber)
        {
            try
            {
                tcpClient = new TcpClient(host, portNumber);
                stream = tcpClient.GetStream();
                Console.WriteLine("Connection Made! with " + host);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Connection Failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    byte[] receiveBuffer = new byte[1024];
                    int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);

                    if (bytesRead == 0)
                    {
                        MessageBox.Show("Connection closed by the server.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }

                    string receivedData = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead).Trim();
                    Console.WriteLine($"Received raw data: {receivedData}");

                    // Split the received string into hand coordinates and emotion
                    string[] splitData = receivedData.Split(' ');
                    if (splitData.Length >= 2)
                    {
                        string coordinatesStr = splitData[0];
                        string emotionStr = splitData[1];

                        // Parse the hand coordinates
                        string[] coordinates = coordinatesStr.Split(';');
                        foreach (var coord in coordinates)
                        {
                            string[] parts = coord.Split(',');
                            if (parts.Length == 2 &&
                                float.TryParse(parts[0], out float x) &&
                                float.TryParse(parts[1], out float y))
                            {
                                Console.WriteLine($"Hand Coordinates: X={x}, Y={y}");

                                this.Invoke((Action)(() =>
                                {
                                    if (isSnakeActive)
                                    {
                                        Console.WriteLine("Snake is active.");
                                        MoveSnake(x / this.ClientSize.Width, y / this.ClientSize.Height);
                                    }
                                    else if (isFoxActive)
                                    {
                                        Console.WriteLine("Fox is active.");
                                        MoveFox(x / this.ClientSize.Width, y / this.ClientSize.Height);
                                    }
                                    else if (isTigerActive)
                                    {
                                        Console.WriteLine("Tiger is active.");
                                        MoveTiger(x / this.ClientSize.Width, y / this.ClientSize.Height);
                                    }
                                }));
                            }
                        }

                        // Parse and handle emotion
                        Console.WriteLine($"Emotion received: {emotionStr}");
                        this.Invoke((Action)(() =>
                        {
                            flagE = ParseEmotion(emotionStr); // Translate emotion string to a flag value
                            this.Invalidate(); // Trigger GUI repaint
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error receiving data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper to map emotion string to flag value
        private int ParseEmotion(string emotion)
        {
            switch (emotion)
            {
                case "Neutral":
                    return 0;
                case "Happy":
                    return 1;
                case "Angry":
                    return 2;
                case "Disgust":
                    return 3;
                case "Surprise":
                    return 4;
                case "Sad":
                    return 5;
                case "Fear":
                    return 6;
                default:
                    return 0;
            }
        }


        private void MoveSnake(float x, float y)
        {
            if (isSnakeActive) // Only move the snake if it's active
            {
                if (!ballTouchedsnake) // Check if the snake has been "touched"
                {
                    int ballX = (xsnake + (snake.Width / 10)) - 10;
                    int ballY = (ysnake + (snake.Height / 10)) - 10;

                    // Scale coordinates to the form size
                    int fingerX = (int)(x * this.ClientSize.Width * 1.5);
                    int fingerY = (int)(y * this.ClientSize.Height * 1.5);

                    double distance = Math.Sqrt(Math.Pow(ballX - fingerX, 2) + Math.Pow(ballY - fingerY, 2));
                    if (distance < 20 + Math.Min(snake.Width, snake.Height) / 10)
                    {
                        ballTouchedsnake = true;
                        movesnake = true;
                    }
                }

                if (ballTouchedsnake && movesnake)
                {
                    xsnake = (int)(x * this.ClientSize.Width * 1.5) - (snake.Width / 10);
                    ysnake = (int)(y * this.ClientSize.Height * 1.5) - (snake.Height / 10);
                }

                if (snakeapper && ballTouchedsnake) // Check if the snake has reached the cage
                {
                    int snakeX = xsnake + (snake.Width / 10);
                    int snakeY = ysnake + (snake.Height / 10);

                    double snakeDistance = Math.Sqrt(Math.Pow(xBall - snakeX, 2) + Math.Pow(yBall - snakeY, 2));
                    if (snakeDistance < ballRadius)
                    {
                        snakeapper = false; // Snake is in the cage
                        isSnakeActive = false; // Deactivate snake
                        isFoxActive = true;   // Activate fox
                        Console.WriteLine("Snake reached the cage. Activating Fox.");
                    }
                }
            }

            this.Invalidate();
        }

        private void MoveFox(float x, float y)
        {
            if (isFoxActive) // Only move the fox if it's active
            {
                if (!ballTouchedfox)
                {
                    int ballX = (xfox + (Fox.Width / 10)) - 10;
                    int ballY = (yfox + (Fox.Height / 10)) - 10;

                    // Scale coordinates to the form size
                    int fingerX = (int)(x * this.ClientSize.Width * 1.5);
                    int fingerY = (int)(y * this.ClientSize.Height * 1.5);

                    double distance = Math.Sqrt(Math.Pow(ballX - fingerX, 2) + Math.Pow(ballY - fingerY, 2));
                    if (distance < 20 + Math.Min(Fox.Width, Fox.Height) / 10)
                    {
                        ballTouchedfox = true;
                    }
                }

                if (ballTouchedfox)
                {
                    xfox = (int)(x * this.ClientSize.Width * 1.5) - (Fox.Width / 10);
                    yfox = (int)(y * this.ClientSize.Height * 1.5) - (Fox.Height / 10);
                }

                if (foxapper && ballTouchedfox)
                {
                    int foxCenterX = xfox + (Fox.Width / 10);
                    int foxCenterY = yfox + (Fox.Height / 10);

                    double foxDistance = Math.Sqrt(Math.Pow(xBallF - foxCenterX, 2) + Math.Pow(yBallF - foxCenterY, 2));
                    if (foxDistance < ballRadiusF)
                    {
                        foxapper = false; // Fox is in the cage
                        isFoxActive = false; // Deactivate fox
                        isTigerActive = true; // Activate tiger
                        Console.WriteLine("Fox reached the cage. Activating Tiger.");
                    }
                }
            }

            this.Invalidate();
        }

        private void MoveTiger(float x, float y)
        {
            if (isTigerActive) // Only move the tiger if it's active
            {
                if (!ballTouchedTiger)
                {
                    int ballX = (xTiger + (Tiger.Width / 10)) - 10;
                    int ballY = (yTiger + (Tiger.Height / 10)) - 10;

                    // Scale coordinates to the form size
                    int fingerX = (int)(x * this.ClientSize.Width * 1.5);
                    int fingerY = (int)(y * this.ClientSize.Height * 1.5);

                    double distance = Math.Sqrt(Math.Pow(ballX - fingerX, 2) + Math.Pow(ballY - fingerY, 2));
                    if (distance < 20 + Math.Min(Tiger.Width, Tiger.Height) / 10)
                    {
                        ballTouchedTiger = true;
                    }
                }

                if (ballTouchedTiger)
                {
                    xTiger = (int)(x * this.ClientSize.Width * 1.5) - (Tiger.Width / 10);
                    yTiger = (int)(y * this.ClientSize.Height * 1.5) - (Tiger.Height / 10);
                }

                if (Tigerapper && ballTouchedTiger)
                {
                    int tigerCenterX = xTiger + (Tiger.Width / 10);
                    int tigerCenterY = yTiger + (Tiger.Height / 10);

                    double tigerDistance = Math.Sqrt(Math.Pow(xBallT - tigerCenterX, 2) + Math.Pow(yBallT - tigerCenterY, 2));
                    if (tigerDistance < ballRadiusT)
                    {
                        Tigerapper = false; // Tiger is in the cage
                        isTigerActive = false; // Deactivate tiger
                        Console.WriteLine("Tiger reached the cage. All animals are in their cages!");
                        MessageBox.Show("All animals are in their cages!");
                    }
                }
            }

            this.Invalidate();
        }


        private void DrawBall(Graphics g, int x, int y, int radius)
        {
            int diameter = radius * 2;
            //  g.FillEllipse(Brushes.Red, x - radius, y - radius, diameter, diameter);
        }

        private enum Animal
        {
            Snake,
            Fox,
            Tiger
        }

        private Animal currentAnimal = Animal.Snake;





        //------------------------------------------------------------------------
        //TIGER
        private void DrawBall1(Graphics g, int xt, int yt, int radiust)
        {
            int diametert = radiust * 2;
            g.FillEllipse(Brushes.Red, xt - radiust, yt - radiust, diametert, diametert);
        }


        //---------------------------------------------------------------------------------------------------
        private void DrawBall2(Graphics g, int xf, int yf, int radiusf)
        {
            int diameterf = radiusf * 2;
            g.FillEllipse(Brushes.Red, xf - radiusf, yf - radiusf, diameterf, diameterf);
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            stream.Close();
            tcpClient.Close();
            Console.WriteLine("Connection terminated.");
        }
    }
}
