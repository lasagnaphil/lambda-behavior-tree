using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaBT
{
    using Cond = Func<bool>;
    using Leaf = Func<State>;
    using FList = FunctionalList;

    public enum State { RUNNING, SUCCESS, FAILURE, ERROR };

    public static class Extensions
    {
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }

    // From Microsoft's Documentation
    public class FunctionalList<T>
    {
        public FunctionalList()
        {
            IsEmpty = true;
        }
        public FunctionalList(T head, FunctionalList<T> tail)
        {
            IsEmpty = false;
            Head = head;
            Tail = tail;
        }
        public bool IsEmpty { get; private set; }
        public T Head { get; private set; }
        public FunctionalList<T> Tail { get; private set; }
    }
    public static class FunctionalList
    {
        public static FunctionalList<T> Empty<T>()
        {
            return new FunctionalList<T>();
        }
        public static FunctionalList<T> Cons<T>(T head, FunctionalList<T> tail)
        {
            return new FunctionalList<T>(head, tail);
        }
        // really inefficient code, but hey, I wish C# had macros
        public static FunctionalList<T> Convert<T>(List<T> list) where T : ICloneable
        {
            List<T> listTail = Extensions.Clone<T>(list);
            listTail.RemoveAt(0);
            return new FunctionalList<T>(list.First(), Convert<T>(listTail));
        }
    }

    public static class BehaviorTree
    {
        public static Random randGen;
        // Composite Nodes
        public static Leaf Selector(Cond cond, Leaf ifTrue, Leaf ifFalse)
        {
            if (cond()) { return ifTrue; } else { return ifFalse; }
        }
        public static Leaf Sequencer(Leaf leaf1, Leaf leaf2)
        {
            return () =>
            {
                State leaf1State = leaf1();
                if (leaf1State != State.SUCCESS) return leaf1State;
                return leaf2();
            };
        }
        public static Leaf Sequencer(FunctionalList<Leaf> leafList)
        {
            return () =>
            {
                if (leafList.IsEmpty) return State.SUCCESS;
                else
                {
                    State headState = leafList.Head();
                    if (headState == State.SUCCESS) return Sequencer(leafList.Tail)();
                    else return headState;
                }
            };
        }
        public static Leaf Selector(FunctionalList<Leaf> leafList)
        {
            return () =>
            {
                if (leafList.IsEmpty) return State.FAILURE;
                else
                {
                    State headState = leafList.Head();
                    if (headState == State.FAILURE) return Selector(leafList.Tail)();
                    else return headState;
                }
            };
        }
        public static Leaf RandomSelector(FunctionalList<Leaf> leafList)
        {
            // placeholder code
            return () => { return State.FAILURE; };
        }

        // Decorator Nodes
        public static Leaf Inverter(Leaf leaf)
        {
            return () =>
            {
                State leafState = leaf();
                switch (leafState)
                {
                    case State.SUCCESS: return State.FAILURE;
                    case State.FAILURE: return State.SUCCESS;
                    default: return leafState;
                }
            };
        }
        public static Leaf Succeeder(Leaf leaf)
        {
            return () => { return State.SUCCESS; };
        }
        public static Leaf Failer(Leaf leaf)
        {
            return () => { return State.FAILURE; };
        }
        public static Leaf Repeater(Leaf leaf, int times)
        {
            return () =>
            {
                State leafState = leaf();
                switch (leafState)
                {
                    case State.SUCCESS:
                    case State.FAILURE:
                        if (times == 0) return State.SUCCESS;
                        else return Repeater(leaf, times - 1)();
                    case State.RUNNING: return State.RUNNING;
                    default: return State.ERROR;
                }
            };
        }
        public static Leaf RepeatUntilFail(Leaf leaf)
        {
            return () =>
            {
                State leafState = leaf();
                switch (leafState)
                {
                    case State.SUCCESS: return RepeatUntilFail(leaf)();
                    case State.FAILURE: return State.SUCCESS;
                    default: return leafState;
                }
            };
        }
    }

    
}

