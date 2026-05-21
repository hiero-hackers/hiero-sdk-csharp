// SPDX-License-Identifier: Apache-2.0
using Hiero.SDK.Cryptocurrency;
using Hiero.SDK.Exceptions;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hiero.SDK
{
	public sealed partial class Client
    {
		/// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:Ping(AccountId)"]' />
		public void Ping(AccountId nodeAccountId)
        {
            Ping(nodeAccountId, RequestTimeout);
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:Ping(AccountId,System.NodaTime.Duration)"]' />
        public void Ping(AccountId nodeAccountId, NodaTime.Duration timeout)
        {
            new AccountBalanceQuery
			{
				AccountId = nodeAccountId,
				NodeAccountIds = nodeAccountId,

			}.Execute(this, timeout);
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAsync(AccountId)"]' />
        public Task PingAsync(AccountId nodeAccountId)
        {
            return PingAsync(nodeAccountId, RequestTimeout);
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAsync(AccountId,System.NodaTime.Duration)"]' />
        public async Task PingAsync(AccountId nodeAccountId, NodaTime.Duration timeout)
        {
			await new AccountBalanceQuery()
			{
				NodeAccountIds = nodeAccountId,

			}.ExecuteAsync(this, timeout);
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAsync(AccountId,System.Action{System.Exception})"]' />
        public void PingAsync(AccountId nodeAccountId, Action<Exception> callback)
        {
            Utils.ActionHelper.Action(PingAsync(nodeAccountId), callback);
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAsync(AccountId,System.NodaTime.Duration,System.Action{System.Exception})"]' />
        public void PingAsync(AccountId nodeAccountId, NodaTime.Duration timeout, Action<Exception> callback)
        {
            Utils.ActionHelper.Action(PingAsync(nodeAccountId, timeout), callback);
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAsync(AccountId,System.Action,System.Action{System.Exception})"]' />
        public void PingAsync(AccountId nodeAccountId, Action onSuccess, Action<Exception> onFailure)
        {
            Utils.ActionHelper.TwoActions(PingAsync(nodeAccountId), onSuccess, onFailure);
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAsync(AccountId,System.NodaTime.Duration,System.Action,System.Action{System.Exception})"]' />
        public void PingAsync(AccountId nodeAccountId, NodaTime.Duration timeout, Action onSuccess, Action<Exception> onFailure)
        {
            Utils.ActionHelper.TwoActions(PingAsync(nodeAccountId, timeout), onSuccess, onFailure);
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAll"]' />
        public void PingAll()
        {
            lock (this)
            {
                PingAll(RequestTimeout);
            }
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAll(System.NodaTime.Duration)"]' />
        public void PingAll(NodaTime.Duration timeoutPerPing)
        {
            lock (this)
            {
                foreach (var nodeAccountId in Network_.GetNetwork().Values)
                {
                    Ping(nodeAccountId, timeoutPerPing);
                }
            }
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAllAsync"]' />
        public Task PingAllAsync()
        {
            lock (this)
            {
                return PingAllAsync(RequestTimeout);
            }
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAllAsync(System.NodaTime.Duration)"]' />
        public Task PingAllAsync(NodaTime.Duration timeoutPerPing)
        {
            lock (this)
            {
                var _Network = Network_.GetNetwork();

                var list = new List<Task>(_Network.Count);
                foreach (var nodeAccountId in _Network.Values)
                {
                    list.Add(PingAsync(nodeAccountId, timeoutPerPing));
                }

                return Task.WhenAll(list);
            }
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAllAsync(System.Action{System.Exception})"]' />
        public void PingAllAsync(Action<Exception> callback)
        {
            Utils.ActionHelper.Action(PingAllAsync(), callback);
        }
		/// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAllAsync(System.Action,System.Action{System.Exception})"]' />
		public void PingAllAsync(Action onSuccess, Action<Exception> onFailure)
		{
			Utils.ActionHelper.TwoActions(PingAllAsync(), onSuccess, onFailure);
		}
		/// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAllAsync(System.NodaTime.Duration,System.Action{System.Exception})"]' />
		public void PingAllAsync(NodaTime.Duration timeoutPerPing, Action<Exception> callback)
        {
            Utils.ActionHelper.Action(PingAllAsync(timeoutPerPing), callback);
        }
        /// <include file="Client.Ping.cs.xml" path='docs/member[@name="M:PingAllAsync(System.NodaTime.Duration,System.Action,System.Action{System.Exception})"]' />
        public void PingAllAsync(NodaTime.Duration timeoutPerPing, Action onSuccess, Action<Exception> onFailure)
        {
            Utils.ActionHelper.TwoActions(PingAllAsync(timeoutPerPing), onSuccess, onFailure);
        }
    }
}