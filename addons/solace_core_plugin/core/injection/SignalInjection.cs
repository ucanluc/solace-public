using System.Collections.Generic;
using Godot;

namespace Solace.addons.solace_core_plugin.core.injection;

/// <summary>
/// Core Feature: Scene-wide signal injections.
/// <para>Intended to persist and apply everywhere.</para>
/// </summary>
/// <remarks>Load early and use sparsely.</remarks>
public partial class SignalInjection:Node
{
    /// <summary>
    /// True if all future nodes in scene will be checked for injection.
    /// </summary>
    private static bool _isAttachedGlobally;

    /// <summary>
    /// True if the registration of new signal-callback pairs is disallowed.
    /// </summary>
    private static bool _registrationDisabled;

    /// <summary>
    /// Maps the signal names to the callables, intended to be persistent.
    /// </summary>
    private static readonly Dictionary<StringName, List<Callable>> SignalToEventHandlerMap =
        new();

    /// <summary>
    /// Nodes that may have skipped signal registration go here.
    /// </summary>
    private readonly Queue<Node> _uncheckedNodes = new();


    public override void _EnterTree()
    {
        base._EnterTree();
        QueueExistingChildren();
        AttachDynamicSignalHooks();
    }


    public override void _Ready()
    {
        base._Ready();
        DisableSignalRegistration();
        InjectIntoQueuedNodes();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveDynamicSignalHooks();
        // If we enter the tree again, 'ready function' should be called to finalise the registration. 
        RequestReady();
    }


    /// <summary>
    /// Marks the end of signal registration, this is necessary to minimise potential nodes  
    /// </summary>
    /// <remarks>Latest registration is 'EnterTree' of the instanced node's last child.</remarks>
    private void DisableSignalRegistration()
    {
        if (SignalToEventHandlerMap.Count > 0)
        {
            SC.Print(nameof(SignalInjection),
                $"Ready; {SignalToEventHandlerMap.Count} signal(s) registered, this may impact performance when adding nodes to scene.");
        }
        else
        {
            SC.Print(nameof(SignalInjection),
                "Ready; No dynamic signal injections registered, no performance impact expected.");
        }

        _registrationDisabled = true;
    }

    /// <summary>
    /// Recursively queues the children of the given node for future injection.
    /// Starts from the scene root if no node is provided.
    /// </summary>
    /// <param name="node"></param>
    private void QueueExistingChildren(Node? node = null)
    {
        var children = node == null ? GetTree().Root.GetChildren() : node.GetChildren();

        foreach (var child in children)
        {
            if (_uncheckedNodes.Contains(child)) continue;

            _uncheckedNodes.Enqueue(child);
            QueueExistingChildren(child);
        }
    }

    /// <summary>
    /// Injects the registered signal callbacks for queued nodes.
    /// </summary>
    private void InjectIntoQueuedNodes()
    {
        while (_uncheckedNodes.TryDequeue(out var node))
        {
            InjectRegisteredSignalHooks(node);
        }
    }

    /// <summary>
    /// Attaches the checking/hooking process for dynamic signal attachments into the scene tree.
    /// </summary>
    private void AttachDynamicSignalHooks()
    {
        if (_isAttachedGlobally)
        {
            SC.Print(nameof(SignalInjection), "Aborting; Tried to initialise multiple times.");
            return;
        }

        SC.Print(nameof(SignalInjection), "Defined Globally; All future nodes will be checked for registered signals.");

        _isAttachedGlobally = true;
        GetTree().NodeAdded += InjectRegisteredSignalHooks;
    }

    /// <summary>
    /// Removes the checking/hooking process for dynamic signal attachments into the scene tree.
    /// </summary>
    private void RemoveDynamicSignalHooks()
    {
        if (_isAttachedGlobally)
        {
            GetTree().NodeAdded -= InjectRegisteredSignalHooks;
            _isAttachedGlobally = false;
            SC.Print(nameof(SignalInjection), "Removed Globally; All future nodes will not be checked for registered signals.");
        }
        else
        {
            SC.Print(nameof(SignalInjection), "No signal hook attachment exists, check for multiple instantiation.");
        }

        _registrationDisabled = false;
    }

    /// <summary>
    /// Checks the nodes for registered signal names, and attaches registered callbacks to matching signals.
    /// </summary>
    /// <param name="node">Node to check for SignalNames</param>
    private void InjectRegisteredSignalHooks(Node node)
    {
        if (SignalToEventHandlerMap.Count == 0)
        {
            return;
        }

        foreach (var (signalName, callables) in SignalToEventHandlerMap)
        {
            if (!node.HasSignal(signalName)) continue;

            foreach (var callable in callables)
            {
                if (node.IsConnected(signalName, callable)) continue;
                node.Connect(signalName, callable);
            }
        }
    }

    /// <summary>
    /// Register a <see cref="Callable"/> to connect automatically with the signal name for all nodes.
    /// <para>Registration of new hooks is not allowed after the loading period.</para>
    /// </summary>
    ///<example>
    /// <code>
    /// CoreSignalInjections.RegisterAutomatedSignalHook(
    ///     BaseButton.SignalName.Pressed, new Callable(this,MethodName.OnButtonPressed) 
    /// </code>
    /// <para> Hooks the method <c>OnButtonPressed</c> to all signals named <c>Pressed</c> in the scene.</para>
    /// </example>
    /// <param name="signalName">SignalName to match and attach by.</param>
    /// <param name="callable">Callable to attach to future nodes.</param>
    /// <returns>True if successfully registered, false if ignored.</returns>
    /// <remarks>Use sparsely, this is intended to be persistent and global across the project.</remarks>
    public static bool RegisterHook(StringName signalName, Callable callable)
    {
        if (!_isAttachedGlobally)
        {
            SC.Print(nameof(SignalInjection),
                $"Signal injection not active; Hook {callable.Method} into {signalName} may be inactive.");
        }

        if (_registrationDisabled)
        {
            SC.Print(nameof(SignalInjection),
                $"Signal injection late registry; Hook for {callable.Method} into {signalName} is ignored.");
            return false;
        }

        SC.Print(nameof(SignalInjection),
            $"Signal injection registered; Hook for {callable.Method} into {signalName}.");

        if (!SignalToEventHandlerMap.ContainsKey(signalName))
        {
            SignalToEventHandlerMap[signalName] = new List<Callable> { callable };
            return true;
        }

        SignalToEventHandlerMap[signalName].Add(callable);
        return true;
    }
}