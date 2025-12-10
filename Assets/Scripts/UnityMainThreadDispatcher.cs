using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A thread-safe dispatcher that allows background threads (e.g., MQTT callbacks,
/// network threads, worker threads) to schedule actions that must be executed on the
/// Unity main thread.
///
/// Unity enforces that almost all engine APIs — including accessing GameObjects,
/// Transforms, Materials, UI elements, and even certain logging operations —
/// may ONLY be called from the main Unity thread. 
///
/// This dispatcher solves that problem by providing a static Enqueue() method
/// that background threads can call. The queued actions are then executed within
/// Unity's next Update() loop on the main thread.
///
/// Example Use Case:
/// - MQTTSubscriber receives messages on a background thread
/// - It cannot directly modify Unity objects or trigger MonoBehaviour callbacks
/// - Instead, it calls:
///     UnityMainThreadDispatcher.Enqueue(() => blackboard.PushEmotionJson(json));
/// - The action is safely executed during the next Update() frame
///
/// This script must be attached to exactly one active GameObject in the scene.
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{
    /// <summary>
    /// A thread-safe queue containing actions that are waiting
    /// to be executed on the Unity main thread.
    /// Background threads enqueue actions; the Unity main thread
    /// dequeues and executes them during Update().
    /// </summary>
    private static readonly Queue<Action> executionQueue = new Queue<Action>();

    /// <summary>
    /// Enqueues an action to be executed on the main Unity thread.
    /// This method is safe to call from any background or worker thread.
    /// </summary>
    /// <param name="action">The callback to execute on the main thread.</param>
    public static void Enqueue(Action action)
    {
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    /// <summary>
    /// Unity Update() callback.
    /// Executes all pending actions queued by background threads.
    /// This method runs on the main Unity thread, ensuring that
    /// queued actions comply with Unity's threading restrictions.
    /// </summary>
    void Update()
    {
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                executionQueue.Dequeue().Invoke();
            }
        }
    }
}
