using Xunit;
using System;

using Lumigo.DotNET.Utilities;

namespace Lumigo.DotNET.Test.Utilities
{
    public class SafeUtilsTest
    {
        [Fact]
        public void SafeExecute_No_Error()
        {
            bool called = false;
            SafeUtils.SafeExecute(() => called = true, "a message");
            Assert.True(called);  // Verify the function was executed.
        }

        [Fact]
        public void SafeExecute_With_Error()
        {
            Action funcWithException = () =>
            {
                int zero = 0;
                int a = 1 / zero;
            };
            // Validate the function cause an exception.
            bool exceptionOccurred = false;
            try
            {
                funcWithException();
            }
            catch
            {
                exceptionOccurred = true;
            }
            Assert.True(exceptionOccurred);


            SafeUtils.SafeExecute(funcWithException, "a message");  // No exception is thrown.
        }
    }
}
