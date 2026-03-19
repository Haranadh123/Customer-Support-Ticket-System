using System.Windows;
using CustomerSupport.Client.Services;
using CustomerSupport.Shared.Enums;

namespace CustomerSupport.Client;

public partial class LoginWindow : Window
{
    private readonly ApiService _apiService = new();

    public LoginWindow()
    {
        InitializeComponent();
    }

    private async void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        var username = TxtUsername.Text;
        var password = TxtPassword.Password;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Username and password are required.");
            return;
        }

        try
        {
            var response = await _apiService.LoginAsync(username, password);
            if (response != null)
            {
                var mainWindow = new MainWindow(response, _apiService);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ShowError("Invalid username or password.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        LblError.Text = message;
        LblError.Visibility = Visibility.Visible;
    }
}
