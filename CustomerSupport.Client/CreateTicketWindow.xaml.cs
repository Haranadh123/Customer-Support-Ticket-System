using System.Windows;
using CustomerSupport.Client.Services;
using CustomerSupport.Shared.DTOs;
using CustomerSupport.Shared.Enums;
using System.Windows.Controls;

namespace CustomerSupport.Client;

public partial class CreateTicketWindow : Window
{
    private readonly ApiService _apiService;

    public CreateTicketWindow(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void BtnSubmit_Click(object sender, RoutedEventArgs e)
    {
        var subject = TxtSubject.Text;
        var description = TxtDescription.Text;
        var priorityTag = (CbPriority.SelectedItem as ComboBoxItem)?.Tag?.ToString();

        if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(description))
        {
            MessageBox.Show("Subject and Description are required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var priority = priorityTag switch
        {
            "Low" => TicketPriority.Low,
            "High" => TicketPriority.High,
            _ => TicketPriority.Medium
        };

        try
        {
            BtnSubmit.IsEnabled = false;
            var success = await _apiService.CreateTicketAsync(new CreateTicketRequest(subject, description, priority));
            if (success)
            {
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to create ticket.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            BtnSubmit.IsEnabled = true;
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        this.Close();
    }
}
