using System;
using Amazon.Runtime.Internal;

namespace Lumigo.DotNET.Instrumentation
{
    public class LumigoPipelineCustomizer : IRuntimePipelineCustomizer
    {
        public string UniqueName => "Lumigo pipeline customizer";

        public void Customize(Type type, RuntimePipeline pipeline)
        {
            var handler = new HandlerFactory(type.Name).GetInstace(type);
            pipeline.AddHandler(handler);
        }
    }
}
