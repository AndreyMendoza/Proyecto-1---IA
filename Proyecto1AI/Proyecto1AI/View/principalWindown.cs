using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto1AI.Controller;
using Proyecto1AI.Model;
using Proyecto1AI.Properties;

namespace Proyecto1AI.View
{


    partial class principalWindown : Form
    {
        // Delegates in order to allow methods to be called outside this class
        delegate void BestPathCallback();
        delegate void CleanPathCallback();
        delegate void DisposeCallback();
        delegate void UpdateItemPositionCallback(List<Node> Positions);
        delegate void UpdateMatrixCallback(int i, int j, int type);

        public Board board { get; set; }// = new Board("Paché", 17, 7, 5);
        Boolean showingPath = false;
        PictureBox[,] visualBoard;
        Node lastPath;

        public principalWindown(Board Board)
        {
            board = Board;
            visualBoard = new PictureBox[Board.Size.Item1 + 1, Board.Size.Item2 + 1];
            InitializeComponent();
        }


        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Load all the items from the board in the frame
        private void Form1_Load(object sender, EventArgs e)
        {
            {
                for (int i = 0; i < board.Size.Item1 + 1; i++)
                {
                    for (int j = 0; j < board.Size.Item2 + 1; j++)
                    {
                        visualBoard[i, j] = new PictureBox();
                        UpDateMatrix(i, j, board.BoardMatrix[i, j]);
                    }
                }
            }

        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Returns the image that is needed
        private Bitmap getAsset(int terrainType)
        {
            switch (terrainType)
            {
                case 0:
                    return Resources.tierra;
                case 1:
                    switch (new Random().Next(1, 5))
                    {
                        case 1:
                            return Resources.obs1;
                        case 2:
                            return Resources.obs2;
                        case 3:
                            return Resources.obs3;
                        case 4:
                            return Resources.obs4;
                    }
                    break;
                case 2:
                    return Resources.IA;
                case 3:
                    return Resources.goal;
                case 4:
                    return Resources.IAOK;
                case 5:
                    return Resources.goalOK;
                case 6:
                    return Resources.IABad;
                case 7:
                    return Resources.goalBad;

            }
            return Resources.path; ;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Paints a single item in the frame
        public void UpDateMatrix(int i, int j, int terrainType)
        {
            if (boardPanel.InvokeRequired)
            {
                Invoke(new UpdateMatrixCallback(UpDateMatrix), new object[] { i, j, terrainType });
            }
            else
            {
                visualBoard[i, j].Image = getAsset(terrainType);
                visualBoard[i, j].Location = new Point(j * 75 + 75, i * 75 + 75);
                visualBoard[i, j].Size = new Size(75, 75);
                visualBoard[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                visualBoard[i, j].Paint += new PaintEventHandler((sender, e) =>
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    e.Graphics.DrawString("[" + (i + 1).ToString() + "," + (j + 1).ToString() + "]", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, 0, 0);
                });
                boardPanel.Controls.Add(visualBoard[i, j]);
            }
        }


        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Erase the best path from the board
        public void CleanPath()
        {
            if (boardPanel.InvokeRequired)
            {
                Invoke(new CleanPathCallback(CleanPath));
            }
            else
            {
                if (showingPath == true)
                {
                    lastPath = board.ShortestPath();
                    UpDateMatrix(board.Agent.Position.Item1, board.Agent.Position.Item2, 2);
                    UpDateMatrix(board.Agent.Goal.Item1, board.Agent.Goal.Item2, 3);
                    
                    while (lastPath != null)
                    {
                        if (board.BoardMatrix[lastPath.X, lastPath.Y] == 0)
                        {
                            UpDateMatrix(lastPath.X, lastPath.Y, 0);
                        }
                        lastPath = lastPath.Parent;
                    }
                }
                showingPath = false;
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Estimate and shows the best path if exists
        public void DrawBestPath()
        {
            if (boardPanel.InvokeRequired)
            {
                Invoke(new BestPathCallback(DrawBestPath));
            }

            else
            {
                Node actualPath = board.ShortestPath();
                lastPath = actualPath;

                if (actualPath == null)
                {
                    UpDateMatrix(board.Agent.Position.Item1, board.Agent.Position.Item2, 6);
                    UpDateMatrix(board.Agent.Goal.Item1, board.Agent.Goal.Item2, 7);
                }
                else
                {
                    UpDateMatrix(board.Agent.Position.Item1, board.Agent.Position.Item2, 4);
                    UpDateMatrix(board.Agent.Goal.Item1, board.Agent.Goal.Item2, 5);
                }

                while (actualPath != null)
                {
                    if (board.BoardMatrix[actualPath.X, actualPath.Y] == 0)
                    {
                        UpDateMatrix(actualPath.X, actualPath.Y, 8);
                    }
                    actualPath = actualPath.Parent;
                }

                showingPath = true;
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Erase the best path from the board
        public void UpdateItemPosition(List<Node> Positions)
        {
            if (boardPanel.InvokeRequired)
            {
                Invoke(new UpdateItemPositionCallback(UpdateItemPosition), new object[] { Positions });
            }
            else
            {
                Node ActualPosition = Positions[0];
                Node NewPosition = Positions[1];

                // Update the tables
                UpDateMatrix(NewPosition.X, NewPosition.Y, board.BoardMatrix[ActualPosition.X, ActualPosition.Y]);
                UpDateMatrix(ActualPosition.X, ActualPosition.Y, 0);
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Dispose the form
        public void DisposeFrame()
        {
            if (boardPanel.InvokeRequired)
            {
                Invoke(new DisposeCallback(DisposeFrame));
            }
            else
            {
                Dispose();
                Close();
            }
        }


        private void boardPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DrawBestPath();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            CleanPath();
        }


        
    }
}
