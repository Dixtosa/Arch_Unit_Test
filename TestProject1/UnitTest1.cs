using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Predicates;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Microsoft.AspNetCore.Mvc;
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

    public class UnitTest1
    {
        private static readonly Architecture Architecture = new ArchLoader().LoadAssemblies(
            System.Reflection.Assembly.Load("WebApplication1"),
            System.Reflection.Assembly.Load("ClassLibrary1")
            ).Build();

        [Fact]
        public void TypesShouldBeInCorrectLayer()
        {
            var cnt = Classes(true).GetObjects(Architecture).Count();

            var domainLayer = System.Reflection.Assembly.Load("ClassLibrary1");

            var rule1 = Classes(true).That().ResideInAssembly(domainLayer).Should().FollowCustomCondition(Predicate.IsNotUsedByAnyControllerAsReturnType(Architecture), 
                "Domain layer models should not be returned in controller", 
                "One of the domain model is returned from Controller!");

            rule1.Check(Architecture);
        }
    }
}