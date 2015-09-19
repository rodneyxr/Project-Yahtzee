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
using System.Threading;

namespace Project_Yahtzee
{
    public partial class MainForm : Form
    {
        /***********************
         * Global Declarations *
         ***********************/
        Boolean moveAssistant = false;
        
        // Score
        String username = "";
        Label[] lblScore;
        Label[] lblSpecScore;
        int[] score = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        int[] specScore = new int[7];

        // GUI
        double[,] ratio;
        double[,] sbRatio;
        PictureBox[] picDice;
        int[] roll = new int[5];
        int rolls = 0;
        Button[] button;
        Boolean[] isHolding = new Boolean[5];
        Image[] imgDice = { Project_Yahtzee.Properties.Resources.Dice1, Project_Yahtzee.Properties.Resources.Dice2, Project_Yahtzee.Properties.Resources.Dice3,
                            Project_Yahtzee.Properties.Resources.Dice4, Project_Yahtzee.Properties.Resources.Dice5, Project_Yahtzee.Properties.Resources.Dice6 };

        /******************
         * Initialization *
         ******************/
        #region initialization
        private void MainForm_Load(object sender, EventArgs e)
        {
            picDice = new PictureBox[] { picDice1, picDice2, picDice3, picDice4, picDice5 };
            lblScore = new Label[] { lblOnes, lblTwos, lblThrees, lblFours, lblFives, lblSixes, lblTOAK, lblFOAK, lblFullHouse, lblSmall, lblLarge, lblYahtzee, lblChance };
            lblSpecScore = new Label[] { lblTotalScore, lblBonus, lblTotal, lblYahtzeeBonus, lblUpperTotal, lblLowerTotal, lblGrandTotal };
            button = new Button[] { btnOnes, btnTwos, btnThrees, btnFours, btnFives, btnSixes, btnTOAK, btnFOAK, btnFullHouse, btnSmall, btnLarge, btnYahtzee, btnChance };
            initRatio();
            adjustGUI();
            newGame();
        }

        public MainForm(string username)
        {
            this.username = username;
            InitializeComponent();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        #endregion initialization

        /*******
         * GUI *
         *******/
        #region GUI
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            adjustGUI();
        }

        private void initRatio()
        {
            int numberOfControls = 0;
            foreach (Control x in this.Controls)
                numberOfControls += 1;
            ratio = new double[numberOfControls, 4];

            int i = 0;
            foreach (Control x in this.Controls)
            {
                ratio[i, 0] = ((double)menuStrip1.Width) / ((double)x.Width);
                ratio[i, 1] = ((double)this.Height) / ((double)x.Height);
                ratio[i, 2] = ((double)menuStrip1.Width) / ((double)x.Location.X);
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
            int picBoxIndex = picDice.Length - 1;
            foreach (Control x in this.Controls)
            {
                
                if (!(x is MenuStrip))
                    x.Size = new Size((int) (menuStrip1.Width / ratio[i, 0]), (int) (this.Height / ratio[i, 1]));
                if (!(x is PictureBox))
                    x.Location = new Point((int)(menuStrip1.Width / ratio[i, 2]), (int)(this.Height / ratio[i, 3]));
                else
                {
                    if(!isHolding[picBoxIndex])
                        x.Location = new Point((int)(menuStrip1.Width / ratio[i, 2]), (int)(this.Height / ratio[i, 3]));
                    else
                        x.Location = new Point((int)(menuStrip1.Width / ratio[i, 2]), (int)(this.Height / ratio[i, 3]) + x.Height);
                    picBoxIndex--;
                }
                i++;
            }

            i = 0;
            foreach (Control x in groupBox1.Controls)
            {
                x.Font = new Font("Serif", (float)(groupBox1.Height / sbRatio[i, 0]), x.Font.Style);
                x.Location = new Point((int)(groupBox1.Width / sbRatio[i, 1]), (int)(groupBox1.Height / sbRatio[i, 2]));
                i++;
            }
            lblError.Font = new Font("Serif", (float)(groupBox1.Height / sbRatio[1, 0]), lblError.Font.Style); // Error Message
        }
        #endregion GUI

        /********
         * Roll *
         ********/
        #region roll
        private void btnRoll_Click(object sender, EventArgs e)
        {
            error("");
            if (rolls < 3 && isHolding.Contains(false))
            {
                Random rnd = new Random();
                for (int x = 0; x <= 4; x++)
                {
                    if (!isHolding[x])
                    {
                        roll[x] = rnd.Next(1, 7);
                        picDice[x].Image = imgDice[roll[x] - 1];
                    }
                    picDice[x].Visible = true;
                }
                rolls++;
                btnRoll.Text = "Roll: " + rolls;
                if (rolls == 3)
                    btnRoll.Enabled = false;
            }
            else
                //MessageBox.Show("You can't roll.\nYou're holding all the dice.", "Illegal Move", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                error("INVALID MOVE: You can't roll. You're holding all the dice.");
        }
        #endregion roll

        /***************
         * Dice Clicks *
         ***************/
        #region Dice_Clicks
        private void picDice1_Click(object sender, EventArgs e)
        {
            toggleHold(0);
        }

        private void picDice2_Click(object sender, EventArgs e)
        {
            toggleHold(1);
        }

        private void picDice3_Click(object sender, EventArgs e)
        {
            toggleHold(2);
        }

        private void picDice4_Click(object sender, EventArgs e)
        {
            toggleHold(3);
        }

        private void picDice5_Click(object sender, EventArgs e)
        {
            toggleHold(4);
        }

        private void toggleHold(int holdSlot)
        {
            if (!isHolding[holdSlot])
            {
                picDice[holdSlot].Location = new Point(picDice[holdSlot].Location.X, picDice[holdSlot].Location.Y + picDice[holdSlot].Size.Height);// initDicePosY + picDice[holdSlot].Size.Height);
                isHolding[holdSlot] = true;
            }
            else
            {
                picDice[holdSlot].Location = new Point(picDice[holdSlot].Location.X, picDice[holdSlot].Location.Y - picDice[holdSlot].Size.Height); //initDicePosY);
                isHolding[holdSlot] = false;
            }
            error("");
        }
        #endregion Dice_Clicks

        /*********
         * Moves *
         *********/
        #region moves
        void processClick(int x)
        {
            if (rolls == 0)
                error("INVALID MOVE: You must roll the dice first.");
            else
            {
                int points = 0;

                switch (x)
                {
                    case 0: // ones
                        points = addFaces(1);
                        break;

                    case 1: // twos
                        points = addFaces(2);
                        break;

                    case 2: // threes
                        points = addFaces(3);
                        break;

                    case 3: // fours
                        points = addFaces(4);
                        break;

                    case 4: // fives
                        points = addFaces(5);
                        break;

                    case 5: // sixes
                        points = addFaces(6);
                        break;

                    case 6: // 3 of a kind
                        if (ofAKind(3) > 0)
                            points = totalDice();
                        break;

                    case 7: // 4 of a kind
                        if (ofAKind(4) > 0)
                            points = totalDice();
                        break;

                    case 8: // full house
                        if (isFullHouse())
                            points = 25;

                        break;

                    case 9: // small
                        if (roll.Contains(1) && roll.Contains(2) && roll.Contains(3) && roll.Contains(4) ||
                            roll.Contains(2) && roll.Contains(3) && roll.Contains(4) && roll.Contains(5) ||
                            roll.Contains(3) && roll.Contains(4) && roll.Contains(5) && roll.Contains(6))
                            points = 30;
                        break;

                    case 10: // large
                        if (roll.Contains(1) && roll.Contains(2) && roll.Contains(3) && roll.Contains(4) && roll.Contains(5) ||
                            roll.Contains(2) && roll.Contains(3) && roll.Contains(4) && roll.Contains(5) && roll.Contains(6))
                            points = 40;
                        break;

                    case 11: // yahtzee
                        if (ofAKind(5) > 0)
                            points = 50;
                        else
                        {
                            lblSpecScore[3].ForeColor = Color.Red;
                            lblSpecScore[3].Text = "x";
                        }
                        break;

                    case 12: // chance
                        points = totalDice();
                        break;

                    default: break;
                }

                if (ofAKind(5) > 0 && score[11] > 0)
                    specScore[3] += 100;

                score[x] = points;
                updateScore(x);
            }
        }

        #region buttonClicks
        private void btnOnes_Click(object sender, EventArgs e)
        {
            processClick(0);
        }

        private void btnTwos_Click(object sender, EventArgs e)
        {
            processClick(1);
        }

        private void btnThrees_Click(object sender, EventArgs e)
        {
            processClick(2);
        }

        private void btnFours_Click(object sender, EventArgs e)
        {
            processClick(3);
        }

        private void btnFives_Click(object sender, EventArgs e)
        {
            processClick(4);
        }

        private void btnSixes_Click(object sender, EventArgs e)
        {
            processClick(5);
        }

        private void btnTOAK_Click(object sender, EventArgs e)
        {
            processClick(6);
        }

        private void btnFOAK_Click(object sender, EventArgs e)
        {
            processClick(7);
        }

        private void btnFullHouse_Click(object sender, EventArgs e)
        {
            processClick(8);
        }

        private void btnSmall_Click(object sender, EventArgs e)
        {
            processClick(9);
        }

        private void btnLarge_Click(object sender, EventArgs e)
        {
            processClick(10);
        }

        private void btnYahtzee_Click(object sender, EventArgs e)
        {
            processClick(11);
        }

        private void btnChance_Click(object sender, EventArgs e)
        {
            processClick(12);
        }
        #endregion buttonClicks
        #endregion moves

        /*********
         * Tools *
         *********/
        #region tools
        private int addFaces(int face)
        {
            int total = 0;
            for (int x = 0; x <= 4; x++)
                if (roll[x] == face)
                    total += face;
            return total;
        }

        private int totalDice()
        {
            int total = 0;
            foreach (int x in roll)
                total += x;
            return total;
        }

        private Boolean isFullHouse()
        {
            if (ofAKind(5) > 0)
                return true;
            int firstKind = ofAKind(3);
            if (firstKind == 0)
                return false;
            int counter;
            for (int x = 1; x <= 6; x++)
            {
                counter = 0;
                if (x != firstKind)
                    for (int y = 0; y <= 4; y++)
                        if (roll[y] == x)
                            counter += 1;
                    if (counter == 2)
                        return true;
            }
            return false;
        }

        private int ofAKind(int amountOfKind)
        {
            int counter;
            for (int x = 1; x <= 6; x++)
            {
                counter = 0;
                for (int y = 0; y <= 4; y++)
                    if (roll[y] == x)
                        counter += 1;
                if (counter >= amountOfKind)
                    return x;
            }
            return 0;
        }

        private void error(String em)
        {
            if (!em.Equals(""))
            {
                lblError.Text = em;
                lblError.Visible = true;
            }
            else
                lblError.Visible = false;
        }
        #endregion tools

        /*********
         * Score *
         *********/
        #region score
        private void updateScore(int i) 
        {
            error("");
            lblScore[i].Text = score[i].ToString(); // update move

            if (i <= 5)
            {
                // Calculate the upper section
                specScore[0] = 0;
                for (int x = 0; x <= 5; x++)
                    if (score[x] > -1)
                        specScore[0] += score[x];

                if (specScore[0] >= 63)
                    specScore[1] = 35;
                specScore[2] = specScore[4] = specScore[0] + specScore[1];

                // Display the upper section
                lblSpecScore[0].Text = specScore[0].ToString();

                if (specScore[1] == 35) // upper bonus
                {
                    lblSpecScore[1].ForeColor = Color.Magenta;
                    lblSpecScore[1].Text = "+35";
                }
                else if (score[0] > -1 && score[1] > -1 && score[2] > -1 && score[3] > -1 && score[4] > -1 && score[5] > -1) // used all 1 - 6s?
                {
                    lblSpecScore[1].ForeColor = Color.Red;
                    lblSpecScore[1].Text = "x";
                }
                else
                    lblSpecScore[1].Text = "-";
                
                lblSpecScore[2].Text = lblSpecScore[4].Text = specScore[2].ToString(); // display Total and uppertotal 
            }

            specScore[5] = 0;
            for (int x = 6; x <= 12; x++)
                if (score[x] > -1)
                    specScore[5] += score[x];
            specScore[5] += specScore[3];
            if (score[11] > 0) // update yahtzee bonus
            {
                lblSpecScore[3].ForeColor = Color.Magenta;
                lblSpecScore[3].Text = "+" + specScore[3];
            }

            if (score[6] != -1 || score[7] != -1 || score[8] != -1 || score[9] != -1 || score[10] != -1 || score[11] != -1 || score[12] != -1)
                lblSpecScore[5].Text = specScore[5].ToString(); // update lower

            // GrandTotal
            specScore[6] = specScore[4] + specScore[5];  // grand = upper + lower
            lblSpecScore[6].Text = specScore[6].ToString(); // display grand

            button[i].Enabled = false;
            lblScore[i].ForeColor = score[i] > 0 ? Color.Green : Color.Red;

            Boolean finished = true; //TODO
            foreach (Button x in button)
                if (x.Enabled)
                {
                    finished = false;
                    break;
                }

            if (finished)
            {
                newMove();
                btnRoll.Enabled = false;
                gameOver();
                
            }
            else
            {
                newMove();
            }
        }

        private void gameOver()
        {
            HighScores hs = new HighScores();
            hs.highscores(specScore[6], username);
            menuHighScores.PerformClick();
        }
        #endregion score

        /*********
         * Reset *
         *********/
        #region reset
        private void newMove()
        {
            for (int x = 0; x < roll.Length; x++) // reset roll values
                roll[x] = 0;

                for (int x = 0; x < roll.Length; x++) // reset dice
                {
                    picDice[x].Visible = false;
                    if (isHolding[x])
                        toggleHold(x);
                }

            rolls = 0;
            btnRoll.Text = "Roll: " + rolls;
            btnRoll.Enabled = true;
        }

        private void newGame()
        {
            for (int x = 0; x < score.Length; x++)
                score[x] = -1;
            for (int x = 0; x < specScore.Length; x++)
                specScore[x] = 0;
            foreach (Label x in lblScore)
            {
                x.ForeColor = Form.DefaultForeColor;
                x.Text = "-";
            }
            foreach (Label x in lblSpecScore)
                x.Text = "-";
            for (int x = 0; x < lblSpecScore.Length; x++)
                if (x <= 3)
                    lblSpecScore[x].ForeColor = SystemColors.HotTrack;
                else if (x < 6)
                    lblSpecScore[x].ForeColor = SystemColors.GrayText;
                else
                    lblSpecScore[x].ForeColor = Form.DefaultForeColor;

            for (int x = 0; x < roll.Length; x++)
                    roll[x] = -1;
            foreach (Button x in button)
                x.Enabled = true;
            newMove();
        }
        #endregion reset

        /**************
         * Menu Strip *
         **************/
        #region MenuStrip
        private void menuNewGame_Click(object sender, EventArgs e)
        {
            newGame();
        }

        private void menuSwitchUsers_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will take you back to the login screen.\nAre you sure you want to quit?",
                "Yahtzee", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Hide();
                Login l = new Login();
                l.Show();
            }
        }

        private void menuHighScores_Click(object sender, EventArgs e)
        {
            HighScores hs = new HighScores();
            hs.Show();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit?", "Yahtzee", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Close();
        }

        private void menuCheatMode_Click(object sender, EventArgs e)
        {

        }

        private void menuRules_Click(object sender, EventArgs e)
        {

        }

        private void menuAbout_Click(object sender, EventArgs e)
        {

        }
        #endregion MenuStrip

    }//end_class
}//end_namespace