
namespace Godot.DependencyInjection.Sample;

public partial class Main : Node
{
    private readonly MessageBusNode _messageBusNode;
    
    [Inject]
    private Main(MessageBusNode messageBusNode)
    {
        _messageBusNode = messageBusNode;
    }

    public override void _EnterTree()
    {
        // Allow HostedServices to stop gracefully.
        GetTree().AutoAcceptQuit = false;
        
        _messageBusNode.MessageReceived += OnMessageReceived;
    }
    
    public override void _ExitTree()
    {
        _messageBusNode.MessageReceived -= OnMessageReceived;
    }
    
    private static void OnMessageReceived(Node node, string message)
    {
        GD.Print($"Message from: {node.Name} received: {message}");
    }

    public void SendMessage(int a, [Inject]MessageBusNode mb)
    {
        mb.Publish(this,$"Hello, World!, from Method {a}");
    }
    
    public override void _Ready()
    {
        _messageBusNode.Publish(this, "Hello, World!");
        GetParent().PrintTree();

        // Call the generated version, without Parameters that have the InjectAttribute.
        SendMessage(5);
    }
}