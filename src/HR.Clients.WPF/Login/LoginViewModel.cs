// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Coddee.Security;
using Coddee.WPF;
using Coddee.WPF.Commands;
using Coddee.WPF.Security;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Login
{
    public class LoginViewModel : ViewModelBase<LoginView>, ILoginViewModel
    {
      
        public LoginViewModel()
        {
#if DEBUG
            //_username = "user";
            //_password = "123";
            //View.Loaded += delegate
            //{
            //    Login();
            //};
#endif
        }

        private IAuthenticationProvider<HRAuthenticationResponse> _authenticationProvider;
        public event EventHandler<AuthenticationResponse> LoggedIn;

        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref this._username, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref this._password, value); }
        }

        private bool _invalidLogin;
        public bool InvalidLogin
        {
            get { return _invalidLogin; }
            set { SetProperty(ref this._invalidLogin, value); }
        }

        public ICommand LoginCommand => new RelayCommand(Login);

        private async void Login()
        {
            //await ((IUserRepository) _authenticationProvider).CreateUser(new User {Username = "user"}, "123");

            var res = await _authenticationProvider.AuthenticationUser(Username, Password);
            if (res.Status == AuthenticationStatus.Successfull)
            {
                _globalVariables.SetValue(Globals.Username, res.Username);
                LoggedIn?.Invoke(this, res);
            }
            else
                InvalidLogin = true;

        }

        protected override Task OnInitialization()
        {
            _authenticationProvider = Resolve<IUserRepository>();
            RegisterInstance<IAuthenticationProvider<HRAuthenticationResponse>>(_authenticationProvider);
            return base.OnInitialization();
        }
    }
}