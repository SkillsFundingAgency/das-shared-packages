using System;
using System.Windows.Forms;
using PubSubSampleGui.SubSystems;

namespace PubSubSampleGui
{
    public partial class Form1 : Form
    {
        private readonly Random _random = new Random();
        private SubSystemBase _system;

        public Form1()
        {
            InitializeComponent();
        }

        private void fileSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSubSystem(new FileSystemSubSystem("C:\\temp\\"));
        }



        private void btnGenId_Click(object sender, EventArgs e)
        {
            txtPubId.Text = Guid.NewGuid().ToString();
        }
        private async void btnPublish_Click(object sender, EventArgs e)
        {
            var balance = (decimal)_random.NextDouble() * 100;
            await _system.PublishAsync(new SampleMessage { Id = txtPubId.Text, Balance = balance });
            lstPubEvents.Items.Insert(0, $"{txtPubId.Text} (Balance {balance})");
        }



        private void btnListen_Click(object sender, EventArgs e)
        {
            if (_system.IsListening)
            {
                _system.StopListening();
                btnListen.Text = "Start";
            }
            else
            {
                _system.StartListening();
                btnListen.Text = "Stop";
            }
        }
        private async void btnPoll_Click(object sender, EventArgs e)
        {
            var message = await _system.PollAsync();
            if (message == null)
            {
                MessageBox.Show("No messages to receive");
                return;
            }
            OnMessageReceived(message);
        }


        private void SetSubSystem(SubSystemBase system)
        {
            _system = system;
            system.SampleMessageReceived += System_SampleMessageReceived;

            txtPubId.Enabled = true;
            btnGenId.Enabled = true;
            btnPublish.Enabled = true;
            lstPubEvents.Enabled = true;

            btnListen.Enabled = true;
            btnPoll.Enabled = true;
            lstSubEvents.Enabled = true;
        }
        private void System_SampleMessageReceived(object sender, SampleMessageReceivedEventArgs e)
        {
            OnMessageReceived(e.Message);
        }
        private void OnMessageReceived(SampleMessage message)
        {
            Invoke(new Action(() =>
            {
                lstSubEvents.Items.Insert(0, $"{message.Id} (Balance {message.Balance})");
            }));
        }
    }
}
