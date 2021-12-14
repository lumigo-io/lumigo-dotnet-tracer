using Amazon.Runtime.Internal;

namespace Lumigo.DotNET.Instrumentation
{
    public static class LumigoBootstrap
    {
        public static void Bootstrap()
        {
            CustomizePipeline();
        }

        private static void CustomizePipeline()
        {
            RuntimePipelineCustomizerRegistry.Instance.Register(new LumigoPipelineCustomizer());
        }
    }
}
