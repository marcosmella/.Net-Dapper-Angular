using AutoMoqCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace VL.Health.IoC.Tests
{
    public class HealthIoCTests
    {
        private readonly AutoMoqer _autoMoqer;

        public HealthIoCTests()
        {
            _autoMoqer = new AutoMoqer();
        }

        [Fact]
        public void ConfigureIoC()
        {
            //ARRANGE
            var service = new ServiceCollection();

            //ACT
            HealthIoC.ConfigureIoC(service);

            //ASSERT
            Assert.Equal(58, service.Count);
        }
    }
}
