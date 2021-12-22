using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaro
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager chessBoard;
        int countPause = 0;
        int countSwitch = 0;

        SocketManager socket;
        #endregion
        public Form1()
        {
            InitializeComponent();

            chessBoard = new ChessBoardManager(pnlChessBoard, PlayerName, picMark);
            chessBoard.EndedGame += ChessBoard_EndedGame;
            chessBoard.PlayerMark += ChessBoard_PlayerMark;
            chessBoard.DrawChessBoard();

            socket = new SocketManager();
        }

        #region Methods
        void EndGame()
        {         
            tmProcess.Stop();
            prcTimer.Value = 0;
            MessageBox.Show("Người chơi " + chessBoard.Player[Math.Abs(chessBoard.CurrentPlayer - 1)].Name + " chiến thắng");
        }

        void Continue()
        {
            tmProcess.Start();
        }

        void NewGame()
        {
            tmEndedGame.Stop();
            chessBoard.DrawChessBoard();
            tmProcess.Stop();
            prcTimer.Value = 0;
        }

        void Pause()
        {
            if (countPause % 2 == 0)
            {               
                pnlChessBoard.Enabled = false;
                tmProcess.Stop();
                MessageBox.Show("Người chơi " + chessBoard.Player[Math.Abs(chessBoard.CurrentPlayer - 1)].Name
                                + " đang tạm dừng");
            }
            else
            {                
                pnlChessBoard.Enabled = true;
                tmProcess.Start();
            }
        }

        void Quit()
        {
            Application.Exit();
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void ChessBoard_PlayerMark(object sender, ButtonClickEvent e)
        {
            tmProcess.Start();
            pnlChessBoard.Enabled = false;
            prcTimer.Value = 0;

            socket.Send(new SocketData((int)SocketCommand.SEND_POINT, "", e.ClickedPoint));

            Listen();
        }

        private void TmProcess_Tick(object sender, EventArgs e)
        {
            prcTimer.PerformStep();
            if (prcTimer.Value == prcTimer.Maximum)
            {
                tmProcess.Stop();
                prcTimer.Value = 0;
                chessBoard.Ramdom();
                Continue();
            }
        }

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            socket.Send(new SocketData((int)SocketCommand.NEW_GAME, ""));
            NewGame();
        }

        private void PauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            socket.Send(new SocketData((int)SocketCommand.PAUSE, ""));
            Pause();
            countPause++;
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát chương trình", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                e.Cancel = true;
        }

        public void TmEndedGame_Tick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (countSwitch % 2 == 0)
                btn.BackgroundImage = Image.FromFile(Application.StartupPath + "\\Resources\\o.png");
            else
                btn.BackgroundImage = Image.FromFile(Application.StartupPath + "\\Resources\\x.png");
            countSwitch++;
        }

        private void BtnLAN_Click(object sender, EventArgs e)
        {
            socket.IP = txbIP.Text;

            if (!socket.ConnectServer())
            {
                socket.isServer = true;
                pnlChessBoard.Enabled = true;
                socket.CreateServer();
            }
            else
            {
                socket.isServer = false;
                pnlChessBoard.Enabled = false;
                Listen();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            txbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            if (string.IsNullOrEmpty(txbIP.Text))
            {
                txbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }

        void Listen()
        {
            Thread listenThread = new Thread(() =>
            {
                try
                {
                    SocketData data = (SocketData)socket.Receive();

                    ProcessData(data);
                }
                catch
                {
                }
            });
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        private void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketCommand.NOTIFY:
                    MessageBox.Show(data.Message);
                    break;
                case (int)SocketCommand.NEW_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        this.NewGame();
                    }));
                    break;
                case (int)SocketCommand.PAUSE:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        this.Pause();
                    }));
                    break;
                case (int)SocketCommand.SEND_POINT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        prcTimer.Value = 0;
                        pnlChessBoard.Enabled = true;
                        tmProcess.Start();
                        chessBoard.OtherPlayerMark(data.Point);
                    }));
                    break;
                case (int)SocketCommand.UNDO:
                    break;
                case (int)SocketCommand.END_GAME:
                    break;
                case (int)SocketCommand.QUIT:
                    break;
                default:
                    break;
            }

            Listen();
        }

        #endregion
    }
}
