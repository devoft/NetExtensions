using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using devoft.Core.Patterns.Scoping;
using System;
using devoft.Core.Patterns;
using System.Diagnostics;
using System.Threading.Tasks;

namespace devoft.Core.Test
{

    [TestClass]
    public class ScopeTaskTests
    {
        ILogger _logger;
        IObserver<string> _loggerObserver;
        ConsoleInterceptor _console;

        [TestInitialize]
        public void Setup()
        {
            Console.SetOut(_console = new ConsoleInterceptor());
        }

        [TestMethod]
        public async Task CompleteScope()
        {
            var result = await  ScopeTaskMoq.Define(sc => sc.Yield(nameof(CompleteScope)))
                                            .StartAsync();
            Assert.AreEqual(nameof(CompleteScope), result);
        }

        [TestMethod]
        public async Task CompleteWithObservationScope()
        {
            _console.Reset();
            var observer = new FunctionObserver<object>(s => Console.WriteLine(s));
            var result = await
                ScopeTaskMoq.Define(sc =>
                                    {
                                        for (var i = 0; i < 3; i++)
                                            sc.Yield(nameof(CompleteWithObservationScope) + i);
                                    })
                            .Observe(ob => ob.Subscribe(observer))
                            .StartAsync();

            Assert.AreEqual(nameof(CompleteWithObservationScope) + 2, result);
            Assert.AreEqual(3, _console.Lines.Count);
        }

        [TestMethod]
        public void TestScopeAspects()
        {
            _console.Reset();
            IScopeAspect aspect = new LogScopeAspect();
            var res = aspect.Begin(new ScopeContext());
            Assert.AreEqual("Start",_console.LastText);
            Assert.IsTrue(res);
            aspect.End(new ScopeContext(), res);
            Assert.AreEqual("End", _console.LastText);
        }
    }

    public class ScopeTaskMoq : ScopeTaskBase<ScopeTaskMoq> { }

    public class LogScopeAspect : IScopeAspect
    {
        public bool Begin(ScopeContext context)
        {
            Console.WriteLine("Start");
            return true;
        }

        public void End(ScopeContext context, bool result)
        {
            Console.WriteLine("End");
        }

        public bool IsInscope(ScopeContext context)
        {
            Console.WriteLine("Asking for scope");
            return true;
        }
    }
}
