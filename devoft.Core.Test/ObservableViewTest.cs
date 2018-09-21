using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace devoft.Core.Test
{
    [TestClass]
    public class ObservableViewTest
    {
        private ConsoleInterceptor _console = new ConsoleInterceptor();

        [TestInitialize]
        public void Setup()
        {
            Console.SetOut(_console);
        }
        
        [TestMethod]
        public void Enumeration()
        {
            var view = new ObservableView<int>(1,2,3,4,5);
            var index = 0;
            foreach (var item in view)
            {
                index++;
                Assert.AreEqual(index, item);
            }
        }

        [TestMethod]
        public void UpdateFromEmpty()
        {
            var view = new ObservableView<int>();
            view.Update(1,2,3,4);
            Assert.IsTrue(view.ToArray().CollectionEquals(new[] { 1, 2, 3, 4 }));
        }

        [TestMethod]
        public void UpdateWithNull()
        {
            var view = new ObservableView<int>(1,2,3,4);
            view.Update(null);
            Assert.IsTrue(view.ToArray().CollectionEquals(new[] { 1, 2, 3, 4 }));
        }

        [TestMethod]
        public void Updates()
        {
            var view = new ObservableView<int>(1, 2, 3, 4);
            view.Update(2, 3, 5);
            Assert.IsTrue(view.ToArray().CollectionEquals(new[] { 2, 3, 5 }));
            view.Update(1, 2, 6, 7);
            Assert.IsTrue(view.ToArray().CollectionEquals(new[] { 1, 2, 6, 7 }));
        }

        [TestMethod]
        public void UpdatesWithNotifications()
        {
            _console.Reset();
            var view = new ObservableView<int>(1, 2, 3, 4);
            view.CollectionChanged += (s,e) => 
            {
                Console.WriteLine($"{e.Action}:{(e.OldItems ?? e.NewItems)?.Cast<object>().ToString(",")}");
            };
            view.Update(2, 3, 5);
            Assert.AreEqual(3, _console.Lines.Count);
            Assert.AreEqual("Add:5", _console.LastText);
        }

    }

    public class Contact
    {
        public string Name { get; set; }
        public string Age { get; set; }
    }
}
