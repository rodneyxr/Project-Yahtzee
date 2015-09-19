using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Project_Yahtzee
{
    public partial class HighScores : Form
    {
        private readonly String fileDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Yahtzee";
        private readonly String filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Yahtzee\highscores.txt";
        private readonly int NUMBER_OF_RANKS = 20;
        private double[,] ratio;
        private double[,] sbRatio;

        public HighScores()
        {
            InitializeComponent();
        }

        private void HighScores_Load(object sender, EventArgs e)
        {
            initRatio();
            loadHighscores();
        }

        /*******
         * GUI *
         *******/
        #region GUI
        private void initRatio()
        {
            int numberOfControls = 0;
            foreach (Control x in this.Controls)
                numberOfControls += 1;
            ratio = new double[numberOfControls, 4];

            int i = 0;
            foreach (Control x in this.Controls)
            {
                ratio[i, 0] = ((double)this.Width) / ((double)x.Width);
                ratio[i, 1] = ((double)this.Height) / ((double)x.Height);
                ratio[i, 2] = ((double)this.Width) / ((double)x.Location.X);
                ratio[i, 3] = ((double)this.Height) / ((double)x.Location.Y);
                i += 1;
            }

            numberOfControls = 0;
            foreach (Control x in groupBox1.Controls)
                numberOfControls += 1;
            sbRatio = new double[numberOfControls, 3];

            i = 0;
            foreach (Control x in groupBox1.Controls)
            {
                sbRatio[i, 0] = ((double)groupBox1.Height) / ((double)x.Font.Size);
                sbRatio[i, 1] = ((double)groupBox1.Width) / ((double)x.Location.X);
                sbRatio[i, 2] = ((double)groupBox1.Height) / ((double)x.Location.Y);
                i += 1;
            }
        }

        private void adjustGUI()
        {
            int i = 0;
            foreach (Control x in this.Controls)
            {

                if (!(x is MenuStrip))
                    x.Size = new Size((int)(this.Width / ratio[i, 0]), (int)(this.Height / ratio[i, 1]));
                if (!(x is PictureBox))
                    x.Location = new Point((int)(this.Width / ratio[i, 2]), (int)(this.Height / ratio[i, 3]));
                i++;
            }

            i = 0;
            foreach (Control x in groupBox1.Controls)
            {
                x.Font = new Font("Serif", (float)(groupBox1.Height / sbRatio[i, 0]), x.Font.Style);
                x.Location = new Point((int)(groupBox1.Width / sbRatio[i, 1]), (int)(groupBox1.Height / sbRatio[i, 2]));
                i++;
            }
        }

        private void HighScores_SizeChanged(object sender, EventArgs e)
        {
            adjustGUI();
        }
        #endregion GUI

        /**************
         * Highscores *
         **************/
        #region highscores
        public void loadHighscores()
        {
            // Load from text file
            if (!File.Exists(filePath))
                clearHighscores();
            String text = "";
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                    text = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                clearHighscores();
                Console.WriteLine(e.Message);
            }

            // set scores and names into variables
            char[] delimiter = { ',', '\n' };
            String[] items = text.Split(delimiter);
            int[] playerscore = new int[items.Length / 2];
            String[] rankedname = new String[items.Length / 2];

            int counter = 0;
            int index = 0;
            while (counter < items.Length - 1)
            {
                playerscore[index] = Convert.ToInt32(items[counter++]);
                rankedname[index++] = items[counter++];
            }

            Label[] label = { label1, label2, label3, label4, label5, label6, label7, label8, label9, label10, label11, label12, label13, label14,
                             label15, label16, label17, label18, label19, label20 };
            
            for (int x = 0; x < NUMBER_OF_RANKS; x++)
                label[x].Text = String.Format("{0}). {1}, {2}", x + 1, playerscore[x], rankedname[x]);    
        }

        private int[] getScoreArray()
        {
            // Load from text file
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Yahtzee\\highscores.txt";
            String text = "";
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    text = sr.ReadToEnd();
                    Console.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            // set scores and names into variables
            char[] delimiter = { ',', '\n' };
            String[] items = text.Split(delimiter);
            int[] playerscore = new int[items.Length / 2];
            int counter = 0;
            int index = 0;
            while (counter < items.Length - 1)
            {
                playerscore[index++] = Convert.ToInt32(items[counter]);
                counter += 2;
            }
            return playerscore;
        }
        private String[] getNameArray()
        {
            // Load from text file
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Yahtzee\\highscores.txt";
            String text = "";
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    text = sr.ReadToEnd();
                    Console.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            char[] delimiter = { ',', '\n' };
            String[] items = text.Split(delimiter);
            String[] rankedname = new String[items.Length / 2];
            int counter = 1;
            int index = 0;
            while (counter < items.Length - 1)
            {
                rankedname[index++] = items[counter];
                counter += 2;
            }
            return rankedname;
        }

        public void highscores(int finalScore, String playername)
        {
            // Highscores
            int[] playerscore = getScoreArray();
            String[] rankedname = getNameArray();
            
            // Determine the rank
            int rank = 0;

            if (finalScore > playerscore[NUMBER_OF_RANKS - 1])
                for (int x = NUMBER_OF_RANKS - 2; x >= 0; x--)
                {
                    if (finalScore <= playerscore[x])
                    {
                        rank = x + 1;
                        break;
                    }
                }
            else
                rank = NUMBER_OF_RANKS;

            // put the new highscore

            if (rank == NUMBER_OF_RANKS - 1)
            {
                playerscore[rank] = finalScore;
                rankedname[rank] = playername;
            }
            else if (rank < NUMBER_OF_RANKS)
            {
                for (int x = NUMBER_OF_RANKS - 1; x > rank; x--)
                {
                    playerscore[x] = playerscore[x - 1];
                    rankedname[x] = rankedname[x - 1];
                }
                playerscore[rank] = finalScore;
                rankedname[rank] = playername;
            }

            String highscore = "";
            for (int x = 0; x < NUMBER_OF_RANKS; x++)
                highscore += String.Format("{0},{1}\n", playerscore[x], rankedname[x]);
            saveHighscores(highscore);
            loadHighscores();
        }

        private void ChangeScores()
        {
            string[] lines = { "First line", "Second line", "Third line" };
            System.IO.File.WriteAllLines(Project_Yahtzee.Properties.Resources.scores, lines);

            System.IO.File.WriteAllText(Project_Yahtzee.Properties.Resources.scores, "");
        }

        public void clearHighscores()
        {
            string highscore = "";
            for (int x = 0; x < NUMBER_OF_RANKS; x++) 
                highscore += "0,-\n";
            saveHighscores(highscore);
        }

        private void saveHighscores(String highscore)
        {
            Directory.CreateDirectory(fileDir);
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
            fs.Close();
            if (File.Exists(filePath))
                System.IO.File.WriteAllText(filePath, highscore);
        }
        #endregion highscores

        private void btnLoad_Click(object sender, EventArgs e)
        {
            loadHighscores();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            String playername = txtName.Text;
            int finalscore = Convert.ToInt32(txtScore.Text);
            highscores(finalscore, playername);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            clearHighscores();
            loadHighscores();
        }
    }
}
