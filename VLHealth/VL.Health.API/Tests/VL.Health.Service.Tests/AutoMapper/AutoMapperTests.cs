using AutoMapper;
using System;
using VL.Health.Service.Mapper;
using Xunit;
namespace VL.Health.Service.Tests.AutoMapper
{
    public class AutoMapperTests
    {
        [Fact]
        public void ConfigMapperTest()
        {
            var mapperConfigurationSuccess = true;

            try
            {
                var configuration = new MapperConfiguration(cfg => {
                    cfg.AddProfile<AutoMapperProfile>();
                });
                configuration.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                mapperConfigurationSuccess = false;
            }

            Assert.True(mapperConfigurationSuccess);
        }
    }
}
