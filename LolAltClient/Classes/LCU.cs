using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PoniLCU;

namespace KiwiClient.Classes
{
    public class LCU : BaseVM
    {
        public event EventHandler Activate;

        public PoniLCU.LeagueClient LeagueClient { get; set; } = new LeagueClient();
        private bool _isQueuePopped;
        public bool IsQueuePopped
        {
            get { return _isQueuePopped; }
            set { _isQueuePopped = value; OnPropertyChanged(); }
        }

        private bool _isAutoJoin;

        public bool IsAutoJoin
        {
            get { return _isAutoJoin; }
            set { _isAutoJoin = value; OnPropertyChanged(); }
        }


        private Brush _connectionColor;

        public Brush ConnectionColor
        {
            get { return _connectionColor; }
            set { _connectionColor = value; }
        }


        private ICommand _AcceptCommand;
        public ICommand AcceptCommand
        {
            get
            {
                if (_AcceptCommand == null)
                    _AcceptCommand = new AcceptCommand();
                return _AcceptCommand;
            }
            set
            {
                _AcceptCommand = value;
            }
        }        

        private ICommand _DeclineCommand;
        public ICommand DeclineCommand
        {
            get
            {
                if (_DeclineCommand == null)
                    _DeclineCommand = new DeclineCommand();
                return _DeclineCommand;
            }
            set
            {
                _DeclineCommand = value;
            }
        }

        public LCU()
        {
            IsQueuePopped = false;
            IsAutoJoin = false;
            ConnectionColor = new SolidColorBrush(Colors.Red);
            if(IsLeagueClientConnected())
            {
                SubscribeToClientEvents();
            }
        }

        ~LCU()
        {
            UnsubscribeFromClientEvents();
        }


        private bool IsLeagueClientConnected()
        {
            for (int i = 0; i < 10; i++)
            {
                if (LeagueClient.IsConnected)
                {
                    ConnectionColor = new SolidColorBrush(Colors.LightGreen);
                    return true;
                }
                System.Threading.Thread.Sleep(1000);
            }
            MessageBox.Show($"Es konnte keine Verbindung zum League Client hergestellt werden. {Environment.NewLine}Stellen Sie bitte sicher das er läuft bevor Kiwi Client gestartet wird.", "Fehler", MessageBoxButton.OK,MessageBoxImage.Error);
            return false;
        }

        private void DecideQueue(OnWebsocketEventArgs obj)
        {
            if (obj.Type.ToUpper() == "UPDATE" && obj.Path == "/lol-matchmaking/v1/ready-check")
            {
                IsQueuePopped = true;
                if(IsAutoJoin)
                {
                    AcceptCommand.Execute(LeagueClient);
                }
                else
                    Activate(null, new EventArgs());
            }
        }

        private void GameFlowChanges(OnWebsocketEventArgs obj)
        {
            if(obj != null && obj.Data is string && ( obj.Data == "ChampSelect" || obj.Data == ""))
            {
                IsQueuePopped = false;
            }

        }

        private void SubscribeToClientEvents()
        {
            LeagueClient.Subscribe("/lol-gameflow/v1/gameflow-phase", DecideQueue);
            LeagueClient.Subscribe("/lol-gameflow/v1/gameflow-phase", GameFlowChanges);
        }

        private void UnsubscribeFromClientEvents()
        {
            LeagueClient.Unsubscribe("/lol-matchmaking/v1/ready-check", DecideQueue);
            LeagueClient.Unsubscribe("/lol-gameflow/v1/gameflow-phase", GameFlowChanges);
        }

    }



    class AcceptCommand : ICommand
    {
        const string POST = "POST";
        const string GET = "GET";
        #region ICommand Members  

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if(parameter is LeagueClient && parameter!= null)
                ((LeagueClient)parameter).Request(POST, "/lol-matchmaking/v1/ready-check/accept", "");
        }
        #endregion
    }

    class DeclineCommand : ICommand
    {
        const string POST = "POST";
        const string GET = "GET";
        #region ICommand Members  

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (parameter is LeagueClient && parameter != null)
                ((LeagueClient)parameter).Request(POST, "/lol-matchmaking/v1/ready-check/decline", "");
        }
        #endregion
    }
}
