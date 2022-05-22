using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EDISE_lab.Tests
{
    [TestClass]
    public class RewriterTests
    {
        [TestMethod]
        public void GenericTest()
        {
            Rewriter rewriter = new Rewriter();
            const string input = @"
class A
{
    public:
        int b (){}
}
";
            rewriter.BuildTree(input.Split(Environment.NewLine).ToList());
            var actual = rewriter.Rewrite();
            const string expected = @"class A
{
    public int b ()
    {
        //TODO: implement this method
    }
}
";
            actual = actual.Replace("\t", "    ");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GenericTestWithFunction()
        {
            Rewriter rewriter = new Rewriter();
            const string input = @"
class A
{
    public:
        int b ()
        {
            asdsa
        }
}
";
            rewriter.BuildTree(input.Split(Environment.NewLine).ToList());
            var actual = rewriter.Rewrite();
            const string expected = @"class A
{
    public int b ()
    {
        //TODO: implement this method
    }
}
";
            actual = actual.Replace("\t", "    ");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GenericTestWithPointer()
        {
            Rewriter rewriter = new Rewriter();
            const string input = @"
class A
{
    public:
        int b (*int c)
}
";
            rewriter.BuildTree(input.Split(Environment.NewLine).ToList());
            var actual = rewriter.Rewrite();
            const string expected = @"class A
{
    public int b (int c)
    {
        //TODO: implement this method
    }
}
";
            actual = actual.Replace("\t", "    ");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IncorrectIndentation1()
        {
            Rewriter rewriter = new Rewriter();
            const string input = @"
class A
{
    public:
        int b (){
}
";
            Assert.ThrowsException<Exception>(() => rewriter.BuildTree(input.Split(Environment.NewLine).ToList()));
        }

        [TestMethod]
        public void IncorrectIndentation2()
        {
            Rewriter rewriter = new Rewriter();
            const string input = @"
class A
{
    public:
        int b (
}
";
            Assert.ThrowsException<Exception>(() => rewriter.BuildTree(input.Split(Environment.NewLine).ToList()));
        }

        [TestMethod]
        public void UnsupportedModifier()
        {
            Rewriter rewriter = new Rewriter();
            const string input = @"
class A
{
    test:
        int b (){}
}
";
            Assert.ThrowsException<Exception>(() => rewriter.BuildTree(input.Split(Environment.NewLine).ToList()));
        }        
    }
}