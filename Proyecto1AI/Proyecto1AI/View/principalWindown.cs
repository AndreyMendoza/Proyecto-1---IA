using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto1AI.Model;
using Proyecto1AI.Properties;

namespace Proyecto1AI.View
{
    

    public partial class principalWindown : Form
    {
        Board board = new Board("Paché", 15, 7, 5);

        public principalWindown()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            {
                for (int i = 0; i < board.Size.Item1+1; i++)
                {
                    for (int j = 0; j < board.Size.Item2+1; j++)
                    {
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

            }
            return Resources.path; ;
        }


        private void UpDateMatrix(int i, int j, int terrainType)
        {
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = getAsset(terrainType);
            pictureBox.Location = new System.Drawing.Point(j * 75 + 75, i * 75 + 75);
            pictureBox.Size = new System.Drawing.Size(75, 75);
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            boardPanel.Controls.Add(pictureBox);

        }

        private void DrawBestPath() {


        }


            private void boardPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
