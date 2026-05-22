
namespace System.Collections.Generic
{
	public class ListGuarded<T> : IEnumerable<T>
	{
		public ListGuarded() : this(_ => { }) { }
		public ListGuarded(params T[] values) : this(_ => { })
		{
            _list = new ListInternal<T>(this);
            _list.AddRange(values);
        }
		public ListGuarded(IEnumerable<T> values) : this(_ => { })
		{
			_list = new ListInternal<T>(this);
			_list.AddRange(values);
        }
		public ListGuarded(Action<ListGuarded<T>> oninit)
		{
			OnRequireNotFrozen = () => { if (IsFrozen) throw new InvalidOperationException("Cannot operate on a frozen list"); };
			OnRequireNotLocked = () => { if (IsLocked) throw new InvalidOperationException("Cannot modify a locked list"); };

            _list = new ListInternal<T>(this);

            oninit.Invoke(this);
        }

		private ListInternal<T> _list;

        public T this[int index]
		{
			get => _list[index];
			set
			{
				OnRequireNotFrozen?.Invoke();
				OnRequireNotLocked?.Invoke();
				OnValidateItem?.Invoke(value);

                OnValidatePre?.Invoke(Read);
                _list[index] = value;
                OnValidatePost?.Invoke(Read);
            }
		}

		public int Index { get; set; }
		public int Count { get => _list.Count; }
		public bool IsReadOnly { get => false; }
		public bool IsFrozen { get; internal set; }
		public bool IsLocked { get; internal set; }
		public bool IsEmpty { get => _list.Count == 0; }

		public T Current { get => _list[Index]; }

        public IReadOnlyList<T> Read { get => _list.AsReadOnly(); }

        public Action OnRequireNotFrozen { get; internal set; }
		public Action OnRequireNotLocked { get; internal set; }
		public Action<T>? OnValidateItem { get; internal set; }
		public Action<IReadOnlyList<T>>? OnValidatePre { get; internal set; }
		public Action<IReadOnlyList<T>>? OnValidatePost { get; internal set; }

        public ListGuarded<T> Operate(Action<ListInternal<T>> list)
        {
            OnRequireNotFrozen?.Invoke();
            OnRequireNotLocked?.Invoke();

            list.Invoke(_list);

            _list = new ListInternal<T>(this, _list);

            return this;
        }
        public ListGuarded<T> Operate(Func<ListInternal<T>, IEnumerable<T>> list)
        {
            OnRequireNotFrozen?.Invoke();
            OnRequireNotLocked?.Invoke();

            IEnumerable<T> enumerable = list.Invoke(_list);


            _list = new ListInternal<T>(this);
            _list.AddRange(list.Invoke(_list));

            return this;
        }

        public void Shuffle()
        {
            OnRequireNotFrozen?.Invoke();
            OnRequireNotLocked?.Invoke();

            var rng = Random.Shared;

            for (int i = _list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (_list[i], _list[j]) = (_list[j], _list[i]);
            }
        }
        public bool Contains(T item)
        {
            return _list.Contains(item);
        }
        public int Advance()
		{
			int index = Index;
			Index = (Index + 1) % _list.Count;
			return index;
		}
        public int EnsureCapacity(int capacity)
        {
            return _list.EnsureCapacity(capacity);
        }

        public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)_list).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

        public static implicit operator ListGuarded<T>(T item) => new(item);
        public static implicit operator ListGuarded<T>(T[] array) => new(array);
		public static implicit operator ListGuarded<T>(List<T> list) => new (list);
		public static implicit operator T[](ListGuarded<T> list) => [.. list];
		public static implicit operator List<T>(ListGuarded<T> list) => [.. list];

		public class ListInternal<TT> : List<TT>
		{
            internal ListInternal(ListGuarded<TT> parent) : this(parent, []) { }
            internal ListInternal(ListGuarded<TT> parent, params TT[] values) : this (parent, values as IEnumerable<TT>) { }
			internal ListInternal(ListGuarded<TT> parent, IEnumerable<TT> values)
			{
				Parent = parent;

                AddRange(values);
			}

            public ListGuarded<TT> Parent { get; }

            public void AddRange(params TT[] items)
            {
                AddRange(items as IEnumerable<TT>);
            }

            public new void Add(TT item)
            {
                Parent.OnValidateItem?.Invoke(item);
                Parent.OnValidatePre?.Invoke(Parent.Read);
                base.Add(item);
                Parent.OnValidatePost?.Invoke(Parent.Read);
            }
            public new void AddRange(IEnumerable<TT> items)
            {
                Parent.OnValidatePre?.Invoke(Parent.Read);

                foreach (TT item in items)
                {
                    Parent.OnValidateItem?.Invoke(item);
                    base.Add(item);
                }

                Parent.OnValidatePost?.Invoke(Parent.Read);
            }
            public new void Clear()
            {                
                Parent.OnValidatePre?.Invoke(Parent.Read);
                base.Clear();
                Parent.OnValidatePost?.Invoke(Parent.Read);
            }
            public new void CopyTo(TT[] array, int arrayIndex)
            {
                base.CopyTo(array, arrayIndex);
            }
            public new int IndexOf(TT item)
            {
                return base.IndexOf(item);
            }
            public new void Insert(int index, TT item)
            {
                Parent.OnValidateItem?.Invoke(item);
                Parent.OnValidatePre?.Invoke(Parent.Read);
                base.Insert(index, item);
                Parent.OnValidatePost?.Invoke(Parent.Read);
            }
            public new bool Remove(TT item)
            {
                bool result;
                Parent.OnValidatePre?.Invoke(Parent.Read);
                result = base.Remove(item);
                Parent.OnValidatePost?.Invoke(Parent.Read);

                return result;
            }
            public new void RemoveAt(int index)
            {
                Parent.OnValidatePre?.Invoke(Parent.Read);
                base.RemoveAt(index);
                Parent.OnValidatePost?.Invoke(Parent.Read);
            }
        }
    }
}