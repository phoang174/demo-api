using NetArchTest.Rules;
using FluentAssertions;

namespace Test
{
    public class UnitTest1
    {
        private const string DomainNamespace = "Domain";
        private const string ApplicationNamespace = "Application";
        private const string InfrastructureNamespace = "Infrastructure";
        private const string WebNameSpace = "demo_api";
        [Fact]
        public void Test1()
        {

            //arange 
            var assembly = typeof(Domain.AssemblyReference).Assembly;
            var otherProjects = new[]
            {
                DomainNamespace, ApplicationNamespace, InfrastructureNamespace, WebNameSpace,
            };
            //Act
            var testResult = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAll(otherProjects).GetResult();

            //assert
            testResult.IsSuccessful.Should().BeTrue();
        }
    }
}