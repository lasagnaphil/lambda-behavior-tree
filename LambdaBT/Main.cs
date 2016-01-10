using System;
using System.Collections.Generic;
using LambdaBT;

namespace TestProgram
{
    using Cond = Func<bool>;
    using Leaf = Func<State>;
    using Conv = FunctionalList.Convert;
    class Program
    {
        static void Main(string[] args)
        {
            Leaf bt = new BehaviorTree.Sequencer(FList.Convert<Leaf>(new List<Leaf> {
            }))
        }
    }
}