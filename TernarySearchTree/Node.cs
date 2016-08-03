namespace TernarySearchTree
{
    internal class Node<TValue>
    {
        private TValue value;

        public Node(char splitCharacter)
        {
            SplitCharacter = splitCharacter;
        }

        public char SplitCharacter { get; private set; }

        public Node<TValue> HigherNode { get; set; }

        public Node<TValue> EqualNode { get; set; }

        public Node<TValue> LowerNode { get; set; }

        public bool HasValue { get; private set; }

        public TValue Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
                HasValue = true;
            }
        }
    }
}
