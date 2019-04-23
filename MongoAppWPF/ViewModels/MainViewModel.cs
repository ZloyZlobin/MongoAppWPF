using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoAppWPF.Interfaces.DTO.Models;
using MongoAppWPF.Interfaces.Users.Service;

namespace MongoAppWPF.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        #region Parameters
        public int Age
        {
            get { return _user.Age; }
            set
            {
                if (_user.Age != value)
                {
                    _user.Age = value;
                    NotifyPropertyChanged(nameof(Age));
                    UpdateCommandsExecutes();
                }
            }
        }

        public string NickName
        {
            get { return _user.NickName; }
            set
            {
                if (_user.NickName != value)
                {
                    _user.NickName = value;
                    NotifyPropertyChanged(nameof(NickName));
                    UpdateCommandsExecutes();
                }
            }
        }

        public string Country
        {
            get { return _user.Country; }
            set
            {
                if (_user.Country != value)
                {
                    _user.Country = value;
                    NotifyPropertyChanged(nameof(Country));
                    UpdateCommandsExecutes();
                }
            }
        }

        private User _user;
        public User User
        {
            get { return _user; }
            set
            {
                if (_user != value)
                {
                    _user = value;
                    NotifyPropertyChanged(nameof(User));
                    //notify all user parameters
                    NotifyPropertyChanged(nameof(Country));
                    NotifyPropertyChanged(nameof(NickName));
                    NotifyPropertyChanged(nameof(Age));
                    //Update user dependent commands executes
                    UpdateCommandsExecutes();
                }
            }
        }

        private IEnumerable<User> _users;
        public IEnumerable<User> Users
        {
            get { return _users; }
            private set
            {
                _users = value;
                NotifyPropertyChanged(nameof(Users));

                //update selected user
                if (SelectedUser != null)
                {
                    SelectedUser = _users.FirstOrDefault(u => u._id.Equals(SelectedUser._id));
                }
            }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                //Setup user from selected user
                User = _selectedUser ?? new User();
                NotifyPropertyChanged(nameof(SelectedUser));
            }
        }

        private string _searchCriteria;
        public string SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                NotifyPropertyChanged(nameof(SearchCriteria));
                //Notify search command
                SearchUserCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion Parameters

        #region Commands

        public AsyncCommand AddUserCommand { get; }
        public AsyncCommand DeleteUserCommand { get; }
        public AsyncCommand UpdateUserCommand { get; }
        public AsyncCommand SearchUserCommand { get; }

        public async Task AddUserAsync()
        {
            await _service.AddUserAsync(User);
            PopulateTable();
        }

        public async Task<bool> RemoveUserAsync()
        {
            var result = await _service.RemoveUserAsync(_selectedUser);
            //refresh table
            if (result)
            {
                PopulateTable();
            }
            return result;
        }

        public async Task<bool> UpdateUserAsync()
        {
            //create user from selected user id and user parameters
            var user = new User
            {
                _id = _selectedUser._id,
                Age = _user.Age,
                Country = _user.Country,
                NickName = _user.NickName
            };
            var result = await _service.UpdateUserAsync(user);
            // refresh table
            if (result)
            {
                PopulateTable();
            }
            return result;
        }

        public async Task<User> GetUserAsync()
        {
            var user = await _service.GetUserAsync(SearchCriteria);
            //Select finded user
            if (user != null)
            {
                SelectedUser = Users.FirstOrDefault(u => u._id.Equals(user._id));
            }

            return user;
        }

        private void UpdateCommandsExecutes()
        {
            AddUserCommand.RaiseCanExecuteChanged();
            DeleteUserCommand.RaiseCanExecuteChanged();
            UpdateUserCommand.RaiseCanExecuteChanged();
        }
        #endregion

        public bool IsUserDataValid =>
            User.Age >= 0 && User.Age < 100 && 
            !string.IsNullOrEmpty(User.Country) &&
            !string.IsNullOrEmpty(User.NickName);

        private readonly IUserService _service;

        public MainViewModel(IUserService service)
        {
            _service = service;

            //init commands
            AddUserCommand = new AsyncCommand(AddUserAsync, () => IsUserDataValid);
            DeleteUserCommand = new AsyncCommand(RemoveUserAsync, () => SelectedUser != null);
            UpdateUserCommand = new AsyncCommand(UpdateUserAsync, () => IsUserDataValid && SelectedUser != null);
            SearchUserCommand = new AsyncCommand(GetUserAsync, () => !string.IsNullOrEmpty(SearchCriteria));

            User = new User();
            PopulateTable();
        }

        private async void PopulateTable()
        {
            Users = await _service.GetAllUsersAsync();
        }

        
    }
}
