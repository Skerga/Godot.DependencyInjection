using Microsoft.Extensions.Hosting;

namespace Godot.DependencyInjection.Sample;

public partial class MessageBusNode : Node
{
    [Signal] 
    public delegate void MessageReceivedEventHandler(Node sender,string message);

    [Inject]
    private MessageBusNode(IHostApplicationLifetime applicationLifetime)
    {
        // Send Quit request to Godot when the Host is shutting down.
        applicationLifetime.ApplicationStopping.Register(() => 
            DependencyInjection.GetRootNode().PropagateNotification((int)NotificationWMCloseRequest));
        
        // Sends the actual quit when the Host stopped.
        applicationLifetime.ApplicationStopped.Register(() => 
            (Engine.GetMainLoop() as SceneTree)?.Quit());
    }
    
    public void Publish(Node node, string message)
    {
        EmitSignalMessageReceived(node, message);
    }

    public override void _Ready()
    {
        Name = "MessageBusNode";
    }
}