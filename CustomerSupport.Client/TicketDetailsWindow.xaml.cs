using System.Windows;
using System.Windows.Media;
using CustomerSupport.Client.Services;
using CustomerSupport.Shared.DTOs;
using CustomerSupport.Shared.Enums;
using System.Collections.Generic;
using System.Linq;

namespace CustomerSupport.Client;

public partial class TicketDetailsWindow : Window
{
    private readonly int _ticketId;
    private readonly LoginResponse _loginInfo;
    private readonly ApiService _apiService;
    private TicketDetailsDto? _details;

    public TicketDetailsWindow(int ticketId, LoginResponse loginInfo, ApiService apiService)
    {
        InitializeComponent();
        _ticketId = ticketId;
        _loginInfo = loginInfo;
        _apiService = apiService;

        if (_loginInfo.Role == UserRole.Admin)
        {
            BrdrAdminActions.Visibility = Visibility.Visible;
            ChkInternal.Visibility = Visibility.Visible;
            LoadAdmins();
        }

        LoadDetails();
    }

    private async void LoadDetails()
    {
        try
        {
            _details = await _apiService.GetTicketDetailsAsync(_ticketId);
            if (_details == null) return;

            var t = _details.Ticket;
            TxtTicketNumber.Text = t.TicketNumber;
            TxtSubject.Text = t.Subject;
            TxtCreatedBy.Text = t.CreatedByUsername;
            TxtCreatedDate.Text = t.CreatedDate.ToString("yyyy-MM-dd HH:mm");
            TxtDescription.Text = t.Description;
            TxtStatus.Text = t.Status.ToString();
            TxtPriority.Text = t.Priority.ToString();
            TxtAssignedTo.Text = string.IsNullOrEmpty(t.AssignedToUsername) ? "Unassigned" : $"Assigned to: {t.AssignedToUsername}";

            // Visuals
            BrdrStatus.Background = t.Status switch
            {
                TicketStatus.Open => Brushes.DarkGreen,
                TicketStatus.InProgress => Brushes.Orange,
                TicketStatus.Closed => Brushes.Gray,
                _ => Brushes.Black
            };

            BrdrPriority.Background = t.Priority switch
            {
                TicketPriority.High => Brushes.Red,
                TicketPriority.Medium => Brushes.Blue,
                TicketPriority.Low => Brushes.DarkGray,
                _ => Brushes.Black
            };

            LbComments.ItemsSource = _details.Comments;
            LvHistory.ItemsSource = _details.History;

            if (_loginInfo.Role == UserRole.Admin)
            {
                // Set current status in combo
                foreach (System.Windows.Controls.ComboBoxItem item in CbChangeStatus.Items)
                {
                    if (item.Tag.ToString() == t.Status.ToString())
                    {
                        CbChangeStatus.SelectedItem = item;
                        break;
                    }
                }
            }

            // Disable comment if closed and not admin
            if (t.Status == TicketStatus.Closed && _loginInfo.Role != UserRole.Admin)
            {
                BtnAddComment.IsEnabled = false;
                TxtNewComment.IsEnabled = false;
                TxtNewComment.Text = "Discussion is closed for this ticket.";
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading: {ex.Message}");
        }
    }

    private async void LoadAdmins()
    {
        try
        {
            var admins = await _apiService.GetAdminsAsync();
            CbAssignTo.ItemsSource = admins;
        }
        catch { }
    }

    private async void BtnAddComment_Click(object sender, RoutedEventArgs e)
    {
        var text = TxtNewComment.Text;
        if (string.IsNullOrWhiteSpace(text)) return;

        try
        {
            var success = await _apiService.AddCommentAsync(_ticketId, text, ChkInternal.IsChecked ?? false);
            if (success)
            {
                TxtNewComment.Clear();
                LoadDetails();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void BtnSaveAdmin_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bool success = true;

            // Update Status
            if (CbChangeStatus.SelectedItem is System.Windows.Controls.ComboBoxItem statusItem)
            {
                var newStatus = Enum.Parse<TicketStatus>(statusItem.Tag.ToString()!);
                if (newStatus != _details?.Ticket.Status)
                {
                    success &= await _apiService.UpdateStatusAsync(_ticketId, newStatus);
                }
            }

            // Update Assignment
            if (CbAssignTo.SelectedValue is int adminId)
            {
                // Assuming we check if it changed, but for simplicity:
                success &= await _apiService.AssignTicketAsync(_ticketId, adminId);
            }

            if (success)
            {
                MessageBox.Show("Changes saved successfully.");
                LoadDetails();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
