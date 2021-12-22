using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaro
{
    public class ChessBoardManager
    {
        #region Properties
        private Panel chessBoard;
        public Panel ChessBoard { get => chessBoard; set => chessBoard = value; }

        private List<Player> player;

        public List<Player> Player
        {
            get { return player; }
            set { player = value; }
        }

        private int currentPlayer;
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public TextBox PlayerName1 { get => PlayerName; set => PlayerName = value; }
        public PictureBox Mark1 { get => Mark; set => Mark = value; }
        public List<List<Button>> Matrix { get => matrix; set => matrix = value; }
        public event EventHandler<ButtonClickEvent> PlayerMark {
            add
            {
                playerMark += value;
            }
            remove
            {
                playerMark -= value;
            }
        }
        public event EventHandler EndedGame {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }

        private TextBox PlayerName;

        private PictureBox Mark;

        List<List<Button>> matrix;

        private event EventHandler<ButtonClickEvent> playerMark;
        private event EventHandler endedGame;

        #endregion

        #region Initialize
        public ChessBoardManager(Panel chessBoard, TextBox PlayerName, PictureBox Mark)
        {
            this.chessBoard = chessBoard;
            this.PlayerName1 = PlayerName;
            this.Mark1 = Mark;
            this.Player = new List<Player>()
            {
                new Player("Player 1", Image.FromFile(Application.StartupPath + "\\Resources\\x.png")),
                new Player("Player 2", Image.FromFile(Application.StartupPath + "\\Resources\\o.png"))
            };
        }
        #endregion

        #region Methods
        public void DrawChessBoard()
        {
            chessBoard.Controls.Clear();
            chessBoard.Enabled = true;
            CurrentPlayer = 0;
            PlayerName1.Text = Player[CurrentPlayer].Name;
            Mark1.Image = Player[CurrentPlayer].Mark;
            Matrix = new List<List<Button>>();
            Button oldButton = new Button() { Width = 0, Location = new Point(0, 0) };
            for (int i = 0; i < ChessBoardInfor.CHESS_BOARD_SIZE; i++)
            {
                Matrix.Add(new List<Button>());
                for (int j = 0; j < ChessBoardInfor.CHESS_BOARD_SIZE; j++)
                {
                    Button btn = new Button()
                    {
                        Width = ChessBoardInfor.CHESS_BOARD_WIDTH,
                        Height = ChessBoardInfor.CHESS_BOARD_HEIGHT,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i
                    };
                    btn.Click += btn_Click;
                    chessBoard.Controls.Add(btn);
                    Matrix[i].Add(btn);
                    oldButton = btn;
                }

                oldButton.Location = new Point(0, oldButton.Location.Y + ChessBoardInfor.CHESS_BOARD_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
        }

        public void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackgroundImage != null)
                return;
            btn.BackgroundImage = Player[CurrentPlayer].Mark;

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;

            PlayerName1.Text = Player[CurrentPlayer].Name;

            Mark1.Image = Player[CurrentPlayer].Mark;

            if (playerMark != null)
                playerMark(this, new ButtonClickEvent(getPoint(btn)));

            if (isEndGame(btn))
            {
                EndGame();
            }
        }

        public void OtherPlayerMark(Point point)
        {
            Button btn = Matrix[point.Y][point.X];
            if (btn.BackgroundImage != null)
                return;
            btn.BackgroundImage = Player[CurrentPlayer].Mark;

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;

            PlayerName1.Text = Player[CurrentPlayer].Name;

            Mark1.Image = Player[CurrentPlayer].Mark;

            if (isEndGame(btn))
            {
                EndGame();
            }
        }

        private Point getPoint(Button btn)
        {
            Point result = new Point();
            result.Y = Convert.ToInt32(btn.Tag);
            result.X = Matrix[result.Y].IndexOf(btn);
            return result;
        }

        public void EndGame()
        {
            if (endedGame != null)
                endedGame(this, new EventArgs());
            ChessBoard.Enabled = false;
        }
        private bool isEndGame(Button btn)
        {
            return isEndVertical(btn) || isEndHorizontal(btn) || isEndMain(btn) || isEndSub(btn);
        }


        private bool isEndVertical(Button btn)
        {

            Point test = getPoint(btn);
            int cUp = 0;
            int cDown = 0;

            for (int i = test.Y + 1; i < ChessBoardInfor.CHESS_BOARD_SIZE; i++)
            {
                if (Matrix[i][test.X].BackgroundImage == btn.BackgroundImage)
                {
                    cUp++;
                }
                else
                    break;
            }
            for (int i = test.Y; i >= 0; i--)
            {
                if (Matrix[i][test.X].BackgroundImage == btn.BackgroundImage)
                {
                    cDown++;
                }
                else
                    break;
            }

            if (cUp + cDown == 5)
                return true;
            return false;
        }

        private bool isEndHorizontal(Button btn)
        {
            Point test = getPoint(btn);
            int cLeft = 0;
            int cRight = 0;

            for (int i = test.X + 1; i < ChessBoardInfor.CHESS_BOARD_SIZE; i++)
            {
                if (Matrix[test.Y][i].BackgroundImage == btn.BackgroundImage)
                    cRight++;
                else
                    break;
            }
            for (int i = test.X; i >= 0; i--)
            {
                if (Matrix[test.Y][i].BackgroundImage == btn.BackgroundImage)
                    cLeft++;
                else
                    break;
            }

            if (cLeft + cRight == 5)
                return true;
            return false;
        }

        private bool isEndMain(Button btn)
        {

            Point test = getPoint(btn);
            int cUp = 0;
            int cDown = 0;

            for (int i = 0; i < ChessBoardInfor.CHESS_BOARD_SIZE; i++)
            {
                if (test.X - i >= 0 && test.Y - i >= 0)
                {
                    if (Matrix[test.Y - i][test.X - i].BackgroundImage == btn.BackgroundImage)
                        cUp++;
                    else
                        break;
                }
            }
            for (int i = 1; i < ChessBoardInfor.CHESS_BOARD_SIZE; i++)
            {
                if (test.X + i <= ChessBoardInfor.CHESS_BOARD_SIZE && test.Y + i <= ChessBoardInfor.CHESS_BOARD_SIZE)
                {
                    if (Matrix[test.Y + i][test.X + i].BackgroundImage == btn.BackgroundImage)
                        cDown++;
                    else
                        break;
                }
            }

            if (cUp + cDown == 5)
                return true;
            return false;
        }

        private bool isEndSub(Button btn)
        {
            Point test = getPoint(btn);
            int cUp = 0;
            int cDown = 0;

            for (int i = 0; i < ChessBoardInfor.CHESS_BOARD_SIZE; i++)
            {
                if (test.X - i >= 0 && test.Y + i <= ChessBoardInfor.CHESS_BOARD_SIZE)
                {
                    if (Matrix[test.Y + i][test.X - i].BackgroundImage == btn.BackgroundImage)
                        cUp++;
                    else
                        break;
                }
            }
            for (int i = 1; i < ChessBoardInfor.CHESS_BOARD_SIZE; i++)
            {
                if (test.X + i <= ChessBoardInfor.CHESS_BOARD_SIZE && test.Y - i >= 0)
                {
                    if (Matrix[test.Y - i][test.X + i].BackgroundImage == btn.BackgroundImage)
                        cDown++;
                    else
                        break;
                }
            }

            if (cUp + cDown == 5)
                return true;
            return false;
        }
        public void Ramdom()
        {
            int i, j;
            Random rd = new Random();
            i = rd.Next(0, ChessBoardInfor.CHESS_BOARD_SIZE - 2);
            j = rd.Next(0, ChessBoardInfor.CHESS_BOARD_SIZE - 3);
            while (Matrix[i][j].BackgroundImage == Player[0].Mark && Matrix[i][j].BackgroundImage == Player[1].Mark)
            {
                i = rd.Next(0, ChessBoardInfor.CHESS_BOARD_SIZE - 2);
                j = rd.Next(0, ChessBoardInfor.CHESS_BOARD_SIZE - 3);
            }
            Matrix[i][j].BackgroundImage = Player[CurrentPlayer].Mark;
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;

            PlayerName1.Text = Player[CurrentPlayer].Name;

            Mark1.Image = Player[CurrentPlayer].Mark;
            return;
        }
        #endregion

    }
    public class ButtonClickEvent : EventArgs
    {
        private Point clickedPoint;

        public Point ClickedPoint
        {
            get { return clickedPoint; }
            set { clickedPoint = value; }
        }

        public ButtonClickEvent(Point point)
        {
            this.ClickedPoint = point;
        }
    }
}
