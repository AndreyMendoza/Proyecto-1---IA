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
        public Board board { get; set; }// = new Board("Paché", 17, 7, 5);
        Boolean showingPath = false;
        PictureBox [,] visualBoard = new PictureBox[7,17];
        Node lastPath;

        public principalWindown(Board Board)
        {
            board = Board;
            //SpeechRecognition mc = new SpeechRecognition();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            {
                for (int i = 0; i < board.Size.Item1+1; i++)
                {
                    for (int j = 0; j < board.Size.Item2+1; j++)
                    {
                        visualBoard[i, j] = new PictureBox();
                        UpDateMatrix(i, j, board.BoardMatrix[i, j]);
                    }
                }
            }
            
        }

        private System.Drawing.Bitmap getAsset(int terrainType) {
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


        public void UpDateMatrix(int i, int j, int terrainType)
        {
            visualBoard[i, j].Image = getAsset(terrainType);
            visualBoard[i, j].Location = new System.Drawing.Point(j * 75 + 75, i * 75 + 75);
            visualBoard[i, j].Size = new System.Drawing.Size(75, 75);
            visualBoard[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
            visualBoard[i, j].Paint += new PaintEventHandler((sender, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                e.Graphics.DrawString("["+(i+1).ToString()+","+(j+1).ToString()+"]", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, 0, 0);
            });
            boardPanel.Controls.Add(visualBoard[i, j]);

        }

        public void DrawBestPath(Board board) {

            Node actualPath = board.ShortestPath();
            lastPath = actualPath;

            if (actualPath == null)
            {
                UpDateMatrix(board.Agent.Position.Item1, board.Agent.Position.Item2, 6);
                UpDateMatrix(board.Agent.Goal.Item1, board.Agent.Goal.Item2, 7);
            }
            else {
                UpDateMatrix(board.Agent.Position.Item1, board.Agent.Position.Item2, 4);
                UpDateMatrix(board.Agent.Goal.Item1, board.Agent.Goal.Item2, 5);
            }

            while (actualPath != null)
            {
                if (board.BoardMatrix[actualPath.X, actualPath.Y] == 0) {
                    UpDateMatrix(actualPath.X, actualPath.Y, 8);
                }
                actualPath = actualPath.Parent;
            }

            showingPath = true;

        }

        public void CleanPath(Board board)
        {

            if (showingPath == true) {
                
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


        private void boardPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           // DrawBestPath();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //CleanPath();
        }
    }
}
