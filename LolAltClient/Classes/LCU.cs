using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PoniLCU;

namespace LolAltClient.Classes
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
            //LeagueClient.OnWebsocketEvent += LeagueClient_OnWebsocketEvent;
            //LeagueClient.Subscribe("/lol-matchmaking/v1/ready-check", DecideQueue);
            LeagueClient.Subscribe("/lol-gameflow/v1/gameflow-phase", DecideQueue);
            LeagueClient.Subscribe("/lol-gameflow/v1/gameflow-phase", GameFlowChanges);
        }

        ~LCU()
        {
            LeagueClient.Unsubscribe("/lol-matchmaking/v1/ready-check", DecideQueue);
            LeagueClient.Unsubscribe("/lol-gameflow/v1/gameflow-phase", GameFlowChanges);
        }

        private void DecideQueue(OnWebsocketEventArgs obj)
        {
            if (obj.Type.ToUpper() == "UPDATE" && obj.Path == "/lol-matchmaking/v1/ready-check")
            {
                IsQueuePopped = true;
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
