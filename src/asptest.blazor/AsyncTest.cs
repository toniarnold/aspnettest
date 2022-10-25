using asp.blazor.Models;
using asp.blazor.Pages;
using asplib.Model;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.Threading;

namespace asptest
{
    public class AsyncTest : StaticComponentTest<EdgeDriver, Async>
    {
        [Test]
        public void NonSynchronized()
        {
            Navigate("/async");
            Assert.That(Component.countNumber.Value, Is.EqualTo(Async.Iterations));
            Click(Component.startButton);
            this.AssertPoll(() => Component.countNumber.Value, () => Is.EqualTo(0));
        }

        [Test]
        public void Synchronized()
        {
            Navigate("/async");
            Assert.That(Component.countNumber.Value, Is.EqualTo(Async.Iterations));
            // Will always render once plus additionally half of the iterations:
            Click(Component.startButton, expectRenders: (Async.Iterations / 2) + 1);
            Assert.That(Component.countNumber.Value, Is.EqualTo(0));
        }
    }
}