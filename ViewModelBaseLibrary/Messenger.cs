using System;
using System.Collections.Generic;
using System.Reflection;
using BaseLibrary;

namespace ViewModelBaseLibrary
{
    public class Messenger : IMessenger
    {
        private readonly MessageToActionsMap _messageToActionsMap = new MessageToActionsMap();

        public void RegisterAction(string message, Action callback)
        {
            AddAction(message, callback);
        }

        public void RegisterAction<T1>(string message, Action<T1> callback)
        {
            AddAction(message, callback, typeof(T1));
        }

        public void RegisterAction<T1, T2>(string message, Action<T1, T2> callback)
        {
            AddAction(message, callback, typeof(T1), typeof(T2));
        }

        public void RegisterAction<T1, T2, T3>(string message, Action<T1, T2, T3> callback)
        {
            AddAction(message, callback, typeof(T1), typeof(T2), typeof(T3));
        }

        private void AddAction(string message, Delegate callback, params Type[] parameterTypes)
        {
            if (String.IsNullOrEmpty(message))
                throw new ArgumentException("'message' cannot be null or empty.");

            if (callback == null)
                throw new ArgumentNullException("callback");

            _messageToActionsMap.AddAction(message, callback.Target, callback.Method, parameterTypes);
        }

        public void Notify(string message, params object[] parameters)
        {
            NotifyTarget(null, message, parameters);
        }

        public void NotifyTarget(object target, string message, params object[] parameters)
        {
            if (String.IsNullOrEmpty(message))
                throw new ArgumentException("'message' cannot be null or empty.");

            Type[] registeredParameterTypes;

            if (_messageToActionsMap.TryGetParameterType(message, out registeredParameterTypes))
            {
                if (parameters == null)
                {
                    throw new TargetParameterCountException("Message " + message + " expects " + registeredParameterTypes.Length + " parameters.");
                }
                if (parameters.Length != registeredParameterTypes.Length)
                {
                    throw new TargetParameterCountException("Message " + message + " expects " + registeredParameterTypes.Length + " parameters.");
                }
            }

            var actions = _messageToActionsMap.GetActions(message);

            if (actions != null)
                actions.ForEach(action =>
                {
                    if (target == null || action.Target == target)
                        action.DynamicInvoke(parameters);
                });
        }

        private class MessageToActionsMap
        {
            internal void AddAction(string message, object target, MethodInfo method, params Type[] paramTypes)
            {
                if (message == null)
                    throw new ArgumentNullException("message");

                if (method == null)
                    throw new ArgumentNullException("method");

                lock (map)
                {
                    if (!map.ContainsKey(message))
                        map[message] = new List<WeakAction>();

                    map[message].Add(new WeakAction(target, method, paramTypes));
                }
            }

            internal void RemoveAction(object target)
            {
                lock (map)
                {
                    foreach (List<WeakAction> actionList in map.Values)
                    {
                        actionList.RemoveAll(wa => wa.TargetType == null ? false : wa.TargetType.Equals(target));
                    }
                }
            }

            internal List<Delegate> GetActions(string message)
            {
                if (message == null)
                    throw new ArgumentNullException("message");

                List<Delegate> actions;

                lock (map)
                {
                    if (!map.ContainsKey(message))
                    {
                        return null;
                    }

                    List<WeakAction> weakActions = map[message];

                    actions = new List<Delegate>(weakActions.Count);

                    for (int i = weakActions.Count - 1; i > -1; --i)
                    {
                        WeakAction weakAction = weakActions[i];

                        if (weakAction == null)
                            continue;

                        Delegate action = weakAction.CreateAction();

                        if (action != null)
                            actions.Add(action);
                        else
                            weakActions.Remove(weakAction);     // The target object is dead, so get rid of the weak action.
                    }

                    // Delete the list from the map if it is now empty.
                    if (weakActions.Count == 0)
                        map.Remove(message);
                }

                // Reverse the list to ensure the callbacks are invoked in the order they were registered.
                actions.Reverse();

                return actions;
            }

            /// <summary>
            /// Get the parameter type of the actions registered for the specified message.
            /// </summary>
            /// <param name="message">The message to check for actions.</param>
            /// <param name="parameterTypes">
            /// When this method returns, contains the type for parameters
            /// for the registered actions associated with the specified message, if any; otherwise, null.
            /// This will also be null if the registered actions have no parameters.
            /// This parameter is passed uninitialized.
            /// </param>
            /// <returns>true if any actions were registered for the message</returns>
            internal bool TryGetParameterType(string message, out Type[] parameterTypes)
            {
                if (message == null)
                    throw new ArgumentNullException("message");

                parameterTypes = null;

                List<WeakAction> weakActions;

                lock (map)
                {
                    if (!map.TryGetValue(message, out weakActions) || weakActions.Count == 0)
                        return false;
                }

                parameterTypes = weakActions[0].ParameterTypes;

                return true;
            }

            // Stores a hash where the key is the message and the value is the list of callbacks to invoke.
            readonly Dictionary<string, List<WeakAction>> map = new Dictionary<string, List<WeakAction>>();
        }

        private class WeakAction
        {
            /// <summary>
            /// Constructs a WeakAction.
            /// </summary>
            /// <param name="target">The object on which the target method is invoked, or null if the method is static.</param>
            /// <param name="method">The MethodInfo used to create the Action.</param>
            /// <param name="parameterTypes">The types of parameters to be passed to the action.</param>
            internal WeakAction(object target, MethodInfo method, params Type[] parameterTypes)
            {
                targetRef = target == null ? null : new WeakReference(target);

                this.method = method;

                ParameterTypes = parameterTypes;

                if (parameterTypes == null || parameterTypes.Length == 0)
                    delegateType = typeof(Action);
                else switch (parameterTypes.Length)
                    {
                        case 1:
                            delegateType = typeof(Action<>).MakeGenericType(parameterTypes);
                            break;
                        case 2:
                            delegateType = typeof(Action<,>).MakeGenericType(parameterTypes);
                            break;
                        case 3:
                            delegateType = typeof(Action<,,>).MakeGenericType(parameterTypes);
                            break;
                    }
            }

            /// <summary>
            /// Creates a "throw away" delegate to invoke the method on the target, or null if the target object is dead.
            /// </summary>
            internal Delegate CreateAction()
            {
                // Rehydrate into a real Action object, so that the method can be invoked.
                if (targetRef == null)
                {
                    return Delegate.CreateDelegate(delegateType, method);
                }

                try
                {
                    object target = targetRef.Target;

                    if (target != null)
                        return Delegate.CreateDelegate(delegateType, target, method);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return null;
            }

            internal readonly Type[] ParameterTypes;

            readonly Type delegateType;
            readonly MethodInfo method;
            readonly WeakReference targetRef;

            internal object TargetType
            {
                get { return targetRef == null ? null : targetRef.Target; }
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        } 
    }
}