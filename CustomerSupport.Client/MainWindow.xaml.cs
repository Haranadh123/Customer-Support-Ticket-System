using System.Windows;
using System.Windows.Controls;
using CustomerSupport.Client.Services;
using CustomerSupport.Shared.DTOs;
using CustomerSupport.Shared.Enums;

namespace CustomerSupport.Client;

public partial class MainWindow : Window
{
    private readonly LoginResponse _loginResponse;
    private readonly ApiService _apiService;

    public MainWindow(LoginResponse loginResponse, ApiService apiService)
    {
        InitializeComponent();
        _loginResponse = loginResponse;
        _apiService = apiService;

        LblUserRole.Text = $"Logged in as: {_loginResponse.Username} ({_loginResponse.Role})";
        
        if (_loginResponse.Role == UserRole.User)
        {
            BtnNewTicket.Visibility = Visibility.Visible;
        }
        else
        {
            BtnNewTicket.Visibility = Visibility.Collapsed;
        }

        LoadTickets();
    }

    private async void LoadTickets()
    {
        try
        {
            StatusText.Text = "Loading tickets...";
            var tickets = await _apiService.GetTicketsAsync();
            DgTickets.ItemsSource = tickets;
            StatusText.Text = "Tickets loaded.";
        }
        catch (Exception ex)
        {
            StatusText.Text = "Error loading tickets.";
            MessageBox.Show($"Failed to load tickets: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        LoadTickets();
    }

    private void BtnNewTicket_Click(object sender, RoutedEventArgs e)
    {
        var createWin = new CreateTicketWindow(_apiService);
        if (createWin.ShowDialog() == true)
        {
            LoadTickets();
        }
    }

    private void BtnViewDetails_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is TicketDto ticket)
        {
            var detailsWin = new TicketDetailsWindow(ticket.Id, _loginResponse, _apiService);
            detailsWin.ShowDialog();
            LoadTickets();
        }
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        _apiService.Logout();
        var loginWin = new LoginWindow();
        loginWin.Show();
        this.Close();
    }
}