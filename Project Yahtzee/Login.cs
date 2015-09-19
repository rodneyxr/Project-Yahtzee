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
    public partial class Login : Form
    {

        private readonly int MAX_NUMBER_OF_ACCOUNTS = 8;
        private readonly int MAX_CHARS = 15;
        private readonly String fileDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Yahtzee";
        private readonly String filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Yahtzee\accounts.txt";
        Button btn = new Button { Dock = DockStyle.Bottom };
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            AcceptButton = btn;
            btn.Click += delegate { enterPressed(); };
            loadAccounts();
        }

        private void enterPressed()
        { 
            if (grpboxLogin.Enabled)
            {
                if (comboUsername.Focused)
                    txtPassword.Focus();
                else if (txtPassword.Focused)
                    btnLogin.PerformClick();
            }
            else
            {
                if (txtNewUsername.Focused)
                    txtNewPassword.Focus();
                else if (txtNewPassword.Focused)
                    txtConfirmPassword.Focus();
                else if (txtConfirmPassword.Focused)
                    btnCreateAccount.PerformClick();
            }
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        #region Login
        private void btnLogin_Click(object sender, EventArgs e)
        {
            String[,] userInfo = getAccounts();
            String username = comboUsername.Text.Trim();
            String password = txtPassword.Text;
            String em = "";
            int index = -1;

            for (int x = 0; x < MAX_NUMBER_OF_ACCOUNTS; x++)
                if (userInfo[x, 0].Equals(username, StringComparison.InvariantCultureIgnoreCase))
                    index = x;
            if (index != -1)
            {
                if (userInfo[index, 1].Equals(password))
                {
                    this.Hide();
                    MainForm m = new MainForm(username);
                    m.Show();
                }
                else
                    em = "Incorrect Password.";
            }
            else
                em = "That username does not exist.";

            if (!em.Equals(""))
            {
                lblError.Text = em;
                lblError.Visible = true;
                return;
            }
            else
                lblError.Visible = false;
        }

        private void btnNewUser_Click(object sender, EventArgs e)
        {
            grpboxNewUser.Visible = true;
            grpboxLogin.Enabled = false;
        }
        #endregion Login

        #region NewUser
        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtNewUsername.Text = "";
            txtNewPassword.Text = "";
            txtConfirmPassword.Text = "";
            grpboxNewUser.Visible = false;
            lblError.Visible = false;
            grpboxLogin.Enabled = true;
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            String[,] userInfo = getAccounts();
            String username = txtNewUsername.Text.Trim();
            String password = txtNewPassword.Text;
            String confirm = txtConfirmPassword.Text;
            String em = ""; // error message

            // Validate password
            if (password.Length == 0)
                em = "You must have a password.";
            else if (password.Length < 5 || password.Length > MAX_CHARS)
                em = "Your password must be from 5 - 15 characters long.";
            else if (!password.Equals(confirm)) // Do passwords match?
                em = "Your password does not match the confirmation.";

            // Validate username
            if (username.Length < 2 || username.Length > MAX_CHARS)
                em = "Your username must be from 2 - 15 characters long.";
            else if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_ ]+$"))
                em = "Your username can only contain alphanumeric characters, '_' and ' '.";
            else if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"[a-zA-Z]"))
                em = "Your username must contain at least 1 letter.";

            // Does the username exist?
            for (int x = 0; x < MAX_NUMBER_OF_ACCOUNTS; x++)
                if (username.Equals(userInfo[x, 0], StringComparison.InvariantCultureIgnoreCase))
                    em = "That username already exists!";

            if (!em.Equals(""))
            {
                lblError.Text = em;
                lblError.Visible = true;
                return;
            }
            else
                lblError.Visible = false;

            int activeAccounts = 0;
            for (int x = 0; x < MAX_NUMBER_OF_ACCOUNTS; x++)
                if (!(userInfo[x, 0].Equals("~void~")))
                    activeAccounts++;
            if (activeAccounts < MAX_NUMBER_OF_ACCOUNTS)
            {
                userInfo[activeAccounts, 0] = username;
                userInfo[activeAccounts, 1] = password;
            }
            else
            {
                lblError.Text = "You have reached the maximum number of accounts!";
                lblError.Visible = true;
                return;
            }

            string accounts = "";
            for (int x = 0; x < MAX_NUMBER_OF_ACCOUNTS; x++)
                accounts += String.Format("username={0}\npassword={1}\n", userInfo[x, 0], userInfo[x, 1]);
            saveAccounts(accounts);
            MessageBox.Show("Account Successfully Created");
            btnCancel.PerformClick();
            loadAccounts();
        }

        private String[,] getAccounts()
        {
            // Load from text file
            if (!File.Exists(filePath))
                clearAllAccounts();
            String text = "";
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                    text = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                clearAllAccounts();
                Console.WriteLine(e.Message);
            }

            String[,] userInfo = new String[MAX_NUMBER_OF_ACCOUNTS, 2]; // 0 = username, 1 = password
            Char[] delimiter = { '\n' };
            String[] items = text.Split(delimiter);
            int counter = 0;

            try
            {
                for (int x = 0; x < MAX_NUMBER_OF_ACCOUNTS; x++)
                {
                    userInfo[x, 0] = items[counter].Substring(items[counter++].IndexOf("=") + 1);
                    userInfo[x, 1] = items[counter].Substring(items[counter++].IndexOf("=") + 1);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("The account file is corrupted.\nA new file has been created.\nPlease reopen the application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                clearAllAccounts();
                Console.WriteLine(e.Message);
                Application.Exit();
            }

            return userInfo;
        }

        private void loadAccounts()
        {
            String[,] userInfo = getAccounts();
            String[] usernames = new String[MAX_NUMBER_OF_ACCOUNTS];

            for (int x = 0; x < MAX_NUMBER_OF_ACCOUNTS; x++)
                usernames[x] = userInfo[x, 0];

            comboUsername.Items.Clear();
            Array.Sort<String>(usernames); // Sort usernames by alphabetical order
            int activeAccounts = 0;
            for (int x = 0; x < MAX_NUMBER_OF_ACCOUNTS; x ++)
                if (!usernames[x].Equals("~void~"))
                {
                    comboUsername.Items.Add(usernames[x]);
                    activeAccounts++;
                }
            if (activeAccounts > 0) // is there any accounts yet?
                comboUsername.Text = comboUsername.Items[0].ToString();
        }

        public void clearAllAccounts()
        {
            string accounts = "";
            for (int x = 0; x < MAX_NUMBER_OF_ACCOUNTS; x++)
                accounts += "username=~void~\npassword=~void~\n";
            saveAccounts(accounts);
        }

        private void saveAccounts(String accounts)
        {
            Directory.CreateDirectory(fileDir);
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
            fs.Close();
            if (File.Exists(filePath))
                System.IO.File.WriteAllText(filePath, accounts);
        }
        #endregion NewUser

        private void btnPlayAsGuest_Click(object sender, EventArgs e)
        {
            //TODO : Implement Guest Login
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            //TODO : Implement Delete Account
        }


    }
}
