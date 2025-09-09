using System;
using System.Collections.Concurrent;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    // --- COLA DE ACCIONES ---
    // Cola segura para múltiples hilos (thread-safe) que almacena acciones pendientes
    // Se usa para pasar trabajo desde otros hilos hacia el hilo principal de Unity.
    private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();

    // --- ENCOLAR ACCIONES ---
    // Permite a otros hilos añadir acciones que deben ejecutarse en el hilo principal.
    public static void Enqueue(Action action)
    {
        _executionQueue.Enqueue(action);
    }

    // --- EJECUCIÓN EN EL HILO PRINCIPAL ---
    // En cada frame, se revisa si hay acciones encoladas y se ejecutan en el contexto de Unity.
    // Esto es necesario porque Unity no permite manipular la mayoría de sus APIs desde hilos secundarios.
    private void Update()
    {
        while (_executionQueue.TryDequeue(out var action))
        {
            action?.Invoke(); // Ejecuta la acción pendiente
        }
    }
}
