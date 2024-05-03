using Godot;

namespace Solace.content.user_interface.debug;

public partial class PerformanceMonitorLabel : Label
{
    public override void _Process(double delta)
    {
        // TODO: Tabular debug data display will replace the need for this.
        // TODO: Register debug data via a common api in SC; which might still use Performance.GetMonitor.
        Text = $"Engine FPS: {Engine.GetFramesPerSecond()}" +
               $"\nUpdate Delta: {delta}" +
               $"\nMonitor FPS: {Performance.GetMonitor(Performance.Monitor.TimeFps)}" +
               $"\nFrame Time: {Performance.GetMonitor(Performance.Monitor.TimeProcess)}" +
               $"\nPhysics Time: {Performance.GetMonitor(Performance.Monitor.TimePhysicsProcess)}" +
               $"\nNavigation Time: {Performance.GetMonitor(Performance.Monitor.TimeNavigationProcess)}" +
               $"\nStatic Memory Used: {Performance.GetMonitor(Performance.Monitor.MemoryStatic):N}" +
               $"\nStatic Memory Available: {Performance.GetMonitor(Performance.Monitor.MemoryStaticMax):N}" +
               $"\nMsg Buffer Max Mem Used: {Performance.GetMonitor(Performance.Monitor.MemoryMessageBufferMax):N}" +
               $"\nObject #: {Performance.GetMonitor(Performance.Monitor.ObjectCount)}" +
               $"\nResource #: {Performance.GetMonitor(Performance.Monitor.ObjectResourceCount)}" +
               $"\nNode #: {Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)}" +
               $"\nOrphan Node #: {Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount)}" +
               $"\nRender Object #: {Performance.GetMonitor(Performance.Monitor.RenderTotalObjectsInFrame)}" +
               $"\nRender Primitive #: {Performance.GetMonitor(Performance.Monitor.RenderTotalPrimitivesInFrame)}" +
               $"\nRender Draw Calls: {Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame)}" +
               $"\nVRAM Used: {Performance.GetMonitor(Performance.Monitor.RenderVideoMemUsed):N}" +
               $"\nTexture Mem Used: {Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed):N}" +
               $"\nRender Buffer Mem Used: {Performance.GetMonitor(Performance.Monitor.RenderBufferMemUsed):N}" +
               $"\n2D Phys Active Obj #: {Performance.GetMonitor(Performance.Monitor.Physics2DActiveObjects)}" +
               $"\n2D Phys Collision Pair #: {Performance.GetMonitor(Performance.Monitor.Physics2DCollisionPairs)}" +
               $"\n2D Phys Island #: {Performance.GetMonitor(Performance.Monitor.Physics2DIslandCount)}" +
               $"\n3D Phys Active Obj #: {Performance.GetMonitor(Performance.Monitor.Physics3DActiveObjects)}" +
               $"\n3D Phys Collision Pair #: {Performance.GetMonitor(Performance.Monitor.Physics3DCollisionPairs)}" +
               $"\n3D Phys Island #: {Performance.GetMonitor(Performance.Monitor.Physics3DIslandCount)}" +
               $"\nNav Active Map #: {Performance.GetMonitor(Performance.Monitor.NavigationActiveMaps)}" +
               $"\nNav Region #: {Performance.GetMonitor(Performance.Monitor.NavigationRegionCount)}" +
               $"\nNav Agent #: {Performance.GetMonitor(Performance.Monitor.NavigationAgentCount)}" +
               $"\nNav Link #: {Performance.GetMonitor(Performance.Monitor.NavigationLinkCount)}" +
               $"\nNav Poly #: {Performance.GetMonitor(Performance.Monitor.NavigationPolygonCount)}" +
               $"\nNav Edge #: {Performance.GetMonitor(Performance.Monitor.NavigationEdgeCount)}" +
               $"\nNav Edge Merge #: {Performance.GetMonitor(Performance.Monitor.NavigationEdgeMergeCount)}" +
               $"\nNav Edge Connection #: {Performance.GetMonitor(Performance.Monitor.NavigationEdgeConnectionCount)}" +
               $"\nNav Edge Free #: {Performance.GetMonitor(Performance.Monitor.NavigationEdgeFreeCount)}";
    }
}