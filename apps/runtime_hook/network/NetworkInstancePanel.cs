using Godot;
using Solace.addons.solace_core_plugin.core;
using Solace.addons.solace_core_plugin.core.network;

namespace Solace.apps.runtime_hook.network;

public partial class NetworkInstancePanel : PanelContainer
{
    [Export] private Button _hostMultiplayerButton;
    [Export] private Button _connectMultiplayerButton;
    [Export] private LineEdit _connectAddressLineEdit;
    [Export] private LineEdit _connectPortLineEdit;
    [Export] private LineEdit _hostPortLineEdit;
    [Export] private Label _multiplayerStatusLabel;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _hostMultiplayerButton.Pressed += HostMultiplayerButtonOnPressed;
        _connectMultiplayerButton.Pressed += ConnectMultiplayerButtonOnPressed;
        // TODO: Network Status 
        // Networking.NetworkStatus.OnNetworkStatusChanged += UpdateNetworkStatusDisplay;
    }

    private void UpdateNetworkStatusDisplay(NetworkStatus networkStatus)
    {
        _multiplayerStatusLabel.Text = networkStatus.ConnectionText;
    }

    private void ConnectMultiplayerButtonOnPressed()
    {
        SC.Print(nameof(NetworkInstancePanel), "Joining!");

        var targetAddress = _connectAddressLineEdit.Text;
        if (targetAddress.Length <= 1)
        {
            targetAddress = _connectAddressLineEdit.PlaceholderText;
        }

        var targetPortText = _connectPortLineEdit.Text;
        var targetPort = _connectPortLineEdit.PlaceholderText.ToInt();
        if (targetPortText.Length < 4)
        {
            SC.Print(nameof(NetworkInstancePanel), $"Using placeholder port: {targetPort}!");
        }

        Networking.ConnectAsClient(targetAddress, targetPort);
    }

    private void HostMultiplayerButtonOnPressed()
    {
        SC.Print(nameof(NetworkInstancePanel), "Hosting!");

        var targetPortText = _hostPortLineEdit.Text;
        var targetPort = _hostPortLineEdit.PlaceholderText.ToInt();
        if (targetPortText.Length < 4)
        {
            SC.Print(nameof(NetworkInstancePanel), $"Using placeholder port: {targetPort}!");
        }

        Networking.ConnectAsHost(targetPort);
    }
}