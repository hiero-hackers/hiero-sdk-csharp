
namespace System.Collections.Generic
{
	public class ListGuarded<T> : List<T>
	{
		public ListGuarded() : this(_ => { }) { }
		public ListGuarded(params T[] values) : this(_ => { })
		{
            AddRange(values);
        }
		public ListGuarded(IEnumerable<T> values) : this(_ => { })
		{
            AddRange(values);
        }
		public ListGuarded(Action<ListGuarded<T>> oninit)
		{
			OnRequireNotFrozen = () => { if (IsFrozen) throw new InvalidOperationException("Cannot operate on a frozen list"); };
			OnRequireNotLocked = () => { if (IsLocked) throw new InvalidOperationException("Cannot modify a locked list"); };

            oninit.Invoke(this);
        }

        public new T this[int index]
		{
			get => base[index];
			set
			{
				OnRequireNotFrozen?.Invoke();
				OnRequireNotLocked?.Invoke();
				OnValidateItem?.Invoke(value);

                OnValidatePre?.Invoke(AsReadOnly());
                base[index] = value;
                OnValidatePost?.Invoke(AsReadOnly());
            }
		}

		public int Index { get; set; }
		public bool IsReadOnly { get => false; }
		public bool IsFrozen { get; internal set; }
		public bool IsLocked { get; internal set; }
		public bool IsEmpty { get => Count == 0; }

		public T Current { get => this[Index]; }

        public Action OnRequireNotFrozen { get; internal set; }
		public Action OnRequireNotLocked { get; internal set; }
		public Action<T>? OnValidateItem { get; internal set; }
		public Action<IReadOnlyList<T>>? OnValidatePre { get; internal set; }
		public Action<IReadOnlyList<T>>? OnValidatePost { get; internal set; }

        public int Advance()
        {
            int index = Index;
            Index = (Index + 1) % Count;
            return index;
        }
        public void AddRange(params T[] items)
        {
            AddRange(items as IEnumerable<T>);
        }
        public void Set(params T[] items)
        {
            Set(items);
        }
        public void Set(IEnumerable<T> items)
        {
            OnValidatePre?.Invoke(AsReadOnly());

            base.Clear();

            foreach (T item in items)
            {
                OnValidateItem?.Invoke(item);
                base.Add(item);
            }

            OnValidatePost?.Invoke(AsReadOnly());
        }
        public void Shuffle()
        {
            OnRequireNotFrozen?.Invoke();
            OnRequireNotLocked?.Invoke();

            var rng = Random.Shared;

            for (int i = Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (this[i], this[j]) = (this[j], this[i]);
            }
        }

        public new void Add(T item)
        {
            OnValidatePre?.Invoke(AsReadOnly());
            OnValidateItem?.Invoke(item);
            base.Add(item);
            OnValidatePost?.Invoke(AsReadOnly());
        }
        public new void AddRange(IEnumerable<T> items)
        {
            OnValidatePre?.Invoke(AsReadOnly());

            foreach (T item in items)
            {
                OnValidateItem?.Invoke(item);
                base.Add(item);
            }

            OnValidatePost?.Invoke(AsReadOnly());
        }
        public new void Clear()
        {
            OnValidatePre?.Invoke(AsReadOnly());
            base.Clear();
            OnValidatePost?.Invoke(AsReadOnly());
        }
        public new void CopyTo(T[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }
        public new int IndexOf(T item)
        {
            return base.IndexOf(item);
        }
        public new void Insert(int index, T item)
        {
            OnValidatePre?.Invoke(AsReadOnly());
            OnValidateItem?.Invoke(item);
            base.Insert(index, item);
            OnValidatePost?.Invoke(AsReadOnly());
        }
        public new bool Remove(T item)
        {
            bool result;
            OnValidatePre?.Invoke(AsReadOnly());
            result = base.Remove(item);
            OnValidatePost?.Invoke(AsReadOnly());

            return result;
        }
        public new void RemoveAt(int index)
        {
            OnValidatePre?.Invoke(AsReadOnly());
            base.RemoveAt(index);
            OnValidatePost?.Invoke(AsReadOnly());
        }

        public static implicit operator ListGuarded<T>(T item) => [item];
    }
}