using System;
using System.Windows.Forms;
using FastTimer;

namespace MicroTimerTest
{
    public partial class FrmMicroTimerTest : Form
    {
        //Create a new MicroTimer
        MicroTimer msTimer = new MicroTimer();
        public FrmMicroTimerTest()
        {
            InitializeComponent();
            //Set the Microseconds and FeedbackMiliseconds
            //properties via the UI (NumericUpDowns)
            numericUpDown1.Value = msTimer.MicroSeconds;
            numericUpDown2.Value = msTimer.FeedbackMilliseconds;
            //Assign the funtion to execute at a high rate
            msTimer.OnTimeout += Dice.Roll;
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            msTimer.MicroSeconds = (int)numericUpDown1.Value;
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            msTimer.FeedbackMilliseconds = (int)numericUpDown2.Value;
        }
        //Test the dice roll code
        private void button1_Click(object sender, EventArgs e)
        {
            Dice.Roll();
            //Same UI Update used by Progress object
            UpdateUI(null, 0);
        }
        //Update UI with dice roll statistics
        void UpdateUI(object sender, int counter)
        {   
            label1.Text = Dice.LastRoll.ToString();
            label3.Text = Dice.Rolls[0].ToString();
            label5.Text = Dice.Rolls[1].ToString();
            label7.Text = Dice.Rolls[2].ToString();
            label9.Text = Dice.Rolls[3].ToString();
            label11.Text = Dice.Rolls[4].ToString();
            label13.Text = Dice.Rolls[5].ToString();
            label15.Text = Dice.TotalRolls.ToString();
            label17.Text = Dice.TotalValues.ToString();
            label19.Text = Dice.MeanValueRolled.ToString();
        }
        //The Progress object
        Progress<int> update;
        private void button2_Click(object sender, EventArgs e)
        {
            //Start/stop the timer
            if (button2.Text == "Start")
            {
                //Change button to stop
                button2.Text = "Stop";
                //Progress used to update UI
                if (update == null)
                {
                    update = new Progress<int>();
                    //also uses the UpdateUI function
                    update.ProgressChanged += UpdateUI;
                    //Assign the Progress object to the MicroTimer
                    msTimer.Updater = update;
                }
                //Start the timer
                msTimer.Start();
            }
            else
            {
                //Stop the timer
                msTimer.Stop();
                //Change button to start
                button2.Text = "Start";
            }
        }
    }
}
