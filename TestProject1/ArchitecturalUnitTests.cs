using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using ClassLibrary1;
using WebApplication1.Controllers;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace TestProject1
{
    public class Predicate
    {
        public string Description => "PredicatePredicate";

        public static Func<Class, bool> IsNotUsedByAnyControllerAsReturnType(Architecture architecture)
        {
            var controllers = architecture.Classes.Where(c => c.NameContains("Controller"));
            return (Class @class) =>
            {
                var result = new List<Class>();
                foreach (var controller in controllers)
                {
                    var isClassReturnedFromAnyController = controller.GetMethodMembers().Any(m => m.ReturnType.IsAssignableTo(@class));
                    if (isClassReturnedFromAnyController)
                    {
                        return false;
                    }
                }
                return true;
            };
        }
    }

    public class ArchitecturalUnitTests
    {
        private static readonly Architecture Architecture = new ArchLoader().LoadAssemblies(
            typeof(WeatherForecastController).Assembly,
            typeof(ExampleDomainModel).Assembly
            ).Build();

        [Fact]
        public void TypesShouldBeInCorrectLayer()
        {
            string testDescription = "Domain layer models should not be returned in Controller";
            string errorMessage = "One of the domain model is returned from Controller!";

            var domainLayer = typeof(ExampleDomainModel).Assembly;
            var rule = Classes(includeReferenced: true).That().ResideInAssembly(domainLayer)
                        .Should()
                        .FollowCustomCondition(Predicate.IsNotUsedByAnyControllerAsReturnType(Architecture), testDescription, errorMessage);

            rule.Check(Architecture);
        }
    }
}