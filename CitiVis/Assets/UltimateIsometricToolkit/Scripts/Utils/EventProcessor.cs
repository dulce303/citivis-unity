using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UltimateIsometricToolkit.Scripts.Utils {
	/// <summary>
	/// Routes actions on the unity main thread
	/// </summary>
	public class EventProcessor : Singleton<EventProcessor> {

		public void QueueEvent(Action action) {
			lock (_mQueueLock) {
				_mQueuedEvents.Add(action);
			}
		}

		void Update() {
			MoveQueuedEventsToExecuting();

			while (_mExecutingEvents.Count > 0) {
				var e = _mExecutingEvents[0];
				_mExecutingEvents.RemoveAt(0);
				try {
					e();
				} catch (Exception exception) {
					Debug.Log(exception);

				}
			}
		}

		private void MoveQueuedEventsToExecuting() {
			lock (_mQueueLock) {
				while (_mQueuedEvents.Count > 0) {
					var e = _mQueuedEvents[0];
					_mExecutingEvents.Add(e);
					_mQueuedEvents.RemoveAt(0);
				}
			}
		}

		private readonly object _mQueueLock = new object();
		private readonly List<Action> _mQueuedEvents = new List<Action>();
		private readonly List<Action> _mExecutingEvents = new List<Action>();
	}
}